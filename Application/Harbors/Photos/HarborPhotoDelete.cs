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

namespace Application.Harbors.Photos
{
    public class HarborPhotoDelete
    {
        public class Command : IRequest<Result<FileDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<FileDto>>
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

            public async Task<Result<FileDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var harborPhoto = await _context.HarborPhotos
                    .Include(x => x.Photo)
                    .Where(x => x.Id.Equals(request.Id))
                    .FirstOrDefaultAsync(cancellationToken);

                if (harborPhoto == null)
                {
                    return Result<FileDto>.Failure("Fail, photo does not exist.");
                }

                var blobRemoveSuccess = await _blobManagerService
                    .RemoveFromBlobStorageAsync(harborPhoto.Photo.Url);

                if (!blobRemoveSuccess)
                {
                    return Result<FileDto>.Failure("Failed to delete photo from blob.");
                }
                
                var fileDto = new FileDto
                {
                    Url = harborPhoto.Photo.Url
                };

                _context.Files.Remove(harborPhoto.Photo);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<FileDto>.Failure("Failed to remove photo from database.");
                }

                return Result<FileDto>.Success(fileDto);
            }
        }
    }
}