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

namespace Application.RelativePositionMetering
{
    public class RelativePositionMeteringCreate
    {
        public class Command : IRequest<Result<RelativePositionMeteringDto>>
        {
            public RelativePositionMeteringDto RelativePositionMetering { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<RelativePositionMeteringDto>>
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

            public async Task<Result<RelativePositionMeteringDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Berths.Any(x => 
                        !x.IsDeleted 
                        && x.Id.Equals(request.RelativePositionMetering.BerthId)))
                {
                    return Result<RelativePositionMeteringDto>.Failure("Fail, the berth does not exist.");
                }

                bool result;
                
                if (!_context.Bookings.Any(x =>
                        x.Berth.Id.Equals(request.RelativePositionMetering.BerthId)
                        && x.BookingCheck != null
                        && x.EndDate >= DateTime.Now
                        && x.StartDate <= DateTime.Now))
                {
                    var meteringsToDelete = await _context.RelativePositionMeterings
                        .Where(x => x.BerthId.Equals(request.RelativePositionMetering.BerthId))
                        .ToListAsync(cancellationToken);
                    
                    if (meteringsToDelete.Any())
                    {
                        _context.RelativePositionMeterings.RemoveRange(meteringsToDelete);
                        
                        result = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            return Result<RelativePositionMeteringDto>.Failure("Failed to remove the relative position metering old data.");
                        }
                    }
                    
                    return Result<RelativePositionMeteringDto>.Failure("No need to save data, there's no ship in the berth.");
                }

                var relativePositionMetering = _mapper.Map<Domain.Entities.RelativePositionMetering>(request.RelativePositionMetering);
                
                relativePositionMetering.Id = Guid.NewGuid();

                await _context.RelativePositionMeterings.AddAsync(relativePositionMetering, cancellationToken);
                
                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<RelativePositionMeteringDto>.Failure("Failed to create the relative position metering.");
                }
                
                return Result<RelativePositionMeteringDto>
                    .Success(_mapper.Map<RelativePositionMeteringDto>(relativePositionMetering));
            }
        }
    }
}