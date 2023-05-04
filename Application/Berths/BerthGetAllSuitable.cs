using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Berths
{
    public class BerthGetAllSuitable
    {
        public class Query : IRequest<Result<List<BerthPreviewDataDto>>>
        {
            public SuitableBerthSearchModel SuitableBerthSearchModel { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BerthPreviewDataDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;


            public Handler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<List<BerthPreviewDataDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var berth = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Where(x =>
                        x.HarborId.Equals(request.SuitableBerthSearchModel.HarborId)
                        && x.SuitableShipTypes
                            .Any(y => y.ShipTypeId
                                .Equals(request.SuitableBerthSearchModel.ShipType.Id)))
                    .ProjectTo<BerthPreviewDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<BerthPreviewDataDto>>.Success(berth);
            }
        }
    }
}