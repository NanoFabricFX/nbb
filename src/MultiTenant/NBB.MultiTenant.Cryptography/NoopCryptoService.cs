using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant.Cryptography
{
    public class NoopCryptoService : ICryptoService
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