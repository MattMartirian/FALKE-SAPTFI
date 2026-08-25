using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace SECURITY
{
    public sealed class Cifrador
    {
        #region Singleton
        private static readonly Lazy<Cifrador> lazyInstance = new Lazy<Cifrador>(() => new Cifrador());

        public static Cifrador CypherInstance => lazyInstance.Value;
        #endregion

        private readonly byte[] key;

        private const int AesKeySizeBits = 256;
        private const int AesBlockSizeBytes = 16;

        private Cifrador()
        {
            string keyBase64 = ConfigurationManager.AppSettings["FALKE_AES_KEY"];

            if (string.IsNullOrWhiteSpace(keyBase64))
            {
                throw new InvalidOperationException("No se encontró la variable de entorno FALKE_AES_KEY.");
            }

            key = Convert.FromBase64String(keyBase64);

            if (key.Length * 8 != AesKeySizeBits)
            {
                throw new InvalidOperationException($"La clave AES configurada no tiene el tamaño esperado ({AesKeySizeBits} bits).");
            }
        }

        public string Encoder(string toEncode)
        {
            if (toEncode == null) throw new ArgumentNullException(nameof(toEncode));

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(toEncode);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder stringBuilder = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }

        public string ReversibleEncrypt(string toEncode)
        {
            if (toEncode == null) throw new ArgumentNullException(nameof(toEncode));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.GenerateIV();

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                    {
                        swEncrypt.Write(toEncode);
                    }

                    // la clave iv se guarda junto al texto cifrado para poder usarlo en el descifrado y no tener q guardar el archivo
                    byte[] cipherBytes = msEncrypt.ToArray();
                    byte[] result = new byte[cipherBytes.Length + aesAlg.IV.Length];
                    Buffer.BlockCopy(cipherBytes, 0, result, 0, cipherBytes.Length);
                    Buffer.BlockCopy(aesAlg.IV, 0, result, cipherBytes.Length, aesAlg.IV.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        public string ReversibleDecrypt(string toDecode)
        {
            if (toDecode == null) throw new ArgumentNullException(nameof(toDecode));

            byte[] fullCipher;
            try
            {
                fullCipher = Convert.FromBase64String(toDecode);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException("El valor a descifrar no es Base64 válido.", ex);
            }

            if (fullCipher.Length <= AesBlockSizeBytes)
            {
                throw new CryptographicException("El valor cifrado no contiene datos suficientes.");
            }

            int cipherLength = fullCipher.Length - AesBlockSizeBytes;
            byte[] cipherBytes = new byte[cipherLength];
            byte[] iv = new byte[AesBlockSizeBytes];
            Buffer.BlockCopy(fullCipher, 0, cipherBytes, 0, cipherLength);
            Buffer.BlockCopy(fullCipher, cipherLength, iv, 0, AesBlockSizeBytes);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = iv;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                {
                    try
                    {
                        return srDecrypt.ReadToEnd();
                    }
                    catch (CryptographicException ex)
                    {
                        throw new CryptographicException("No fue posible descifrar el valor. Puede haberse alterado o haberse cifrado con otra clave.", ex);
                    }
                }
            }
        }
    }
}