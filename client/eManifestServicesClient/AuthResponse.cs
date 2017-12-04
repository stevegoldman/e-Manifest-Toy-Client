using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eManifestServicesClient
{
    public class AuthResponse : eManifestType
    {
        public string Token {get; set;}
        public string Expiration { get; set; }

            
    }
}
