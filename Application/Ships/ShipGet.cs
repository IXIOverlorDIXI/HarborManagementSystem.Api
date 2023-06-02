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
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ships
{
    public class ShipGet
    {
        public class Query : IRequest<Result<ShipDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ShipDataDto>>
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

            public async Task<Result<ShipDataDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ships = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .ProjectTo<ShipDataDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);

                return Result<ShipDataDto>.Success(ships);
            }
        }
    }
}