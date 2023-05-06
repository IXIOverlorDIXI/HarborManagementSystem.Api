using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence;

namespace Application.RelativePositionMetering
{
    public class RelativePositionMeteringMqttHandler : IMqttHandler
    {
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;

        public RelativePositionMeteringMqttHandler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
        {
            _mapper = mapper;
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<KeyValuePair<bool, string>> MqttAddHandle(string payload)
        {
            RelativePositionMeteringDto relativePositionMeteringFromMessage = null;
            try
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.MissingMemberHandling = MissingMemberHandling.Error;

                relativePositionMeteringFromMessage = JsonConvert
                    .DeserializeObject<RelativePositionMeteringDto>(payload, serializerSettings);
            }
            catch (JsonSerializationException e)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }

            if (relativePositionMeteringFromMessage == null)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }


            if (!_context.Berths.Any(x =>
                    !x.IsDeleted
                    && x.Id.Equals(relativePositionMeteringFromMessage.BerthId)))
            {
                return new KeyValuePair<bool, string>(false, "Fail, the berth does not exist.");
            }

            bool result;

            if (!_context.Bookings.Any(x =>
                    x.Berth.Id.Equals(relativePositionMeteringFromMessage.BerthId)
                    && x.BookingCheck != null
                    && x.EndDate >= DateTime.Now
                    && x.StartDate <= DateTime.Now))
            {
                var meteringsToDelete = await _context.RelativePositionMeterings
                    .Where(x => x.BerthId.Equals(relativePositionMeteringFromMessage.BerthId))
                    .ToListAsync();

                if (meteringsToDelete.Any())
                {
                    _context.RelativePositionMeterings.RemoveRange(meteringsToDelete);

                    result = await _context.SaveChangesAsync() > 0;

                    if (!result)
                    {
                        return new KeyValuePair<bool, string>(false,
                            "Failed to remove the relative position metering old data.");
                    }
                }

                return new KeyValuePair<bool, string>(false, "No need to save data, there's no ship in the berth.");
            }

            var relativePositionMetering =
                _mapper.Map<Domain.Entities.RelativePositionMetering>(relativePositionMeteringFromMessage);

            relativePositionMetering.Id = Guid.NewGuid();

            await _context.RelativePositionMeterings.AddAsync(relativePositionMetering);

            result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return new KeyValuePair<bool, string>(false, "Failed to create the relative position metering.");
            }

            return new KeyValuePair<bool, string>(true, "Successful create the relative position metering.");
        }
    }
}