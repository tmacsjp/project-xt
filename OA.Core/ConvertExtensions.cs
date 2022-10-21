using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#pragma warning disable 0168

namespace OA.Core
{

    /// <summary>
    /// 类型转换类
    /// </summary>
    public static class ConvertExtensions
    {
        /// <summary>
        /// 1970-01-01
        /// </summary>
        public static readonly DateTime TimespanStart = new DateTime(1970, 1, 1);

        /// <summary>
        /// 北京当前时间
        /// </summary>
        public static DateTime BeiJingNow => new DateTime(DateTime.UtcNow.AddHours(8).Ticks, DateTimeKind.Local);

        /// <summary>
        /// 转为erp标准时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToErpDateTime(this DateTime? dt)
        {
            if (dt == null)
                return "";
            return dt.Value.ToErpDateTime();
        }
        /// <summary>
        /// 转为erp标准时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToErpDateTime(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 转为erp标准时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToErpTime(this DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 转为erp标准时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToErpDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        static readonly DateTime beginTime = new DateTime(1970, 1, 1);
        /// <summary>
        /// 不对时间处理
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime dt)
        {
            return (long)dt.Subtract(beginTime).TotalSeconds;
        }
        /// <summary>
        /// 不对时间处理
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimestampMilliseconds(this DateTime dt)
        {
            return (long)dt.Subtract(beginTime).TotalMilliseconds;
        }

