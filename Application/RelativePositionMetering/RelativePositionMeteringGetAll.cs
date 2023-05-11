using System;
using System.Collections.Generic;
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
    public class RelativePositionMeteringGetAll
    {
        public class Query : IRequest<Result<List<RelativePositionMeteringDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<RelativePositionMeteringDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(IMapper mapper, DataContext context, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<RelativePositionMeteringDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ship = await _context.Ships
                    .Where(x => x.Id.Equals(request.Id))
                    .Include(x => x.Bookings)
                    .ThenInclude(x => x.Berth)
                    .ThenInclude(x => x.RelativePositionMeterings)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ship == null)
                {
                    return Result<List<RelativePositionMeteringDto>>.Failure("Ship not found.");
                }

                var meterings = ship.Bookings?
                    .Where(x => x.EndDate >= DateTime.Now && x.StartDate <= DateTime.Now)
                    .SelectMany(x => x.Berth?.RelativePositionMeterings
                        .Where(y => y.MeteringDate <= x.EndDate && y.MeteringDate >= x.StartDate))
                    .OrderBy(x => x.MeteringDate)
                    .ToList();

                return Result<List<RelativePositionMeteringDto>>
                    .Success(_mapper.Map<List<RelativePositionMeteringDto>>(meterings));
            }
        }
    }
}