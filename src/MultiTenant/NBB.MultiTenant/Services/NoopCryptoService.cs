using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant.Services
{
    class NoopCryptoService : ICryptoService
    {
        public string Decrypt(string cipherText)
        {
            return cipherText;
        }

        public string Encrypt(string text)
        {
            return text;
        }
    }
}