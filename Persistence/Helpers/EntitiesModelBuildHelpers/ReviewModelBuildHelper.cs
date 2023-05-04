using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void ReviewEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Review>().HasKey(x => x.Id);
            builder.Entity<Review>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x=>x.BerthId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Review>()
                .HasOne(x => x.Reviewer)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x=>x.ReviewerId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Review>().Property(x => x.ReviewerId).IsRequired(false);
        }
    }
}