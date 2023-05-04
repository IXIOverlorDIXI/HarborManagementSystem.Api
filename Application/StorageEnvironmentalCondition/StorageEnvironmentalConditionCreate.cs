using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.StorageEnvironmentalCondition
{
    public class StorageEnvironmentalConditionCreate
    {
        public class Command : IRequest<Result<StorageEnvironmentalConditionDto>>
        {
            public StorageEnvironmentalConditionDto StorageEnvironmentalCondition { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<StorageEnvironmentalConditionDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<StorageEnvironmentalConditionDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Berths.Any(x => 
                        !x.IsDeleted 
                        && x.Id.Equals(request.StorageEnvironmentalCondition.BerthId)))
                {
                    return Result<StorageEnvironmentalConditionDto>.Failure("Fail, the berth does not exist.");
                }
                
                bool result;
                
                if (!_context.Bookings.Any(x =>
                        x.Berth.Id.Equals(request.StorageEnvironmentalCondition.BerthId)
                        && x.BookingCheck != null
                        && x.EndDate >= DateTime.Now
                        && x.StartDate <= DateTime.Now))
                {
                    var meteringsToDelete = await _context.StorageEnvironmentalConditions
                        .Where(x => x.BerthId.Equals(request.StorageEnvironmentalCondition.BerthId))
                        .ToListAsync(cancellationToken);
                    
                    if (meteringsToDelete.Any())
                    {
                        _context.StorageEnvironmentalConditions.RemoveRange(meteringsToDelete);
                        
                        result = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            return Result<StorageEnvironmentalConditionDto>.Failure("Failed to remove the storage environmental condition old data.");
                        }
                    }
                    
                    return Result<StorageEnvironmentalConditionDto>.Failure("No need to save data, there's no ship in the berth.");
                }

                var storageEnvironmentalCondition = _mapper
                    .Map<Domain.Entities.StorageEnvironmentalCondition>(request.StorageEnvironmentalCondition);
                
                storageEnvironmentalCondition.Id = Guid.NewGuid();

                await _context.StorageEnvironmentalConditions.AddAsync(storageEnvironmentalCondition, cancellationToken);
                
                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<StorageEnvironmentalConditionDto>.Failure("Failed to create the storage environmental condition metering.");
                }
                
                return Result<StorageEnvironmentalConditionDto>
                    .Success(_mapper.Map<StorageEnvironmentalConditionDto>(storageEnvironmentalCondition));
            }
        }
    }
}