//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using ICSharpCode.SharpZipLib;
//using ICSharpCode.SharpZipLib.Zip;
//using SharpCompress.Archives.Rar;
//using SharpCompress.Archives.SevenZip;
//using SharpCompress.Archives.Zip;
//using SharpCompress.Common;
//using SharpCompress.Readers;

//namespace OA.Core.Tools
//{
//    public class ZipUtils
//    {
//        public static Stream ToZip(Dictionary<string, Stream> files)
//        {
//            System.IO.MemoryStream ms = new MemoryStream();
//            var zipStream = new ZipOutputStream(ms);
//            foreach (var a in files)
//            {
//                string name = a.Key;
//                Stream stream = a.Value;
//                zipStream.PutNextEntry(new ZipEntry(name));
//                stream.Position = 0;
//                byte[] bf = new byte[1024 * 5];
//                int l = 0;
//                while ((l = stream.Read(bf, 0, bf.Length)) > 0)
//                {
//                    zipStream.Write(bf, 0, l);
//                }
//            }
//            zipStream.Finish();
//            ms.Position = 0;
//            return ms;
//        }
//        public static IEnumerable<(Stream Stream, ZipEntry Entry)> FromZip(Stream zipStream)
//        {
//            using (var archive = new ZipInputStream(zipStream))
//            {
//                while (archive.GetNextEntry() is ZipEntry zipEntry)
//                {
//                    if (zipEntry.IsFile)
//                    {
//                        yield return (archive, zipEntry);
//                    }
//                }
//            }
//        }
//        public static IEnumerable<(Stream Stream, RarArchiveEntry Entry)> FromRar(Stream zipStream)
//        {
//            using (var archive = RarArchive.Open(zipStream))
//            {
//                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
//                {
//                    using (var stream = entry.OpenEntryStream())
//                    {
//                        yield return (stream, entry);
//                    }
//                }
//            }
//        }

//        public static IEnumerable<(Stream Stream, SevenZipArchiveEntry Entry)> FromSevenZip(Stream zipStream)
//        {
//            using (var archive = SevenZipArchive.Open(zipStream))
//            {
//                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
//                {
//                    using (var stream = entry.OpenEntryStream())
//                    {
//                        yield return (stream, entry);
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// 通用的解压
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <returns></returns>
//        public static IEnumerable<(Stream Stream, IEntry Entry)> Decompress(Stream inputStream)
//        {
//            using (var reader = ReaderFactory.Open(inputStream))
//            {
//                while (reader.MoveToNextEntry())
//                {
//                    if (!reader.Entry.IsDirectory)
//                    {
//                        using (var stream = reader.OpenEntryStream())
//                        {
//                            yield return (stream, reader.Entry);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
