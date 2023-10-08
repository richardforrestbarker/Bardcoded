using Bardcoded.API.Data.Requests;
using Bardcoded.Data.Responses;
using Bardcoded.Data.Store;

namespace Bardcoded.API
{
    internal class IOMapper
    {
        internal BarcodeData Map(BardcodeInjestRequest request)
        {
            return new BarcodeData() {
                Bard = request.Bard,
                Base64Image = request.Base64Image,
                Description = request.Description,
                ImageType = request.ImageType,
                Source = request.Source,
                Name = request.Name
            };
        }

        internal BarcodeData Map(BardcodeUpdateRequest request)
        {
            return new BarcodeData()
            {
                Id = request.Id,
                Bard = request.Bard,
                Base64Image = request.Base64Image,
                Description = request.Description,
                ImageType = request.ImageType,
                Source = request.Source,
                Name = request.Name
            };
        }

        internal BarcodeView Map(BarcodeData data)
        {
            return BarcodeView.Create(data.Bard, data.Name, data.Description, data.Base64Image, data.ImageType);
        }
    }
}