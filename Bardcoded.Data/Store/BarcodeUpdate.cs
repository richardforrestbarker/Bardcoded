namespace Bardcoded.Data.Store
{
    public class BarcodeUpdate
    {
        public Guid UpdateId { get; set; }
        public Guid BarcodeId { get; set; }
        public String OldBarcodeJson { get; set; }
        public String NewBarcodeJson { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
