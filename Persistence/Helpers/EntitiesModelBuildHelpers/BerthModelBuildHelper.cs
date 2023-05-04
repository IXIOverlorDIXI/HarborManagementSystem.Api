using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void BerthEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Berth>().HasKey(x => x.Id);
            builder.Entity<Berth>()
                .HasMany(x => x.BerthPhotos)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.Reviews)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.EnvironmentalConditions)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.RelativePositionMeterings)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.StorageEnvironmentalConditions)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.Bookings)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasMany(x => x.SuitableShipTypes)
                .WithOne(x => x.Berth)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<Berth>()
                .HasOne(x => x.Harbor)
                .WithMany(x => x.Berths)
                .HasForeignKey(x => x.HarborId);
        }
    }
}