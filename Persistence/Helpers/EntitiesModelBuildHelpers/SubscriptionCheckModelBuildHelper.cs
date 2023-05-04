using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void SubscriptionСheckEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<SubscriptionСheck>().HasKey(x => x.Id);
            builder.Entity<SubscriptionСheck>()
                .HasOne(x => x.AppUser)
                .WithMany(x => x.SubscriptionСhecks)
                .HasForeignKey(x => x.UserId);
            builder.Entity<SubscriptionСheck>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.SubscriptionСhecks)
                .HasForeignKey(x => x.SubscriptionId);
        }
    }
}