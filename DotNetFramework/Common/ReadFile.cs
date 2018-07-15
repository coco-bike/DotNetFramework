using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common
{
    /// <summary>
    /// INI文件读写类。
    /// Copyright (C) Maticsoft
    /// </summary>
	public class ReadFile
    {
        public string path;

        public ReadFile(string INIPath)
        {
            path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);


        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
            return temp;

        }


        /// <summary>
        /// 删除ini文件下所有段落
        /// </summary>
        public void ClearAllSection()
        {
            IniWriteValue(null, null, null);
        }
        /// <summary>
        /// 删除ini文件下personal段落下的所有键
        /// </summary>
        /// <param name="Section"></param>
        public void ClearSection(string Section)
        {
            IniWriteValue(Section, null, null);
        }
    }

    public class JsonFile
    {
        /// <summary>
        /// 读取Json文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadJsonFile(string fileName)
        {
            string CacheKey = "xml-" + fileName;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    StringBuilder strContent = new StringBuilder();
                    using (StreamReader sReader = new StreamReader(ConfigHelper.GetConfigString("curPath") + "/XML/" + fileName, Encoding.UTF8))
                    {
                        string line;
                        while ((line = sReader.ReadLine()) != null)
                        {
                            strContent.Append(line);
                        }
                    }
                    objModel = strContent.ToString();
                    if (objModel != null)
                    {
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddHours(1), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                    return null;
                }
            }
            return (string)objModel;

        }
        /// <summary>
        /// 读取Json文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadJsonFile(string path, string fileName)
        {
            string CacheKey = "xml-" + fileName;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    StringBuilder strContent = new StringBuilder();
                    using (StreamReader sReader = new StreamReader(path + fileName, Encoding.UTF8))
                    {
                        string line;
                        while ((line = sReader.ReadLine()) != null)
                        {
                            strContent.Append(line);
                        }
                    }
                    objModel = strContent.ToString();
                    if (objModel != null)
                    {
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddHours(1), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                    return null;
                }
            }
            return (string)objModel;

        }

        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XmlDocument ReadXmlFile(string fileName)
        {
            string CacheKey = "xml-" + fileName;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigHelper.GetConfigString("curPath") + "/XML/" + fileName);
                    objModel = doc;
                    if (objModel != null)
                    {
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddHours(1), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(ex);
                    return null;
                }
            }
            return (XmlDocument)objModel;
        }

        /// <summary>
        /// XML序列化与反序列化类
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        public class XMLConfigHelper<T>
        {        /// <summary>
                 /// XML转对象
                 /// </summary>
                 /// <param name="xmlPath">XML的地址</param>
                 /// <returns>返回对象</returns>
            public T ReadXML(string xmlPath)
            {
                T t;
                XmlSerializer myXmlSerializer = new XmlSerializer(typeof(T));
                FileStream myFileStream = new FileStream(xmlPath, FileMode.Open);
                t = (T)myXmlSerializer.Deserialize(myFileStream);
                myFileStream.Close();
                return t;

            }
            /// <summary>
            /// 对象转XML
            /// </summary>
            /// <param name="xmlPath">Xml地址</param>
            /// <param name="t">类型</param>
            /// <returns>ture为转化成功，否则为失败</returns>
            public bool ObjectToXML(string xmlPath, T t)
            {
                XmlDocument doc = new XmlDocument();
                if (!File.Exists(xmlPath))
                {
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
                    XmlElement root = doc.CreateElement(typeof(T).Name);
                    doc.AppendChild(root);
                }
                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                StreamWriter myWriter = new StreamWriter(xmlPath);
                mySerializer.Serialize(myWriter, t);
                myWriter.Close();
                doc.Save(xmlPath);
                return true;
            }

        }
    }
}
