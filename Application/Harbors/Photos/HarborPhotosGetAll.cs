using System;
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
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Application.Harbors.Photos
{
    public class HarborPhotosGetAll
    {
        public class Query : IRequest<Result<List<HarborPhotoDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<HarborPhotoDto>>>
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

            public async Task<Result<List<HarborPhotoDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var harbor = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.Id.Equals(request.Id))
                    .Include(x => x.HarborPhotos)
                    .ThenInclude(x => x.Photo)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (harbor == null)
                {
                    return Result<List<HarborPhotoDto>>.Failure("Fail, harbor does not exist");
                }
                
                var files = _mapper.Map<List<HarborPhotoDto>>(harbor.HarborPhotos);

                return Result<List<HarborPhotoDto>>.Success(files);
            }
        }
    }
}