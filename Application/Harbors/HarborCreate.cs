using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Harbors
{
    public class HarborCreate
    {
        public class Command : IRequest<Result<HarborDataDto>>
        {
            public HarborDataDto Harbor { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<HarborDataDto>>
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

            public async Task<Result<HarborDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (_context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Any(x => x.DisplayName.Equals(request.Harbor.DisplayName)))
                {
                    return Result<HarborDataDto>.Failure("This harbor name has already taken.");
                }

                var user = await _context.Users
                    .Include(x => x.Subscription)
                    .Include(x => x.Harbors)
                    .FirstOrDefaultAsync(x => x.UserName.Equals(
                            _userAccessor.GetUsername()),
                        cancellationToken);

                if (user.Subscription == null)
                {
                    return Result<HarborDataDto>.Failure("This user has not subscription.");
                }
                
                if (user.Subscription.MaxHarborAmount <= user.Harbors.Count(x => !x.IsDeleted))
                {
                    return Result<HarborDataDto>.Failure("This user has not enough slots for harbors by current subscription.");
                }
                
                var harbor = _mapper.Map<Harbor>(request.Harbor);
                
                harbor.Id = Guid.NewGuid();
                harbor.IsDeleted = false;
                
                harbor.OwnerId = user.Id;

                await _context.Harbors.AddAsync(harbor, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<HarborDataDto>.Failure("Failed to create the harbor.");
                }
                
                return Result<HarborDataDto>.Success(_mapper.Map<HarborDataDto>(harbor));
            }
        }
    }
}