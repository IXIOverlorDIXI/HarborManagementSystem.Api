using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void HarborDocumentEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<HarborDocument>().HasKey(x => x.Id);
            builder.Entity<HarborDocument>()
                .HasOne(x => x.Harbor)
                .WithMany(x => x.HarborDocuments)
                .HasForeignKey(x => x.HarborId);
            builder.Entity<HarborDocument>()
                .HasOne(x => x.Document)
                .WithOne(x => x.HarborDocument)
                .HasForeignKey<HarborDocument>(x => x.DocumentId);
            builder.Entity<HarborDocument>()
                .Property(x => x.DateOfDelete)
                .IsRequired(false);
        }
    }
}