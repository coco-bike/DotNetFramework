using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.DEncrypt
{
    /// <summary>
    /// 公司自带的加密方法,MD5
    /// </summary>
    public class KeyAndReg_MD5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s">待加密字符串</param>
        /// <returns></returns>
        public static String MD5(String s)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }

        /// <summary>
        /// DES   Base64 加密
        /// </summary>
        /// <param name="input">明文字符串</param>
        /// <returns>已加密字符串</returns>
        public static string DesBase64Encrypt(string input)
        {
            System.Security.Cryptography.DES des = System.Security.Cryptography.DES.Create();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            byte[] Key = new byte[8] { 51, 50, 43, 16, 56, 37, 23, 49 };
            byte[] IV = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            ct = des.CreateEncryptor(Key, IV);

            byt = Encoding.GetEncoding("GB2312").GetBytes(input); //根据 GB2312 编码对字符串处理，转换成 byte 数组

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            byte[] answer = ms.ToArray();
            for (int j = 0; j < answer.Length; j++)
            {
                Console.Write(answer[j].ToString());
            }
            Console.WriteLine();
            return Convert.ToBase64String(ms.ToArray()); // 将加密的 byte 数组依照 Base64 编码转换成字符串
        }

        /// <summary>
        /// DES   Base64 解密
        /// </summary>
        /// <param name="input">密文字符串</param>
        /// <returns>解密字符串</returns>
        public static string DesBase64Decrypt(string input)
        {
            System.Security.Cryptography.DES des = System.Security.Cryptography.DES.Create();
            des.Mode = System.Security.Cryptography.CipherMode.ECB;
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            byte[] Key = new byte[8] { 51, 50, 43, 16, 56, 37, 23, 49 };
            byte[] IV = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            ct = des.CreateDecryptor(Key, IV);
            byt = Convert.FromBase64String(input); // 将 密文 以 Base64 编码转换成 byte 数组

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Encoding.GetEncoding("GB2312").GetString(ms.ToArray()); // 将 明文 以 GB2312 编码转换成字符串
        }
    }
}
