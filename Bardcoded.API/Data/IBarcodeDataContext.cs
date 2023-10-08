using Bardcoded.Data.Store;

namespace Bardcoded.API.Data
{
    public interface IBarcodeDataContext
    {
        Task<List<BarcodeData>> GetAll();
        Task DeleteBarcode(string bard);
        Task DeleteAll();
        Task<BarcodeData> GetBarcode(string bard);

        Task<Guid> InsertBarcode(BarcodeData data);

        Task<BarcodeData> UpdateBarcode(BarcodeData data);
    }
}
