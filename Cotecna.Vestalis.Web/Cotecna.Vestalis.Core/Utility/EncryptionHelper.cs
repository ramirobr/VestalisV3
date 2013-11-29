using System;
using System.Security.Cryptography;

namespace Cotecna.Vestalis.Core
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// Encrypts a specified text with AES algorithm
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptAes(string text)
        {

            byte[] salt = System.Text.Encoding.Default.GetBytes("95311567");
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes("Sb12XM32", salt);

            byte[] key = keyGenerator.GetBytes(16);
            byte[] iv = keyGenerator.GetBytes(16);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            ICryptoTransform transform = aes.CreateEncryptor(key, iv);

            byte[] buffer = System.Text.Encoding.Default.GetBytes(text);

            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //CryptoStream cs = new CryptoStream(ms, trans, CryptoStreamMode.Write);
            //cs.Write(buffer, 0, buffer.Length);
            //cs.FlushFinalBlock();
            byte[] result = transform.TransformFinalBlock(buffer, 0, buffer.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts an AES encrypted text with the same salt and password used with the EncryptAes method
        /// </summary>
        /// <param name="aesEncryptedText"></param>
        /// <returns></returns>
        public static string DecryptAes(string aesEncryptedText)
        {

            byte[] salt = System.Text.Encoding.Default.GetBytes("95311567");
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes("Sb12XM32", salt);

            byte[] key = keyGenerator.GetBytes(16);
            byte[] iv = keyGenerator.GetBytes(16);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            ICryptoTransform transform = aes.CreateDecryptor(key, iv);

            byte[] buffer = Convert.FromBase64String(aesEncryptedText);

            //System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            //CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read);
            //cs.Read(result, 0, result.Length);

            byte[] result = transform.TransformFinalBlock(buffer, 0, buffer.Length);
            return System.Text.Encoding.Default.GetString(result);
        }

        /// <summary>
        /// <para>An Url encoded string is decoded automatically when received by the controller. </para>
        /// <para>The string is decrypted and converted to Int32.</para>
        /// <para>Decrypts the AES encrypted text with the same salt and password used with the EncryptAes method</para>
        /// <para>Int32.MinValue is returned in case of an error.</para>
        /// </summary>
        /// <param name="aesEncryptedUrl"></param>
        /// <returns></returns>
        public static int DecryptAesIdentityKeyFromUrl(string aesEncryptedUrl)
        {
            try
            {
                string decryptedString = DecryptAes(aesEncryptedUrl);
                int id = Convert.ToInt32(decryptedString);
                return id;
            }
            catch
            {
                return Int32.MinValue;
            }
        }
    }
}