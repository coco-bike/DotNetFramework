using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TimeParser
    {
        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <returns></returns>
        public static int SecondToMinute(int Second)
        {
            decimal mm = (decimal)((decimal)Second / (decimal)60);
            return Convert.ToInt32(Math.Ceiling(mm));
        }

        #region 返回某年某月最后一天
        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(int year, int month)
        {
            DateTime lastDay = new DateTime(year, month, new System.Globalization.GregorianCalendar().GetDaysInMonth(year, month));
            int Day = lastDay.Day;
            return Day;
        }
        #endregion

        #region 返回时间差
        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="DateTime1">开始时间</param>
        /// <param name="DateTime2">结束时间</param>
        /// <returns></returns>
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                TimeSpan ts = DateTime2 - DateTime1;
                if (ts.Days >= 1)
                {
                    dateDiff = DateTime1.Month.ToString() + "月" + DateTime1.Day.ToString() + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours.ToString() + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟前";
                    }
                }
            }
            catch
            { }
            return dateDiff;
        }

        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="DateTime1">开始时间</param>
        /// <param name="DateTime2">结束时间</param>
        /// <returns></returns>
        public static int DaysDiff(DateTime? DateTime1, DateTime? DateTime2)
        {
            int dateDiff = 0;
            try
            {
                //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                TimeSpan ts = Convert.ToDateTime(DateTime2 ?? DateTime.Now) - Convert.ToDateTime(DateTime1 ?? DateTime.Now);
                dateDiff = ts.Days;
            }
            catch
            { }
            return dateDiff;
        }

        #endregion
        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        /// <param name="strDateTime">时间</param>
        /// <param name="strFormat">格式</param>
        /// <returns></returns>
        public static string ConvertToDataFormat(object strDateTime, string strFormat)
        {
            try
            {
                DateTime temp = Convert.ToDateTime(strDateTime);
                return temp.ToString(strFormat);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 按照“yyyy-MM-dd”返回指定日期格式
        /// </summary>
        /// <param name="strDateTime">时间</param>
        /// <returns></returns>
        public static string ConvertToDataFormat(object strDateTime)
        {
            try
            {
                DateTime temp = Convert.ToDateTime(strDateTime);
                return temp.ToString("yyyy-MM-dd");
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获取当前时间字符串，格式为19位的yyyyMMddHHmmssfff 
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeNowString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public static string GetDayOfWeek(DateTime obj)
        {
            int dayOfWeek = (int)obj.DayOfWeek;
            string value = "";
            switch (dayOfWeek)
            {
                case 0:
                    value = "星期日"; break;
                case 1:
                    value = "星期一"; break;
                case 2:
                    value = "星期二"; break;
                case 3:
                    value = "星期三"; break;
                case 4:
                    value = "星期四"; break;
                case 5:
                    value = "星期五"; break;
                case 6:
                    value = "星期六"; break;
                case 7:
                    value = "星期日"; break;
            }
            return value;
        }

        #region unix/datatime 时间转换
        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp);
            return dtStart.AddMilliseconds(lTime);
        }
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        #endregion

        #region 还款计划时间
        /// <summary>
        /// 结算到期结束时间
        /// </summary>
        /// <param name="startdate">开始时间</param>
        /// <param name="term">期数</param>
        /// <param name="spankind">每期计算方式【月、半月、周、天】</param>
        /// <returns></returns>
        public static DateTime GetEndDate(DateTime startdate, int term, int spanAmount, string spankind, bool isreduce = true)
        {
            DateTime enddate = DateTime.Now;
            if (spankind == "月")
            {
                enddate = startdate.AddMonths(term * spanAmount);
            }
            else if (spankind == "半月")
            {
                enddate = startdate.AddMonths(term * spanAmount / 2).AddDays(15 * (term * spanAmount % 2));
            }
            else if (spankind == "周")
            {
                enddate = startdate.AddDays(term * 7 * spanAmount);
            }
            else
            {
                enddate = startdate.AddDays(term * spanAmount);
            }
            if (isreduce)
            {
                enddate = enddate.AddDays(-1);
            }
            return enddate;
        }
        #endregion
    }
}
