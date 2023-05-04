using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void ShipEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Ship>().HasKey(x => x.Id);
            builder.Entity<Ship>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Ships)
                .HasForeignKey(x=>x.OwnerId);
            builder.Entity<Ship>()
                .HasOne(x => x.ShipType)
                .WithMany(x => x.Ships)
                .HasForeignKey(x=>x.ShipTypeId);
            builder.Entity<Ship>()
                .HasOne(x => x.Photo)
                .WithOne(x => x.Ship)
                .HasForeignKey<Ship>(x=>x.PhotoId);
            builder.Entity<Ship>()
                .HasMany(x => x.Bookings)
                .WithOne(x => x.Ship)
                .HasForeignKey(x => x.ShipId);
            builder.Entity<Ship>().Property(x => x.PhotoId).IsRequired(false);
        }
    }
}