using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MyTableAttribute : Attribute
    {
        public string Prefix { get; set; }
        public bool LowerCase { get; set; }
        public bool SplitWord { get; set; }
        public string TableName { get; set; }
        public MyTableAttribute(string? tableName = null, string? prefix = null, bool lowerCase = false, bool splitWord = false)
        {
            TableName = tableName ?? "";
            Prefix = prefix ?? "";
            LowerCase = lowerCase;
            SplitWord = splitWord;
        }
    }

    public class FieldDefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; set; }

        public FieldDefaultValueAttribute(string value)
        {
            DefaultValue = value;
        }
        public FieldDefaultValueAttribute(int value)
        {
            DefaultValue = value;
        }
    }
}
