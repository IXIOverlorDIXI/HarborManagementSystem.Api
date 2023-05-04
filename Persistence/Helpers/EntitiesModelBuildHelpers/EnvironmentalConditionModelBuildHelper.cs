using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void EnvironmentalConditionEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<EnvironmentalCondition>().HasKey(x => x.Id);
            builder.Entity<EnvironmentalCondition>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.EnvironmentalConditions)
                .HasForeignKey(x => x.BerthId);
        }
    }
}