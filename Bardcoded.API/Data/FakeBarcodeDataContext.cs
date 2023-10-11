using Bardcoded.Data.Store;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Bardcoded.API.Data
{
    public class FakeBarcodeDataContext : DbContext, IBarcodeDataContext
    {
        
        public FakeBarcodeDataContext() : this(TestDatas.DefaultBarcodes)
        {

        }

        public FakeBarcodeDataContext(List<BarcodeData> barcodes)
        {
            Barcodes = new Dictionary<string, BarcodeData>();
            BarcodeUpdates = new Dictionary<string, BarcodeUpdate>();
            barcodes.ForEach(x => Barcodes.Add(x.Bard, x));
        }

        public Dictionary<string, BarcodeData> Barcodes { get; set; }
        public Dictionary<string, BarcodeUpdate> BarcodeUpdates { get; set; }

        public Task DeleteAll()
        {
            Barcodes = new Dictionary<string, BarcodeData>();
            return Task.CompletedTask;
        }

        public Task DeleteBarcode(string bard)
        {
            Barcodes.Remove(bard);
            return Task.CompletedTask;
        }

        public Task<List<BarcodeData>> GetAll()
        {
            return Task.FromResult(Barcodes.Values.ToList());
        }

        public Task<BarcodeData> GetBarcode(string bard)
        {
            return Task.FromResult(Barcodes.SingleOrDefault(c => c.Key.Equals(bard)).Value);
        }

        public async Task<Guid> InsertBarcode(BarcodeData data)
        {
            try
            {
                var code = await GetBarcode(data.Bard);
                if (code != default(BarcodeData)) throw new InvalidOperationException("Barcode exists.");
                Barcodes.Add(data.Bard, data);
                data.Id = Guid.NewGuid();
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
                var newid = new Guid();
                BarcodeUpdates.Add(newid.ToString(), new BarcodeUpdate()
                {
                    UpdateId = newid,
                    BarcodeId = data.Id,
                    UpdateDate = DateTime.Now,
                    NewBarcodeJson = JsonSerializer.Serialize(data),
                    OldBarcodeJson = JsonSerializer.Serialize(code)
                });
                Barcodes[code.Bard] = data;
                return data;
            }
            catch (InvalidOperationException inval)
            {
                throw new InvalidOperationException("Can't update the entry for that barcode.", inval);
            }
        }
    }

}
