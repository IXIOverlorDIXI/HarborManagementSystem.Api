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

namespace Application.Profiles.Photos
{
    public class ProfilePhotoUpload
    {
        public class Command : IRequest<Result<ProfilePhotoDataDto>>
        {
            public ProfilePhotoDataDto ProfilePhoto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ProfilePhotoDataDto>>
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

            public async Task<Result<ProfilePhotoDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await _context.Users
                    .Include(user => user.Photo)
                    .Where(user => user.UserName == _userAccessor.GetUsername())
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser == null)
                {
                    return Result<ProfilePhotoDataDto>.Failure("Fail, user does not exist.");
                }
                
                bool result;
            
                if (currentUser.Photo != null)
                {
                    var blobRemoveSuccess = await _blobManagerService
                        .RemoveFromBlobStorageAsync(currentUser.Photo.Url);

                    if (!blobRemoveSuccess)
                    {
                        return Result<ProfilePhotoDataDto>.Failure("Failed to delete photo from blob.");
                    }

                    _context.Files.Remove(currentUser.Photo);

                    result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        return Result<ProfilePhotoDataDto>.Failure("Failed to remove photo from database.");
                    }
                }
            
                var id = Guid.NewGuid();
            
                var blobUrl = await _blobManagerService.UploadToBlobStorageAsync(
                    id,
                    request.ProfilePhoto.FileNameWithExtension,
                    new MemoryStream(request.ProfilePhoto.FileStream),
                    BlobType.UserPhotos);

                if (string.IsNullOrEmpty(blobUrl))
                {
                    return Result<ProfilePhotoDataDto>.Failure("Failed to upload photo onto blob.");
                }

                var file = new File
                {
                    Id = id,
                    Url = blobUrl,
                    AppUser = currentUser
                };

                await _context.Files.AddAsync(file, cancellationToken);

                result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ProfilePhotoDataDto>.Failure("Failed to save photo into database.");
                }

                return Result<ProfilePhotoDataDto>.Success(request.ProfilePhoto);
            }
        }
    }
}