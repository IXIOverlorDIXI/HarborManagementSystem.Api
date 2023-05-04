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

namespace Application.Berths
{
    public class BerthDelete
    {
        public class Command : IRequest<Result<BerthDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<BerthDataDto>>
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

            public async Task<Result<BerthDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var berths = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!berths.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<BerthDataDto>.Failure("This berth does not exists.");
                }

                var berth = await _context.Berths
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.BerthPhotos)
                    .ThenInclude(x => x.Photo)
                    .Include(x => x.SuitableShipTypes)
                    .Include(x => x.Reviews)
                    .Include(x => x.RelativePositionMeterings)
                    .Include(x => x.EnvironmentalConditions)
                    .Include(x => x.StorageEnvironmentalConditions)
                    .Include(x => x.Harbor)
                    .ThenInclude(x => x.Owner)
                    .Include(x => x.Bookings)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);

                if (!berth.Harbor.Owner.UserName.Equals(_userAccessor.GetUsername()))
                {
                    return Result<BerthDataDto>.Failure("Wrong user.");
                }

                if (berth.Bookings.Any(x => x.EndDate < DateTime.Now))
                {
                    return Result<BerthDataDto>.Failure("Fail, this berth is still in use.");
                }

                berth.IsDeleted = true;
                berth.IsActive = false;

                foreach (var photo in berth.BerthPhotos)
                {
                    var blobRemoveSuccess = await _blobManagerService
                        .RemoveFromBlobStorageAsync(photo.Photo.Url);

                    if (!blobRemoveSuccess)
                    {
                        return Result<BerthDataDto>.Failure("Failed to delete photo from blob.");
                    }
                }

                _context.BerthPhotos.RemoveRange(berth.BerthPhotos);
                _context.SuitableShipTypes.RemoveRange(berth.SuitableShipTypes);
                _context.EnvironmentalConditions.RemoveRange(berth.EnvironmentalConditions);
                _context.StorageEnvironmentalConditions.RemoveRange(berth.StorageEnvironmentalConditions);
                _context.RelativePositionMeterings.RemoveRange(berth.RelativePositionMeterings);
                _context.Reviews.RemoveRange(berth.Reviews);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<BerthDataDto>.Failure("Failed to remove berth from database.");
                }

                return Result<BerthDataDto>.Success(_mapper.Map<BerthDataDto>(berth));
            }
        }
    }
}