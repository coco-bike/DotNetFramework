using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    /// <summary>
    /// 字符串操作
    /// </summary>
    public class StringPlus
    {
        /// <summary>
        /// 按照指定字符分隔字符串并判断是否需要转换成小写
        /// </summary>
        /// <param name="str">待转字符串</param>
        /// <param name="speater">指定字符</param>
        /// <param name="toLower">是否小写</param>
        /// <returns></returns>
        public static List<string> GetStrArray(string str, char speater, bool toLower)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(speater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != speater.ToString())
                {
                    string strVal = s;
                    if (toLower)
                    {
                        strVal = s.ToLower();
                    }
                    list.Add(strVal);
                }
            }
            return list;
        }
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }

        #region 对数组进行排序操作
        /// <summary>
        /// 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> Para_filter(Dictionary<string, string> dicArrayPre)
        {
            SortedDictionary<string, string> dicArray = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                dicArray.Add(temp.Key, temp.Value);
            }
            return dicArray;
        }

        /// <summary>
        /// 把每个参数值以“|”字符连接起来
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string Create_linkstring(SortedDictionary<string, string> dicArray, string mark = "|")
        {
            StringBuilder prestr = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Value + mark);
            }

            string str = prestr.ToString();
            if (String.IsNullOrEmpty(mark))
            {
                return str;
            }
            else
            {
                return str.Substring(0, str.Length - 1);
            }
        }

        /// <summary>
        /// 将一个Dictionary按Key值排序,并拼接成key1=value1&key2=value2...keyN=valueN的形式
        /// </summary>
        /// <param name="param"></param>
        /// <returns>排序好的URL请求字符串</returns>
        public static string SortParam(SortedDictionary<string, string> sortParam, string mark = "&")
        {
            var result = new StringBuilder();
            foreach (KeyValuePair<string, string> item in sortParam)
            {
                result.Append(item.Key).Append("=").Append(UrlOper.UrlEncode_UTF8(item.Value)).Append(mark);
            }
            return result.Length > 0 ? result.Remove(result.Length - 1, 1).ToString() : result.ToString();
        }
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string CreatDoPostData(Dictionary<string, object> dicArrayPre, string mark = "&")
        {
            Hashtable ht = new Hashtable();
            string[] arr = new string[dicArrayPre.Count];
            StringBuilder prestr = new StringBuilder();
            int i = 0;
            foreach (var temp in dicArrayPre)
            {
                arr[i] = temp.Key;
                ht.Add(temp.Key, temp.Value.ToString());
                i++;
            }
            Array.Sort(arr, string.CompareOrdinal);
            for (int j = 0; j < arr.Count(); j++)
            {
                prestr.Append(arr[j] + "=" + ht[arr[j]] + mark);
            }
            string str = prestr.ToString();
            return str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string CreatDoPostData(Dictionary<string, string> dicArrayPre, string mark = "&")
        {
            Hashtable ht = new Hashtable();
            string[] arr = new string[dicArrayPre.Count];
            StringBuilder prestr = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                arr[i] = temp.Key;
                ht.Add(temp.Key, temp.Value);
                i++;
            }
            Array.Sort(arr, string.CompareOrdinal);
            for (int j = 0; j < arr.Count(); j++)
            {
                prestr.Append(arr[j] + "=" + ht[arr[j]] + mark);
            }
            string str = prestr.ToString();
            return str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GreatDoPostData_List(List<string[]> list)
        {
            if (list == null || list.Count < 1)
            {
                return null;
            }
            StringBuilder prestr = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                string[] nvPairStr = list[i];
                if (i > 0)
                {
                    prestr.Append("&");
                }
                prestr.Append(UrlOper.UrlEncode_UTF8(nvPairStr[0])).Append("=").Append(UrlOper.UrlEncode_UTF8(nvPairStr[1]));
            }
            return prestr.ToString();
        }

        public static string CreatDoPostData_Model(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            SortedDictionary<string, string> sPara = new SortedDictionary<string, string>();//要签名的字符串
            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (p.GetValue(obj, null) != null && !String.IsNullOrEmpty(p.GetValue(obj, null).ToString()))
                {
                    sPara.Add(p.Name, p.GetValue(obj, null).ToString());
                }
            }
            return SortParam(sPara);
        }
        #endregion

        #region 删除最后一个字符之后的字符

        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }

        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }

        #endregion

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        ///  转半角的函数(SBC case)
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        public static List<string> GetSubStringList(string o_str, char sepeater)
        {
            List<string> list = new List<string>();
            string[] ss = o_str.Split(sepeater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != sepeater.ToString())
                {
                    list.Add(s);
                }
            }
            return list;
        }


        #region 将字符串样式转换为纯字符串
        /// <summary>
        /// 将字符串样式转换为纯字符串
        /// </summary>
        /// <param name="StrList"></param>
        /// <param name="SplitString"></param>
        /// <returns></returns>
        public static string GetCleanStyle(string StrList, string SplitString)
        {
            string RetrunValue = "";
            //如果为空，返回空值
            if (StrList == null)
            {
                RetrunValue = "";
            }
            else
            {
                //返回去掉分隔符
                string NewString = "";
                NewString = StrList.Replace(SplitString, "");
                RetrunValue = NewString;
            }
            return RetrunValue;
        }
        #endregion

        #region 将字符串转换为新样式
        /// <summary>
        /// 将字符串转换为新样式
        /// </summary>
        /// <param name="StrList"></param>
        /// <param name="NewStyle"></param>
        /// <param name="SplitString"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public static string GetNewStyle(string StrList, string NewStyle, string SplitString, out string Error)
        {
            string ReturnValue = "";
            //如果输入空值，返回空，并给出错误提示
            if (StrList == null)
            {
                ReturnValue = "";
                Error = "请输入需要划分格式的字符串";
            }
            else
            {
                //检查传入的字符串长度和样式是否匹配,如果不匹配，则说明使用错误。给出错误信息并返回空值
                int strListLength = StrList.Length;
                int NewStyleLength = GetCleanStyle(NewStyle, SplitString).Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    Error = "样式格式的长度与输入的字符长度不符，请重新输入";
                }
                else
                {
                    //检查新样式中分隔符的位置
                    string Lengstr = "";
                    for (int i = 0; i < NewStyle.Length; i++)
                    {
                        if (NewStyle.Substring(i, 1) == SplitString)
                        {
                            Lengstr = Lengstr + "," + i;
                        }
                    }
                    if (Lengstr != "")
                    {
                        Lengstr = Lengstr.Substring(1);
                    }
                    //将分隔符放在新样式中的位置
                    string[] str = Lengstr.Split(',');
                    foreach (string bb in str)
                    {
                        StrList = StrList.Insert(int.Parse(bb), SplitString);
                    }
                    //给出最后的结果
                    ReturnValue = StrList;
                    //因为是正常的输出，没有错误
                    Error = "";
                }
            }
            return ReturnValue;
        }
        #endregion

        #region XML 操作
        /// <summary>
        /// 从XML 中获取指定节点内容
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="attriName"></param>
        /// <returns></returns>
        public static ArrayList GetXmlToArray(XmlNodeList nodeList, string attriName)
        {
            try
            {
                ArrayList list = new ArrayList();
                if (nodeList != null)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        list.Add(node.Attributes[attriName].InnerText);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将XML文档解析为实体对象 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public static List<T> XmlDataToModel<T>(XmlNodeList nodeList)
        {
            List<T> list = new List<T>();
            foreach (XmlNode childNode in nodeList)
            {
                T objModel = System.Activator.CreateInstance<T>();
                foreach (XmlNode item in childNode.Attributes)
                {
                    PropertyInfo pi = objModel.GetType().GetProperty(item.Name);
                    if (pi == null)
                    {
                        continue;
                    }
                    if (!String.IsNullOrEmpty(item.InnerXml.Trim()))
                    {
                        pi.SetValue(objModel, item.InnerXml, null);
                    }
                }
                list.Add(objModel);
            }
            return list;
        }
        #endregion
    }
}
