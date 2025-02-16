using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace api.Utils
{
    public class PasswordHasher
    {
        private const int KeySize = 256;
        private const int Iterations = 10000;
        private const string Salt = "mystrongPassword!";

        // Encrypts the password using AES algorithm
        public string EncryptPassword(string password, string key)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(Salt);
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(key, saltBytes);
                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypts the encrypted password using AES algorithm
        public string DecryptPassword(string encryptedPassword, string key)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(Salt);
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedPassword);
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(key, saltBytes);
                aesAlg.Mode = CipherMode.CBC;

                int ivLength = BitConverter.ToInt32(cipherTextBytes, 0);
                aesAlg.IV = cipherTextBytes.Skip(sizeof(int)).Take(ivLength).ToArray();
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherTextBytes, sizeof(int) + ivLength, cipherTextBytes.Length - sizeof(int) - ivLength))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        // Generates a secure key using PBKDF2 algorithm
        private static byte[] GenerateKey(string key, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(key, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(KeySize / 8);
            }
        }
    }
}