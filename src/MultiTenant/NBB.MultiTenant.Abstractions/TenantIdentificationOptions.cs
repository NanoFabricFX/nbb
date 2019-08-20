namespace NBB.MultiTenant.Abstractions
{
     public class TenantIdentificationOptions
    {
        public bool UseHost { get; set; } = false;
        public bool UseHeaders { get; set; } = false;
        public bool UseIp { get; set; } = false;
        public bool UseMessagingHeaders { get; set; } = false;

        public string TenantHeadersKey { get; set; } = "tenantId";
        public string TenantMessagingKey { get; set; } = "nbb-tenant-id";
    }
}