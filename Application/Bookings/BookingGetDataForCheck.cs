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
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Bookings
{
    public class BookingGetDataForCheck
    {
        public class Query : IRequest<Result<BookingDataForCheckDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query,Result<BookingDataForCheckDto>>
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

            public async Task<Result<BookingDataForCheckDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                if ((await _context.Bookings
                        .Include(x => x.BookingCheck)
                        .FirstOrDefaultAsync(x => x.Id
                            .Equals(request.Id), cancellationToken)).BookingCheck != null)
                {
                    return Result<BookingDataForCheckDto>.Failure("Fail, booking has already been payed.");
                }
                
                var booking = await _context.Bookings
                    .Where(x => x.Id.Equals(request.Id))
                    .ProjectTo<BookingDataForCheckDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (booking == null)
                {
                    return Result<BookingDataForCheckDto>.Failure("Fail, booking does not exist.");
                }

                return Result<BookingDataForCheckDto>
                    .Success(_mapper.Map<BookingDataForCheckDto>(booking));
            }
        }
    }
}