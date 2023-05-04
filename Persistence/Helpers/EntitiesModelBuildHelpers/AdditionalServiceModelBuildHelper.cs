using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void AdditionalServiceEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<AdditionalService>().HasKey(x => x.Id);
            builder.Entity<AdditionalService>()
                .HasOne(x => x.Service)
                .WithMany(x => x.AdditionalServices)
                .HasForeignKey(x => x.ServiceId);
            builder.Entity<AdditionalService>()
                .HasOne(x => x.Booking)
                .WithMany(x => x.AdditionalServices)
                .HasForeignKey(x => x.BookingId);
        }
    }
}