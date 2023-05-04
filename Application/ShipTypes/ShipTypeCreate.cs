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
using Persistence;

namespace Application.ShipTypes
{
    public class ShipTypeCreate
    {
        public class Command : IRequest<Result<ShipTypeDto>>
        {
            public ShipTypeDto ShipType { get; set; }
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
                
                if (_context.ShipTypes.Any(x => x.TypeName.Equals(request.ShipType.TypeName)))
                {
                    return Result<ShipTypeDto>.Failure("Fail, this ship type has already exist.");
                }
                
                var shipType = _mapper.Map<ShipType>(request.ShipType);
                
                shipType.Id = Guid.NewGuid();

                await _context.ShipTypes.AddAsync(shipType, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipTypeDto>.Failure("Failed to create the subscription check.");
                }
                
                return Result<ShipTypeDto>.Success(_mapper.Map<ShipTypeDto>(shipType));
            }
        }
    }
}