using System.Security.Cryptography;
using System.Text;

namespace runSyncBackend.Services
{
    public class CryptoService
    {
        private readonly RSA _rsa;
        
        public CryptoService()
        {
            _rsa = RSA.Create(2048); // Generate RSA key pair
        }

        // RSA Methods
        public string GetPublicKey()
        {
            return Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        }

        public string RSAEncrypt(string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            var encryptedData = _rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Convert.ToBase64String(encryptedData);
        }

        public string RSADecrypt(string encryptedText)
        {
            var data = Convert.FromBase64String(encryptedText);
            var decryptedData = _rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedData);
        }

        // AES Methods
        public (string encryptedData, string key, string iv) AESEncrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return (
                Convert.ToBase64String(encryptedBytes),
                Convert.ToBase64String(aes.Key),
                Convert.ToBase64String(aes.IV)
            );
        }

        public string AESDecrypt(string encryptedData, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using var decryptor = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}