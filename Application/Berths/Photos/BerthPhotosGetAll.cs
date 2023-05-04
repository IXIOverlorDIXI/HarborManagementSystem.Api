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

namespace Application.Berths.Photos
{
    public class BerthPhotosGetAll
    {
        public class Query : IRequest<Result<List<BerthPhotoDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BerthPhotoDto>>>
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

            public async Task<Result<List<BerthPhotoDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var berth = await _context.Berths
                    .Where(x =>
                        !x.IsDeleted
                        && x.Id.Equals(request.Id))
                    .Include(x => x.BerthPhotos)
                    .ThenInclude(x => x.Photo)
                    .FirstOrDefaultAsync(cancellationToken);

                if (berth == null)
                {
                    return Result<List<BerthPhotoDto>>.Failure("Fail, berth does not exist");
                }

                var files = _mapper.Map<List<BerthPhotoDto>>(berth.BerthPhotos);

                return Result<List<BerthPhotoDto>>.Success(files);
            }
        }
    }
}