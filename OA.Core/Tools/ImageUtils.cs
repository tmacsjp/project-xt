//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Drawing.Imaging;
//using System.Drawing;

//namespace OA.Core.Tools
//{
//    public static class ImageUtils
//    {
//        public static MemoryStream SuitImageMaxWidthHeight(Stream imageStream, int maxWith, int maxHeight, string format)
//        {
//            var bitMap = Bitmap.FromStream(imageStream);
//            //大于等于1 ，不需要变
//            var w_rate = maxWith / (double)bitMap.Width;
//            var h_rate = maxHeight / (double)bitMap.Height;
//            var real_rate = Math.Min(w_rate, h_rate);
//            if (real_rate >= 1)
//                real_rate = 1;
//            var ms = ResizeImage(bitMap, real_rate, format);
//            return ms;
//        }

//        private static MemoryStream ResizeImage(Image image, double real_rate, string format)
//        {
//            if (string.IsNullOrWhiteSpace(format) && image is Bitmap x)
//            {
//                format = x.RawFormat.ToString();
//            }
//            format = (format ?? "").ToLower();
//            ImageFormat imgformat = ImageFormat.Jpeg;
//            switch (format)
//            {
//                case "png":
//                    imgformat = ImageFormat.Png;
//                    break;
//                case "jpg":
//                case "jpeg":
//                default:
//                    imgformat = ImageFormat.Jpeg;
//                    break;
//            }
//            var ms = new MemoryStream();
//            using (var newBitmap = ResizeImage(image, real_rate))
//            {
//                newBitmap.Save(ms, imgformat);
//            }
//            ms.Position = 0;
//            return ms;
//        }

//        private static Image ResizeImage(Image image, double real_rate)
//        {
//            if (real_rate == 1)
//            {
//                return image;
//            }
//            var towidth = (int)(real_rate * image.Width);
//            var toheight = (int)(real_rate * image.Height);
//            //新建一个bmp图片
//            var newBitmap = new System.Drawing.Bitmap(towidth, toheight);
//            using (var g = System.Drawing.Graphics.FromImage(newBitmap))
//            {
//                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
//                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
//                g.Clear(Color.Transparent);
//                g.DrawImage(image, new Rectangle(0, 0, towidth, toheight));
//            }
//            return newBitmap;
//        }

//        public static MemoryStream SuitImageMaxSpaceSize(Stream image, int maxBytes, string format)
//        {
//            MemoryStream imagstre = null;
//            if (image is MemoryStream ms)
//            {
//                imagstre = ms;
//            }
//            else
//            {
//                imagstre = new MemoryStream();
//                image.CopyTo(imagstre);
//                imagstre.Position = 0;
//            }
//            var rate = 0.75;
//            while (imagstre.Length > maxBytes)
//            {
//                var bitMap = Bitmap.FromStream(imagstre);
//                imagstre = ResizeImage(bitMap, rate, format);
//            }
//            return imagstre;
//        }
//    }
//}
