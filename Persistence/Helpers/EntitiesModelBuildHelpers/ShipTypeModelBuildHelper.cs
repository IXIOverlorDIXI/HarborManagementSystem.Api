using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void ShipTypeEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<ShipType>().HasKey(x => x.Id);
            builder.Entity<ShipType>()
                .HasMany(x => x.Ships)
                .WithOne(x => x.ShipType)
                .HasForeignKey(x=>x.ShipTypeId);
            builder.Entity<ShipType>()
                .HasMany(x => x.SuitableShipTypes)
                .WithOne(x => x.ShipType)
                .HasForeignKey(x=>x.ShipTypeId);
        }
    }
}