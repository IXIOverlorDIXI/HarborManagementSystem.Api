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

namespace Application.Profiles.Settings
{
    public class ProfileSettingsGet
    {
        public class Query : IRequest<Result<SettingsDto>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<SettingsDto>>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;

            public Handler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<SettingsDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<SettingsDto>.Failure("Wrong username.");
                }
                
                var currentUser = await _context.Users
                    .Where(user => user.UserName.Equals(_userAccessor.GetUsername()))
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentUser == null)
                {
                    return Result<SettingsDto>.Failure("User not found.");
                }

                var settingsDto = _mapper.Map<SettingsDto>(currentUser);

                return Result<SettingsDto>.Success(settingsDto);
            }
        }
    }
}