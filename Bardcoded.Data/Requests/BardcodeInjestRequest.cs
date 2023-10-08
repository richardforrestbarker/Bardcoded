using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bardcoded.API.Data.Requests
{
    public class BardcodeInjestRequest
    {
        [Description("The barcode.")]
        [Required(ErrorMessage = $"{nameof(Bard)} is required.")]
        [StringLength(100, ErrorMessage=$"{nameof(Bard)} can't be longer than 100 characters.")]
        [JsonPropertyName("bard")]
        public String Bard { get; set; }

        [Description("Where the barcode was loaded into the system from - it's injest method.")]
        [Required(ErrorMessage = $"{nameof(Source)} is required.")]
        [StringLength(100, ErrorMessage = $"{nameof(Source)} can't be longer than 100 characters.")]
        [JsonPropertyName("Source")]
        public String Source { get; set; }
        [Description("The name of the item")]
        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [StringLength(100, ErrorMessage = $"{nameof(Name)} can't be longer than 100 characters.")]
        [JsonPropertyName("Name")]
        public String Name { get; set; }

        [Description("A description of the item.")]
        [StringLength(4096, ErrorMessage = $"{nameof(Description)} can't be longer than 4096 characters.")]
        [JsonPropertyName("description")]
        public String Description { get; set; }

        [Description("An image of the item's label / brand.")]
        [Required(ErrorMessage = $"Image is required.")]
        [JsonPropertyName("base64Image")]
        public String Base64Image { get; set; }
        [Description("The image's type; i.e jpg, png.")]
        [Required(ErrorMessage = $"Image type is required.")]
        [JsonPropertyName("imageType")]
        public String ImageType { get; set; }

        [Description("The \"size\" of the item.")]
        [Required(ErrorMessage = $"{nameof(WeightVolume)} is required.")]
        [StringLength(1024, ErrorMessage = $"{nameof(WeightVolume)} can't be longer than 1024 characters.")]
        [JsonPropertyName("weightVolume")]
        public string WeightVolume { get; set; }
        
    }


    public class BardcodeUpdateRequest {
        public BardcodeUpdateRequest(String Bard, String Source, Guid id)
        {
            this.Bard = Bard; // these are un-update-able
            this.Source = Source;
            Id = id;
        }
        public String Bard { get; } 
        public String Source { get; }
        public Guid Id { get; }


        [Description("The name of the item")]
        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [StringLength(100, ErrorMessage = $"{nameof(Name)} can't be longer than 100 characters.")]
        public String Name { get; set; }

        [Description("A description of the item.")]
        [StringLength(4096, ErrorMessage = $"{nameof(Description)} can't be longer than 4096 characters.")] 
        public String Description { get; set; }

        [Description("An image of the item's label / brand.")]
        [Required(ErrorMessage = $"Image is required.")]
        public String Base64Image { get; set; }
        
        [Description("The image's type; i.e jpg, png.")]
        [Required(ErrorMessage = $"Image type is required.")]
        public String ImageType { get; set; }
       
        [Description("The \"size\" of the item.")]
        [Required(ErrorMessage = $"{nameof(WeightVolume)} is required.")]
        [StringLength(1024, ErrorMessage = $"{nameof(WeightVolume)} can't be longer than 1024 characters.")]
        public string WeightVolume { get; set; }
    }
}
