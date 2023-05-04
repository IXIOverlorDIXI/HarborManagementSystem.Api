using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Consts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Review
{
    public class ReviewDelete
    {
        public class Command : IRequest<Result<ReviewDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ReviewDataDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IBlobManagerService _blobManagerService;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context, IBlobManagerService blobManagerService)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
                _blobManagerService = blobManagerService;
            }

            public async Task<Result<ReviewDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var reviews = await _context.Reviews
                    .ToListAsync(cancellationToken);

                if (!reviews.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<ReviewDataDto>.Failure("This review does not exists.");
                }

                var review = await _context.Reviews
                    .Include(x => x.Reviewer)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);

                if (!review.Reviewer.UserName.Equals(_userAccessor.GetUsername())
                    && !_userAccessor.IsInRole(Roles.Admin))
                {
                    return Result<ReviewDataDto>.Failure("Fail, you have not enough permission.");
                }

                _context.Reviews.Remove(review);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ReviewDataDto>.Failure("Failed to remove service from database.");
                }

                return Result<ReviewDataDto>.Success(_mapper.Map<ReviewDataDto>(review));
            }
        }
    }
}