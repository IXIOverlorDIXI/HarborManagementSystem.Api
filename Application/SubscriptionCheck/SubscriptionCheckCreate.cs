using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.SubscriptionCheck
{
    public class SubscriptionCheckCreate
    {
        public class Command : IRequest<Result<SubscriptionCheckDto>>
        {
            public SubscriptionCheckDto SubscriptionCheck { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<SubscriptionCheckDto>>
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

            public async Task<Result<SubscriptionCheckDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Subscriptions.Any(x => x.Id.Equals(request.SubscriptionCheck.SubscriptionId)))
                {
                    return Result<SubscriptionCheckDto>.Failure("Fail, this subscription does not exist.");
                }
                
                var subscriptionCheck = _mapper.Map<SubscriptionСheck>(request.SubscriptionCheck);

                var user = await _context.Users.FirstOrDefaultAsync(
                    x => x.UserName.Equals(_userAccessor.GetUsername()),
                    cancellationToken);
                
                subscriptionCheck.Id = Guid.NewGuid();
                subscriptionCheck.UserId = user.Id;

                await _context.SubscriptionСhecks.AddAsync(subscriptionCheck, cancellationToken);

                user.SubscriptionId = subscriptionCheck.SubscriptionId;
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<SubscriptionCheckDto>.Failure("Failed to create the subscription check.");
                }
                
                return Result<SubscriptionCheckDto>.Success(_mapper.Map<SubscriptionCheckDto>(subscriptionCheck));
            }
        }
    }
}