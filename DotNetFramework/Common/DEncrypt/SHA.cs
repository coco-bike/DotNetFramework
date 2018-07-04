using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.DEncrypt
{
    /// <summary>
    /// SHA-1
    /// </summary>
    public class SHA
    {
        /// <summary>
        /// 对输入字符串进行SHA-1计算，得到签名,并转换成16进制小写编码
        /// </summary>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        public static string GetSHAValue(string inputDate)
        {

            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputDate));
            StringBuilder secpwd = new StringBuilder();
            foreach (byte b in hash)
            {
                secpwd.AppendFormat("{0:x2}", b);
            }
            return secpwd.ToString();

        }
    }
}
