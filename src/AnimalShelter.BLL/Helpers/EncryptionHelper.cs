using System.Security.Cryptography;
using System.Text;

namespace AnimalShelter.BLL.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string _key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
        ?? throw new InvalidOperationException("ENCRYPTION_KEY is missing in environment variables.");

        public static byte[] Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memStream = new MemoryStream();

            // On stocke l'IV au début du flux pour pouvoir déchiffrer plus tard
            memStream.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return memStream.ToArray();
        }

        public static string Decrypt(byte[] cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key);

            using var memStream = new MemoryStream(cipherText);
            byte[] iv = new byte[aes.BlockSize / 8];
            memStream.Read(iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var cs = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        public static byte[] Hash(string input)
        {
            // On utilise SHA256 pour un hashage robuste et déterministe
            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }
    }
}
