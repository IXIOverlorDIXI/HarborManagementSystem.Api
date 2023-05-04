using System.Collections.Generic;
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

namespace Application.SubscriptionCheck
{
    public class SubscriptionCheckGetAll
    {
        public class Query : IRequest<Result<List<SubscriptionCheckDto>>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<SubscriptionCheckDto>>>
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

            public async Task<Result<List<SubscriptionCheckDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<List<SubscriptionCheckDto>>.Failure("Wrong username.");
                }
                
                var subscriptionСhecks = await _context.SubscriptionСhecks
                    .Include(x => x.AppUser)
                    .Where(x => x.AppUser.UserName.Equals(_userAccessor.GetUsername()))
                    .ToListAsync(cancellationToken);

                var subscriptionСhecksDtos = _mapper.Map<List<SubscriptionCheckDto>>(subscriptionСhecks);

                return Result<List<SubscriptionCheckDto>>.Success(subscriptionСhecksDtos);
            }
        }
    }
}