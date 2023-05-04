using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Consts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Subscriptions
{
    public class SubscriptionCreate
    {
        public class Command : IRequest<Result<SubscriptionDto>>
        {
            public SubscriptionDto Subscription { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<SubscriptionDto>>
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

            public async Task<Result<SubscriptionDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_userAccessor.IsInRole(Roles.Admin))
                {
                    return Result<SubscriptionDto>.Failure("You have not right permission.");
                }

                if (request.Subscription.Price < 0)
                {
                    return Result<SubscriptionDto>.Failure("Price must be non-negative.");
                }

                if (request.Subscription.MaxHarborAmount < 0)
                {
                    return Result<SubscriptionDto>.Failure("Max harbor amount must be non-negative.");
                }

                if (request.Subscription.TaxOnBooking < 0)
                {
                    return Result<SubscriptionDto>.Failure("Tax on booking must be non-negative.");
                }

                if (request.Subscription.TaxOnServices < 0)
                {
                    return Result<SubscriptionDto>.Failure("Tax on services must be non-negative.");
                }

                var subscriptions = await _context.Subscriptions
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.MaxHarborAmount)
                    .ThenBy(x => x.TaxOnBooking)
                    .ThenBy(x => x.TaxOnServices)
                    .ProjectTo<SubscriptionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                if (subscriptions.Any(x => 
                        x.MaxHarborAmount == request.Subscription.MaxHarborAmount
                        && x.TaxOnBooking == request.Subscription.TaxOnBooking
                        && x.TaxOnServices == request.Subscription.TaxOnServices))
                {
                    return Result<SubscriptionDto>.Failure("Same subscription already exists.");
                }

                var subscription = _mapper.Map<Subscription>(request.Subscription);

                subscription.Id = Guid.NewGuid();
                subscription.IsDeleted = false;

                await _context.Subscriptions.AddAsync(subscription, cancellationToken);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<SubscriptionDto>.Failure("Failed to save subscription into database.");
                }

                return Result<SubscriptionDto>.Success( _mapper.Map<SubscriptionDto>(subscription));
            }
        }
    }
}