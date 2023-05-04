using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.EnvironmentalCondition
{
    public class EnvironmentalConditionCreate
    {
        public class Command : IRequest<Result<EnvironmentalConditionDto>>
        {
            public EnvironmentalConditionDto EnvironmentalCondition { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<EnvironmentalConditionDto>>
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

            public async Task<Result<EnvironmentalConditionDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                Enum.TryParse<ShipRelativeWindDirection>(
                    request.EnvironmentalCondition.ShipRelativeWindDirection.ToString(),
                    out ShipRelativeWindDirection enumResult);
                
                if (!Enum.IsDefined(typeof(ShipRelativeWindDirection), enumResult))
                {
                    return Result<EnvironmentalConditionDto>.Failure("Fail, the environmental condition has wrong wind direction.");
                }
                
                if (!_context.Berths.Any(x => 
                        !x.IsDeleted 
                        && x.Id.Equals(request.EnvironmentalCondition.BerthId)))
                {
                    return Result<EnvironmentalConditionDto>.Failure("Fail, the berth does not exist.");
                }

                bool result;
                
                if (!_context.Bookings.Any(x =>
                        x.Berth.Id.Equals(request.EnvironmentalCondition.BerthId)
                        && x.BookingCheck != null
                        && x.EndDate >= DateTime.Now
                        && x.StartDate <= DateTime.Now))
                {
                    var meteringsToDelete = await _context.EnvironmentalConditions
                        .Where(x => x.BerthId.Equals(request.EnvironmentalCondition.BerthId))
                        .ToListAsync(cancellationToken);
                    
                    if (meteringsToDelete.Any())
                    {
                        _context.EnvironmentalConditions.RemoveRange(meteringsToDelete);
                        
                        result = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            return Result<EnvironmentalConditionDto>.Failure("Failed to remove the environmental condition old data.");
                        }
                    }
                    
                    return Result<EnvironmentalConditionDto>.Failure("No need to save data, there's no ship in the berth.");
                }
                
                var environmentalCondition = _mapper.Map<Domain.Entities.EnvironmentalCondition>(request.EnvironmentalCondition);
                
                environmentalCondition.Id = Guid.NewGuid();

                await _context.EnvironmentalConditions.AddAsync(environmentalCondition, cancellationToken);
                
                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<EnvironmentalConditionDto>.Failure("Failed to create the environmental condition metering.");
                }
                
                return Result<EnvironmentalConditionDto>
                    .Success(_mapper.Map<EnvironmentalConditionDto>(environmentalCondition));
            }
        }
    }
}