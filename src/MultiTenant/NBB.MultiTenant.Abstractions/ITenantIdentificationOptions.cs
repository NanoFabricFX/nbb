using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantIdentificationOptions
    {
        List<TenantIdentificationType> IdentitificationTypes { get; }
        List<Type> RegisteredServices { get; }
        string TenantHeadersKey { get; set; }
        string TenantMessagingKey { get; set; }

        ITenantIdentificationOptions AddIdentificationService(IEnumerable<TenantIdentificationType> identificationTypes);
        ITenantIdentificationOptions AddIdentificationService(TenantIdentificationType identificationType);
        ITenantIdentificationOptions AddIdentificationService<T>() where T : ITenantIdentificationService;
        ITenantIdentificationOptions RemoveIdentificationService(IEnumerable<TenantIdentificationType> identificationTypes);
        ITenantIdentificationOptions RemoveIdentificationService(TenantIdentificationType identificationType);
        ITenantIdentificationOptions RemoveIdentificationService<T>() where T : ITenantIdentificationService;
        bool ShoudUse(TenantIdentificationType type);
        ITenantIdentificationOptions WithDefaultOptions();
    }
}