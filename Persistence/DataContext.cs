using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Helpers.EntitiesModelBuildHelpers;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        
        public DbSet<Berth> Berths { get; set; }
        
        public DbSet<BerthPhoto> BerthPhotos { get; set; }
        
        public DbSet<Booking> Bookings { get; set; }
        
        public DbSet<BookingCheck> BookingChecks { get; set; }
        
        public DbSet<EnvironmentalCondition> EnvironmentalConditions { get; set; }
        
        public DbSet<Domain.Entities.File> Files { get; set; }
        
        public DbSet<Harbor> Harbors { get; set; }
        
        public DbSet<HarborDocument> HarborDocuments { get; set; }
        
        public DbSet<HarborPhoto> HarborPhotos { get; set; }
        
        public DbSet<RelativePositionMetering> RelativePositionMeterings { get; set; }
        
        public DbSet<Review> Reviews { get; set; }
        
        public DbSet<Service> Services { get; set; }
        
        public DbSet<Ship> Ships { get; set; }
        
        public DbSet<ShipType> ShipTypes { get; set; }
        
        public DbSet<StorageEnvironmentalCondition> StorageEnvironmentalConditions { get; set; }
        
        public DbSet<Subscription> Subscriptions { get; set; }
        
        public DbSet<SubscriptionСheck> SubscriptionСhecks { get; set; }
        
        public DbSet<SuitableShipType> SuitableShipTypes { get; set; }

        public DataContext(DbContextOptions<DataContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ModelBuildHelper.AdditionalServiceEntityBuilder(builder);
            
            ModelBuildHelper.BerthEntityBuilder(builder);
            
            ModelBuildHelper.BerthPhotoEntityBuilder(builder);
            
            ModelBuildHelper.BookingEntityBuilder(builder);
            
            ModelBuildHelper.BookingCheckEntityBuilder(builder);
            
            ModelBuildHelper.EnvironmentalConditionEntityBuilder(builder);
            
            ModelBuildHelper.FileEntityBuilder(builder);
            
            ModelBuildHelper.HarborEntityBuilder(builder);
            
            ModelBuildHelper.HarborDocumentEntityBuilder(builder);
            
            ModelBuildHelper.HarborPhotoEntityBuilder(builder);
            
            ModelBuildHelper.RelativePositionMeteringEntityBuilder(builder);
            
            ModelBuildHelper.ReviewEntityBuilder(builder);
            
            ModelBuildHelper.ServiceEntityBuilder(builder);
            
            ModelBuildHelper.ShipEntityBuilder(builder);
            
            ModelBuildHelper.ShipTypeEntityBuilder(builder);
            
            ModelBuildHelper.StorageEnvironmentalConditionEntityBuilder(builder);
            
            ModelBuildHelper.SubscriptionEntityBuilder(builder);
            
            ModelBuildHelper.SubscriptionСheckEntityBuilder(builder);
            
            ModelBuildHelper.SuitableShipTypeEntityBuilder(builder);
            
            ModelBuildHelper.UserEntityBuilder(builder);
        }
    }
}