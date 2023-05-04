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

namespace Application.Harbors
{
    public class HarborUpdate
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
                var harbors = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .ProjectTo<HarborDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                if (!harbors.Any(x => x.Id.Equals(request.Harbor.Id)))
                {
                    return Result<HarborDataDto>.Failure("This harbor does not exists.");
                }

                if (harbors.Any(x =>
                        x.DisplayName.Equals(request.Harbor.DisplayName)
                        && !x.Id.Equals(request.Harbor.Id)))
                {
                    return Result<HarborDataDto>.Failure("This harbor name has already taken.");
                }
                
                var harbor = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Owner)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Harbor.Id),
                        cancellationToken);

                if (!harbor.Owner.UserName.Equals(_userAccessor.GetUsername()))
                {
                    return Result<HarborDataDto>.Failure("Wrong user.");
                }
                
                _mapper.Map(request.Harbor, harbor);
                
                harbor.IsDeleted = false;
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<HarborDataDto>.Failure("Failed to update harbor in database.");
                }

                return Result<HarborDataDto>.Success(request.Harbor);
            }
        }
    }
}