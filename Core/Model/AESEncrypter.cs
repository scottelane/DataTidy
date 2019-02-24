using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provide basic encryption functions based on Mud's code at http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp. This class is not intended to provide strong encryption.
    /// </summary>
    public class AESEncrypter
    {
        private static byte[] key = { 231, 45, 191, 150, 86, 172, 251, 238, 121, 56, 25, 27, 242, 252, 243, 189, 173, 167, 153, 148, 21, 162, 71, 146, 116, 149, 84, 116, 82, 87, 126, 205 };
        private static byte[] vector = { 204, 6, 198, 154, 206, 167, 13, 60, 129, 150, 213, 254, 189, 50, 202, 102 };
        private ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;

        public AESEncrypter()
        {
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            encoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {
            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }
    }
}
