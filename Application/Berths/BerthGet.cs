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

namespace Application.Berths
{
    public class BerthGet
    {
        public class Query : IRequest<Result<BerthPreviewDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<BerthPreviewDataDto>>
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

            public async Task<Result<BerthPreviewDataDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                var berth = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Reviews)
                    .ProjectTo<BerthPreviewDataDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken);

                var reviews = await _context.Reviews
                    .Where(x => x.BerthId.Equals(berth.Id))
                    .ToListAsync(cancellationToken);
                berth.AverageRate = !reviews.Any() ? 0 : reviews.Average(x => x.ReviewMark);
                berth.ReviewsAmount = reviews.Count;
                
                try
                {
                    berth.IsOwner = _context.Users
                        .Where(x => x.UserName
                            .Equals(_userAccessor
                                .GetUsername()))
                        .Any(a=> a.Harbors
                            .Any(h => h.Berths
                                .Any(b => b.Id
                                    .Equals(berth.Id))));
                }
                catch (Exception e)
                {
                    berth.IsOwner = false;
                }

                return Result<BerthPreviewDataDto>.Success(berth);
            }
        }
    }
}