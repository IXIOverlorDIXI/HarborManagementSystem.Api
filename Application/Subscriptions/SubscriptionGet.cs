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

namespace Application.Subscriptions
{
    public class SubscriptionGet
    {
        public class Query : IRequest<Result<SubscriptionDto>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<SubscriptionDto>>
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

            public async Task<Result<SubscriptionDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<SubscriptionDto>.Failure("Wrong username.");
                }
                
                var currentUser = await _context.Users
                    .Where(user => user.UserName.Equals(_userAccessor.GetUsername()))
                    .Include(user => user.Subscription)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser == null)
                {
                    return Result<SubscriptionDto>.Failure("User not found.");
                }

                if (currentUser.Subscription == null)
                {
                    currentUser.Subscription = await _context.Subscriptions
                        .Where(x => !x.IsDeleted)
                        .FirstOrDefaultAsync(x => x.Price == 0, cancellationToken);

                    if (currentUser.Subscription == null)
                    {
                        return Result<SubscriptionDto>.Failure("Failed to find free subscription in database.");
                    }
                    
                    currentUser.SubscriptionId = currentUser.Subscription.Id;

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<SubscriptionDto>.Failure("Failed to update user subscription in database.");
                    }
                }
                
                var currentSubscription = _mapper.Map<SubscriptionDto>(currentUser.Subscription);

                return Result<SubscriptionDto>.Success(currentSubscription);
            }
        }
    }
}