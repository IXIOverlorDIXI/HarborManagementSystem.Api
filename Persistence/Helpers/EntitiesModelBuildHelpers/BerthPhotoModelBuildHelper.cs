using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void BerthPhotoEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<BerthPhoto>().HasKey(x => x.Id);
            builder.Entity<BerthPhoto>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.BerthPhotos)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<BerthPhoto>()
                .HasOne(x => x.Photo)
                .WithOne(x => x.BerthPhoto)
                .HasForeignKey<BerthPhoto>(x => x.PhotoId);
        }
    }
}