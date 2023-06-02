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

namespace Application.Review
{
    public class ReviewByHarborGetAll
    {
        public class Query : IRequest<Result<List<ReviewPreviewDataDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ReviewPreviewDataDto>>>
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

            public async Task<Result<List<ReviewPreviewDataDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                if (!_context.Harbors.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<List<ReviewPreviewDataDto>>.Failure("Fail, the harbor does not exist.");
                }

                var reviews = await _context.Reviews
                    .Where(x => x.Berth.HarborId.Equals(request.Id))
                    .ProjectTo<ReviewPreviewDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                foreach (var review in reviews)
                {
                    review.IsAuthor = _context.Reviews
                        .Any(x =>
                            x.Reviewer.UserName.Equals(_userAccessor.GetUsername())
                            && x.Id.Equals(review.Id));
                }

                return Result<List<ReviewPreviewDataDto>>.Success(reviews);
            }
        }
    }
}