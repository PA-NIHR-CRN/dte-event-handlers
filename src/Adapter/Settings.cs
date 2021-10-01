using Adapter.Contracts;
using Microsoft.Extensions.Configuration;

namespace Adapter
{
    public class Settings 
    {
        public string EventStore_ProcessorLink { get; set; }
        public string EventStore_Username { get; set; }
        public string EventStore_Password { get; set; }
        public string CertificateFqdn { get; set; }
        public string CryptoKey { get; set; }
    }
}