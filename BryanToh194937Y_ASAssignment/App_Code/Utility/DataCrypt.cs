using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Security.Cryptography;

namespace BryanToh194937Y_ASAssignment.App_Code.Utility
{
    public static class DataCrypt
    {
        /// <summary>
        /// Takes in a encoded ciphertext, encoded initialization vector and encoded decryption key bytes
        /// All encoding are in base64
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name=""></param>
        /// <returns>Decrypted Plaintext in String</returns>
        public static string Decrypt(string cipherText, string iv, string key)
        {
            // converts back to bytes
            byte[] bCipherText = Convert.FromBase64String(cipherText);
            byte[] IV = Convert.FromBase64String(iv);
            byte[] Key = Convert.FromBase64String(key);

            string plainText = "";

            if (bCipherText == null || bCipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            using (RijndaelManaged cipherAlgorithm = new RijndaelManaged())
            {
                cipherAlgorithm.IV = IV;
                cipherAlgorithm.Key = Key;
                ICryptoTransform decryptTransform = cipherAlgorithm.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(bCipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }
    }
}