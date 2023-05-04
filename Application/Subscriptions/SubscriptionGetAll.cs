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
    public class SubscriptionGetAll
    {
        public class Query : IRequest<Result<AllSubscriptionsDto>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AllSubscriptionsDto>>
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

            public async Task<Result<AllSubscriptionsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<AllSubscriptionsDto>.Failure("Wrong username.");
                }
                
                var currentUser = await _context.Users
                    .Where(user => user.UserName.Equals(_userAccessor.GetUsername()))
                    .Include(user => user.Subscription)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser == null)
                {
                    return Result<AllSubscriptionsDto>.Failure("User not found.");
                }

                var subscriptions = new AllSubscriptionsDto();

                subscriptions.Subscriptions = await _context.Subscriptions
                    .Where(x => !x.IsDeleted)
                    .OrderBy(subscription => subscription.Price)
                    .ThenBy(x => x.MaxHarborAmount)
                    .ThenBy(x => x.TaxOnBooking)
                    .ThenBy(x => x.TaxOnServices)
                    .ProjectTo<SubscriptionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                if (currentUser.Subscription == null)
                {
                    subscriptions.CurrentSubscriptionIndex = -1;
                }
                else
                {
                    subscriptions.CurrentSubscriptionIndex = subscriptions.Subscriptions
                        .FindIndex(x => x.Id.Equals(currentUser.SubscriptionId));
                }
                
                return Result<AllSubscriptionsDto>.Success(subscriptions);
            }
        }
    }
}