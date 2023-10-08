namespace Bardcoded.Data.Store
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public BarcodeData Barcode { get; set; }
        public Guid BarcodeId { get; set; }
        public int Count { get; set; }
        public long CostInPennies { get; set; }
        public List<Purchase> PurchasedIn { get; set; }
    }
}
