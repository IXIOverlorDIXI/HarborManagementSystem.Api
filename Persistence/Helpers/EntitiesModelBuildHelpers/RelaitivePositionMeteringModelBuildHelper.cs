using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void RelativePositionMeteringEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<RelativePositionMetering>().HasKey(x => x.Id);
            builder.Entity<RelativePositionMetering>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.RelativePositionMeterings)
                .HasForeignKey(x => x.BerthId);
        }
    }
}