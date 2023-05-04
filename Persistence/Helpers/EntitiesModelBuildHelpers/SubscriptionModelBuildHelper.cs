using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void SubscriptionEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Subscription>().HasKey(x => x.Id);
            builder.Entity<Subscription>()
                .HasMany(x => x.Users)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x=>x.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Subscription>()
                .HasMany(x => x.SubscriptionСhecks)
                .WithOne(x => x.Subscription)
                .HasForeignKey(x=>x.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Subscription>().HasData(DataHelper.SeedSubscriptions());
        }
    }
}