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

namespace Application.Service
{
    public class ServiceCreate
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
                
                if (_context.Services
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.HarborId.Equals(request.Service.HarborId))
                    .Any(x => x.DisplayName.Equals(request.Service.DisplayName)))
                {
                    return Result<ServiceDto>.Failure("This service name has already taken.");
                }
                
                var service = _mapper.Map<Domain.Entities.Service>(request.Service);
                
                service.Id = Guid.NewGuid();
                service.IsDeleted = false;

                await _context.Services.AddAsync(service, cancellationToken);
                
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ServiceDto>.Failure("Failed to create the service.");
                }
                
                return Result<ServiceDto>.Success(_mapper.Map<ServiceDto>(service));
            }
        }
    }
}