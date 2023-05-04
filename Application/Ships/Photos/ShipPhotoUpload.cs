using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using File = Domain.Entities.File;

namespace Application.Ships.Photos
{
    public class ShipPhotoUpload
    {
        public class Command : IRequest<Result<ShipPhotoDataDto>>
        {
            public ShipPhotoDataDto File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ShipPhotoDataDto>>
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

            public async Task<Result<ShipPhotoDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ship = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Photo)
                    .Where(x => x.Id.Equals(request.File.ShipId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (ship == null)
                {
                    return Result<ShipPhotoDataDto>.Failure("Fail, ship does not exist.");
                }
                
                bool result;
            
                if (ship.Photo != null)
                {
                    var blobRemoveSuccess = await _blobManagerService
                        .RemoveFromBlobStorageAsync(ship.Photo.Url);

                    if (!blobRemoveSuccess)
                    {
                        return Result<ShipPhotoDataDto>.Failure("Failed to delete photo from blob.");
                    }

                    _context.Files.Remove(ship.Photo);

                    result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<ShipPhotoDataDto>.Failure("Failed to remove photo from database.");
                    }
                }
            
                var id = Guid.NewGuid();
            
                var blobUrl = await _blobManagerService.UploadToBlobStorageAsync(
                    id,
                    request.File.FileNameWithExtension,
                    new MemoryStream(request.File.FileStream),
                    BlobType.ShipPhotos);

                if (string.IsNullOrEmpty(blobUrl))
                {
                    return Result<ShipPhotoDataDto>.Failure("Failed to upload photo onto blob.");
                }

                //ship.PhotoId = id;
                
                var file = new File
                {
                    Id = id,
                    Url = blobUrl,
                    Ship = ship
                };

                await _context.Files.AddAsync(file, cancellationToken);

                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipPhotoDataDto>.Failure("Failed to save photo into database.");
                }

                return Result<ShipPhotoDataDto>.Success(request.File);
            }
        }
    }
}