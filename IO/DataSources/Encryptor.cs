using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.IO.DataSources
{
    internal static class Encryptor
    {
        private const string PassPhrase = "MegaManEngine";

        public static byte[] Encrypt(byte[] plainTextBytes)
        {
            var algorithm = GetAlgorithm();
            var encryptor = algorithm.CreateEncryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] cipherBytes)
        {
            var algorithm = GetAlgorithm();
            var decryptor = algorithm.CreateDecryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypts a string using a given password.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="password">The password.</param>
        public static string DecryptString(string cipherText, string password)
        {
            var algorithm = GetAlgorithm();
            var decryptor = algorithm.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
                return Encoding.Unicode.GetString(ms.ToArray());
            }
        }

        private static SymmetricAlgorithm GetAlgorithm()
        {
            var algorithm = Rijndael.Create();
            var rdb = new Rfc2898DeriveBytes(PassPhrase, new byte[] {
                0x53,0x6f,0x64,0x69,0x75,0x6d,0x20,             // salty goodness
                0x43,0x68,0x6c,0x6f,0x72,0x69,0x64,0x65
            });
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);
            return algorithm;
        }
    }
}
