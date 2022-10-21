using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OA.Core
{
    public class Security
    {
        public static readonly DateTime TimespanStart = new DateTime(1970, 1, 1);
        /// <summary>
        /// 北京当前时间
        /// </summary>
        public static DateTime BeiJingNow => new DateTime(DateTime.UtcNow.AddHours(8).Ticks, DateTimeKind.Local);
        public static DateTime UtcNow => DateTime.UtcNow;
        public static string GUID = Guid.NewGuid().ToString("N");

        public static long Timestamp => DateTime.UtcNow.GetTimestamp();
        public static long TimestampMillisecond => DateTime.UtcNow.GetTimestampMillisecond();
        /// <summary>返回md5算法32位小写</summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string MD5(string Text)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(Text);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

        /// <summary>DES加密</summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string EnDES(string Source)
        {
            return EnDES(Source, "wSht8570");
        }

        /// <summary>DES加密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string EnDES(string Source, string Key)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(@"[^a-zA-Z]+", Key))
            {
                throw new Exception("EnDES函数的密码必须为英文字母!");
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中  
            //原来使用的UTF8编码，我改成Unicode编码了，不行  
            byte[] inputByteArray = Encoding.Default.GetBytes(Source);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt);  

            //建立加密对象的密钥和偏移量  
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            //使得输入密码必须输入英文文本  
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream  
            //(It  will  end  up  in  the  memory  stream)  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string  
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //Format  as  hex  
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        public static string DeDES(string Source)
        {
            return DeDES(Source, "wSht8570");
        }

        /// <summary>DES解密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string DeDES(string Source, string Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //Put  the  input  string  into  the  byte  array  
            byte[] inputByteArray = new byte[Source.Length / 2];
            for (int x = 0; x < Source.Length / 2; x++)
            {
                int i = (Convert.ToInt32(Source.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //建立加密对象的密钥和偏移量，此值重要，不能修改  
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //Get  the  decrypted  data  back  from  the  memory  stream  
            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>取随机数字
        /// </summary>
        /// <param name="aMin">最小</param>
        /// <param name="aMax">最大</param>
        public static int GetRandomNumeric(int min, int max)
        {
            Random ro = new Random(unchecked((int)DateTime.Now.Ticks));
            var num = ro.Next(min, max);
            return num;
        }

    }
}