        /// <summary>
        /// 取得时间是 前天，昨天 今天 明天 后天，不是则返回空字符
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static string DateTimeToOffNow(this DateTime dt, DateTime? now)
        {
            now = now ?? BeiJingNow;
            var days = (int)(new DateTime(now.Value.Year, now.Value.Month, now.Value.Day) - new DateTime(dt.Year, dt.Month, dt.Day)).TotalDays;
            switch (days)
            {
                case 2:
                    return "前天";
                case 1:
                    return "昨天";
                case 0:
                    return "今天";
                case -1:
                    return "明天";
                case -2:
                    return "后天";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);
            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        /// <summary>返回有关指定对象是否为 System.TypeCode.DBNull 类型的指示。</summary>
        /// <param name="obj">一个对象</param>
        /// <returns></returns>
        public static bool IsDbNull(this object obj)
        {
            return Convert.IsDBNull(obj);
        }

        /// <summary>字符串转Unicode
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string StringToUnicode(this string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x2"), bytes[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        public static string UnicodeToString(this string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }


        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp() => GetTimestamp(DateTime.UtcNow);

        /// <summary>取时间戳，不管时区，请外部处理</summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimestamp(this DateTime time)
        {
            var tspan = time - TimespanStart;
            return (long)tspan.TotalSeconds;
        }

        /// <summary>取时间戳，不管时区，请外部处理</summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimestampMillisecond(this DateTime time)
        {
            var tspan = time - TimespanStart;
            return (long)tspan.TotalMilliseconds;
        }


        /// <summary>string类型转换为short类型</summary>
        public static short StrToShort(this string str)
        {
            str = str ?? "";
            if (str.Contains(","))
            {
                str = str.Replace(",", "");
            }
            short iRet = 0;
            short.TryParse(str, out iRet);
            return iRet;
        }

        /// <summary>string类型转换为int类型</summary>
        public static int StrToInt(this string str)
        {
            str = str ?? "";
            if (str.Contains(","))
            {
                str = str.Replace(",", "");
            }
            int iRet = 0;
            int.TryParse(str, out iRet);
            return iRet;
        }

        /// <summary>string类型转换为long类型</summary>
        public static long StrToLong(this string str)
        {
            str = str ?? "";
            if (str.Contains(","))
            {
                str = str.Replace(",", "");
            }
            long iRet = 0;
            long.TryParse(str, out iRet);
            return iRet;
        }

        /// <summary>string类型转换为double类型</summary>
        public static double StrToDouble(this string str)
        {
            str = str ?? "";
            double dRet = 0;
            double.TryParse(str, out dRet);
            if (double.IsNaN(dRet)) return 0;
            return dRet;
        }

        /// <summary>string类型转换为float类型</summary>
        public static float StrToFloat(this string str)
        {
            str = str ?? "";
            float fRet = 0;
            float.TryParse(str, out fRet);
            return fRet;
        }

        /// <summary>string类型转换为decimal类型
        /// </summary>
        public static decimal StrToDecimal(this string str)
        {
            str = str ?? "";
            decimal dRet = 0;
            decimal.TryParse(str, out dRet);
            return dRet;
        }

        /// <summary>string类型转换为日期类型,转换失败返回DateTime.Now
        /// </summary>
        public static DateTime StrToDateTime(this string str)
        {
            str = str ?? "";
            DateTime dRet = TimespanStart;
            if (DateTime.TryParse(str, out dRet))
                return dRet;
            else
                return dRet;
        }


        /// <summary>string类型转换为日期类型</summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="aDefault">如果转换失败的默认值</param>
        /// <returns></returns>
        public static DateTime StrToDateTime(this string str, DateTime @default)
        {
            str = str ?? "";
            DateTime dRet = @default;
            if (DateTime.TryParse(str, out dRet))
                return dRet;
            else
                return dRet;
        }

        /// <summary>string类型转换为BOOL类型,转换失败返回False</summary>
        public static bool StrToBoolean(this string str)
        {
            str = str ?? "";
            bool result;
            if (!bool.TryParse(str, out result))
            {
                return false;
            }
            return result;
        }

        /// <summary>int类型转换为bool类型(1为TRUE，其余为FALSE)</summary>
        public static bool IntToBool(this int value)
        {
            return (value == 1);
        }

        /// <summary>bool类型转换为int类型(TRUE为1，FALSE为0)</summary>
        public static int BoolToInt(this bool value)
        {
            if (value)
                return 1;
            else
                return 0;
        }



        /// <summary>object类型转换为int类型</summary>
        public static int ObjToInt(this object value)
        {
            if (value == null) return 0;
            if (value.GetType() == typeof(bool))
                return ((bool)value) ? 1 : 0;
            string s = value.ToString();
            if (value.GetType() == typeof(string) && s.Contains(","))
            {
                s = s.Replace(",", "");
            }
            else if (value.GetType() == typeof(decimal) ||
                     value.GetType() == typeof(double) ||
                     value.GetType() == typeof(float))
            {
                return (int)ObjToDouble(value);
            }

            if (s.ToLower() == "true")
                return 1;
            if (s.ToLower() == "false")
                return 0;

            int iRet = 0;
            int.TryParse(s, out iRet);
            return iRet;
        }

        /// <summary>object类型转换为Int64类型</summary>
        public static long ObjToLong(this object value)
        {
            if (value == null) return 0;
            if (value is long)
                return (long)value;
            if (value is int)
                return (int)value;
            string s = value.ToString();
            if (value.GetType() == typeof(string) && s.Contains(","))
            {
                s = s.Replace(",", "");
            }
            else if (value.GetType() == typeof(decimal) ||
                     value.GetType() == typeof(double) ||
                     value.GetType() == typeof(float))
            {
                return (long)ObjToDouble(value);
            }
            Int64 iRet = 0;
            Int64.TryParse(s, out iRet);
            return iRet;
        }

        /// <summary>object类型转换为double类型</summary>
        public static double ObjToDouble(this object value)
        {
            if (value == null) return 0;
            double dRet = 0;
            double.TryParse(value.ToString(), out dRet);
            if (double.IsNaN(dRet)) return 0;
            return dRet;
        }

        /// <summary>object类型转换为decimal类型</summary>
        public static decimal ObjToDecimal(this object value)
        {
            if (value == null) return 0;
            decimal dRet = 0;
            decimal.TryParse(value.ToString(), out dRet);
            return dRet;
        }

        /// <summary>object类型转换为float类型
        /// </summary>
        public static float ObjToFloat(this object value)
        {
            if (value == null) return 0;
            float dRet = 0;
            float.TryParse(value.ToString(), out dRet);
            return dRet;
        }

        /// <summary>object类型转换为datetime类型</summary>
        public static DateTime ObjToDateTime(this object value)
        {
            DateTime dRet = new DateTime();
            if (value == null) return dRet;

            try
            {
                if (value is DateTime)
                    return (DateTime)value;
                var x = ObjToStr(value);
                if (x.Length == 4)
                {
                    return new DateTime(Convert.ToInt32(x), 1, 1);
                }
                return Convert.ToDateTime(value);
            }
            catch (Exception ex)
            {
                return dRet;
            }
        }
        /// <summary>object类型转换为bool类型，直接强制转换(bool)value</summary>
        public static bool ObjToBoolPython(this object obj)
        {
            if (obj == null) return false;
            if (obj is byte)
            {
                return (byte)obj != 0;
            }
            if (obj is bool)
            {
                return (bool)obj;
            }
            if (obj is int)
            {
                return (int)obj != 0;
            }
            if (obj is long)
            {
                return (long)obj != 0;
            }
            if (obj is float)
            {
                return (float)obj != 0;
            }
            if (obj is decimal)
            {
                return (decimal)obj != 0;
            }
            if (obj is double)
            {
                return (double)obj != 0;
            }
            if (obj is string)
            {
                var sv = obj.ToString().ToLower();
                if (string.IsNullOrEmpty(sv))
                    return false;
                if (sv == "true")
                    return true;
                if (sv == "false")
                    return false;
                double dv = 0;
                if (!double.TryParse(sv, out dv))
                {
                    return true;
                }
                return dv != 0;
            }
            if (obj is Array)
            {
                return ((Array)obj).Length != 0;
            }
            return true;
        }

        /// <summary>object类型转换为bool类型，直接强制转换(bool)value</summary>
        public static bool ObjToBool(this object value)
        {
            if (value == null) return false;
            if (value is bool)
            {
                return (bool)value;
            }
            if (value.ToString().ToLower() == "true") return true;
            if (ObjToInt(value) == 1)
                return true;
            else
                return false;
        }

        /// <summary>object类型转换为string类型</summary>
        public static string NullToStr(this object value)
        {
            if (value == null) return "";
            if (Convert.IsDBNull(value)) return "";
            return value.ToString();
        }
        /// <summary>相当于NullToStr</summary>
        public static string ObjToStr(this object value)
        {
            return NullToStr(value);
        }
          

        /// <summary>金额小写转换成大写</summary>
        /// <param name="Value">数字</param>
        /// <param name="AState">状态，true表示完整，false表示简写</param>
        /// <returns>返回字符串，如：叁佰贰拾伍元整</returns>
        public static string MoneyToUpper(this double value, bool state)
        {
            const string NUM = "零壹贰叁肆伍陆柒捌玖";
            const string WEI = "分角元拾佰仟万拾佰仟亿拾佰仟";
            string F = "";
            double d = StrToDouble(value.ToString("0.00"));
            if (state)
            {
                if (d < 0)
                {
                    d = -d;
                    F = "负";
                }
                else if (d == 0)
                {
                    F = "零元零角零分";
                }
                else
                {
                    F = "";
                }
                int L = (int)Math.Truncate(Math.Log(d + 0.0000001, 10));
                string Str = "";
                for (int i = L; i >= -2; i--)
                {
                    Int64 T = (Int64)(Math.Round(d, 2) / Math.Pow(10, i) + 0.000001);
                    int N = (int)(T % 10);
                    Str = Str + NUM.Substring(N, 1) + WEI.Substring(i + 2, 1);
                }
                Str = F + Str;
                return Str;
            }
            else
            {
                if (d < 0)
                {
                    d = -d;
                    F = "负";
                }
                else if (d == 0)
                {
                    F = "零元整";
                }
                else
                {
                    F = "";
                }
                int L = (int)(Math.Floor(Math.Log(d + 0.0000001, 10)));
                string Str = "";
                bool Zero = false;  //上一位是不是0
                for (int i = L; i >= -2; i--)
                {
                    Int64 T = (Int64)(Math.Round(d, 2) / Math.Pow(10, i) + 0.000001);
                    int N = (int)(T % 10);
                    if (N == 0)
                    {
                        if (i == 0 || i == 4 || i == 8) //碰到元、万、亿必须显示
                        {
                            Str = Str + WEI.Substring(i + 2, 1);
                            Zero = false;
                        }
                        else
                        {
                            Zero = true;
                        }
                    }
                    else
                    {
                        if (Zero)
                        {
                            Str = Str + NUM.Substring(0, 1);
                        }
                        Str = Str + NUM.Substring(N, 1) + WEI.Substring(i + 2, 1);
                        Zero = false;
                    }
                    if (Math.Abs(d - T * Math.Pow(10, i)) < 0.001 && i > -2 && i < 5)
                    {
                        if (i > 0)
                        {
                            Str = Str + "元整";
                        }
                        else
                        {
                            Str = Str + "整";
                        }
                        break;
                    }
                }
                Str = F + Str;
                return Str;
            }
        }

        /// <summary>取得金额某位的大写</summary>
        /// <param name="Value">数字</param>
        /// <param name="B">位置，-2分-1角0元1十2百3千4万，以此类推</param>
        /// <returns>返回数字，如：贰</returns>
        public static string MoneyBitUpper(this double value, int b)
        {
            const string NUM = "零壹贰叁肆伍陆柒捌玖";
            string S = "";
            double d = StrToDouble(value.ToString("0.00"));
            if (d < 0)
            {
                S = "负";
                d = -d;
            }
            int P = (int)(d / Math.Pow(10, b) + 0.001);
            if (P == 0)
            {
                return "¤";
            }
            else
            {
                if (P >= 10) S = "";
                P = P % 10;
                return S + NUM.Substring(P, 1);
            }
        }

        /// <summary>对象转换成字节数组,自动判断isDbNull,返回null</summary>
        /// <param name="obj">对象</param>
        /// <returns>失败返回null</returns>
        public static byte[] ObjToBytes(this object obj)
        {
            if (obj == null || Convert.IsDBNull(obj))
            {
                return null;
            }
            if (obj.GetType() == typeof(string))
            {
                return StrToBytes(obj.ToString());
            }
            byte[] bytes = (byte[])obj;
            if (bytes.Length == 0)
            {
                return null;
            }
            else
            {
                return bytes;
            }
        }
 
        /// <summary>字符串转换成字节数组(采用UTF8)</summary>
        /// <param name="Str">字符串</param>
        /// <returns></returns>
        public static byte[] StrToBytes(this string value)
        {
            if (value == null) return new byte[0];

            byte[] result = new byte[Encoding.UTF8.GetByteCount(value)];
            result = Encoding.UTF8.GetBytes(value);
            return result;
        }

        /// <summary>字节数组转换成字符串(采用UTF8)（过期函数，建议采用ObjToBytesToStr）</summary>
        /// <param name="bytes">字节数组，DataSet返回的数据可以直接使用,如:(byte[])Ds.Tables[0].Rows[0]["f_fromsql"]</param>
        /// <returns></returns>
        public static string BytesToStr(this byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }
            else if (bytes.Length == 0)
            {
                return "";
            }
            else
            {
                return Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);
            }
        }

