using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant.Cryptography
{
    public class DatabaseTenantConfigurationDecorator : Data.Abstractions.IConnectionStringConfiguration
    {
        private readonly ICryptoService _cryptoService;
        private readonly Data.Abstractions.IConnectionStringConfiguration _inner;
        public DatabaseTenantConfigurationDecorator(ICryptoService cryptoService, Data.Abstractions.IConnectionStringConfiguration inner)
        {
            _cryptoService = cryptoService;
            _inner = inner;
        }

        public string GetConnectionString()
        {
            var connectionString= _inner.GetConnectionString();
            return _cryptoService.Decrypt(connectionString);
        }

        public void SetConnectionString(string s)
        {
            var connectionString = _cryptoService.Encrypt(s);
            _inner.SetConnectionString(connectionString);
        }
    }
}