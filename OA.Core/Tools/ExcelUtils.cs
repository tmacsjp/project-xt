//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Text;
//using ExcelDataReader;

//namespace OA.Core.Tools
//{
//    public class ExcelUtils
//    {
//        public static DataSet GetWorkbook(string aFileName)
//        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//            Encoding encoding1 = Encoding.GetEncoding(1252);
//            using (FileStream file = new FileStream(aFileName, FileMode.Open, FileAccess.Read))
//            {
//                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(file))
//                {
//                    var _ds = reader.AsDataSet();
//                    for (int t = 0; t < _ds.Tables.Count; t++)
//                    {
//                        for (int i = 0; i < _ds.Tables[t].Columns.Count; i++)
//                        {
//                            _ds.Tables[t].Columns[i].ColumnName = ConvertExtensions.ObjToStr(_ds.Tables[t].Rows[0][i]);
//                        }
//                        _ds.Tables[t].Rows.RemoveAt(0);
//                    }
//                    return _ds;
//                }
//            }
//        }
//        public static DataSet GetWorkbook(Stream aStream)
//        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//            Encoding encoding1 = Encoding.GetEncoding(1252);
//            using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(aStream))
//            {
//                var _ds = reader.AsDataSet();
//                for (int t = 0; t < _ds.Tables.Count; t++)
//                {
//                    for (int i = 0; i < _ds.Tables[t].Columns.Count; i++)
//                    {
//                        _ds.Tables[t].Columns[i].ColumnName = ConvertExtensions.ObjToStr(_ds.Tables[t].Rows[0][i]);
//                    }
//                    _ds.Tables[t].Rows.RemoveAt(0);
//                }
//                return _ds;
//            }
//        }

//        public static DataSet GetDataSetFromCSV(Stream aStream, Encoding encoding = null)
//        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//            ExcelReaderConfiguration configuration = null;
//            if (encoding != null)
//            {
//                configuration = new ExcelReaderConfiguration()
//                {
//                    FallbackEncoding = encoding
//                };
//            }
//            using (var reader = ExcelDataReader.ExcelReaderFactory.CreateCsvReader(aStream, configuration))
//            {
//                var _ds = reader.AsDataSet();
//                foreach (DataTable tb in _ds.Tables)
//                {
//                    if (tb.Rows.Count == 0)
//                        continue;
//                    var row = tb.Rows[0];
//                    foreach (DataColumn col in tb.Columns)
//                    {
//                        var tmpcolname = ConvertExtensions.ObjToStr(row[col]);
//                        if (tmpcolname == col.ColumnName)
//                            continue;
//                        int index = 2;
//                        var newcolname = tmpcolname;
//                        while (tb.Columns.Contains(newcolname))
//                        {
//                            newcolname = tmpcolname + (index++).ToString(); 
//                        }
//                        col.ColumnName = newcolname;
//                    }
//                    tb.Rows.RemoveAt(0);
//                }
                
//                return _ds;
//            }
//        }
//        public static DataTable GetSheet(DataSet aDs, int aIndex)
//        {
//            return aDs.Tables[aIndex];
//        }

//        public static DataTable GetSheet(DataSet aDs, string aName)
//        {
//            return aDs.Tables[aName];
//        }

//        public static string GetValue(DataTable aDt, int aRowIndex, int aColIndex)
//        {
//            string _result = "";
//            try
//            {
//                _result = ConvertExtensions.ObjToStr(aDt.Rows[aRowIndex][aColIndex]);
//                DateTime _dt = new DateTime();
//                if (DateTime.TryParse(_result, out _dt))
//                    _result = _dt.ToString("yyyy-MM-dd HH:mm:ss");
//            }
//            catch
//            {
//            }
//            return _result;
//        }


//        static System.Text.RegularExpressions.Regex DatetimeReg =
//            new System.Text.RegularExpressions.Regex(@"^[0-9]{4}[-/.]{1}[0-9]{2}[-/.]{1}[0-9]{2}");
//        public static string GetValue(DataTable aDt, int aRowIndex, string aColName)
//        {
//            string _result = "";
//            try
//            {
//                if (aColName == "售价")
//                {
//                    string a = "1";
//                }

//                _result = ConvertExtensions.ObjToStr(aDt.Rows[aRowIndex][aColName]);
//                DateTime _dt = new DateTime();
//                double _d = 0;
//                if (!double.TryParse(_result, out _d))
//                {
//                    if (DatetimeReg.IsMatch(_result))
//                    {
//                        if (DateTime.TryParse(_result, out _dt))
//                            _result = _dt.ToString("yyyy-MM-dd HH:mm:ss");
//                    }
//                }
//            }
//            catch
//            {
//            }
//            return _result;
//        }
//        public static string GetStringValue(DataTable aDt, int aRowIndex, string aColName)
//        {
//            string _result = "";
//            try
//            {
//                _result = ConvertExtensions.ObjToStr(aDt.Rows[aRowIndex][aColName]);
//            }
//            catch
//            {
//            }
//            return _result;
//        }
//    }
//}
