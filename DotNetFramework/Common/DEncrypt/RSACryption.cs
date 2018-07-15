using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Common.DEncrypt
{
    /// <summary> 
    /// RSA加密解密及RSA签名和验证
    /// </summary> 
    public class RSACryption
    {
        public string strPublicKey = "";
        public string strPrivateKey = "";

        public RSACryption()
        {
            //strPublicKey = ConfigHelper.GetConfigString("PublicKey");
            //strPrivateKey = ConfigHelper.GetConfigString("PrivateKey");
            strPublicKey = @"<RSAKeyValue><Modulus>kI30/vq/MjDnlJ1oiuXdt/8cni/0+W9IKWHn2ASLUpDcLrBlPPf8LxerWGzRNvmrdB9rvV5zvURHZXxKbkE9zkronHll7M2CexIqBSJicvuVo+wH6y1eA0PTwXC7gkrUY6ppXZ4QIAnibM6H2NrhR0SMLwzIsV87B8DtcS4lr2s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            strPrivateKey = @"<RSAKeyValue><Modulus>kI30/vq/MjDnlJ1oiuXdt/8cni/0+W9IKWHn2ASLUpDcLrBlPPf8LxerWGzRNvmrdB9rvV5zvURHZXxKbkE9zkronHll7M2CexIqBSJicvuVo+wH6y1eA0PTwXC7gkrUY6ppXZ4QIAnibM6H2NrhR0SMLwzIsV87B8DtcS4lr2s=</Modulus><Exponent>AQAB</Exponent><P>uTFit4cg33o9kkCWnCVlzAXUjnEqsk805leWeW+Eh9kisuS60UJOGv0R4Cbg69sXlmqO0qngEiN9O4+D1nj0rQ==</P><Q>x9LmgIMBw8fPCxy0vegIkUJGyoC8USXMgKPBSUyYiXIMo/ILtoRvpBKCubblc1Ku5BffrswKrM18s2YF3dcfdw==</Q><DP>oECrAtNsz9WQyCkZ6x61exokd+pXnxrJVPXAIW8tyPxNeW5SdtggjFwnhsc95Pqe66tj0cnsOuX47XxgAkyCtQ==</DP><DQ>ndz46ygUedZdtFquU6V/NzrT8GF55OXmxO4Z4k2X6GXopJCxPjUsecOB8TQT22LD9kECHvblrBT+4j/EfLjaCw==</DQ><InverseQ>X4H/vht7RCIkHEHr3Hm7MG661b0WP0qBjj08atgcBRAM0+j+tcsg0ZchJiwJvnuY70O85sjLdW6CxT/4YKk+lA==</InverseQ><D>FMWpaoiKuAIqDQOPlcIRWdLZgGvAxqU0l+m/QJ3qjVtOoJDtg1TZUsUSdLryV3WKKz4mo8gyVoS8w7sYEP7kDA0I9+PA9mya5KR5FMtSWD8NQi5sr/17w65CFPGRbTDW5AOMYZSsx+ibosDynYRUF5Iw04/RvcQj0BeXiZZK2S0=</D></RSAKeyValue>";
        }


        #region RSA 加密解密 

        #region RSA 的密钥产生 

        /// <summary>
        /// RSA 的密钥产生 产生私钥 和公钥 
        /// </summary>
        /// <param name="xmlKeys"></param>
        /// <param name="xmlPublicKey"></param>
        public void RSAKey(out string xmlKeys, out string xmlPublicKey)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }
        #endregion

        #region RSA的加密函数 
        //############################################################################## 
        //RSA 方式加密 
        //说明KEY必须是XML的行式,返回的是字符串 
        //在有一点需要说明！！该加密方式有 长度 限制的！！ 
        //############################################################################## 

        //RSA的加密函数  string
        public string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {

            byte[] data = Encoding.GetEncoding("gb2312").GetBytes(m_strEncryptString);
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            int keySize = rsa.KeySize / 8;
            int bufferSize = keySize - 11;
            byte[] buffer = new byte[bufferSize];
            MemoryStream msInput = new MemoryStream(data);
            MemoryStream msOutput = new MemoryStream();
            int readLen = msInput.Read(buffer, 0, bufferSize);
            while (readLen > 0)
            {
                byte[] dataToEnc = new byte[readLen];
                Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                byte[] encData = rsa.Encrypt(dataToEnc, false);
                msOutput.Write(encData, 0, encData.Length);
                readLen = msInput.Read(buffer, 0, bufferSize);
            }
            msInput.Close();
            CypherTextBArray = msOutput.ToArray();    //得到加密结果
            msOutput.Close();
            rsa.Clear();
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;
        }

        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="resData">需要加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public string EncryptData(string publicKey, string resData)
        {
            byte[] DataToEncrypt = Encoding.ASCII.GetBytes(resData);
            string result = RSAEncrypt(publicKey, DataToEncrypt);
            return result;
        }

        //RSA的加密函数 byte[]
        public string RSAEncrypt(string xmlPublicKey, byte[] EncryptString)
        {

            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            CypherTextBArray = rsa.Encrypt(EncryptString, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;

        }
        #endregion

        #region RSA的解密函数 
        //RSA的解密函数  string
        public string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] dataEnc = Convert.FromBase64String(m_strDecryptString);   //加载密文
            rsa.FromXmlString(xmlPrivateKey);
            int keySize = rsa.KeySize / 8;
            byte[] buffer = new byte[keySize];
            MemoryStream msInput = new MemoryStream(dataEnc);
            MemoryStream msOutput = new MemoryStream();
            int readLen = msInput.Read(buffer, 0, keySize);
            while (readLen > 0)
            {
                byte[] dataToDec = new byte[readLen];
                Array.Copy(buffer, 0, dataToDec, 0, readLen);
                byte[] decData = rsa.Decrypt(dataToDec, false);
                msOutput.Write(decData, 0, decData.Length);
                readLen = msInput.Read(buffer, 0, keySize);
            }
            msInput.Close();
            byte[] result = msOutput.ToArray();    //得到解密结果
            msOutput.Close();
            rsa.Clear();
            Result = Encoding.GetEncoding("gb2312").GetString(result);
            return Result;

        }

        //RSA的解密函数  byte
        public string RSADecrypt(string xmlPrivateKey, byte[] DecryptString)
        {
            byte[] DypherTextBArray;
            string Result;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            DypherTextBArray = rsa.Decrypt(DecryptString, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;

        }
        #endregion

        #endregion

        #region RSA数字签名 

        #region 获取Hash描述表 
        //获取Hash描述表 
        public bool GetHash(string m_strSource, ref byte[] HashData)
        {
            //从字符串中取得Hash描述 
            byte[] Buffer;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);

            return true;
        }

        //获取Hash描述表 
        public bool GetHash(string m_strSource, ref string strHashData)
        {

            //从字符串中取得Hash描述 
            byte[] Buffer;
            byte[] HashData;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);

            strHashData = Convert.ToBase64String(HashData);
            return true;

        }

        //获取Hash描述表 
        public bool GetHash(System.IO.FileStream objFile, ref byte[] HashData)
        {

            //从文件中取得Hash描述 
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            return true;

        }

        //获取Hash描述表 
        public bool GetHash(System.IO.FileStream objFile, ref string strHashData)
        {

            //从文件中取得Hash描述 
            byte[] HashData;
            System.Security.Cryptography.HashAlgorithm MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();

            strHashData = Convert.ToBase64String(HashData);

            return true;

        }
        #endregion

        #region RSA签名 
        //RSA签名 
        public bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref byte[] EncryptedSignatureData)
        {

            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPrivate);
            System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            return true;

        }

        //RSA签名 
        public bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref string m_strEncryptedSignatureData)
        {

            byte[] EncryptedSignatureData;

            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPrivate);
            System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

            return true;

        }

        //RSA签名 
        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref byte[] EncryptedSignatureData)
        {

            byte[] HashbyteSignature;

            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPrivate);
            System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            return true;

        }

        //RSA签名 
        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
        {

            byte[] HashbyteSignature;
            byte[] EncryptedSignatureData;

            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPrivate);
            System.Security.Cryptography.RSAPKCS1SignatureFormatter RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);

            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);

            return true;

        }
        #endregion

        #region RSA 签名验证 

        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {

            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, byte[] DeformatterData)
        {

            byte[] HashbyteDeformatter;

            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);

            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, string p_strDeformatterData)
        {

            byte[] DeformatterData;

            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            DeformatterData = Convert.FromBase64String(p_strDeformatterData);

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {

            byte[] DeformatterData;
            byte[] HashbyteDeformatter;

            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
            System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();

            RSA.FromXmlString(p_strKeyPublic);
            System.Security.Cryptography.RSAPKCS1SignatureDeformatter RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");

            DeformatterData = Convert.FromBase64String(p_strDeformatterData);

            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        #endregion


        #endregion

        #region 从Cer中导入PublicKey
        /// <summary>
        /// 数字签名
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <returns>签名</returns>
        public string HashAndSignString(string plaintext, string certPath, string password = "")
        {
            //根据证书友好名称查找证书
            X509Certificate2 x509Certificate2 = GetX509Certificate2(certPath, password);
            if (x509Certificate2 != null)
            {
                //将证书中私钥转为rsa对象
                string xmlPublicKey = x509Certificate2.PublicKey.Key.ToXmlString(false);

                byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(plaintext);
                byte[] CypherTextBArray;
                string Result;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPublicKey);
                int keySize = rsa.KeySize / 8;
                int bufferSize = keySize - 11;
                byte[] buffer = new byte[bufferSize];
                MemoryStream msInput = new MemoryStream(data);
                MemoryStream msOutput = new MemoryStream();
                int readLen = msInput.Read(buffer, 0, bufferSize);
                while (readLen > 0)
                {
                    byte[] dataToEnc = new byte[readLen];
                    Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                    byte[] encData = rsa.Encrypt(dataToEnc, false);
                    msOutput.Write(encData, 0, encData.Length);
                    readLen = msInput.Read(buffer, 0, bufferSize);
                }
                msInput.Close();
                CypherTextBArray = msOutput.ToArray();    //得到加密结果
                msOutput.Close();
                rsa.Clear();
                Result = ByteToHexStr(CypherTextBArray);
                return Result;


            }
            else
            {
                //找不到证书  记录日志到本地，没有的可以删掉
                LogHelper.WriteLog("签名时在系统中找不到证书,明文：" + plaintext);
                return "";
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="signedData">签名</param>
        /// <returns></returns>
        public static bool VerifySigned(string plaintext, string signedData, string certPath, string password = "")
        {
            //根据证书友好名称查找证书
            X509Certificate2 x509Certificate2 = GetX509Certificate2(certPath, password);
            if (x509Certificate2 != null)
            {
                //将证书公钥转为rsa对象
                RSACryptoServiceProvider RSAalg = x509Certificate2.PublicKey.Key as RSACryptoServiceProvider;
                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                //将明文转byte[]
                byte[] dataToVerifyBytes = ByteConverter.GetBytes(plaintext);
                //将签名转byte[]
                byte[] signedDataBytes = Convert.FromBase64String(signedData);
                //验证签名
                bool isSuccess = RSAalg.VerifyData(dataToVerifyBytes, new SHA1CryptoServiceProvider(), signedDataBytes);
                RSAalg.Clear();
                RSAalg.Dispose();
                return isSuccess;
            }
            else
            {
                //找不到证书  记录日志到本地，没有的可以删掉
                LogHelper.WriteLog("验证签名时在系统中找不到证书,明文：" + plaintext + ",密文：" + signedData);
                return false;
            }
        }

        /// <summary>
        /// 获取证书对象 （楼主这用的是物理路径获取证书)
        /// </summary>
        /// <returns>为null即找不到证书</returns>
        public static X509Certificate2 GetX509Certificate2(string cerPath, string password)
        {
            //证书路径和密码放到了配置文件里   第一个参数是证书的绝对路径，第二个是证书密码，没有就空
            return new X509Certificate2(cerPath, password);
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        #endregion

        #region 从Pfx中导入PrivateKey
        /// <summary>
        /// 数字签名
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="pfxPath">pfx路径</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public string EncryptByPriPfxFile(string plaintext, string pfxPath, string password = "")
        {
            //根据证书友好名称查找证书
            X509Certificate2 x509Certificate2 = GetCertificateFromPfxFile(pfxPath, password);
            if (x509Certificate2 != null)
            {
                //将证书中私钥转为rsa对象
                string xmlPrivateKey = x509Certificate2.PrivateKey.ToXmlString(true);

                var Result = RSAEncryptByPrivateKey(xmlPrivateKey, plaintext);

                return Hex.ToHexString(Result);
            }
            else
            {
                //找不到证书  记录日志到本地，没有的可以删掉
                LogHelper.WriteLog("签名时在系统中找不到证书,明文：" + plaintext);
                return "";
            }

        }

        /// <summary>     
        /// 根据私钥证书得到证书实体，得到实体后可以根据其公钥和私钥进行加解密     
        /// 加解密函数使用DEncrypt的RSACryption类     
        /// </summary>     
        /// <param name="pfxFileName"></param>     
        /// <param name="password"></param>     
        /// <returns></returns>     
        public static X509Certificate2 GetCertificateFromPfxFile(string pfxFileName,
            string password)
        {
            try
            {
                return new X509Certificate2(pfxFileName, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(e);
                return null;
            }
        }

        /// <summary>用私钥给数据进行RSA加密  
        ///   
        /// </summary>  
        /// <param name="xmlPrivateKey">私钥</param>  
        /// <param name="m_strEncryptString">待加密数据</param>  
        /// <returns>加密后的数据（Base64）</returns>  
        public static byte[] RSAEncryptByPrivateKey(string xmlPrivateKey, string strEncryptString)
        {
            //加载私钥  
            RSACryptoServiceProvider privateRsa = new RSACryptoServiceProvider();
            privateRsa.FromXmlString(xmlPrivateKey);

            //转换密钥  
            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateRsa);

            IBufferedCipher c = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");// 参数与Java中加密解密的参数一致       

            c.Init(true, keyPair.Private); //第一个参数为true表示加密，为false表示解密；第二个参数表示密钥
            byte[] DataToEncrypt = Encoding.UTF8.GetBytes(strEncryptString);

            int bufferSize = c.GetBlockSize();//获取分段长度
            byte[] buffer = new byte[bufferSize];
            MemoryStream msInput = new MemoryStream(DataToEncrypt);
            MemoryStream msOutput = new MemoryStream();
            int readLen = msInput.Read(buffer, 0, bufferSize);
            while (readLen > 0)
            {
                byte[] dataToEnc = new byte[readLen];
                Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                byte[] encData = c.DoFinal(dataToEnc);
                msOutput.Write(encData, 0, encData.Length);
                readLen = msInput.Read(buffer, 0, bufferSize);
            }
            msInput.Close();
            byte[] outBytes = msOutput.ToArray();    //得到加密结果
            msOutput.Close();
            privateRsa.Clear();
            return outBytes;
        }
        #endregion
    }


    /// <summary>
    /// RSA加密和验证签名
    /// </summary>
    public class EncryptUtil
    {
        /// <summary>
        /// 将SortedDictionary中的键值拼接成一个大字符串，然后使用RSA生成签名
        /// </summary>
        /// <returns></returns>
        public static string handleRSA(SortedDictionary<string, object> sd, string privatekey, string type)
        {
            StringBuilder sbuffer = new StringBuilder();
            foreach (KeyValuePair<string, object> item in sd)
            {
                sbuffer.Append(item.Value);
            }
            string sign = "";
            //生成签名
            sign = RSAFromPkcs8.sign(sbuffer.ToString(), privatekey, type);
            return sign;
        }
        /// <summary>
        /// 字符串RSA公钥加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privatekey"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string handleRSA(string data, string publicKey, string type)
        {
            string sign = "";
            //生成签名
            sign = RSAFromPkcs8.encryptData(data.ToString(), publicKey, type);
            return sign;
        }



        /// <summary>
        /// 对一键支付返回的业务数据进行验签
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encrypt_key"></param>
        /// <param name="yibaoPublickKey"></param>
        /// <param name="merchantPrivateKey"></param>
        /// <returns></returns>
        public static bool checkDecryptAndSign(string data, string encrypt_key, string yibaoPublickKey, string merchantPrivateKey)
        {
            /** 1.使用YBprivatekey解开aesEncrypt。 */
            string AESKey = "";
            try
            {
                AESKey = RSAFromPkcs8.decryptData(encrypt_key, merchantPrivateKey, "UTF-8");
            }
            catch (Exception)
            {
                /** AES密钥解密失败 */
                return false;
            }

            /** 2.用aeskey解开data。取得data明文 */
            string realData = AES.Decrypt(data, AESKey);


            //amount  有问题 
            SortedDictionary<string, object> sd = Newtonsoft.Json.JsonConvert.DeserializeObject<SortedDictionary<string, object>>(realData);

            //
            JavaScriptSerializer jss = new JavaScriptSerializer();
            SortedDictionary<string, object> sd_temp = jss.Deserialize<SortedDictionary<string, object>>(realData);


            /** 3.取得data明文sign。 */
            string sign = (string)sd["sign"];

            /** 4.对map中的值进行验证 */
            StringBuilder signData = new StringBuilder();
            foreach (var item in sd)
            {
                /** 把sign参数隔过去 */
                if (item.Key == "sign")
                {
                    continue;
                }
                /** 把amount提出来*/
                if (item.Key == "amount")
                {
                    signData.Append(sd_temp[item.Key]);
                    continue;
                }
                signData.Append(item.Value);
            }

            signData = signData.Replace("\r", "");
            signData = signData.Replace("\n", "");
            signData = signData.Replace("    ", "");
            signData = signData.Replace("  ", "");
            signData = signData.Replace("\": \"", "\":\"");
            signData = signData.Replace("\": ", "\":");

            /**5. result为true时表明验签通过 */
            bool result = RSAFromPkcs8.checkSign(Convert.ToString(signData), sign, yibaoPublickKey, "UTF-8");

            return result;
        }

        /// <summary>
        /// 一键支付回调信息解密
        /// </summary>
        /// <param name="data">返回的data</param>
        /// <param name="encryptkey">返回的encryptkey</param>
        /// <param name="yibaoPublickey">易宝公钥</param>
        /// <param name="merchantPrivatekey">商户私钥</param>
        /// <param name="type">编码格式</param>
        /// <returns></returns>
        public static string checkYbCallbackResult(string data, string encryptkey, string yibaoPublickey, string merchantPrivatekey, string type)
        {
            string yb_encryptkey = encryptkey;
            string yb_data = data;
            //将易宝返回的结果进行验签名
            bool passSign = EncryptUtil.checkDecryptAndSign(yb_data, yb_encryptkey, yibaoPublickey, merchantPrivatekey);
            if (passSign)
            {
                string yb_aeskey = RSAFromPkcs8.decryptData(yb_encryptkey, merchantPrivatekey, type);

                string payresult_view = AES.Decrypt(yb_data, yb_aeskey);
                //返回易宝支付回调的业务数据明文
                return payresult_view;
            }
            else
            {
                return "验签未通过";
            }

        }


        public static string checkYbCallbackResultDocument(string data, string encryptkey, string yibaoPublickey, string merchantPrivatekey, string type)
        {
            string yb_encryptkey = encryptkey;
            string yb_data = data;
            //将易宝返回的结果进行验签名
            bool passSign = EncryptUtil.checkDecryptAndSignDocument(yb_data, yb_encryptkey, yibaoPublickey, merchantPrivatekey);
            if (passSign)
            {
                string yb_aeskey = RSAFromPkcs8.decryptData(yb_encryptkey, merchantPrivatekey, type);

                string payresult_view = AES.Decrypt(yb_data, yb_aeskey);
                //返回易宝支付回调的业务数据明文
                return payresult_view;
            }
            else
            {
                return "验签未通过";
            }

        }

        /// <summary>
        /// 下载文件验证签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encrypt_key"></param>
        /// <param name="yibaoPublickKey"></param>
        /// <param name="merchantPrivateKey"></param>
        /// <returns></returns>
        public static bool checkDecryptAndSignDocument(string data, string encrypt_key, string yibaoPublickKey, string merchantPrivateKey)
        {
            /** 1.使用YBprivatekey解开aesEncrypt。 */
            string AESKey = "";
            try
            {
                AESKey = RSAFromPkcs8.decryptData(encrypt_key, merchantPrivateKey, "UTF-8");
            }
            catch (Exception)
            {
                /** AES密钥解密失败 */
                return false;
            }

            /** 2.用aeskey解开data。取得data明文 */
            string realData = AES.Decrypt(data, AESKey);


            SortedDictionary<string, object> sd = Newtonsoft.Json.JsonConvert.DeserializeObject<SortedDictionary<string, object>>(realData);


            /** 3.取得data明文sign。 */
            string sign = (string)sd["sign"];

            /** 4.对map中的值进行验证 */
            StringBuilder signData = new StringBuilder();
            foreach (var item in sd)
            {
                /** 把sign参数隔过去 */
                if (item.Key == "sign")
                {
                    continue;
                }
                signData.Append(item.Value);
            }

            /**5. result为true时表明验签通过 */
            bool result = RSAFromPkcs8.checkSign(Convert.ToString(signData), sign, yibaoPublickKey, "UTF-8");

            return result;
        }
    }

    /// <summary>
    /// 类名：RSAFromPkcs8
    /// 功能：RSA加密、解密、签名、验签
    /// 详细：该类对Java生成的密钥进行解密和签名以及验签专用类，不需要修改
    /// 版本：1.0
    /// 日期：2013-07-08
    /// </summary>
    public sealed class RSAFromPkcs8
    {
        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="content">待签名字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>签名后字符串</returns>
        public static string sign(string content, string privateKey, string input_charset)
        {
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] signData = rsa.SignData(Data, sh);
            return Convert.ToBase64String(signData);
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="content">待验签字符串</param>
        /// <param name="signedString">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>true(通过)，false(不通过)</returns>
        public static bool checkSign(string content, string signedString, string publicKey, string input_charset)
        {
            bool result = false;
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            byte[] data = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(Data, "SHA1", data);
            return result;
        }




        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="resData">需要加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string encryptData(string resData, string publicKey, string input_charset)
        {
            byte[] DataToEncrypt = Encoding.ASCII.GetBytes(resData);
            string result = encrypt(DataToEncrypt, publicKey, input_charset);
            return result;
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="resData">加密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string decryptData(string resData, string privateKey, string input_charset)
        {
            byte[] DataToDecrypt = Convert.FromBase64String(resData);
            string result = "";
            for (int j = 0; j < DataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {

                    buf[i] = DataToDecrypt[i + 128 * j];
                }
                result += decrypt(buf, privateKey, input_charset);
            }
            return result;
        }

        #region 内部方法

        private static string encrypt(byte[] data, string publicKey, string input_charset)
        {
            RSACryptoServiceProvider rsa = DecodePemPublicKey(publicKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] result = rsa.Encrypt(data, false);

            return Convert.ToBase64String(result);
        }

        private static string decrypt(byte[] data, string privateKey, string input_charset)
        {
            string result = "";
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] source = rsa.Decrypt(data, false);
            char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(source, 0, source.Length)];
            Encoding.GetEncoding(input_charset).GetChars(source, 0, source.Length, asciiChars, 0);
            result = new string(asciiChars);
            //result = ASCIIEncoding.ASCII.GetString(source);
            return result;
        }

        private static RSACryptoServiceProvider DecodePemPublicKey(String pemstr)
        {
            byte[] pkcs8publickkey;
            pkcs8publickkey = Convert.FromBase64String(pemstr);
            if (pkcs8publickkey != null)
            {
                RSACryptoServiceProvider rsa = DecodeRSAPublicKey(pkcs8publickkey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemstr);
            if (pkcs8privatekey != null)
            {
                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)	//expect an Octet string 
                    return null;

                bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(publickey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }



            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion

        #region 解析.net 生成的Pem
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {

            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion

    }
}
