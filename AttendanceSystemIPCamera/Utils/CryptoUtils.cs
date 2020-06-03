using AttendanceSystemIPCamera.Services.NetworkService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera.Utils
{
    public static class CryptoUtils
    {
        private static AESKey secretKey = null;

        public static AESKey GetAESKeyInstance()
        {
            if (secretKey == null)
            {
                byte[] binaryKey = File.ReadAllBytes(Constant.AES_KEY_PATH);
                secretKey = binaryKey.Deserialize<AESKey>();
            }
            return secretKey;
        }

        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Binder = new AESKeyDeserializationBinder();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)bf.Deserialize(memStream);
                return obj;
            }
        }

        public static byte[] Encrypt(byte[] toEncrypt)
        {
            try
            {
                AESKey aes = CryptoUtils.GetAESKeyInstance();
                MemoryStream mStream = new MemoryStream();

                CryptoStream cStream = new CryptoStream(mStream,
                    new AesCryptoServiceProvider().CreateEncryptor(aes.Key, aes.IV),
                    CryptoStreamMode.Write);

                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                byte[] ret = mStream.ToArray();

                cStream.Close();
                mStream.Close();

                return ret;
            }
            catch (CryptographicException e)
            {
                throw e;
            }
        }

        public static byte[] Decrypt(byte[] data)
        {
            try
            {
                AESKey aes = CryptoUtils.GetAESKeyInstance();
                MemoryStream msDecrypt = new MemoryStream(data);

                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    new AesCryptoServiceProvider().CreateDecryptor(aes.Key, aes.IV),
                    CryptoStreamMode.Read);

                byte[] fromEncrypt = new byte[data.Length];

                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                throw e;
            }
        }
    }
}
