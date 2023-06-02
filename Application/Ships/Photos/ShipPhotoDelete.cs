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
using Persistence;

namespace Application.Ships.Photos
{
    public class ShipPhotoDelete
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
                var ship = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Photo)
                    .Where(x => x.Id.Equals(request.Id))
                    .FirstOrDefaultAsync(cancellationToken);

                if (ship == null)
                {
                    return Result<FileDto>.Failure("Fail, ship does not exist.");
                }

                if (ship.Photo == null)
                {
                    return Result<FileDto>.Failure("Fail, this ship has not any photo.");
                }

                var blobRemoveSuccess = await _blobManagerService
                    .RemoveFromBlobStorageAsync(ship.Photo.Url);

                if (!blobRemoveSuccess)
                {
                    return Result<FileDto>.Failure("Failed to delete photo from blob.");
                }

                _context.Files.Remove(ship.Photo);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<FileDto>.Failure("Failed to remove photo from database.");
                }

                var fileDto = new FileDto
                {
                    Url = DefaultFileLinks.DefaultImage
                };

                return Result<FileDto>.Success(fileDto);
            }
        }
    }
}