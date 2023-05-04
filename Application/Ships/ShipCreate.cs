using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Consts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ships
{
    public class ShipCreate
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
                if (_context.Ships
                    .Where(x => !x.IsDeleted)
                    .Any(x => x.DisplayName.Equals(request.Ship.DisplayName)))
                {
                    return Result<ShipDataDto>.Failure("This ship name has already taken.");
                }
                
                var ship = _mapper.Map<Ship>(request.Ship);
                
                ship.Id = Guid.NewGuid();
                ship.IsDeleted = false;
                
                ship.OwnerId = (await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.UserName.Equals(_userAccessor.GetUsername()),
                        cancellationToken)).Id;

                await _context.Ships.AddAsync(ship, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipDataDto>.Failure("Failed to create the ship.");
                }
                
                return Result<ShipDataDto>.Success(_mapper.Map<ShipDataDto>(ship));
            }
        }
    }
}