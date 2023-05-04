using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Consts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ships
{
    public class ShipUpdate
    {
        public class Command : IRequest<Result<ShipDataDto>>
        {
            public ShipDataDto Ship { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ShipDataDto>>
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

            public async Task<Result<ShipDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ships = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .ProjectTo<ShipDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                if (!ships.Any(x => x.Id.Equals(request.Ship.Id)))
                {
                    return Result<ShipDataDto>.Failure("This ship does not exists.");
                }

                if (ships.Any(x =>
                        x.DisplayName.Equals(request.Ship.DisplayName)
                        && !x.Id.Equals(request.Ship.Id)))
                {
                    return Result<ShipDataDto>.Failure("This ship name has already taken.");
                }
                
                var ship = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Ship.Id),
                        cancellationToken);

                _mapper.Map(request.Ship, ship);
                
                ship.IsDeleted = false;
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipDataDto>.Failure("Failed to update ship in database.");
                }

                return Result<ShipDataDto>.Success(request.Ship);
            }
        }
    }
}