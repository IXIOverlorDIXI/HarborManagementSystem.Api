using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Consts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;

namespace Application.Profiles.Photos
{
    public class ProfilePhotoGet
    {
        public class Query : IRequest<Result<FileDto>>
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<FileDto>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IConfiguration _config;

            public Handler(IMapper mapper, 
                DataContext context, 
                IConfiguration config, 
                IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _context = context;
                _config = config;
                _userAccessor = userAccessor;
            }

            public async Task<Result<FileDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.Username.Equals(_userAccessor.GetUsername()))
                {
                    return Result<FileDto>.Failure("Wrong username.");
                }
                
                var file = await _context.Files
                    .Where(file => file.AppUser.UserName == request.Username)
                    .ProjectTo<FileDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (file == null)
                {
                    return Result<FileDto>.Success(new FileDto
                    {
                        Url = DefaultFileLinks.DefaultUserPhoto
                    });
                }
                
                return Result<FileDto>.Success(file);
            }
        }
    }
}