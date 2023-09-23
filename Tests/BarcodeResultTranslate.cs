using BardCoded;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

namespace Tests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
    /// </summary>
    public class BarcodeResultTranslate : TestContext
    {
        [Fact]
        public void CanActuallyParseBase64Barcodes()
        {
            var testDataFile = File.ReadAllText("test_data.json");
            var testData = JsonConvert.DeserializeObject<Testdata>(testDataFile);
            var type = "data:image/png;base64";
            
            foreach(var item in testData.expected) {
                var image = File.ReadAllText($"{testData.fileLocation}/{item.Key}");
                var actual = BarcodeResult.translateFromBase64(image, type);
                Assert.Equivalent(item.Value, actual);
            }
        }

    }

    public class Testdata
    {
        public string fileLocation { get; set; }
        public Dictionary<string, BarcodeResult> expected { get; set; }
    }
}