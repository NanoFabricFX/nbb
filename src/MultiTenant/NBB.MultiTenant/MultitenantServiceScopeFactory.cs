using Microsoft.Extensions.DependencyInjection;
using System;

namespace NBB.MultiTenant
{
    public class MultitenantServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MultitenantServiceScopeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            var scope = _serviceProvider.CreateScope();
            return scope;
        }
    }
}