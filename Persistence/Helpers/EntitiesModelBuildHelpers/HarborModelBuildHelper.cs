using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void HarborEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Harbor>().HasKey(x => x.Id);
            builder.Entity<Harbor>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Harbors)
                .HasForeignKey(x => x.OwnerId);
            builder.Entity<Harbor>()
                .HasMany(x => x.HarborDocuments)
                .WithOne(x => x.Harbor)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<Harbor>()
                .HasMany(x => x.HarborPhotos)
                .WithOne(x => x.Harbor)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<Harbor>()
                .HasMany(x => x.Services)
                .WithOne(x => x.Harbor)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<Harbor>()
                .HasMany(x => x.Berths)
                .WithOne(x => x.Harbor)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<Harbor>()
                .Property(x => x.Geolocation)
                .HasColumnType("geometry")
                .HasAnnotation("SRID", 4326);
        }
    }
}