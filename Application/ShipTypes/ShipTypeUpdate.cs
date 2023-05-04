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

namespace Application.ShipTypes
{
    public class ShipTypeUpdate
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
                
                var shipTypes = await _context.ShipTypes
                    .ProjectTo<ShipTypeDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                if (!shipTypes.Any(x => x.Id.Equals(request.ShipType.Id)))
                {
                    return Result<ShipTypeDto>.Failure("This ship type does not exists.");
                }

                if (shipTypes.Any(x =>
                        x.TypeName.Equals(request.ShipType.TypeName)
                        && !x.Id.Equals(request.ShipType.Id)))
                {
                    return Result<ShipTypeDto>.Failure("This ship type name has already taken.");
                }
                
                var shipType = await _context.ShipTypes
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.ShipType.Id),
                        cancellationToken);

                _mapper.Map(request.ShipType, shipType);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipTypeDto>.Failure("Failed to update ship type in database.");
                }

                return Result<ShipTypeDto>.Success(request.ShipType);
            }
        }
    }
}