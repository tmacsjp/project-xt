using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Core
{
    public enum OrmParamType
    {
        /// <summary>
        /// 默认无参
        /// </summary>
        Default = 0,
        /// <summary>16位的有符号整数，相当于Sql中的SmallInt、Oracle中的Int16、Oledb中的SmallInt、.net中的System.Int16</summary>
        Int16 = 1,
        /// <summary>32位的有符号整数，相当于Sql中的Int、Oracle中的Int32、Oledb中的Integer、.net中的System.Int32</summary>
        Int32 = 2,
        /// <summary>64位的有符号整数，相当于Sql中的BigInt、Oracle中无此类型可用Number代替、Oledb中的BigInt、.net中的System.Int64</summary>
        Int64 = 3,
        /// <summary>单精度浮点值，相当于Sql中的Real、Oracle中的Float、Oledb中的Single、.net中的System.Single</summary>
        Single = 11,
        /// <summary>双精度浮点值，相当于Sql中的Float、Oracle中的Double、Oledb中的Double、.net中的System.Double</summary>
        Double = 12,
        /// <summary>定点精度和小数位数数值，相当于Sql中的Decimal、Oracle中的Number、Oledb中的Decimal、.net中的System.Decimal</summary>
        Decimal = 13,
        /// <summary>固定长度字符串，相当于Sql中的Char、Oracle中的Char、Oledb中的Char、.net中的System.String</summary>
        Char = 21,
        /// <summary>可变长度字符串，相当于Sql中的VarChar、Oracle中的VarChar、Oledb中的VarChar、.net中的System.String</summary>
        VarChar = 22,
        /// <summary>双字节char</summary>
        NVarchar = 23,
        NChar = 24,
        /// <summary>二进制数据，相当于Sql中的Binary、Oracle中的Blob、Oledb中的Binary、.net中的System.Byte[]</summary>
        Binary = 31,
        /// <summary>日期时间类型，相当于Sql中的DateTime、Oracle中的DateTime、Oledb中的DBDate、.net中的System.DateTime</summary>
        DateTime = 41
    }
}
