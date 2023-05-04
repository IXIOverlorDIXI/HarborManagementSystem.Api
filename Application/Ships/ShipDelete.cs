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

namespace Application.Ships
{
    public class ShipDelete
    {
        public class Command : IRequest<Result<ShipDataDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ShipDataDto>>
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

            public async Task<Result<ShipDataDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var ships = await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (!ships.Any(x => x.Id.Equals(request.Id)))
                {
                    return Result<ShipDataDto>.Failure("This ship does not exists.");
                }

                var ship= await _context.Ships
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Photo)
                    .FirstOrDefaultAsync(
                        x => x.Id.Equals(request.Id),
                        cancellationToken);

                ship.IsDeleted = true;
                //_context.Ships.Remove(ship);

                if (ship.Photo != null)
                {
                    var blobRemoveSuccess = await _blobManagerService
                        .RemoveFromBlobStorageAsync(ship.Photo.Url);

                    if (!blobRemoveSuccess)
                    {
                        return Result<ShipDataDto>.Failure("Failed to delete photo from blob.");
                    }

                    _context.Files.Remove(ship.Photo);
                }
            
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ShipDataDto>.Failure("Failed to remove ship from database.");
                }

                return Result<ShipDataDto>.Success(_mapper.Map<ShipDataDto>(ship));
            }
        }
    }
}