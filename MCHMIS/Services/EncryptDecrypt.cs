using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MCHMIS.Services
{
    public interface IEncryptDecrypt
    {

        string DecryptString(string Message);
        string EncryptString(string Message);

    }
    public class EncryptDecrypt: IEncryptDecrypt
    {
        public string key = "DPKenyaDecrypt5656";
        public string DecryptString(string Message)
        {

            string Passphrase = key;
            if (Message != "")
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
                byte[] DataToDecrypt = Convert.FromBase64String(Message);
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }

                var base64EncodedBytes = System.Convert.FromBase64String(UTF8.GetString(Results));

                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                //  return UTF8.GetString(Results);
            }
            return "";
        }
        public string EncryptString(string Message)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Message);
            Message = System.Convert.ToBase64String(plainTextBytes);

            string Passphrase = key;
            if (Message != "")
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider { Key = TDESKey, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
                byte[] DataToEncrypt = UTF8.GetBytes(Message);
                try
                {
                    ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                return Convert.ToBase64String(Results);
            }
            return "";
        }

    }
}
