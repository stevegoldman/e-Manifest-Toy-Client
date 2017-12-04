using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eManifestServicesClient
{
    public class errorResponse:eManifestType
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string ErrorID { get; set; }
        public string Date { get; set; }
    }
}
