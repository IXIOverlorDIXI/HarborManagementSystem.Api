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

namespace Application.Service
{
    public class ServiceDelete
    {
        public class Command : IRequest<Result<ServiceDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ServiceDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IBlobManagerService _blobManagerService;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context, IBlobManagerService blobManagerService)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
                _blobManagerService = blobManagerService;
            }

            public async Task<Result<ServiceDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var services = await _context.Services
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!services.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<ServiceDto>.Failure("This service does not exists.");
                }

                var service = await _context.Services
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);

                service.IsDeleted = true;

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ServiceDto>.Failure("Failed to remove service from database.");
                }

                return Result<ServiceDto>.Success(_mapper.Map<ServiceDto>(service));
            }
        }
    }
}