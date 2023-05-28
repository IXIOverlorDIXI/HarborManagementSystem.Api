using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using File = Domain.Entities.File;

namespace Application.Harbors.Photos
{
    public class HarborPhotoUpload
    {
        public class Command : IRequest<Result<HarborPhotoDto>>
        {
            public HarborPhotoDataDto File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<HarborPhotoDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IBlobManagerService _blobManagerService;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context,
                IBlobManagerService blobManagerService)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
                _blobManagerService = blobManagerService;
            }

            public async Task<Result<HarborPhotoDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var harbor = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.HarborPhotos)
                    .Where(x => x.Id.Equals(request.File.HarborId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (harbor == null)
                {
                    return Result<HarborPhotoDto>.Failure("Fail, harbor does not exist.");
                }
                
                var id = Guid.NewGuid();
            
                var blobUrl = await _blobManagerService.UploadToBlobStorageAsync(
                    id,
                    request.File.FileNameWithExtension,
                    new MemoryStream(request.File.FileStream),
                    BlobType.HarborPhotos);

                if (string.IsNullOrEmpty(blobUrl))
                {
                    return Result<HarborPhotoDto>.Failure("Failed to upload photo onto blob.");
                }
                
                var harborPhoto = new HarborPhoto
                {
                    Id = Guid.NewGuid(),
                    HarborId = harbor.Id,
                    PhotoId = id
                };
                
                var file = new File
                {
                    Id = id,
                    Url = blobUrl,
                };

                await _context.Files.AddAsync(file, cancellationToken);
                await _context.HarborPhotos.AddAsync(harborPhoto, cancellationToken);

                bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<HarborPhotoDto>.Failure("Failed to save photo into database.");
                }

                return Result<HarborPhotoDto>.Success(new HarborPhotoDto{PhotoId = id, Url = blobUrl});
            }
        }
    }
}