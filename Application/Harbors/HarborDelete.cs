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

namespace Application.Harbors
{
    public class HarborDelete
    {
        public class Command : IRequest<Result<HarborDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<HarborDataDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IBlobManagerService _blobManagerService;

            public Handler(IMapper mapper,
                IUserAccessor userAccessor,
                DataContext context, IBlobManagerService blobManagerService)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
                _blobManagerService = blobManagerService;
            }

            public async Task<Result<HarborDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var harbors = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!harbors.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<HarborDataDto>.Failure("This harbor does not exists.");
                }

                var harbor= await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.HarborPhotos)
                    .ThenInclude(x => x.Photo)
                    .Include(x => x.HarborDocuments)
                    .ThenInclude(x => x.Document)
                    .Include(x => x.Owner)
                    .Include(x => x.Berths)
                    .ThenInclude(x => x.BerthPhotos)
                    .ThenInclude(x => x.Photo)
                    .Include(x => x.Berths)
                    .ThenInclude(x => x.SuitableShipTypes)
                    .Include(x => x.Berths)
                    .ThenInclude(x => x.Bookings)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);
                
                if (!harbor.Owner.UserName.Equals(_userAccessor.GetUsername()))
                {
                    return Result<HarborDataDto>.Failure("Wrong user.");
                }

                if (harbor.Berths
                    .Any(x => x.Bookings
                        .Any(y => y.EndDate < DateTime.Now)))
                {
                    return Result<HarborDataDto>.Failure("Fail, this harbor is still in use.");
                }
                
                harbor.IsDeleted = true;

                foreach (var photo in harbor.HarborPhotos)
                {
                    var blobRemoveSuccess = await _blobManagerService
                        .RemoveFromBlobStorageAsync(photo.Photo.Url);

                    if (!blobRemoveSuccess)
                    {
                        return Result<HarborDataDto>.Failure("Failed to delete photo from blob.");
                    }
                }

                _context.HarborPhotos.RemoveRange(harbor.HarborPhotos);
                
                foreach (var document in harbor.HarborDocuments)
                {
                    document.IsDeleted = true;
                    document.DateOfDelete = DateTime.Now;
                }
                
                foreach (var berth in harbor.Berths)
                {
                    berth.IsDeleted = true;
                    berth.IsActive = false;
                    
                    foreach (var photo in berth.BerthPhotos)
                    {
                        var blobRemoveSuccess = await _blobManagerService
                            .RemoveFromBlobStorageAsync(photo.Photo.Url);

                        if (!blobRemoveSuccess)
                        {
                            return Result<HarborDataDto>.Failure("Failed to delete photo from blob.");
                        }
                    }
                    
                    _context.SuitableShipTypes.RemoveRange(berth.SuitableShipTypes);
                    _context.BerthPhotos.RemoveRange(berth.BerthPhotos);
                }

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<HarborDataDto>.Failure("Failed to remove harbor from database.");
                }

                return Result<HarborDataDto>.Success(_mapper.Map<HarborDataDto>(harbor));
            }
        }
    }
}