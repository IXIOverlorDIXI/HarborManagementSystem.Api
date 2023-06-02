using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Consts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Application.Ships.Photos
{
    public class ShipPhotoGet
    {
        public class Query : IRequest<Result<FileDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<FileDto>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IConfiguration _config;

            public Handler(IMapper mapper, 
                DataContext context, 
                IConfiguration config, 
                IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _context = context;
                _config = config;
                _userAccessor = userAccessor;
            }

            public async Task<Result<FileDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ship = await _context.Ships.Where(x =>
                    !x.IsDeleted
                    && x.Id.Equals(request.Id))
                    .Include(x => x.Photo)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (ship.Photo == null)
                {
                    return Result<FileDto>.Success(new FileDto
                    {
                        Url = DefaultFileLinks.DefaultImage
                    });
                }

                var file = _mapper.Map<FileDto>(ship.Photo);

                if (file == null)
                {
                    return Result<FileDto>.Success(new FileDto
                    {
                        Url = DefaultFileLinks.DefaultImage
                    });
                }
                
                return Result<FileDto>.Success(file);
            }
        }
    }
}