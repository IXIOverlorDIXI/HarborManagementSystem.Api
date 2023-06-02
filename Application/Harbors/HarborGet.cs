using System;
using System.Collections.Generic;
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
using Persistence;

namespace Application.Harbors;

public class HarborGet
{
    public class Query : IRequest<Result<HarborPreviewDataDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<HarborPreviewDataDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;
            

        public Handler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
        {
            _mapper = mapper;
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<Result<HarborPreviewDataDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var harbor = await _context.Harbors
                .Include(x => x.Owner)
                .Where(x => !x.IsDeleted)
                .ProjectTo<HarborPreviewDataDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id.Equals(request.Id),cancellationToken);

            var reviews = await _context.Reviews
                .Where(x => x.Berth.HarborId.Equals(harbor.Id))
                .ToListAsync(cancellationToken);
            harbor.AverageRate = !reviews.Any() ? 0 : reviews.Average(x => x.ReviewMark);
            harbor.ReviewsAmount = reviews.Count;
            try
            {
                harbor.IsOwner = _userAccessor.GetUsername().Equals(harbor.OwnerUserName);
            }
            catch (Exception e)
            {
                harbor.IsOwner = false;
            }
            
            if (!harbor.Photos.Any())
            {
                harbor.Photos = new List<string>
                {
                    DefaultFileLinks.DefaultImage
                };
            }

            return Result<HarborPreviewDataDto>.Success(harbor);
        }
    }
}