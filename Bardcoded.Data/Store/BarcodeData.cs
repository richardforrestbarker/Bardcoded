using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bardcoded.Data.Store
{

    public class BarcodeData
    {
        public Guid Id { get; set; }
        public String Bard { get; set; }
        public String Source { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Base64Image { get; set; } // needs to be stored as clob or blob
        public String ImageType { get; set; }
    }
}
