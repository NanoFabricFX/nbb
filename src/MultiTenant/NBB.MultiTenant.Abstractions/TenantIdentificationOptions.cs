using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.Abstractions
{
    public class TenantIdentificationOptions
    {
        public string TenantHeadersKey { get; set; } = "tenantId";
        public string TenantMessagingKey { get; set; } = "nbb-tenant-id";
        public List<TenantIdentificationType> IdentitificationTypes { get; protected set; } = new List<TenantIdentificationType>();
        public List<Type> RegisteredServices { get; protected set; } = new List<Type>();
        public TenantIdentificationOptions WithDefaultOptions()
        {
            IdentitificationTypes = new List<TenantIdentificationType> { TenantIdentificationType.Headers, TenantIdentificationType.Host, TenantIdentificationType.Ip, TenantIdentificationType.MessagingHeaders };
            return this;
        }

        public TenantIdentificationOptions AddIdentificationService<T>() where T: ITenantIdentificationService
        {
            var type = typeof(T);
            if (!RegisteredServices.Contains(type))
            {
                RegisteredServices.Add(type);
            }
            return this;
        }

        public TenantIdentificationOptions RemoveIdentificationService<T>() where T : ITenantIdentificationService
        {
            var type = typeof(T);
            if (RegisteredServices.Contains(type))
            {
                RegisteredServices.Remove(type);
            }
            return this;
        }

        public TenantIdentificationOptions AddIdentificationService(TenantIdentificationType identificationType)
        {
            if (!IdentitificationTypes.Contains(identificationType))
            {
                IdentitificationTypes.Add(identificationType);
            }
            return this;
        }

        public TenantIdentificationOptions AddIdentificationService(IEnumerable<TenantIdentificationType> identificationTypes)
        {
            foreach (var identificationType in identificationTypes)
            {
                if (!IdentitificationTypes.Contains(identificationType))
                {
                    IdentitificationTypes.Add(identificationType);
                }
            }

            return this;
        }

        public TenantIdentificationOptions RemoveIdentificationService(TenantIdentificationType identificationType)
        {
            if (IdentitificationTypes.Contains(identificationType))
            {
                IdentitificationTypes.Remove(identificationType);
            }
            return this;
        }

        public TenantIdentificationOptions RemoveIdentificationService(IEnumerable<TenantIdentificationType> identificationTypes)
        {
            foreach(var identificationType in identificationTypes)
            {
                if (IdentitificationTypes.Contains(identificationType))
                {
                    IdentitificationTypes.Remove(identificationType);
                }
            }
            
            return this;
        }

        public bool ShoudUse(TenantIdentificationType type)
        {
            return IdentitificationTypes.Contains(type);
        }
    }    
}