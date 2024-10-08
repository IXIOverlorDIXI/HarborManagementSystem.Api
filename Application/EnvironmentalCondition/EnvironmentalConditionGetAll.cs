﻿using System;
using System.Collections.Generic;
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

namespace Application.EnvironmentalCondition
{
    public class EnvironmentalConditionGetAll
    {
        public class Query : IRequest<Result<List<EnvironmentalConditionDto>>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<EnvironmentalConditionDto>>>
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

            public async Task<Result<List<EnvironmentalConditionDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var ship = await _context.Ships
                    .Where(x => x.Id.Equals(request.Id))
                    .Include(x => x.Bookings)
                    .ThenInclude(x => x.Berth)
                    .ThenInclude(x => x.EnvironmentalConditions)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ship == null)
                {
                    return Result<List<EnvironmentalConditionDto>>.Failure("Ship not found.");
                }

                var meterings = ship.Bookings?
                    .Where(x => x.EndDate >= DateTime.Now && x.StartDate <= DateTime.Now)
                    .SelectMany(x => x.Berth?.EnvironmentalConditions
                        .Where(y => y.MeteringDate <= x.EndDate && y.MeteringDate >= x.StartDate))
                    .OrderBy(x => x.MeteringDate)
                    .ToList();

                return Result<List<EnvironmentalConditionDto>>
                    .Success(_mapper.Map<List<EnvironmentalConditionDto>>(meterings));
            }
        }
    }
}