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

namespace Application.Berths
{
    public class BerthTurnOfOn
    {
        public class Command : IRequest<Result<BerthDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BerthDataDto>>
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

            public async Task<Result<BerthDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var berth = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.Id.Equals(request.Id))
                    .FirstOrDefaultAsync(cancellationToken);

                if (berth == null)
                {
                    return Result<BerthDataDto>.Failure("This berth does not exists.");
                }

                berth.IsActive = !berth.IsActive;
                berth.IsDeleted = false;

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BerthDataDto>.Failure("Failed to turn off/on berth in database.");
                }

                return Result<BerthDataDto>.Success(_mapper.Map<BerthDataDto>(berth));
            }
        }
    }
}