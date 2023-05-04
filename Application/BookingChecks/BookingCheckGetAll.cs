using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.BookingChecks
{
    public class BookingCheckGetAll
    {
        public class Query : IRequest<Result<List<BookingCheckDataDto>>>
        {
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BookingCheckDataDto>>>
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

            public async Task<Result<List<BookingCheckDataDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!request.UserName.Equals(_userAccessor.GetUsername()))
                {
                    return Result<List<BookingCheckDataDto>>.Failure("Wrong username.");
                }
                
                var user = await _context.Users
                    .Where(x => x.UserName.Equals(request.UserName))
                    .Include(x => x.Ships)
                    .ThenInclude(x => x.Bookings)
                    .ThenInclude(x => x.BookingCheck)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null)
                {
                    return Result<List<BookingCheckDataDto>>.Failure("User not found.");
                }

                var bookingChecks = user.Ships
                    .Where(x => !x.IsDeleted)
                    .SelectMany(x => x.Bookings.Select(x => x.BookingCheck))
                    .OrderByDescending(x => x.Date)
                    .ToList();

                return Result<List<BookingCheckDataDto>>
                    .Success(_mapper.Map<List<BookingCheckDataDto>>(bookingChecks));
            }
        }
    }
}