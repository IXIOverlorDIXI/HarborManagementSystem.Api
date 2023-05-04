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

namespace Application.ShipTypes
{
    public class ShipTypeGetAll
    {
        public class Query : IRequest<Result<List<ShipTypeDto>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<ShipTypeDto>>>
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

            public async Task<Result<List<ShipTypeDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var shipTypes = await _context.ShipTypes
                    .ProjectTo<ShipTypeDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<ShipTypeDto>>.Success(shipTypes);
            }
        }
    }
}