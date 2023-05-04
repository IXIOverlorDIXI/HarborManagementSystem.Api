using System;
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
    public class SubscriptionChange
    {
        public class Command : IRequest<Result<SubscriptionChangeDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<SubscriptionChangeDto>>
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

            public async Task<Result<SubscriptionChangeDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await _context.Users
                    .Where(user => user.UserName.Equals(_userAccessor.GetUsername()))
                    .Include(user => user.Subscription)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser.SubscriptionId.Equals(request.Id))
                {
                    return Result<SubscriptionChangeDto>.Failure("This is the same subscription.");
                }
                
                var subscriptions = await _context.Subscriptions
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.MaxHarborAmount)
                    .ThenBy(x => x.TaxOnBooking)
                    .ThenBy(x => x.TaxOnServices)
                    .ProjectTo<SubscriptionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                var subscription = subscriptions.FirstOrDefault(x => x.Id.Equals(request.Id));
                
                if (subscription == null)
                {
                    return Result<SubscriptionChangeDto>.Failure("This subscription does not exists.");
                }

                var subscriptionCheckInfoDto = new SubscriptionChangeDto
                {
                    ChangeCost = currentUser.Subscription == null
                        ? subscription.Price
                        : currentUser.Subscription.Price - subscription.Price < 0
                            ? 0
                            : currentUser.Subscription.Price - subscription.Price,
                    NewSubscription = subscription
                };

                return Result<SubscriptionChangeDto>.Success(subscriptionCheckInfoDto);
            }
        }
    }
}