using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LogHelper
    {
        static Object obj = new Object();
        private static void WriteText(string text, string path)
        {
            lock (obj)
            {
                string newpath = ConfigHelper.GetConfigString("logPath") + path;
                int start_index = newpath.LastIndexOf('/');
                if (Directory.Exists(newpath.Substring(0, start_index)) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(newpath.Substring(0, start_index));
                }
                FileStream fs = null;
                if (!File.Exists(newpath))
                {
                    fs = new FileStream(newpath, FileMode.CreateNew);
                }
                else
                {
                    fs = new FileStream(newpath, FileMode.Append);
                }
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "】" + text);
                }
            }
        }

        public static void WriteLog(string info)
        {
            WriteException(info);
        }
        public static void WriteLog(string info, Exception ex)
        {
            WriteException(info + ":\r\t" + ex.Source.ToString() + "：\r\t" + ex.Message);
        }
        public static void WriteLog(Exception ex)
        {
            WriteException(ex.StackTrace.ToString() + ":\r\t" + ex.Source + "：\r\t" + ex.Message);
        }

        public static async void WriteLogAsync(string info)
        {
            await WriteExceptionAsync(info);
        }
        public static async void WriteLogAsync(string info, Exception ex)
        {
            await WriteExceptionAsync(info + ":\r\t" + ex.Source.ToString() + "：\r\t" + ex.Message);
        }
        public static async void WriteLogAsync(Exception ex)
        {
            await WriteExceptionAsync(ex.StackTrace.ToString() + ":\r\t" + ex.Source + "：\r\t" + ex.Message);
        }
        /// <summary>
        /// 回掉记录日志
        /// </summary>
        /// <param name="text"></param>
        public static void WriteCallBackLog(string text)
        {
            string path = "/Callback/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        /// <summary>
        /// 回掉记录日志
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async void WriteCallBackLogAsync(string text)
        {
            await Task.Run(() =>
            {
                string path = "/Callback/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text">内容</param>
        public static void WriteOther(string text)
        {
            string path = "/Other/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        public static async void WriteOtherAsync(string text)
        {
            await Task.Run(() =>
            {
                string path = "/Other/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }
        private static void WriteException(string text)
        {
            string path = "/Error/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        private static async Task WriteExceptionAsync(string text)
        {
            await Task.Run(() =>
            {
                string path = "/Error/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }

        public static void WriteReport(string text)
        {
            string path = "/Report/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }

        public static async void WriteReportAsync(string text)
        {
            await Task.Run(() => {
                string path = "/Report/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }
        public static void WriteAppLog(string text)
        {
            string path = "/App/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        public static async void WriteAppLogAsync(string text)
        {
            await Task.Run(() =>
            {
                string path = "/App/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }
        public static void WriteGmsReport(string text)
        {
            string path = "/GMSReport/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        public static async void WriteGmsReportAsync(string text)
        {
            await Task.Run(() =>
            {
                string path = "/GMSReport/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }
        public static void WritePathReport(string mypath, string text)
        {
            string path = "/" + mypath + "/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            WriteText(text, path);
        }
        public static async void WritePathReportAsync(string mypath, string text)
        {
            await Task.Run(() =>
            {
                string path = "/" + mypath + "/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                WriteText(text, path);
            });
        }

    }
}
