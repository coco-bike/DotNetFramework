using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;

namespace ThirdParty.MyMVCWeixin.Common
{
    public class Utils
    {
        #region HTTP请求相关操作


        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <returns>响应信息</returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";//设置请求的方法
            request.Accept = "*/*";//设置Accept标头的值
            string responseStr = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())//获取响应
            {
                using (StreamReader reader =
        new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            return responseStr;
        }

        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        public static string HttpPost(string url, string param, string certpath = "", string certpwd = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            if (!string.IsNullOrEmpty(certpath) && !string.IsNullOrEmpty(certpwd))
            {
                X509Certificate2 cer = new X509Certificate2(certpath, certpwd,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                request.ClientCertificates.Add(cer);
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (StreamWriter requestStream =
            new StreamWriter(request.GetRequestStream()))
            {
                requestStream.Write(param);//将请求的数据写入请求流
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();//获取响应
                }
            }
            return responseStr;
        }

        /// <summary>
        /// HttpPost上传文件流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string HttpPost(string url, Stream stream)
        {
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (var reqstream = request.GetRequestStream())
            {
                stream.Position = 0L;
                stream.CopyTo(reqstream);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();//获取响应
                }
            }
            return responseStr;
        }

        // /// <summary>
        // /// 表单Post请求
        // /// </summary>
        // /// <param name="url"></param>
        // /// <param name="form"></param>
        // /// <returns></returns>
        // public static string HttpPostForm(string url, List<FormEntity> form)
        // {
        //     //分割字符串
        //     var boundary = "----" + DateTime.Now.Ticks.ToString("x");
        //     HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //     request.Method = "POST";
        //     ServicePointManager.ServerCertificateValidationCallback =
        //        new RemoteCertificateValidationCallback((a, b, c, d) => true);
        //     request.ContentType = "multipart/form-data; boundary=" + boundary;
        //     MemoryStream stream = new MemoryStream();
        //     #region 将非文件表单写入内存流
        //     foreach (var entity in form.Where(f => f.IsFile == false))
        //     {
        //         var temp = new StringBuilder();
        //         temp.AppendFormat("\r\n--{0}", boundary);
        //         temp.Append("\r\nContent-Disposition: form-data;");
        //         temp.AppendFormat("name=\"{0}\"", entity.Name);
        //         temp.Append("\r\n\r\n");
        //         temp.Append(entity.Value);
        //         byte[] b = Encoding.UTF8.GetBytes(temp.ToString());
        //         stream.Write(b, 0, b.Length);
        //     }
        //     #endregion
        //     #region 将文件表单写入内存流
        //     foreach (var entity in form.Where(f => f.IsFile == true))
        //     {
        //         byte[] filedata = null;
        //         var filename = Path.GetFileName(entity.Value);
        //         //表示是网络资源
        //         if (entity.Value.Contains("http"))
        //         {
        //             //处理网络文件
        //             using (var client = new WebClient())
        //             {
        //                 filedata = client.DownloadData(entity.Value);
        //             }
        //         }
        //         else
        //         {
        //             //处理物理路径文件
        //             using (FileStream file = new FileStream(entity.Value,
        //             FileMode.Open, FileAccess.Read))
        //             {
        //                 filedata = new byte[file.Length];
        //                 file.Read(filedata, 0, (int)file.Length);
        //             }
        //         }
        //         var temp = string.Format("\r\n--{0}\r\nContent-Disposition: " +
        //         "form-data; name=\"{1}\"; filename=\"{2}\"\r\n\r\n",
        //            boundary, entity.Name, filename);
        //         byte[] b = Encoding.UTF8.GetBytes(temp);
        //         stream.Write(b, 0, b.Length);
        //         stream.Write(filedata, 0, filedata.Length);
        //     }
        //     #endregion
        //     //结束标记
        //     byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary +
        //     "--\r\n");
        //     stream.Write(foot_data, 0, foot_data.Length);
        //     Stream reqStream = request.GetRequestStream();
        //     stream.Position = 0L;
        //     //将Form表单生成流写入请求流
        //     stream.CopyTo(reqStream);
        //     stream.Close();
        //     reqStream.Close();
        //     using (HttpWebResponse response = (HttpWebResponse)request.
        //     GetResponse())
        //     {
        //         using (StreamReader reader =
        //new StreamReader(response.GetResponseStream(), Encoding.UTF8))
        //         {
        //             return reader.ReadToEnd();//获取响应
        //         }
        //     }
        // }

        /// <summary>
        /// 通过文件下载Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        public static void DownLoadByPost(string url, string data, Stream stream)
        {
            using (var webclient = new WebClient())
            {
                var retdata = webclient.UploadData(url, "POST", Encoding.UTF8.GetBytes(data));
                stream.Write(retdata, 0, retdata.Length);
            }
        }

        ///// <summary>
        ///// 根据文件路径转换成文件流
        ///// </summary>
        ///// <param name="filepath"></param>
        ///// <returns></returns>
        //public static FileStreamInfo GetFileStreamInfo(string filepath)
        //{
        //    var fsi = new FileStreamInfo();
        //    if (!filepath.Contains("http"))//判断是否是网络路径
        //    {
        //        //获取文件流
        //        using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.
        //        Read))
        //        {
        //            fs.Position = 0L;
        //            fs.CopyTo(fsi);
        //        }
        //    }
        //    else
        //    {
        //        //获取文件流
        //        using (var client = new WebClient())
        //        {
        //            var bytes = client.DownloadData(filepath);
        //            fsi.Write(bytes, 0, bytes.Length);
        //        }
        //    }
        //    fsi.FileName = Path.GetFileName(filepath);//获取文件名
        //    return fsi;
        //}

        /// <summary>
        /// 泛型返回数据类
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="stream">文件流</param>
        /// <param name="url">保存物理地址</param>
        /// <returns></returns>
        public static T PostResult<T>(Stream stream, string url)
        {
            var retdata = HttpPost(url, stream);
            return JsonConvert.DeserializeObject<T>(retdata);
        }


        /// <summary>
        /// 发起POST请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="obj">数据实体</param>
        /// <param name="url">接口地址</param>
        public static T PostResult<T>(object obj, string url)
        {
            //序列化设置
            var setting = new JsonSerializerSettings();
            //解决枚举类型序列化时，被转换成数字的问题
            setting.Converters.Add(new StringEnumConverter());
            var retdata = HttpPost(url, JsonConvert.SerializeObject(obj, setting));
            return JsonConvert.DeserializeObject<T>(retdata);
        }

        /// <summary>
        /// 发起postForm请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="formEntities">数据实体</param>
        /// <param name="url">接口地址</param>
        //public static T PostFormResult<T>(List<FormEntity> formEntities,
        // string url)
        //{
        //    var retdata = HttpPostForm(url, formEntities);
        //    return JsonConvert.DeserializeObject<T>(retdata);
        //}

        /// <summary>
        /// 发起GET请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">接口地址</param>
        public static T GetResult<T>(string url)
        {
            var retdata = HttpGet(url);
            return JsonConvert.DeserializeObject<T>(retdata);
        }
        #endregion

        //    /// <summary>
        //    /// 下载文件流到客户端
        //    /// </summary>
        //    /// <param name="stream">文件流</param>
        //    public static void DownLoadSteam(FileStreamInfo stream)
        //    {
        //        var bytes = stream.ToArray();
        //        HttpContext.Current.Response.ContentType = "application/octet-stream";
        //        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" +
        //HttpContext.Current.Server.UrlDecode(stream.FileName));
        //        HttpContext.Current.Response.AddHeader("Content-Length",
        //    bytes.Length.ToString());
        //        HttpContext.Current.Response.BinaryWrite(bytes);
        //        HttpContext.Current.Response.Flush();
        //        HttpContext.Current.Response.End();
        //    }

        //    /// <summary>
        //    /// 下载文件
        //    /// </summary>
        //    /// <param name="fileUrl">文件路径.可为网络资源，也可以是文件物理路径</param>
        //    /// <param name="fileName">文件名</param>
        //    public static void DownLoadFile(string fileUrl, string fileName)
        //    {
        //        using (var client = new WebClient())
        //        {
        //            //将网络资源下载为二进制数据
        //            var bytes = client.DownloadData(fileUrl);
        //            using (var fsi = new FileStreamInfo())
        //            {
        //                //将二进制数据写入文件流
        //                fsi.Write(bytes, 0, bytes.Length);
        //                fsi.FileName = fileName;
        //                DownLoadSteam(fsi);
        //            }
        //        }
        //    }


        /// <summary>
        /// 获取当前请求的数据包内容
        /// </summary>
        public static string GetRequestData()
        {
            //获取传入的 HTTP 实体主体的内容	
            using (var stream = HttpContext.Current.Request.InputStream)
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        ///// <summary>
        ///// 将XML转换成对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="xmlstr"></param>
        ///// <returns></returns>
        //public static T ConvertObj<T>(string xmlstr)
        //{
        //    try
        //    {
        //        XElement xdoc = XElement.Parse(xmlstr);
        //        //获取转换的数据类型
        //        var type = typeof(T);
        //        //创建实例
        //        var t = Activator.CreateInstance<T>();
        //        #region 基础属性赋值
        //        var ToUserName = type.GetProperty("ToUserName");
        //        ToUserName.SetValue(t, Convert.ChangeType(xdoc.Element("ToUserName").Value, ToUserName.PropertyType), null);
        //        xdoc.Element("ToUserName").Remove();

        //        var FromUserName = type.GetProperty("FromUserName");
        //        FromUserName.SetValue(t, Convert.ChangeType(xdoc.Element("FromUserName").Value, FromUserName.PropertyType), null);
        //        xdoc.Element("FromUserName").Remove();

        //        var CreateTime = type.GetProperty("CreateTime");
        //        CreateTime.SetValue(t, Convert.ChangeType(xdoc.Element("CreateTime").Value, CreateTime.PropertyType), null);
        //        xdoc.Element("CreateTime").Remove();

        //        var MsgType = type.GetProperty("MsgType");
        //        string msgtype = xdoc.Element("MsgType").Value.ToUpper();
        //        MsgType.SetValue(t, (MsgType)Enum.Parse(typeof(MsgType), msgtype), null);
        //        xdoc.Element("MsgType").Remove();

        //        //判断消息类型是否是事件
        //        if (msgtype == "EVENT")
        //        {
        //            //获取事件类型
        //            var EventType = type.GetProperty("Event");
        //            string eventtype = xdoc.Element("Event").Value.ToUpper();
        //            EventType.SetValue(t, (EventType)Enum.Parse(typeof(EventType), eventtype), null);
        //            xdoc.Element("Event").Remove();
        //        }
        //        #endregion


        //        //遍历XML节点
        //        foreach (XElement element in xdoc.Elements())
        //        {
        //            if (msgtype == "EVENT")
        //            {
        //                if (element.Name == "ScanCodeInfo")
        //                {
        //                    type.GetProperty("ScanType").SetValue(t, Convert.ChangeType(element.Element("ScanType").Value, TypeCode.String), null);
        //                    type.GetProperty("ScanResult").SetValue(t, Convert.ChangeType(element.Element("ScanResult").Value, TypeCode.String), null);
        //                    continue;
        //                }
        //                if (element.Name == "SendPicsInfo")
        //                {
        //                    type.GetProperty("Count").SetValue(t, Convert.ChangeType(element.Element("Count").Value, TypeCode.Int32), null);
        //                    List<string> picMd5List = new List<string>();
        //                    foreach (XElement xElement in element.Element("PicList").Elements())
        //                    {
        //                        picMd5List.Add(xElement.Element("PicMd5Sum").Value);
        //                    }
        //                    type.GetProperty("PicMd5SumList").SetValue(t, picMd5List, null);
        //                    continue;
        //                }
        //            }
        //            //根据XML节点的名称，获取实体的属性
        //            var pr = type.GetProperty(element.Name.ToString());
        //            if (pr != null)
        //                //给属性赋值
        //                pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
        //        }
        //        return t;
        //    }
        //    catch (Exception)
        //    {
        //        return default(T);
        //    }
        //}


        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.
             ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static int GetTimeStamp()
        {
            return ConvertDateTimeInt(DateTime.Now);
        }
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        ///// <summary>
        ///// 生成二维码流
        ///// </summary>
        ///// <param name="qrcontent">二维码的内容</param>
        ///// <param name="size">生成的二维码的尺寸。单位是像素</param>
        ///// <returns>内存流。可保存为文件，或下载到客户端</returns>
        //public static FileStreamInfo GetQrCodeStream(string qrcontent, int size)
        //{
        //    //误差校正水平
        //    ErrorCorrectionLevel ecLevel = ErrorCorrectionLevel.M;
        //    //空白区域
        //    QuietZoneModules quietZone = QuietZoneModules.Zero;
        //    int ModuleSize = size;//大小
        //    Gma.QrCodeNet.Encoding.QrCode qrCode;
        //    var encoder = new QrEncoder(ecLevel);
        //    //对内容进行编码，并保存生成的矩阵
        //    if (encoder.TryEncode(qrcontent, out qrCode))
        //    {
        //        var render = new GraphicsRenderer(new FixedCodeSize(ModuleSize, quietZone));
        //        var stream = new FileStreamInfo();
        //        render.WriteToStream(qrCode.Matrix, ImageFormat.Jpeg, stream);
        //        stream.FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
        //        return stream;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 获取微信发送过来的数据包，并处理加解密问题。如果进行了加密，则将密文解密后返回，否则直接返回接收到的字符
        /// </summary>
        /// <param name="param">加解密接入参数</param>
        /// <returns>如果进行了加密，则将密文解密后返回；否则直接返回接收到的字符</returns>
        //public static string GetRequestData(EnterParam param)
        //{
        //    //获取当前请求的原始数据包
        //    var reqdata = GetRequestData();
        //    var timestamp = HttpContext.Current.Request.QueryString["timestamp"];
        //    var nonce = HttpContext.Current.Request.QueryString["nonce"];
        //    var msg_signature =
        //HttpContext.Current.Request.QueryString["msg_signature"];
        //    var encrypt_type = HttpContext.Current.Request.QueryString["encrypt_type"];
        //    string postStr = null;
        //    if (encrypt_type == "aes")
        //    {
        //        //如果进行了加密，则加密成功后，直接返回解密后的明文。解密失败则返回null
        //        param.IsAes = true;
        //        var ret = new WXBizMsgCrypt(param.token, param.EncodingAESKey,
        //        param.appid);
        //        if (ret.DecryptMsg(msg_signature, timestamp, nonce, reqdata, ref
        //postStr) != 0)
        //        {
        //            return null;
        //        }
        //        return postStr;
        //    }
        //    else
        //    {
        //        param.IsAes = false;
        //        return reqdata;
        //    }

        //}
        #region URL编码
        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        #endregion
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pwd">要加密的字符串</param>
        /// <param name="encoding">字符编码方法。默认utf-8</param>
        /// <returns>加密后的密文</returns>
        public static string MD5(string pwd, string encoding = "utf-8")
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.GetEncoding(encoding).GetBytes(pwd);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }
        /// <summary>
        /// 获取当前请请求的URL
        /// </summary>
        /// <returns></returns>
        public static string GetRequestUrl()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }
        #region 微信支付相关
        /// <summary>
        /// 将实体对象转换成键值对。移除值为null的属性
        /// </summary>
        /// <param name="obj">参数实体</param>
        /// <returns>数据键值对</returns>
        public static Dictionary<string, string> EntityToDictionary(object obj, int? index = null)
        {
            var type = obj.GetType();
            var dic = new Dictionary<string, string>();
            var pis = type.GetProperties();
            foreach (var pi in pis)
            {
                //获取属性的值
                var val = pi.GetValue(obj, null);
                //移除值为null，以及字符串类型的值为空字符的。另外，签名字符串本身不参与签名，
                //在验证签名正确性时，需移除sign
                if (val == null || val.ToString() == "" || pi.Name == "sign" ||
              pi.PropertyType.IsGenericType)
                    continue;
                if (index != null)
                {
                    dic.Add(pi.Name + "_" + index, val.ToString());
                }
                else
                {
                    dic.Add(pi.Name, val.ToString());
                }
            }
            var classlist = pis.Where(p => p.PropertyType.IsGenericType);
            foreach (var info in classlist)
            {
                var val = info.GetValue(obj, null);
                if (val == null)
                {
                    continue;
                }
                int count = (int)info.PropertyType.GetProperty("Count").GetValue(val, null);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        object ol = info.PropertyType.GetMethod("get_Item").Invoke
                        (val, new object[] { i });
                        var tem = EntityToDictionary(ol, i);//递归调用
                        foreach (var t in tem)
                        {
                            dic.Add(t.Key, t.Value);
                        }
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取支付签名
        ///</summary>
        ///<param name="dictionary">数据集合</param>
        ///<param name="key">支付密钥</param>
        ///<returns>签名</returns>
        public static string GetPaySign(Dictionary<string, string> dictionary,
        string key)
        {
            var arr = dictionary.OrderBy(d => d.Key).
                Select(d => string.Format("{0}={1}", d.Key, d.Value)).ToArray();
            string stringA = string.Join("&", arr);
            return MD5(string.Format("{0}&key={1}", stringA, key)).ToUpper();
        }
        /// <summary>
        /// 获取支付签名
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPaySign(object obj, string key)
        {
            var dic = EntityToDictionary(obj);
            return GetPaySign(dic, key);
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="obj">参数集实体</param>
        /// <param name="sign">签名</param>
        /// <param name="key">支付密钥</param>
        /// <returns></returns>
        public static bool ValidSign(object obj, string sign, string key)
        {
            var tempsign = GetPaySign(obj, key);
            return tempsign == sign;
        }
        /// <summary>
        /// 获取32位GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 集合转换成XML
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string parseXML(Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            var arr = parameters.Select(p => string.Format(Regex.IsMatch(p.Value,
            @"^[0-9.]$") ? "<{0}>{1}</{0}>" : "<{0}><![CDATA[{1}]]></{0}>", p.Key,
            p.Value));
            sb.Append(string.Join("", arr));
            sb.Append("</xml>");
            return sb.ToString();
        }
        /// <summary>
        /// XML转换成实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlToEntity<T>(string xml)
        {
            var type = typeof(T);
            //创建实例
            var t = Activator.CreateInstance<T>();
            var pr = type.GetProperties();
            var xdoc = XElement.Parse(xml);
            var eles = xdoc.Elements();
            var ele = eles.Where(e => new Regex(@"_\d{1,}$").IsMatch(e.Name.
            ToString()));//获取带下标的节点
            if (ele.Count() > 0)
            {
                var selele = ele.Select(e =>
                {
                    var temp = e.Name.ToString().Split('_');
                    var index = int.Parse(temp[temp.Length - 1]);
                    return new
                    {
                        Index = index,
                        Property = e.Name.ToString().
                    Replace("_" + index.ToString(), ""),
                        Value = e.Value
                    };
                });//转换为方便操作的匿名对象。
                var max = selele.Max(m => m.Index);//获取最大索引的值
                var infos = pr.FirstOrDefault(f => f.PropertyType.IsGenericType);
                //获取类型为泛型的属性
                if (infos != null)
                {
                    var infotype = infos.PropertyType.GetGenericArguments().First();                            //获取泛型的真实类型
                    Type listType = typeof(List<>).MakeGenericType(new[] { infotype });                         //创建泛型列表
                    var datalist = Activator.CreateInstance(listType);//创建对象
                    var infoprs = infotype.GetProperties();
                    for (int j = 0; j <= max; j++)
                    {
                        var temp = Activator.CreateInstance(infotype);
                        var list = selele.Where(s => s.Index == 0);
                        foreach (var v in list)
                        {
                            var p = infoprs.FirstOrDefault(f => f.Name == v.Property);
                            if (p == null) continue;
                            p.SetValue(temp, v.Value, null);
                        }
                        listType.GetMethod("Add").Invoke((object)datalist, new[] { temp });//将对象添加到列表中
                    }
                    infos.SetValue(t, datalist, null);//最后给泛型属性赋值
                }
                ele.Remove();//将有下标的节点从集合中移除
            }
            foreach (var element in eles)
            {
                var p = pr.First(f => f.Name == element.Name);
                p.SetValue(t, Convert.ChangeType(element.Value, p.PropertyType),
                null);
            }
            return t;
        }

        /// <summary>
        /// 获取微信版本号，返回0时，说明不是微信
        /// </summary>
        /// <returns></returns>
        public static decimal GetWxVersion()
        {
            var ua = HttpContext.Current.Request.UserAgent;
            Regex reg = new Regex(@"MicroMessenger/(\d+).(\d+)");
            var result = reg.Match(ua).Groups;
            var temparr = result[0].Value.Split('/');
            return temparr.Length == 2 ? decimal.Parse(temparr[1]) : 0;
        }

        /// <summary>
        /// 获得当前服务器端的IP
        /// </summary>
        /// <returns>当前服务器端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(result) || !Regex.IsMatch(result, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                return "127.0.0.1";
            return result;
        }

        #endregion


        public static string GetParamSign(object obj)
        {
            var type = obj.GetType();
            var pis = type.GetProperties();
            var arr = pis.Select(pi =>
            {
                var o = pi.GetValue(obj, null);
                return o == null ? "" : o.ToString();
            }).ToArray();
            //字典排序
            Array.Sort(arr);
            //拼接成字符串
            var temp = string.Join("", arr);
            return FormsAuthentication.HashPasswordForStoringInConfigFile(temp,
            "SHA1");
        }

        public static void WriteTxt(string text, string filepath = "")
        {
            if (string.IsNullOrEmpty(filepath))
                filepath = HttpContext.Current.Request.MapPath("/bug.txt");
            using (var fs = new FileStream(filepath, FileMode.Append, FileAccess.Write))
            {
                using (var write = new StreamWriter(fs))
                {
                    write.WriteLine(text);
                    write.Flush();
                }
            }
        }
        /// <summary>
        /// 检查签名是否正确:
        /// http://mp.weixin.qq.com/wiki/index.php?title=%E6%8E%A5%E5%85%A5%E6%8C%87%E5%8D%97
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="token">AccessToken</param>
        /// <returns>
        /// true: check signature success
        /// false: check failed, 非微信官方调用!
        /// </returns>
        public static bool CheckSignature(string signature, string timestamp, string nonce, string token, out string ent)
        {
            var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            StringBuilder enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }
            ent = enText.ToString();
            return signature == sha1Arr.ToString();
        }

    }
}
