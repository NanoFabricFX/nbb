using NBB.MultiTenant.Abstractions.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NBB.MultiTenant.Cryptography
{
    public class AesCryptoService : ICryptoService
    {
        private readonly string _cryptoKey;        

        public AesCryptoService(TenantEncryptionConfiguration tenantEncryptionConfiguration)
        {
            _cryptoKey = tenantEncryptionConfiguration.EncryptionKey;
        }

        public string Encrypt(string text)
        {
            var key = Encoding.UTF8.GetBytes(_cryptoKey);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                {
                    return null;
                }

                using (var transform = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, transform, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = RiffleShuffle(iv, decryptedContent);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var (iv, cipher) = RiffleDeshuffle(fullCipher);
            var key = Encoding.UTF8.GetBytes(_cryptoKey);

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                {
                    return null;
                }

                using (var transform = aesAlg.CreateDecryptor(key, iv))
                using (var msDecrypt = new MemoryStream(cipher))
                using (var csDecrypt = new CryptoStream(msDecrypt, transform, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private static byte[] RiffleShuffle(byte[] iv, byte[] content)
        {
            var shuffledContent = new byte[32];
            for (var i = 0; i < 16; i++)
            {
                shuffledContent[2 * i] = content[i];
                shuffledContent[2 * i + 1] = iv[i];
            }

            if (content.Length == 16)
            {
                return shuffledContent;
            }

            var result = new byte[iv.Length + content.Length];
            Buffer.BlockCopy(shuffledContent, 0, result, 0, 32);
            Buffer.BlockCopy(content, 16, result, 32, content.Length - 16);

            return result;
        }

        private static (byte[] iv, byte[] cipher) RiffleDeshuffle(byte[] fullCipher)
        {
            if (fullCipher.Length < 32)
            {
                throw new ArgumentException("Bad cipher");
            }

            var iv = new byte[16];
            var content = new byte[fullCipher.Length - 16];

            for (var i = 0; i < 16; i++)
            {
                content[i] = fullCipher[2 * i];
                iv[i] = fullCipher[2 * i + 1];
            }

            if (fullCipher.Length > 32)
            {
                Buffer.BlockCopy(fullCipher, 32, content, 16, fullCipher.Length - 32);
            }

            return (iv, content);
        }
    }
}
