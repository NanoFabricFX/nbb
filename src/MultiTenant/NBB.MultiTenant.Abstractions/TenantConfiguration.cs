using System;

namespace NBB.MultiTenant.Abstractions
{
    public class TenantConfiguration
    {
        public string ConnectionString { get; set; }
        public string EncryptionKey { get; set; }
        public bool UseConnectionStringEncryption { get; set; }
        public Type TenantStoreType { get; set; } = Type.GetType("NBB.MultiTenant.Stores.DatabaseStore");
        public Type CryptoServiceType { get; set; } = Type.GetType("NBB.MultiTenant.Cryptography.AesCryptoService");

        public ITenantIdentificationOptions IdentificationOptions { get; set; }
        public bool UseDefaultValueOnSave { get; set; } = true;        
        public bool RestrictCrossTenantAccess { get; set; } = true;

        public bool IsReadOnly { get; set; }
        public Action<Tenant> ConfigureConnection { get; set; }
        public TenantConfiguration()
        {

        }

        public TenantConfiguration(string connectionString, string encryptionKey, bool useConnectionStringEncryption = false, ITenantIdentificationOptions tenantIdentificationOptions = null, bool useCache = false)
        {
            ConnectionString = connectionString;
            EncryptionKey = encryptionKey;

            if (tenantIdentificationOptions != null)
            {
                IdentificationOptions = tenantIdentificationOptions;
            }

            UseConnectionStringEncryption = useConnectionStringEncryption;
        }
    }
}