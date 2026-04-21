using System.Security.Cryptography;
using System.Text;

namespace RSign.Common.Helpers
{
    public static class EncryptDecryptQueryString
    {
        private static byte[] key = { };
        private static readonly byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            try
            {
                if (string.IsNullOrEmpty(stringToDecrypt)) return null;

                key = Encoding.UTF8.GetBytes(sEncryptionKey);
                var des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms,
                  des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static string Encrypt(string stringToEncrypt, string SEncryptionKey)
        {
            try
            {
                if (string.IsNullOrEmpty(stringToEncrypt)) return null;

                key = Encoding.UTF8.GetBytes(SEncryptionKey);
                var des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms,
                  des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
