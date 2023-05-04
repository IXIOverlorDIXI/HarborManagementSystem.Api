using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Berths
{
    public class BerthCreate
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
                if (_context.Berths
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.HarborId.Equals(request.Berth.HarborId))
                    .Any(x => x.DisplayName.Equals(request.Berth.DisplayName)))
                {
                    return Result<BerthDataDto>.Failure("This berth name has already taken.");
                }

                var berth = _mapper.Map<Domain.Entities.Berth>(request.Berth);

                berth.Id = Guid.NewGuid();
                berth.IsDeleted = false;
                berth.IsActive = true;

                var shipTypes = new List<SuitableShipType>();

                foreach (var shipType in request.Berth.SuitableShipTypes)
                {
                    if (!_context.ShipTypes.Any(x => x.Id.Equals(shipType.Id)))
                    {
                        return Result<BerthDataDto>.Failure(String.Concat("Fail, type: \"", shipType.TypeName,
                            "\" does not exist."));
                    }

                    shipTypes.Add(new SuitableShipType
                    {
                        Id = Guid.NewGuid(),
                        ShipTypeId = shipType.Id,
                        BerthId = berth.Id
                    });
                }

                await _context.Berths.AddAsync(berth, cancellationToken);
                await _context.SuitableShipTypes.AddRangeAsync(shipTypes, cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BerthDataDto>.Failure("Failed to create the berth.");
                }

                return Result<BerthDataDto>.Success(_mapper.Map<BerthDataDto>(berth));
            }
        }
    }
}