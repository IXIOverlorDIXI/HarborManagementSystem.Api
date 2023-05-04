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

namespace Application.Berths.Photos
{
    public class BerthPhotoUpload
    {
        public class Command : IRequest<Result<BerthPhotoDataDto>>
        {
            public BerthPhotoDataDto File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BerthPhotoDataDto>>
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

            public async Task<Result<BerthPhotoDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var berth = await _context.Berths
                    .Where(x =>
                        !x.IsDeleted
                        && x.Id.Equals(request.File.BerthId))
                    .Include(x => x.BerthPhotos)
                    .FirstOrDefaultAsync(cancellationToken);

                if (berth == null)
                {
                    return Result<BerthPhotoDataDto>.Failure("Fail, berth does not exist.");
                }

                var id = Guid.NewGuid();

                var blobUrl = await _blobManagerService.UploadToBlobStorageAsync(
                    id,
                    request.File.FileNameWithExtension,
                    new MemoryStream(request.File.FileStream),
                    BlobType.BerthPhotos);

                if (string.IsNullOrEmpty(blobUrl))
                {
                    return Result<BerthPhotoDataDto>.Failure("Failed to upload photo onto blob.");
                }

                var berthPhoto = new BerthPhoto()
                {
                    Id = Guid.NewGuid(),
                    BerthId = berth.Id,
                    PhotoId = id
                };

                var file = new File
                {
                    Id = id,
                    Url = blobUrl,
                };

                await _context.Files.AddAsync(file, cancellationToken);
                await _context.BerthPhotos.AddAsync(berthPhoto, cancellationToken);

                bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BerthPhotoDataDto>.Failure("Failed to save photo into database.");
                }

                return Result<BerthPhotoDataDto>.Success(request.File);
            }
        }
    }
}