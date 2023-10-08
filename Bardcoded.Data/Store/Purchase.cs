namespace Bardcoded.Data.Store
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public long TotalAmountInPennies { get; set; }
        public IList<BarcodeData> Items { get; set; }
        public String RecieptBase64Image { get; set; } // needs to be stored as clob or blob
        public String ImageType { get; set; }
    }
}
