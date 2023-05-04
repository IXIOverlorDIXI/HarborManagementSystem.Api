using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void StorageEnvironmentalConditionEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<StorageEnvironmentalCondition>().HasKey(x => x.Id);
            builder.Entity<StorageEnvironmentalCondition>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.StorageEnvironmentalConditions)
                .HasForeignKey(x => x.BerthId);
        }
    }
}