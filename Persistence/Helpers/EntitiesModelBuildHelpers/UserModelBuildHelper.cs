using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void UserEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .HasMany(x => x.Ships)
                .WithOne(x => x.Owner)
                .HasForeignKey(x=>x.OwnerId);
            builder.Entity<AppUser>()
                .HasMany(x => x.Harbors)
                .WithOne(x => x.Owner)
                .HasForeignKey(x=>x.OwnerId);
            builder.Entity<AppUser>()
                .HasMany(x => x.SubscriptionСhecks)
                .WithOne(x => x.AppUser)
                .HasForeignKey(x=>x.UserId);
            builder.Entity<AppUser>()
                .HasMany(x => x.Reviews)
                .WithOne(x => x.Reviewer)
                .HasForeignKey(x=>x.ReviewerId);
            builder.Entity<AppUser>()
                .HasOne(x => x.Subscription)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.SubscriptionId);
            builder.Entity<AppUser>()
                .HasOne(x => x.Photo)
                .WithOne(x => x.AppUser)
                .HasForeignKey<AppUser>(x => x.PhotoId);
            builder.Entity<AppUser>().Property(x => x.PhotoId).IsRequired(false);
            builder.Entity<AppUser>().Property(x => x.SubscriptionId).IsRequired(false);
            builder.Entity<AppUser>().Property(x => x.Settings).IsRequired(false);
        }
    }
}