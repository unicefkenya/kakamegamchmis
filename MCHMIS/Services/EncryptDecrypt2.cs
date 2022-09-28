using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MCHMIS.Services
{
    public class EncryptDecrypt2
    {
        private const string initVector = "tu89geji340t89u2";
        private const int keysize = 256;
        public static string Encrypt(string Text, string Key)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Text);
            byte[] keyBytes;
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null))
            {
                keyBytes = password.GetBytes(keysize / 8);
            }
            ICryptoTransform encryptor;
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            }

            byte[] Encrypted;
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            Encrypted = memoryStream.ToArray();
            //cryptoStream.Close();
            cryptoStream.Dispose();
            //memoryStream.Dispose();


            return Convert.ToBase64String(Encrypted);
        }
        public static string Decrypt(string EncryptedText, string Key)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            //  byte[] DeEncryptedText = Convert.FromBase64String(EncryptedText);
            EncryptedText = EncryptedText.Replace(" ", "+");
            byte[] DeEncryptedText = Convert.FromBase64String(EncryptedText);
            byte[] keyBytes;
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null))
            {
                keyBytes = password.GetBytes(keysize / 8);
            }
            ICryptoTransform decryptor;
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            }

            byte[] plainTextBytes;
            int decryptedByteCount;
            using (MemoryStream memoryStream = new MemoryStream(DeEncryptedText))
            {
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                plainTextBytes = new byte[DeEncryptedText.Length];
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                //memoryStream.Close();

            }

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        public static string Encrypt(string Text)
        {
            try
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    return Encrypt(Text, "CVM2");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string Decrypt(string Text)
        {
            try
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    return Decrypt(Text, "CVM2");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}

