using Bardcoded.Data.Store;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bardcoded.API.Data
{
    public class BarcodeDataContext : DbContext, IBarcodeDataContext
    {
        public DbSet<BarcodeData> Barcodes { get; set; }
        public DbSet<BarcodeUpdate> BarcodeUpdates { get; set; }

        public Task DeleteAll()
        {

            return Task.CompletedTask;
        }

        public async Task DeleteBarcode(string bard)
        {
            var entity = await Barcodes.Where(x => x.Bard.Equals(bard)).FirstAsync();
            Barcodes.Remove(entity);
            await SaveChangesAsync();
        }

        public Task<List<BarcodeData>> GetAll()
        {
            return Task.FromResult(Barcodes.ToList());
        }

        public Task<BarcodeData> GetBarcode(string bard)
        {
            return Barcodes.Where(c => c.Bard.Equals(bard)).SingleOrDefaultAsync();
        }

        public async Task<Guid> InsertBarcode(BarcodeData data)
        {
            try
            {
                var code = await GetBarcode(data.Bard);
                if (code != default(BarcodeData))
                {
                    throw new InvalidOperationException("Barcode exists.");
                }
                Barcodes.Add(data);
                await SaveChangesAsync();
                return data.Id;
            }
            catch (InvalidOperationException inval)
            {
                throw new InvalidOperationException("Can't create an entry for that barcode.", inval);
            }

        }

        public async Task<BarcodeData> UpdateBarcode(BarcodeData data)
        {
            try
            {
                var code = await GetBarcode(data.Bard);
                if (code == default(BarcodeData))
                {
                    throw new InvalidOperationException("Barcode doesn't exist.");
                }
                BarcodeUpdates.Add(new BarcodeUpdate()
                {
                    BarcodeId = data.Id,
                    UpdateDate = DateTime.Now,
                    NewBarcodeJson = JsonSerializer.Serialize(data),
                    OldBarcodeJson = JsonSerializer.Serialize(code)
                });
                Barcodes.Update(data);
                await SaveChangesAsync();
                return data;
            }
            catch (InvalidOperationException inval)
            {
                throw new InvalidOperationException("Can't update the entry for that barcode.", inval);
            }
        }
    }
}
