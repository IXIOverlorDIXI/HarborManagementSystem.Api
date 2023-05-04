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
using Persistence;

namespace Application.Profiles.Photos;

public class ProfilePhotoDelete
{
    public class Command : IRequest<Result<FileDto>>
    {
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
            var currentUser = await _context.Users
                .Include(x => x.Photo)
                .Where(user => user.UserName == _userAccessor.GetUsername())
                .FirstOrDefaultAsync(cancellationToken);

            if (currentUser == null)
            {
                return Result<FileDto>.Failure("Fail, user does not exist.");
            }
            
            if (currentUser.Photo == null)
            {
                return Result<FileDto>.Failure("Fail, this profile has not any photo.");
            }
            
            var blobRemoveSuccess = await _blobManagerService
                .RemoveFromBlobStorageAsync(currentUser.Photo.Url);

            if (!blobRemoveSuccess)
            {
                return Result<FileDto>.Failure("Failed to delete photo from blob.");
            }

            _context.Files.Remove(currentUser.Photo);
            
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                return Result<FileDto>.Failure("Failed to remove photo from database.");
            }

            var fileDto = new FileDto
            {
                Url = DefaultFileLinks.DefaultUserPhoto
            };
            
            return Result<FileDto>.Success(fileDto);
        }
    }
}