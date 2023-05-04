using System;
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

namespace Application.Profiles.Settings
{
    public class ProfileSettingsUpdate
    {
        public class Command : IRequest<Result<SettingsDto>>
        {
            public SettingsDto Settings { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<SettingsDto>>
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

            public async Task<Result<SettingsDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = await _context.Users
                    .Where(user => user.UserName.Equals(_userAccessor.GetUsername()))
                    .FirstOrDefaultAsync(cancellationToken);
                
                _mapper.Map(request.Settings, currentUser);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<SettingsDto>.Failure("Failed to update settings in database.");
                }

                return Result<SettingsDto>.Success(request.Settings);
            }
        }
    }
}