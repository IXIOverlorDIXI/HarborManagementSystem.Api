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

namespace Application.Ships
{
    public class ShipGetAll
    {
        public class Query : IRequest<Result<List<ShipPreviewDataDto>>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ShipPreviewDataDto>>>
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

            public async Task<Result<List<ShipPreviewDataDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ships = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.Owner.UserName.Equals(request.Username))
                    .ProjectTo<ShipPreviewDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return Result<List<ShipPreviewDataDto>>.Success(ships);
            }
        }
    }
}