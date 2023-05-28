using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using File = Domain.Entities.File;

namespace Application.Harbors.Documents
{
    public class HarborDocumentUpload
    {
        public class Command : IRequest<Result<HarborDocumentDto>>
        {
            public HarborDocumentDataDto File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<HarborDocumentDto>>
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

            public async Task<Result<HarborDocumentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var harbor = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.HarborDocuments)
                    .Where(x => x.Id.Equals(request.File.HarborId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (harbor == null)
                {
                    return Result<HarborDocumentDto>.Failure("Fail, harbor does not exist.");
                }
                
                var id = Guid.NewGuid();
            
                var blobUrl = await _blobManagerService.UploadToBlobStorageAsync(
                    id,
                    request.File.FileNameWithExtension,
                    new MemoryStream(request.File.FileStream),
                    BlobType.HarborDocuments);

                if (string.IsNullOrEmpty(blobUrl))
                {
                    return Result<HarborDocumentDto>.Failure("Failed to upload document onto blob.");
                }
                
                var harborDocument = new HarborDocument()
                {
                    Id = Guid.NewGuid(),
                    HarborId = harbor.Id,
                    DocumentId = id,
                    DateOfUpload = DateTime.Now
                };
                
                var file = new File
                {
                    Id = id,
                    Url = blobUrl,
                };

                await _context.Files.AddAsync(file, cancellationToken);
                await _context.HarborDocuments.AddAsync(harborDocument, cancellationToken);

                bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<HarborDocumentDto>.Failure("Failed to save document into database.");
                }

                return Result<HarborDocumentDto>.Success(new HarborDocumentDto{DocumentId = id, Url = blobUrl});
            }
        }
    }
}