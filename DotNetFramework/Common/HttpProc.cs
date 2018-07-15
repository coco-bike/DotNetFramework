using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>    
    /// 上传数据参数    
    /// </summary>    
    public class UploadEventArgs : EventArgs
    {
        int bytesSent;
        int totalBytes;
        /// <summary>    
        /// 已发送的字节数    
        /// </summary>    
        public int BytesSent
        {
            get { return bytesSent; }
            set { bytesSent = value; }
        }
        /// <summary>    
        /// 总字节数    
        /// </summary>    
        public int TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; }
        }
    }
    /// <summary>    
    /// 下载数据参数    
    /// </summary>    
    public class DownloadEventArgs : EventArgs
    {
        int bytesReceived;
        int totalBytes;
        byte[] receivedData;
        /// <summary>    
        /// 已接收的字节数    
        /// </summary>    
        public int BytesReceived
        {
            get { return bytesReceived; }
            set { bytesReceived = value; }
        }
        /// <summary>    
        /// 总字节数    
        /// </summary>    
        public int TotalBytes
        {
            get { return totalBytes; }
            set { totalBytes = value; }
        }
        /// <summary>    
        /// 当前缓冲区接收的数据    
        /// </summary>    
        public byte[] ReceivedData
        {
            get { return receivedData; }
            set { receivedData = value; }
        }
    }

    public class WebClient
    {
        Encoding encoding = Encoding.GetEncoding("UTF-8");
        string respHtml = "";
        WebProxy proxy;
        static CookieContainer cc;
        WebHeaderCollection requestHeaders;
        WebHeaderCollection responseHeaders;
        int bufferSize = 15240;
        public event EventHandler<UploadEventArgs> UploadProgressChanged;
        public event EventHandler<DownloadEventArgs> DownloadProgressChanged;
        static WebClient()
        {
            LoadCookiesFromDisk();
        }
        /// <summary>    
        /// 创建WebClient的实例    
        /// </summary>    
        public WebClient()
        {
            requestHeaders = new WebHeaderCollection();
            responseHeaders = new WebHeaderCollection();
        }
        /// <summary>    
        /// 设置发送和接收的数据缓冲大小    
        /// </summary>    
        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }
        /// <summary>    
        /// 获取响应头集合    
        /// </summary>    
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }
        /// <summary>    
        /// 获取请求头集合    
        /// </summary>    
        public WebHeaderCollection RequestHeaders
        {
            get { return requestHeaders; }
        }
        /// <summary>    
        /// 获取或设置代理    
        /// </summary>    
        public WebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }
        /// <summary>    
        /// 获取或设置请求与响应的文本编码方式    
        /// </summary>    
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }
        /// <summary>    
        /// 获取或设置响应的html代码    
        /// </summary>    
        public string RespHtml
        {
            get { return respHtml; }
            set { respHtml = value; }
        }
        /// <summary>    
        /// 获取或设置与请求关联的Cookie容器    
        /// </summary>    
        public CookieContainer CookieContainer
        {
            get { return cc; }
            set { cc = value; }
        }
        /// <summary>    
        ///  获取网页源代码    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <returns></returns>    
        public string GetHtml(string url)
        {
            HttpWebRequest request = CreateRequest(url, "GET");
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }
        /// <summary>    
        /// 下载文件    
        /// </summary>    
        /// <param name="url">文件URL地址</param>    
        /// <param name="filename">文件保存完整路径</param>    
        public void DownloadFile(string url, string filename)
        {
            FileStream fs = null;
            try
            {
                HttpWebRequest request = CreateRequest(url, "GET");
                byte[] data = GetData(request);
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                fs.Write(data, 0, data.Length);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// 从指定URL下载数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <returns></returns>    
        public byte[] GetData(string url)
        {
            HttpWebRequest request = CreateRequest(url, "GET");
            return GetData(request);
        }

        /// <summary>    
        /// 从指定URL下载数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <returns></returns>    
        public async Task<string> GetDataAsync(string url)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest request = CreateRequest(url, "GET");
                return System.Text.Encoding.UTF8.GetString(GetData(request)); ;
            });
        }
        /// <summary>    
        /// 向指定URL发送文本数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <param name="postData">urlencode编码的文本数据</param>    
        /// <returns></returns>    
        public async Task<string> PostAsync(string url, string postData)
        {
            return await Task.Run(() =>
            {
                try
                {
                    byte[] data = encoding.GetBytes(postData);
                    return Post(url, data);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("url=" + url + "\r\t postData=" + postData, ex);
                    return String.Empty;
                }
            });
        }

        /// <summary>    
        /// 向指定URL发送文本数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <param name="postData">urlencode编码的文本数据</param>    
        /// <returns></returns>    
        public string Post(string url, string postData)
        {
            try
            {
                byte[] data = encoding.GetBytes(postData);
                return Post(url, data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("url=" + url + "\r\t postData=" + postData, ex);
                return String.Empty;
            }
        }

        /// <summary>    
        /// 向指定URL发送字节数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <param name="postData">发送的字节数组</param>    
        /// <returns></returns>    
        public string Post(string url, byte[] postData, string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = CreateRequest(url, "POST");
            request.ContentType = contentType;
            request.ContentLength = postData.Length;
            request.KeepAlive = true;
            PostData(request, postData);
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }
        /// <summary>    
        /// 向指定网址发送mulitpart编码的数据    
        /// </summary>    
        /// <param name="url">网址</param>    
        /// <param name="mulitpartForm">mulitpart form data</param>    
        /// <returns></returns>    
        public string Post(string url, MultipartForm mulitpartForm)
        {
            HttpWebRequest request = CreateRequest(url, "POST");
            request.ContentType = mulitpartForm.ContentType;
            request.ContentLength = mulitpartForm.FormData.Length;
            request.KeepAlive = true;
            PostData(request, mulitpartForm.FormData);
            respHtml = encoding.GetString(GetData(request));
            return respHtml;
        }

        /// <summary>    
        /// 读取请求返回的数据    
        /// </summary>    
        /// <param name="request">请求对象</param>    
        /// <returns></returns>    
        private byte[] GetData(HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            responseHeaders = response.Headers;
            //SaveCookiesToDisk();

            DownloadEventArgs args = new DownloadEventArgs();
            if (responseHeaders[HttpResponseHeader.ContentLength] != null)
                args.TotalBytes = Convert.ToInt32(responseHeaders[HttpResponseHeader.ContentLength]);

            MemoryStream ms = new MemoryStream();
            int count = 0;
            byte[] buf = new byte[bufferSize];
            while ((count = stream.Read(buf, 0, buf.Length)) > 0)
            {
                ms.Write(buf, 0, count);
                if (this.DownloadProgressChanged != null)
                {
                    args.BytesReceived += count;
                    args.ReceivedData = new byte[count];
                    Array.Copy(buf, args.ReceivedData, count);
                    this.DownloadProgressChanged(this, args);
                }
            }
            stream.Close();
            //解压    
            if (ResponseHeaders[HttpResponseHeader.ContentEncoding] != null)
            {
                MemoryStream msTemp = new MemoryStream();
                count = 0;
                buf = new byte[100];
                switch (ResponseHeaders[HttpResponseHeader.ContentEncoding].ToLower())
                {
                    case "gzip":
                        GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
                        while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    case "deflate":
                        DeflateStream deflate = new DeflateStream(ms, CompressionMode.Decompress);
                        while ((count = deflate.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    default:
                        break;
                }
            }
            return ms.ToArray();
        }
        /// <summary>    
        /// 发送请求数据    
        /// </summary>    
        /// <param name="request">请求对象</param>    
        /// <param name="postData">请求发送的字节数组</param>    
        private void PostData(HttpWebRequest request, byte[] postData)
        {
            int offset = 0;
            int sendBufferSize = bufferSize;
            int remainBytes = 0;
            Stream stream = request.GetRequestStream();
            UploadEventArgs args = new UploadEventArgs()
            {
                TotalBytes = postData.Length
            };
            while ((remainBytes = postData.Length - offset) > 0)
            {
                if (sendBufferSize > remainBytes) sendBufferSize = remainBytes;
                stream.Write(postData, offset, sendBufferSize);
                offset += sendBufferSize;
                if (this.UploadProgressChanged != null)
                {
                    args.BytesSent = offset;
                    this.UploadProgressChanged(this, args);
                }
            }
            stream.Close();
        }
        /// <summary>    
        /// 创建HTTP请求    
        /// </summary>    
        /// <param name="url">URL地址</param>    
        /// <returns></returns>    
        private HttpWebRequest CreateRequest(string url, string method)
        {
            Uri uri = new Uri(url);

            if (uri.Scheme == "https")
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);

            // Set a default policy level for the "http:" and "https" schemes.    
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            HttpWebRequest.DefaultCachePolicy = policy;

            //ServicePointManager.DefaultConnectionLimit = 24;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = false;
            request.AllowWriteStreamBuffering = false;
            request.Method = method;
            if (proxy != null)
                request.Proxy = proxy;
            request.CookieContainer = cc;
            foreach (string key in requestHeaders.Keys)
            {
                request.Headers.Add(key, requestHeaders[key]);
            }
            requestHeaders.Clear();
            return request;
        }

        public String DoPostRequest(string url, byte[] bData)
        {
            System.Net.HttpWebRequest hwRequest;
            System.Net.HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = 5000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;

                System.IO.Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch (System.Exception err)
            {
                LogHelper.WriteLog("url【" + url + "】", err);
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), encoding);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (System.Exception err)
            {
                LogHelper.WriteLog(err);
            }
            return strResult;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>    
        /// 将Cookie保存到磁盘    
        /// </summary>    
        private static void SaveCookiesToDisk()
        {
            string cookieFile = System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies) + "\\webclient.cookie";
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Create);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formater.Serialize(fs, cc);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// 从磁盘加载Cookie    
        /// </summary>    
        private static void LoadCookiesFromDisk()
        {
            cc = new CookieContainer();
            string cookieFile = System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies) + "\\webclient.cookie";
            if (!File.Exists(cookieFile))
                return;
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                cc = (CookieContainer)formater.Deserialize(fs);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// post json 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strparam"></param>
        /// <returns></returns>
        public string PostJson(string url, string strparam)
        {
            string content = "";
            try
            {
                byte[] byteJson = System.Text.Encoding.UTF8.GetBytes(strparam);
                string newJsonString = Convert.ToBase64String(byteJson);

                HttpWebRequest request = CreateRequest(url, "POST");

                request.ContentType = "application/json";
                using (StreamWriter dataStream = new StreamWriter(request.GetRequestStream()))
                {
                    dataStream.Write(newJsonString);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();

                byte[] buffer = Convert.FromBase64String(retString);
                string sourthUrl = System.Text.Encoding.UTF8.GetString(buffer);
                content = System.Web.HttpUtility.UrlDecode(sourthUrl);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("url=" + url + "\r\t postData=" + strparam, ex);
            }
            return content;
        }

    }

    /// <summary>    
    /// 对文件和文本数据进行Multipart形式的编码    
    /// </summary>    
    public class MultipartForm
    {
        private Encoding encoding;
        private MemoryStream ms;
        private string boundary;
        private byte[] formData;
        /// <summary>    
        /// 获取编码后的字节数组    
        /// </summary>    
        public byte[] FormData
        {
            get
            {
                if (formData == null)
                {
                    byte[] buffer = encoding.GetBytes("--" + this.boundary + "--\r\n");
                    ms.Write(buffer, 0, buffer.Length);
                    formData = ms.ToArray();
                }
                return formData;
            }
        }
        /// <summary>    
        /// 获取此编码内容的类型    
        /// </summary>    
        public string ContentType
        {
            get { return string.Format("multipart/form-data; boundary={0}", this.boundary); }
        }
        /// <summary>    
        /// 获取或设置对字符串采用的编码类型    
        /// </summary>    
        public Encoding StringEncoding
        {
            set { encoding = value; }
            get { return encoding; }
        }
        /// <summary>    
        /// 实例化    
        /// </summary>    
        public MultipartForm()
        {
            boundary = string.Format("--{0}--", Guid.NewGuid());
            ms = new MemoryStream();
            encoding = Encoding.Default;
        }
        /// <summary>    
        /// 添加一个文件    
        /// </summary>    
        /// <param name="name">文件域名称</param>    
        /// <param name="filename">文件的完整路径</param>    
        public void AddFlie(string name, string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("尝试添加不存在的文件。", filename);
            FileStream fs = null;
            byte[] fileData = { };
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                this.AddFlie(name, Path.GetFileName(filename), fileData, fileData.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// 添加一个文件    
        /// </summary>    
        /// <param name="name">文件域名称</param>    
        /// <param name="filename">文件名</param>    
        /// <param name="fileData">文件二进制数据</param>    
        /// <param name="dataLength">二进制数据大小</param>    
        public void AddFlie(string name, string filename, byte[] fileData, int dataLength)
        {
            if (dataLength <= 0 || dataLength > fileData.Length)
            {
                dataLength = fileData.Length;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this.boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n", name, filename);
            sb.AppendFormat("Content-Type: {0}\r\n", this.GetContentType(filename));
            sb.Append("\r\n");
            byte[] buf = encoding.GetBytes(sb.ToString());
            ms.Write(buf, 0, buf.Length);
            ms.Write(fileData, 0, dataLength);
            byte[] crlf = encoding.GetBytes("\r\n");
            ms.Write(crlf, 0, crlf.Length);
        }
        /// <summary>    
        /// 添加字符串    
        /// </summary>    
        /// <param name="name">文本域名称</param>    
        /// <param name="value">文本值</param>    
        public void AddString(string name, string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this.boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", name);
            sb.Append("\r\n");
            sb.AppendFormat("{0}\r\n", value);
            byte[] buf = encoding.GetBytes(sb.ToString());
            ms.Write(buf, 0, buf.Length);
        }
        /// <summary>    
        /// 从注册表获取文件类型    
        /// </summary>    
        /// <param name="filename">包含扩展名的文件名</param>    
        /// <returns>如：application/stream</returns>    
        private string GetContentType(string filename)
        {
            Microsoft.Win32.RegistryKey fileExtKey = null; ;
            string contentType = "application/stream";
            try
            {
                fileExtKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));
                contentType = fileExtKey.GetValue("Content Type", contentType).ToString();
            }
            finally
            {
                if (fileExtKey != null) fileExtKey.Close();
            }
            return contentType;
        }

        /// <summary>
        /// Http下载文件支持断点续传
        /// </summary>
        /// <param name="uri">下载地址</param>
        /// <param name="filefullpath">存放完整路径（含文件名）</param>
        /// <param name="size">每次多的大小</param>
        /// <returns>下载操作是否成功</returns>
        public static bool HttpDownLoadFiles(string uri, string filefullpath, int size = 1024)
        {
            try
            {
                string fileDirectory = System.IO.Path.GetDirectoryName(filefullpath);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                string fileFullPath = filefullpath;
                string fileTempFullPath = filefullpath;
                if (System.IO.File.Exists(fileFullPath))
                {
                    return true;
                }
                else
                {
                    if (System.IO.File.Exists(fileTempFullPath))
                    {
                        FileStream fs = new FileStream(fileTempFullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        byte[] buffer = new byte[512];
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                        request.Timeout = 10000;
                        request.AddRange((int)fs.Length);
                        Stream ns = request.GetResponse().GetResponseStream();
                        long contentLength = request.GetResponse().ContentLength;
                        int length = ns.Read(buffer, 0, buffer.Length);
                        while (length > 0)
                        {
                            fs.Write(buffer, 0, length);
                            buffer = new byte[512];
                            length = ns.Read(buffer, 0, buffer.Length);
                        }
                        fs.Close();
                        System.IO.File.Move(fileTempFullPath, fileFullPath);
                    }
                    else
                    {
                        FileStream fs = new FileStream(fileTempFullPath, FileMode.Create);
                        byte[] buffer = new byte[512];
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                        request.Timeout = 10000;
                        request.AddRange((int)fs.Length);
                        Stream ns = request.GetResponse().GetResponseStream();
                        long contentLength = request.GetResponse().ContentLength;
                        int length = ns.Read(buffer, 0, buffer.Length);
                        while (length > 0)
                        {
                            fs.Write(buffer, 0, length);
                            buffer = new byte[512];
                            length = ns.Read(buffer, 0, buffer.Length);
                        }
                        fs.Close();
                        System.IO.File.Move(fileTempFullPath, fileFullPath);
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
