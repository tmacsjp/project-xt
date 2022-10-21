using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OA.Core
{


    /// <summary>存储过程参数</summary>
    public class OrmParameter
    {
        /// <summary>参数名称</summary>
        public string Name;
        /// <summary>参数类型</summary>
        public OrmParamType ParType;
        /// <summary>参数大小</summary>
        public int Size;
        /// <summary>参数方向</summary>
        public OrmParamDirection Direction = OrmParamDirection.Input;
        /// <summary>参数值</summary>
        public object Value;

        /// <summary>
        /// 总位数
        /// </summary>
        public int Precision = 0;

        /// <summary>
        /// 小数位数
        /// </summary>
        public int Scale = 6;

        /// <summary>默认构造函数</summary>
        public OrmParameter()
        {
        }

        /// <summary>构造函数</summary>
        public OrmParameter(string name, OrmParamType type, int size, OrmParamDirection direction, object value, int precision, int scale)
        {
            Name = name;
            ParType = type;
            Size = size;
            Direction = direction;
            Value = value;
            Precision = precision;
            Scale = scale;
        }

        public OrmParameter(string name, OrmParamType type, int size, OrmParamDirection direction, object value)
        {
            Name = name;
            ParType = type;
            Size = size;
            Direction = direction;
            Value = value;
        }

        /// <summary>构造函数</summary>
        public OrmParameter(string name, OrmParamType type, int size, object value)
        {
            Name = name;
            ParType = type;
            Size = size;
            Direction = OrmParamDirection.Input;
            Value = value;
        }

        public OrmParameter(string name, object value)
        {
            Name = name;
            ParType = OrmParamType.Default;
            Size = 0;
            Direction = OrmParamDirection.Input;
            Value = value;
        }
        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, decimal value)
        {
            Name = name;
            ParType = OrmParamType.Decimal;
            Size = 28;
            Precision = 0;
            Scale = 6;
            Direction = OrmParamDirection.Input;
            Value = value;
        }

        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, int value)
        {
            Name = name;
            ParType = OrmParamType.Int32;
            Size = 4;
            Direction = OrmParamDirection.Input;
            Value = value;
        }
        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, long value)
        {
            Name = name;
            ParType = OrmParamType.Int64;
            Size = 8;
            Direction = OrmParamDirection.Input;
            Value = value;
        }






        public OrmParameter(string name, OrmParamDirection direction, object value)
        {
            Name = name;
            ParType = OrmParamType.Default;
            Size = 2000;
            Direction = direction;
            Value = value;
        }
        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, OrmParamDirection direction, decimal value)
        {
            Name = name;
            ParType = OrmParamType.Decimal;
            Size = 28;
            Precision = 0;
            Scale = 6;
            Direction = direction;
            Value = value;
        }

        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, OrmParamDirection direction, int value)
        {
            Name = name;
            ParType = OrmParamType.Int32;
            Size = 4;
            Direction = direction;
            Value = value;
        }
        /// <summary>构造函数 修改支持无类型参数</summary>
        /// <param name="_name">参数名称</param>
        /// <param name="_value">参数值</param>
        public OrmParameter(string name, OrmParamDirection direction, long value)
        {
            Name = name;
            ParType = OrmParamType.Int64;
            Size = 8;
            Direction = direction;
            Value = value;
        }


        public override string ToString()
        {
            var t = ParType.ToString();
            var v = Value == null ? "[NULL]" : (Value.ToString() + "(" + Value.GetType().Name + ")");
            return $"{Name}({t}):{v}";
        }
    }

    public class Params : List<OrmParameter>
    {
        public Params Append(OrmParameter para)
        {
            this.Add(para);
            return this;
        }

        public Params Append(string name, object value)
        {
            this.Add(new OrmParameter(name, value));
            return this;
        }
        public Params Append(string name, OrmParamDirection direction, object value)
        {
            this.Add(new OrmParameter(name, direction, value));
            return this;
        }
        public Params Append(string name, OrmParamType type, int size, object value)
        {
            this.Add(new OrmParameter(name, type, size, value));
            return this;
        }

        public Params Append(string name, OrmParamType type, int size, OrmParamDirection direction, object value)
        {
            this.Add(new OrmParameter(name, type, size, direction, value));
            return this;
        }

        public Params AppendVChar(string name, int size, string value)
        {
            this.Add(new OrmParameter(name, OrmParamType.VarChar, size, value));
            return this;
        }

        public Params AppendNVChar(string name, int size, string value)
        {
            this.Add(new OrmParameter(name, OrmParamType.NVarchar, size, value));
            return this;
        }

        public Params AppendDecimal(string name, decimal value)
        {
            this.Add(new OrmParameter(name, value));
            return this;
        }

        public string ToSqlDeclareString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var a in this)
            {
                sb.AppendLine(string.Format("declare @{0} varchar(256);set @{0}='{1}';", a.Name.TrimStart('@'), a.Value));
            }
            return sb.ToString();
        }
    }
}
