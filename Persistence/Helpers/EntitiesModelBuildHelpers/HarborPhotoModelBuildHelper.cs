using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void HarborPhotoEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<HarborPhoto>().HasKey(x => x.Id);
            builder.Entity<HarborPhoto>()
                .HasOne(x => x.Harbor)
                .WithMany(x => x.HarborPhotos)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<HarborPhoto>()
                .HasOne(x => x.Photo)
                .WithOne(x => x.HarborPhoto)
                .HasForeignKey<HarborPhoto>(x => x.PhotoId);
        }
    }
}