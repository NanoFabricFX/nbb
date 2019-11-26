﻿using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Subscription<T>
    {
        public Subscription()
        {
            SubscriptionFeatures = new HashSet<SubscriptionFeature<T>>();
            TenantSubcriptions = new HashSet<TenantSubcription<T>>();
        }

        public Guid SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<SubscriptionFeature<T>> SubscriptionFeatures { get; set; }
        public virtual ICollection<TenantSubcription<T>> TenantSubcriptions { get; set; }
    }
}