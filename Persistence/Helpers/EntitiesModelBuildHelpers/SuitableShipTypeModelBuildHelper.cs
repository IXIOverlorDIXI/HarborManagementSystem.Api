using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Helpers.EntitiesModelBuildHelpers
{
    public static partial class ModelBuildHelper
    {
        public static void SuitableShipTypeEntityBuilder(ModelBuilder builder)
        {
            builder.Entity<SuitableShipType>().HasKey(x => x.Id);
            builder.Entity<SuitableShipType>()
                .HasOne(x => x.Berth)
                .WithMany(x => x.SuitableShipTypes)
                .HasForeignKey(x => x.BerthId);
            builder.Entity<SuitableShipType>()
                .HasOne(x => x.ShipType)
                .WithMany(x => x.SuitableShipTypes)
                .HasForeignKey(x => x.ShipTypeId);
        }
    }
}