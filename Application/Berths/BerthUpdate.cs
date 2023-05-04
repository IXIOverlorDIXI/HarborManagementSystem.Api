using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Berths
{
    public class BerthUpdate
    {
        public class Command : IRequest<Result<BerthDataDto>>
        {
            public BerthDataDto Berth { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BerthDataDto>>
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

            public async Task<Result<BerthDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var berths = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.HarborId.Equals(request.Berth.HarborId))
                    .ProjectTo<BerthDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                if (!berths.Any(x => x.Id.Equals(request.Berth.Id)))
                {
                    return Result<BerthDataDto>.Failure("This berth does not exists.");
                }

                if (berths.Any(x =>
                        x.DisplayName.Equals(request.Berth.DisplayName)
                        && !x.Id.Equals(request.Berth.Id)))
                {
                    return Result<BerthDataDto>.Failure("This berth name has already taken.");
                }

                var berth = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.SuitableShipTypes)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Berth.Id),
                        cancellationToken);

                _mapper.Map(request.Berth, berth);

                berth.IsDeleted = false;
                berth.IsActive = true;

                var shipTypes = new List<SuitableShipType>();

                foreach (var shipType in request.Berth.SuitableShipTypes)
                {
                    if (!_context.ShipTypes.Any(x => x.Id.Equals(shipType.Id)))
                    {
                        return Result<BerthDataDto>.Failure(String.Concat("Fail, type: ", shipType.TypeName,
                            " does not exist."));
                    }

                    shipTypes.Add(new SuitableShipType
                    {
                        Id = Guid.NewGuid(),
                        ShipTypeId = shipType.Id,
                        BerthId = berth.Id
                    });
                }

                var shipTypesToAdd = shipTypes
                    .Where(x => !berth.SuitableShipTypes
                        .Any(y => y.ShipTypeId.Equals(x.ShipTypeId)))
                    .ToList();

                var shipTypesToRemove = berth.SuitableShipTypes
                    .Where(x => !shipTypes.Any(y => y.ShipTypeId.Equals(x.ShipTypeId)))
                    .ToList();

                _context.SuitableShipTypes.RemoveRange(shipTypesToRemove);
                await _context.SuitableShipTypes.AddRangeAsync(shipTypesToAdd, cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BerthDataDto>.Failure("Failed to update berth in database.");
                }

                return Result<BerthDataDto>.Success(request.Berth);
            }
        }
    }
}