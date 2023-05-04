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

namespace Application.Harbors.Documents
{
    public class HarborDocumentDelete
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
                var harborDocument = await _context.HarborDocuments
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Document)
                    .Where(x => x.Id.Equals(request.Id))
                    .FirstOrDefaultAsync(cancellationToken);

                if (harborDocument == null)
                {
                    return Result<FileDto>.Failure("Fail, document does not exist.");
                }

                var fileDto = new FileDto
                {
                    Url = harborDocument.Document.Url
                };

                harborDocument.IsDeleted = true;
                harborDocument.DateOfDelete = DateTime.Now;
                
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