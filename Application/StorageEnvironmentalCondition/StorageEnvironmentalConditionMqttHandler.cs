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

namespace Application.StorageEnvironmentalCondition
{
    public class StorageEnvironmentalConditionMqttHandler : IMqttHandler
    {
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;

        public StorageEnvironmentalConditionMqttHandler(IMapper mapper, IUserAccessor userAccessor, DataContext context)
        {
            _mapper = mapper;
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<KeyValuePair<bool, string>> MqttAddHandle(string payload)
        {
            StorageEnvironmentalConditionDto StorageEnvironmentalConditionFromMessage = null;
            try
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.MissingMemberHandling = MissingMemberHandling.Error;

                StorageEnvironmentalConditionFromMessage = JsonConvert
                    .DeserializeObject<StorageEnvironmentalConditionDto>(payload, serializerSettings);
            }
            catch (JsonSerializationException e)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }

            if (StorageEnvironmentalConditionFromMessage == null)
            {
                return new KeyValuePair<bool, string>(false, "Wrong json class model.");
            }


            if (!_context.Berths.Any(x =>
                    !x.IsDeleted
                    && x.Id.Equals(StorageEnvironmentalConditionFromMessage.BerthId)))
            {
                return new KeyValuePair<bool, string>(false, "Fail, the berth does not exist.");
            }

            bool result;

            if (!_context.Bookings.Any(x =>
                    x.Berth.Id.Equals(StorageEnvironmentalConditionFromMessage.BerthId)
                    && x.BookingCheck != null
                    && x.EndDate >= DateTime.Now
                    && x.StartDate <= DateTime.Now))
            {
                var meteringsToDelete = await _context.StorageEnvironmentalConditions
                    .Where(x => x.BerthId.Equals(StorageEnvironmentalConditionFromMessage.BerthId))
                    .ToListAsync();

                if (meteringsToDelete.Any())
                {
                    _context.StorageEnvironmentalConditions.RemoveRange(meteringsToDelete);

                    result = await _context.SaveChangesAsync() > 0;

                    if (!result)
                    {
                        return new KeyValuePair<bool, string>(false,
                            "Failed to remove the relative position metering old data.");
                    }
                }

                return new KeyValuePair<bool, string>(false, "No need to save data, there's no ship in the berth.");
            }

            var storageEnvironmentalCondition = _mapper
                .Map<Domain.Entities.StorageEnvironmentalCondition>(StorageEnvironmentalConditionFromMessage);

            storageEnvironmentalCondition.Id = Guid.NewGuid();

            await _context.StorageEnvironmentalConditions.AddAsync(storageEnvironmentalCondition);

            result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return new KeyValuePair<bool, string>(false, "Failed to create the storage environmental condition.");
            }

            return new KeyValuePair<bool, string>(true, "Successful create the storage environmental condition.");
        }
    }
}