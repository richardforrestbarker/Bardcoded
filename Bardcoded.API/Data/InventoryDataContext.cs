using Bardcoded.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace Bardcoded.API.Data
{
    public class InventoryDataContext : DbContext
    {
        public DbSet<InventoryItem> Barcodes { get; set; }
    }
}
