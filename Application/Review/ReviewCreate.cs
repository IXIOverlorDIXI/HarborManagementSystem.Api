using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Review
{
    public class ReviewCreate
    {
        public class Command : IRequest<Result<ReviewDataDto>>
        {
            public ReviewDataDto Review { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ReviewDataDto>>
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

            public async Task<Result<ReviewDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Berths.Any(x => x.Id.Equals(request.Review.BerthId)))
                {
                    return Result<ReviewDataDto>.Failure("Fail, the berth does not exist.");
                }

                var user = await _context.Users
                    .Include(x => x.Reviews)
                    .FirstOrDefaultAsync(x =>
                            x.UserName.Equals(_userAccessor.GetUsername()),
                        cancellationToken);

                if (user == null)
                {
                    return Result<ReviewDataDto>.Failure("Fail. user does not exist.");
                }
                
                if (user.Reviews.Any(x => x.BerthId.Equals(request.Review.BerthId)))
                {
                    return Result<ReviewDataDto>.Failure("Fail, user has already leave his review.");
                }
                
                var review = _mapper.Map<Domain.Entities.Review>(request.Review);
                
                review.Id = Guid.NewGuid();
                review.ReviewerId = user.Id;

                await _context.Reviews.AddAsync(review, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ReviewDataDto>.Failure("Failed to create the review.");
                }
                
                return Result<ReviewDataDto>.Success(_mapper.Map<ReviewDataDto>(review));
            }
        }
    }
}