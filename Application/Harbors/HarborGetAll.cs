﻿using System;
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

namespace Application.Harbors
{
    public class HarborGetAll
    {
        public class Query : IRequest<Result<List<HarborPreviewDataDto>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<HarborPreviewDataDto>>>
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

            public async Task<Result<List<HarborPreviewDataDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var harbors = await _context.Harbors
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Owner)
                    .ProjectTo<HarborPreviewDataDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                
                foreach (var harbor in harbors)
                {
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
                }

                return Result<List<HarborPreviewDataDto>>.Success(harbors);
            }
        }
    }
}