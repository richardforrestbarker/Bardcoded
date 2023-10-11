using Bardcoded.Data.Store;

namespace Bardcoded.API
{
    public class TestDatas
    {
        public const string Base64EncodedMockImage = "iVBORw0KGgoAAAANSUhEUgAAACcAAAAnCAYAAACMo1E1AAAC5UlEQVRYR82YvY8xURTGj040ZDtK9CRaep1Ggk5CJNRCopDQ8Qcg6ElUGkEnlCQkSpQ0CI3o7J775sw7xpgvs8utcO8853fPeebu3WOYTqe3r68vMBqN8Cnjer3C8XgEw2azueEHp9MJZrP57Xzn8xlWqxVgwgzb7fZmMpnYD+8GJDDkuFwu/+CsVivwJ96RQWH83W73Hw7r+S5AsbgPcO8AfJYQUbi/BJSq1FO4vwCUs5Ak3G8CyoFhbFm43wBUAqYYTk9ApWCq4PQAVAOmGu4VQLVgmuC0AGoB0wynBlAr2EtwSgBfAXsZTgrwVTBd4MQA9QDTDY4PiFcvPNn1uBcq+guh9Gr8cy9kYAhos9mUPvZ0nW5wVMqPy5zQYx/juWcgegC+VFY5ALl5OVNqhlMaWOk6MVBNcGoDql1PoKrhtAbS8twD3GQyAZ/Px2V5vV6D3W5n3xeLBbjdbtE5Kf8INefzObhcLvbIT7cBHA6HqOYdHC0koHK5DPV6nXUCcOcWiwUKhQLk83mggLfbTdLXQs1isQiNRgOWyyVrfRgMBiiVSpDJZB40JctKALjTXq8HuVwO+DB+vx+i0SiEw2G5F4+bJ81utwv7/R5isdhTTUk4zFytVoNOp8NKKiZEJa9Wq1yQdrsNkUgExuMxeL3eO3Cqxmw2g0qlIrph1MQ5UTi+D5rNJgSDQTgcDswbVAJak0wmmRBmEUXT6TRb12q17jLK1yRo8jBZRagp+R//aDSCQCDABeIbGwHw5kG7xPSgf3AQsFitCYDgB4MB2xgOoaZsryQUCt0B8AMiXCKRYGbGgSXLZrN3HhIDpCxjxnHwjxmPx8NpynaZhEIUTJgB+k677/f7T18SMU0EFFbqDo5STOcQ39h4P8PA5JdUKgXD4ZAdMzj4WcTyUtnICnQ8SWnG43FABjpmODjqbNJbSdvmv3EkTHN0rCAYDgIVvq3C56Q0T6cT12Flnc2P7gl/cjf9Gy3Pm4BKSykfAAAAAElFTkSuQmCC";

        public static List<BarcodeData> DefaultBarcodes { get; internal set; } = new List<BarcodeData>()
        {
            new BarcodeData() { Id = Guid.NewGuid(), Bard = "bardcode 1", Base64Image = TestDatas.Base64EncodedMockImage, Description = "a thing", Name ="a thing", ImageType ="png", Source ="Manually Injested", },
            new BarcodeData() { Id = Guid.NewGuid(), Bard = "bardcode 2", Base64Image = TestDatas.Base64EncodedMockImage, Description = "a thing", Name ="a thing", ImageType ="png", Source ="Manually Injested", },
            new BarcodeData() { Id = Guid.NewGuid(), Bard = "bardcode 3", Base64Image = TestDatas.Base64EncodedMockImage, Description = "a thing", Name ="a thing", ImageType ="png", Source ="Manually Injested", },
            new BarcodeData() { Id = Guid.NewGuid(), Bard = "bardcode 4", Base64Image = TestDatas.Base64EncodedMockImage, Description = "a thing", Name ="a thing", ImageType ="png", Source ="Manually Injested", },
            new BarcodeData() { Id = Guid.NewGuid(), Bard = "769498031919", Base64Image = TestDatas.Base64EncodedMockImage, Description = "a cup, actually.", Name ="a thing", ImageType ="png", Source ="Manually Injested", },
        };
    }
}
