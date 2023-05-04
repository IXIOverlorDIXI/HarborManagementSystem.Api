using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Consts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.ShipTypes
{
    public class ShipTypeDelete
    {
        public class Command : IRequest<Result<ShipTypeDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ShipTypeDto>>
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

            public async Task<Result<ShipTypeDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_userAccessor.IsInRole(Roles.Admin))
                {
                    return Result<ShipTypeDto>.Failure("You have not right permission.");
                }

                var shipTypes = await _context.ShipTypes
                    .Include(x => x.Ships)
                    .Include(x => x.SuitableShipTypes)
                    .ToListAsync(cancellationToken);

                if (!shipTypes.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<ShipTypeDto>.Failure("This ship type does not exists.");
                }

                if (shipTypes.FirstOrDefault(x => x.Id.Equals(request.Id)).Ships.Any()
                    || shipTypes.FirstOrDefault(x => x.Id.Equals(request.Id)).SuitableShipTypes.Any())
                {
                    return Result<ShipTypeDto>.Failure("Fail, ship type is in use.");
                }

                var shipType = await _context.ShipTypes
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);
                
                _context.ShipTypes.Remove(shipType);
            
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipTypeDto>.Failure("Failed to remove ship type from database.");
                }

                return Result<ShipTypeDto>.Success(_mapper.Map<ShipTypeDto>(shipType));
            }
        }
    }
}