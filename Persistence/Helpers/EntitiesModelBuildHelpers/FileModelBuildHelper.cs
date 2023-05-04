using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void FileEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Domain.Entities.File>().HasKey(x => x.Id);
            builder.Entity<Domain.Entities.File>()
                .HasOne(x => x.AppUser)
                .WithOne(x => x.Photo)
                .HasForeignKey<AppUser>(x => x.PhotoId);
            builder.Entity<Domain.Entities.File>()
                .HasOne(x => x.Ship)
                .WithOne(x => x.Photo)
                .HasForeignKey<Ship>(x => x.PhotoId);
            builder.Entity<Domain.Entities.File>()
                .HasOne(x => x.HarborDocument)
                .WithOne(x => x.Document)
                .HasForeignKey<HarborDocument>(x => x.DocumentId);
            builder.Entity<Domain.Entities.File>()
                .HasOne(x => x.HarborPhoto)
                .WithOne(x => x.Photo)
                .HasForeignKey<HarborPhoto>(x => x.PhotoId);
            builder.Entity<Domain.Entities.File>()
                .HasOne(x => x.BerthPhoto)
                .WithOne(x => x.Photo)
                .HasForeignKey<BerthPhoto>(x => x.PhotoId);
        }
    }
}