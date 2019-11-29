namespace NBB.MultiTenant.Abstractions
{
    public class TenantOptions
    {
        public string ConnectionString { get; set; }
        public string EncryptionKey { get; set; }
        public bool UseConnectionStringEncryption { get; set; }

        public TenantStoreType TenantStoreType { get; set; } = TenantStoreType.Sql;

        public TenantIdentificationOptions IdentificationOptions { get; set; } = new TenantIdentificationOptions();
        public bool UseDefaultValueOnInsert { get; set; } = true;        
        public bool RestrictCrossTenantAccess { get; set; } = true;

        public bool IsReadOnly { get; set; }

        public TenantOptions()
        {

        }

        public TenantOptions(string connectionString, string encryptionKey, bool useConnectionStringEncryption = false, TenantIdentificationOptions tenantIdentificationOptions = null, bool useCache = false)
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

    public enum TenantStoreType
    {
        Sql = 0,
        AzureBlob = 1
    }
}