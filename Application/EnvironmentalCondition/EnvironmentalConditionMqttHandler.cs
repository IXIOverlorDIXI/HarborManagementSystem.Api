using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Persistence;

namespace Application.EnvironmentalCondition
{
    public class EnvironmentalConditionMqttHandler : IMqttHandler
    {
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;

        public EnvironmentalConditionMqttHandler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
        {
            _mapper = mapper;
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<KeyValuePair<bool, string>> MqttAddHandle(string payload)
        {
            EnvironmentalConditionDto EnvironmentalConditionFromMessage = null;
            try
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.MissingMemberHandling = MissingMemberHandling.Error;

                EnvironmentalConditionFromMessage = JsonConvert
                    .DeserializeObject<EnvironmentalConditionDto>(payload, serializerSettings);
            }
            catch (JsonSerializationException e)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }

            if (EnvironmentalConditionFromMessage == null)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }

            Enum.TryParse<ShipRelativeWindDirection>(
                EnvironmentalConditionFromMessage.ShipRelativeWindDirection.ToString(),
                out ShipRelativeWindDirection enumResult);

            if (!Enum.IsDefined(typeof(ShipRelativeWindDirection), enumResult))
            {
                return new KeyValuePair<bool, string>(false,
                    "Fail, the environmental condition has wrong wind direction.");
            }

            if (!_context.Berths.Any(x =>
                    !x.IsDeleted
                    && x.Id.Equals(EnvironmentalConditionFromMessage.BerthId)))
            {
                return new KeyValuePair<bool, string>(false, "Fail, the berth does not exist.");
            }

            bool result;

            if (!_context.Bookings.Any(x =>
                    x.Berth.Id.Equals(EnvironmentalConditionFromMessage.BerthId)
                    && x.BookingCheck != null
                    && x.EndDate >= DateTime.Now
                    && x.StartDate <= DateTime.Now))
            {
                var meteringsToDelete = await _context.EnvironmentalConditions
                    .Where(x => x.BerthId.Equals(EnvironmentalConditionFromMessage.BerthId))
                    .ToListAsync();

                if (meteringsToDelete.Any())
                {
                    _context.EnvironmentalConditions.RemoveRange(meteringsToDelete);

                    result = await _context.SaveChangesAsync() > 0;

                    if (!result)
                    {
                        return new KeyValuePair<bool, string>(false,
                            "Failed to remove the environmental condition old data.");
                    }
                }

                return new KeyValuePair<bool, string>(false, "No need to save data, there's no ship in the berth.");
            }

            var environmentalCondition =
                _mapper.Map<Domain.Entities.EnvironmentalCondition>(EnvironmentalConditionFromMessage);

            environmentalCondition.Id = Guid.NewGuid();

            await _context.EnvironmentalConditions.AddAsync(environmentalCondition);

            result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return new KeyValuePair<bool, string>(false, "Failed to create the environmental condition metering.");
            }

            return new KeyValuePair<bool, string>(true, "Successful create the relative position metering.");
        }
    }
}