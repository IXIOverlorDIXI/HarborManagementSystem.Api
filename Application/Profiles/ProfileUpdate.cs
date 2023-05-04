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

namespace Application.Profiles
{
    public class ProfileUpdate
    {
        public class Command : IRequest<Result<ProfileDto>>
        {
            public ProfileDto Profile { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<ProfileDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(IMapper mapper, DataContext context, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<ProfileDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!request.Profile.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<ProfileDto>.Failure("Wrong username.");
                }

                if (await _context.Users
                        .CountAsync(x => x.UserName
                            .Equals(request.Profile.Username), cancellationToken: cancellationToken) > 1)
                {
                    return Result<ProfileDto>.Failure("Username already taken.");
                }
                
                var currentUser = await _context.Users
                    .Where(user => user.UserName == request.Profile.Username)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser == null)
                {
                    return Result<ProfileDto>.Failure("User not found.");
                }

                _mapper.Map(request.Profile, currentUser);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<ProfileDto>.Failure("Failed to update profile in database.");
                }

                var response = _mapper.Map<ProfileDto>(currentUser);

                return Result<ProfileDto>.Success(response);
            }
        }
    }
}