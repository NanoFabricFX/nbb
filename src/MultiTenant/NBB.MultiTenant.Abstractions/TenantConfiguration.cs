using System;

namespace NBB.MultiTenant.Abstractions
{
    public class TenantConfiguration
    {
        public Type TenantStoreType { get; set; } 
        public Type CryptoServiceType { get; set; }
        public string EncryptionKey { get; set; }
        public ITenantIdentificationOptions IdentificationOptions { get; set; }
        
        public TenantConfiguration()
        {

        }

        public TenantConfiguration(ITenantIdentificationOptions tenantIdentificationOptions = null, bool useCache = false)
        {
            if (tenantIdentificationOptions != null)
            {
                IdentificationOptions = tenantIdentificationOptions;
            }
        }
    }
}