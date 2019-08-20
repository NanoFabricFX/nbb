namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ICryptoService
    {
        string Encrypt(string text);
        string Decrypt(string cipherText);
    }
}