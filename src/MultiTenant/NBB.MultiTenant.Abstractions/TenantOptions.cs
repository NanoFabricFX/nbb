namespace NBB.MultiTenant.Abstractions
{
    public class TenantOptions
    {
        public string ConnectionString { get; set; }
        public string EncryptionKey { get; set; }

        public TenantStoreType TenantStoreType { get; set; } = TenantStoreType.Sql;

        public TenantIdentificationOptions IdentificationOptions { get; set; } = new TenantIdentificationOptions();

        public TenantOptions()
        {

        }

        public TenantOptions(string connectionString, string encryptionKey, TenantIdentificationOptions tenantIdentificationOptions = null, bool useCache = false)
        {
            ConnectionString = connectionString;
            EncryptionKey = encryptionKey;

            if (tenantIdentificationOptions != null)
            {
                IdentificationOptions = tenantIdentificationOptions;
            }
        }
    }

    public enum TenantStoreType
    {
        Sql = 0,
        AzureBlob = 1
    }
}