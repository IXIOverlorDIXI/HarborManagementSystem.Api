using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void ServiceEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<Service>().HasKey(x => x.Id);
            builder.Entity<Service>()
                .HasOne(x => x.Harbor)
                .WithMany(x => x.Services)
                .HasForeignKey(x=>x.HarborId);
            builder.Entity<Service>()
                .HasMany(x => x.AdditionalServices)
                .WithOne(x => x.Service)
                .HasForeignKey(x=>x.ServiceId);
        }
    }
}