        /// <summary>对象转换成字节数组再转换成字符串(采用UTF8)</summary>
        /// <param name="obj">对象,如:Ds.Tables[0].Rows[0]["f_image"]</param>
        /// <returns></returns>
        public static string ObjToBytesToStr(this object obj)
        {
            if (Convert.IsDBNull(obj)) return "";
            return System.Text.Encoding.UTF8.GetString((byte[])obj).Replace("\0", string.Empty);
        }


        /// <summary>取字符串左边N个字符
        /// </summary>
        public static string LeftStr(this string str, int num)
        {
            if (str.Length <= num)
                return str;
            return str.Substring(0, num);
        }

        /// <summary>取字符串右边N个字符
        /// </summary>
        public static string RightStr(this string str, int num)
        {
            if (str.Length <= num)
                return str;
            return str.Substring(str.Length-num, num);
        }


        /// <summary>取字符串长度(按UTF8字节计算) </summary>
        public static int GetByteLength(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str).Length;
        }
        /// <summary>byte数组转16进制字符串，一个字节两个字母</summary>
        public static string BytesToByteStr(this byte[] bs)
        {
            if (bs == null)
                return "";
            StringBuilder sbtobs = new StringBuilder();
            foreach (var a in bs)
            {
                string tss = a.ToString("x2");
                sbtobs.Append(tss);
            }
            return sbtobs.ToString();
        }


        /// <summary>16进制字符串转byte数组，一个字节两个字母</summary>
        public static byte[] ByteStrToBytes(this string str)
        {
            if (str == null)
                return new byte[0];
            byte[] bs = new byte[str.Length/2];
            for (var k = 0; k < bs.Length; k ++)
            {
                bs[k] = Convert.ToByte(str.Substring(k*2, 2),16);
            }
            return bs;
        }

        /// <summary>公历转农历日期字符串</summary>
        /// <param name="Dt"></param>
        /// <returns></returns>
        public static string DateToChineseDate(this DateTime date)
        {
            System.Globalization.ChineseLunisolarCalendar cCalendar = new System.Globalization.ChineseLunisolarCalendar();
            int lyear = cCalendar.GetYear(date);
            int lmonth = cCalendar.GetMonth(date);
            int lday = cCalendar.GetDayOfMonth(date);
            int leapMonth = cCalendar.GetLeapMonth(lyear);
            //bool isleap = false;//闰月
            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }
            return lyear.ToString() + "-" + lmonth.ToString("00") + "-" + lday.ToString("00");
        }

        public static void ReThrow(this Exception exception)
        {
            throw exception;
        }

        #region 转换扩展方法
        /// <summary>
        /// 字符串数组转换为Long数组
        /// </summary>
        /// <param name="stringList">转换字符串数组</param>
        /// <returns>List{long}</returns>
        public static List<long> ToLongList(this string[] stringList)
        {
            List<long> list = new List<long>();
            foreach (var item in stringList)
            {
                Int64 iRet = 0;
                Int64.TryParse(item, out iRet);
                list.Add(iRet);
            }
            return list;
        }

        /// <summary>
        /// 字符串转int类型
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        public static List<long> ToLongList(this List<string> source)
        {
            long i = 0;
            List<long> list = new List<long>();
            foreach (var item in source)
            {
                long.TryParse(item, out i);
                list.Add(i);
            }

            return list;
        }

        /// <summary>
        /// List转datatable (未测)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static bool IsNullable(this Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Type GetCoreType(this Type type)
        {
            if (type != null && IsNullable(type))
            {
                if (!type.IsValueType)
                {
                    return type;
                }
                else
                {
                    return Nullable.GetUnderlyingType(type);
                }
            }
            else
            {
                return type;
            }
        }
        #endregion

        public static T Random<T>(this IList<T> items)
        {
            if (items.Count == 0)
                return default(T);
            return items[new Random().Next(0, items.Count)];
        }

        public static byte[] Encode(this string str,Encoding enc = null)
        {
            str = str ?? "";
            enc  = enc?? Encoding.UTF8;
            return enc.GetBytes(str);
        }
        public static string Decode(this byte[] data, Encoding enc = null)
        {
            enc = enc ?? Encoding.UTF8;
            return enc.GetString(data);
        }

    }
}
