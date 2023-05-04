using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void BookingEntityBuilder(ModelBuilder builder)
        {
                builder.Entity<Booking>().HasKey(x => x.Id);
                builder.Entity<Booking>()
                    .HasMany(x => x.AdditionalServices)
                    .WithOne(x => x.Booking)
                    .HasForeignKey(x => x.BookingId);
                builder.Entity<Booking>()
                    .HasOne(x => x.Ship)
                    .WithMany(x => x.Bookings)
                    .HasForeignKey(x => x.ShipId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.Entity<Booking>()
                    .HasOne(x => x.Berth)
                    .WithMany(x => x.Bookings)
                    .HasForeignKey(x => x.BerthId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.Entity<Booking>()
                    .HasOne(x => x.BookingCheck)
                    .WithOne(x => x.Booking)
                    .HasForeignKey<BookingCheck>(x => x.BookingId)
                    .IsRequired();
                builder.Entity<Booking>().Property(x => x.BerthId).IsRequired(false);
                builder.Entity<Booking>().Property(x => x.ShipId).IsRequired(false);
        }
    }
}