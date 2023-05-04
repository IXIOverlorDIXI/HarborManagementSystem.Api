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
    public class SubscriptionDelete
    {
        public class Command : IRequest<Result<SubscriptionDto>>
        {
            public Guid Id { get; set; }
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

                var subscriptions = await _context.Subscriptions
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.MaxHarborAmount)
                    .ThenBy(x => x.TaxOnBooking)
                    .ThenBy(x => x.TaxOnServices)
                    .Include(x => x.Users)
                    .ToListAsync(cancellationToken);

                if (!subscriptions.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<SubscriptionDto>.Failure("This subscription does not exists.");
                }

                if (subscriptions.FirstOrDefault(x => x.Id.Equals(request.Id)).Users.Any())
                {
                    return Result<SubscriptionDto>.Failure("Fail, subscription is in use.");
                }

                if (subscriptions.Count(x => x.Price == 0) == 1
                    && subscriptions.FirstOrDefault(x => x.Id.Equals(request.Id)).Price == 0)
                {
                    return Result<SubscriptionDto>.Failure("Fail, it is the last free subscription.");
                }
                
                var subscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);
                
                //_context.Subscriptions.Remove(subscription);

                subscription.IsDeleted = true;
            
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<SubscriptionDto>.Failure("Failed to remove subscription from database.");
                }

                return Result<SubscriptionDto>.Success(_mapper.Map<SubscriptionDto>(subscription));
            }
        }
    }
}