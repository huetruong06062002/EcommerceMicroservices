using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;

namespace Product.API.Persistence
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        { }

        public DbSet<CatalogProduct> Products { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //Initialling modified
            var modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified ||
                       e.State == EntityState.Added ||
                       e.State == EntityState.Detached);

            foreach(var item in modified)
            {
                switch(item.State) //Check state of entity
                {
                    case EntityState.Added: //case Add
                        if(item.Entity is IDateTracking addedEntity) // if entity have date tracking
                        {
                            addedEntity.CreatedDate = DateTime.UtcNow;
                            item.State = EntityState.Added;
                        }
                        break;

                    case EntityState.Modified: //case update
                        Entry(item.Entity).Property("Id").IsModified = false; //No modify Id
                        if(item.Entity is IDateTracking modifiedEntity)
                        {
                            modifiedEntity.LastModifiedDate = DateTime.UtcNow;
                            item.State = EntityState.Modified;
                        }
                        break;
                    case EntityState.Deleted:

                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
