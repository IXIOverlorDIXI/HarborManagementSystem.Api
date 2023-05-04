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

namespace Application.Service
{
    public class ServiceUpdate
    {
        public class Command : IRequest<Result<ServiceDto>>
        {
            public ServiceDto Service { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ServiceDto>>
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

            public async Task<Result<ServiceDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_context.Harbors.Any(x => 
                        !x.IsDeleted 
                        && x.Id.Equals(request.Service.HarborId)))
                {
                    return Result<ServiceDto>.Failure("Fail, the harbor does not exist.");
                }
                
                var services = await _context.Services
                    .Where(x => !x.IsDeleted)
                    .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                if (!services.Any(x => x.Id.Equals(request.Service.Id)))
                {
                    return Result<ServiceDto>.Failure("This service does not exists.");
                }

                if (services.Any(x =>
                        x.DisplayName.Equals(request.Service.DisplayName)
                        && !x.Id.Equals(request.Service.Id)))
                {
                    return Result<ServiceDto>.Failure("This service name has already taken.");
                }
                
                var service = await _context.Services
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Service.Id),
                        cancellationToken);

                _mapper.Map(request.Service, service);
                
                service.IsDeleted = false;
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ServiceDto>.Failure("Failed to update ship in database.");
                }

                return Result<ServiceDto>.Success(request.Service);
            }
        }
    }
}