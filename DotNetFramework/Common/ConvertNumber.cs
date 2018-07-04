using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 转换操作
    /// </summary>
    public class ConvertNumber
    {
        /// <summary>
        /// 字符串转Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ConvertToInt(string str)
        {
            int iToInt = 0;
            try
            {
                if (str.Trim() != "")
                {
                    iToInt = Convert.ToInt32(str);
                }
            }
            catch
            {
                iToInt = 0;
            }
            return iToInt;
        }
        /// <summary>
        /// Object 转 Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ConvertToInt(Object obj)
        {
            int iToInt = 0;
            try
            {
                iToInt = Convert.ToInt32(obj);
            }
            catch
            {
                iToInt = 0;
            }
            return iToInt;
        }
        /// <summary>
        /// String 转 Double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ConvertToDouble(string str)
        {
            double dToDouble = 0;
            try
            {
                if (str.Trim() != "")
                {
                    dToDouble = Convert.ToDouble(str);
                }
            }
            catch
            {
                dToDouble = 0;
            }
            return dToDouble;
        }
        /// <summary>
        /// Object 转 Double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ConvertToDouble(Object obj)
        {
            double dToDouble = 0;
            try
            {
                dToDouble = Convert.ToDouble(obj);
            }
            catch
            {
                dToDouble = 0;
            }
            return dToDouble;
        }
        /// <summary>
        /// String 转 Double,保留指定位数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public static double ConvertToDoubleN(string str, int N)
        {
            double dToDouble = 0;
            try
            {
                if (str.Trim() != "")
                {
                    dToDouble = Convert.ToDouble(str);
                    switch (N)
                    {
                        case 1:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.#}", dToDouble));
                            break;
                        case 2:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.##}", dToDouble));
                            break;
                        case 3:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.###}", dToDouble));
                            break;
                        case 4:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.####}", dToDouble));
                            break;
                        case 5:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.#####}", dToDouble));
                            break;
                        case 6:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.######}", dToDouble));
                            break;
                        case 7:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.#######}", dToDouble));
                            break;
                        case 8:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.########}", dToDouble));
                            break;
                        case 9:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.#########}", dToDouble));
                            break;
                        case 10:
                            dToDouble = Convert.ToDouble(string.Format("{0:0.##########}", dToDouble));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                dToDouble = 0;
            }
            return dToDouble;
        }

        /// <summary>
        /// 将字符串转换成Decimal类型
        /// </summary>
        /// <param name="strNumber">待转字符串</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(object strNumber)
        {
            try
            {
                decimal result = Convert.ToDecimal(strNumber);
                result = Convert.ToDecimal(result.ToString("f2"));
                return result;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        ///  转换成时间字符串，错误输出指定时间
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strvalue"></param>
        /// <returns></returns>
        public static string ConvertToDateTime(string str, string strvalue)
        {
            string temp = "";
            try
            {
                temp = Convert.ToDateTime(str).ToString("yyyy-MM-dd");
            }
            catch
            {
                temp = strvalue;
            }
            return temp;
        }
        /// <summary>
        /// 转换成时间字符串，错误输出指定时间
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strvalue"></param>
        /// <returns></returns>
        public static string ConvertToDateTime_obj(object obj, string strvalue)
        {
            string temp = "";
            try
            {
                temp = Convert.ToDateTime(obj).ToString("yyyy-MM-dd");
            }
            catch
            {
                temp = strvalue;
            }
            return temp;
        }
        /// <summary>
        /// 将字符串转换成时间，错误的话，使用默认值
        /// </summary>
        /// <param name="str">待转字符串</param>
        /// <param name="ctime">默认值</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string str, DateTime ctime)
        {
            try
            {
                if (String.IsNullOrEmpty(str))
                {
                    return ctime;
                }
                DateTime dtime = Convert.ToDateTime(str);
                return dtime;
            }
            catch
            {
                return ctime;
            }
        }
        /// <summary>
        /// 将时间字符串转换成指定格式的字符串
        /// </summary>
        /// <param name="str">待转字符串</param>
        /// <returns>YYYY-MM-DD</returns>
        public static string ConverToDateTimeStr(string str)
        {
            try
            {
                DateTime dtime = Convert.ToDateTime(str);
                return dtime.ToString("yyyy-MM-dd");
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 将手机号码处理成指定格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns>137****1234</returns>
        public static string ConvertToMovtel(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else if (PageValidate.IsMobile(value))
            {
                return value.Substring(0, 3) + "****" + value.Substring(7, 4);
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 身份证号码处理成指定格式
        /// </summary>
        public static string ConvertToCard(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else if (PageValidate.IsCard(value))
            {
                return value.Substring(0, 6) + "********" + value.Substring(14, 4);
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 地址处理成指定格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToAddress(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                return value.Substring(0, 3) + "****" + value.Substring(value.Length - 4, 4);
            }
        }
        /// <summary>
        /// 车牌号处理成指定格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToCarNo(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                return value.Substring(0, 2) + "****" + value.Substring(value.Length - 2, 2);
            }
        }
        /// <summary>
        /// 银行卡号处理成指定格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToBankNo(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                return value.Substring(0, 6) + "**********" + value.Substring(value.Length - 4, 4);
            }
        }
        /// <summary>
        /// 计算违约金
        /// </summary>
        /// <param name="htMoney"></param>
        /// <param name="yqDays"></param>
        /// <returns></returns>
        public static decimal CalcWyj(object htMoney, object yqDays)
        {
            int yqdays = ConvertToInt(yqDays) > 0 ? ConvertToInt(yqDays) : 0;

            return ConvertToDecimal(htMoney) * yqdays * decimal.Parse("0.01");
        }
        /// <summary>
        /// 提取身份证号中生日
        /// </summary>
        /// <param name="cardid"></param>
        /// <returns></returns>
        public static DateTime CardIdToBirthDay(string cardid)
        {
            try
            {
                if (PageValidate.IsCard(cardid))
                {
                    if (cardid.Length == 18)
                    {
                        string birth = cardid.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        return DateTime.Parse(birth);
                    }
                    else
                    {
                        string birth = cardid.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        return DateTime.Parse(birth);
                    }
                }
                else
                {
                    return DateTime.Parse("1900-01-01");
                }
            }
            catch
            {
                return DateTime.Parse("1900-01-01");
            }
        }
    }
}
