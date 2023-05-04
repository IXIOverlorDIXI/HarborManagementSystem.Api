using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void BookingCheckEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<BookingCheck>().HasKey(x => x.Id);
            builder.Entity<BookingCheck>()
                .HasOne(x => x.Booking)
                .WithOne(x => x.BookingCheck)
                .HasForeignKey<BookingCheck>(x => x.BookingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}