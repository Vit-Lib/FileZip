#region << 版本注释 - v2 >>
/*
 * ========================================================================
 * 版本：v2
 * 时间：2021-06-09
 * 作者：Lith   
 * 邮箱：serset@yeah.net
 * ========================================================================
*/
#endregion

using System;
using System.Linq;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using SharpCompress.Readers;

namespace App.Logical
{
    /*    
     
        new App.Logical.FileUnpack
        {
            inputPath = @"T:\temp\blue-sh.zip",
            outputPath = @"T:\temp\blue-sh",
            OnFile = (int sumCount,int curCount,string fileName)=>{ },
            OnDir = (int sumCount,int curCount,string fileName)=>{ }, 
            OnProgress = (float progress, int sumCount, int curCount) => { Console.WriteLine($"[{curCount}/{sumCount}]:"+ progress);  }                
        }.Unpack();  

     */
    public class FileUnpack
    {

        const int stepMaxEntityCount = 10000;

        ReaderOptions readOptions = new ReaderOptions();
        ExtractionOptions extractionOptions = new ExtractionOptions() { ExtractFullPath = true, Overwrite = true };


        /// <summary>
        ///  是否可解压。支持格式: zip,gz,bzip2,tar,rar,lzip,xz,7zip,7z
        /// </summary>
        /// <param name="filePath"> 如  "a.zip"</param>
        /// <returns></returns>
        public static bool Supported(string filePath)
        {
            var format = Path.GetExtension(filePath)?.ToLower();
            return new[] { ".zip", ".gz", ".bzip2", ".tar", ".rar", ".lzip", ".xz", ".7zip", ".7z" }.Any(f => f == format);
        }


        public FileUnpack()
        {
            //readOptions.ArchiveEncoding.Default = System.Text.Encoding.UTF8;
            //readOptions.ArchiveEncoding.Default = System.Text.Encoding.Default;
            readOptions.ArchiveEncoding.Default = System.Text.Encoding.GetEncoding("GB2312");
        }



        public string inputPath;
        public string outputPath;
        
       
        public void Unpack()
        {
            dirCount = 0;
            fileCount = 0;
            sumEntityCount = 0;

            var format = Path.GetExtension(inputPath)?.ToLower();


            //Supported Reader Formats: Zip, GZip, BZip2, Tar, Rar, LZip, XZ
            if (new[] { ".zip", ".gz", ".bzip2", ".tar", ".rar", ".lzip", ".xz" }.Any(f => f == format))
            {
                UnpackByReader();
                return;
            }

            //Supported Archive Formats: Zip, GZip, Tar, Rar, 7Zip
            if (new[] { ".zip", ".gz", ".tar", ".rar", ".7z" }.Any(f => f == format))
            {
                UnpackByEntry();
                return;
            }

            throw new InvalidOperationException("Cannot Support file format.  Supported Formats: Zip, GZip, BZip2, Tar, Rar, LZip, XZ, 7Zip, 7Z");

        }

        public int sumEntityCount;

        public int fileCount;
        public int dirCount;


        /// <summary>
        ///  调用OnProgress的步骤（默认0.1）
        /// </summary>
        public float progressStep = 0.1f;
        /// <summary>
        ///  OnProgress = (float progress,int sumCount,int curCount)=>{ };
        /// </summary>
        public Action<float, int, int> OnProgress;


        /// <summary>
        ///  OnFile = (int sumCount,int curCount,string fileName)=>{ };
        /// </summary>
        public Action<int, int,string> OnFile;

        /// <summary>
        ///  OnDir = (int sumCount,int curCount,string fileName)=>{ };
        /// </summary>
        public Action<int, int, string> OnDir;

        /// <summary>
        /// Supported Archive Formats: Zip, GZip, Tar, Rar, 7Zip
        /// </summary>
        public void UnpackByEntry()
        {
            //System.IO.Compression.ZipFile.ExtractToDirectory(@"xxx.zip", @"xx\tttt", System.Text.CodePagesEncodingProvider.Instance.GetEncoding("GB2312"), true);

            dirCount = 0;
            fileCount = 0;
            sumEntityCount = 0;

            using (var archive = ArchiveFactory.Open(inputPath, readOptions))
            {
                sumEntityCount = archive.Entries.Count();

                if (sumEntityCount == 0)
                {
                    OnProgress?.Invoke(1, 0, 0);
                    return;
                }

                int importedEntityCount = 0; 
                int step = Math.Min((int)(sumEntityCount * progressStep), stepMaxEntityCount);
                int nextStep = step;


                //(法.1)流式读取
                using (var reader = archive.ExtractAllEntries())
                {
                    while (reader.MoveToNextEntry())
                    {
                        var entry = reader.Entry;
                        reader.WriteEntryToDirectory(outputPath, extractionOptions);

                        importedEntityCount++;

                        if (importedEntityCount >= nextStep)
                        {
                            nextStep = Math.Min(sumEntityCount, nextStep + step);
                            OnProgress?.Invoke(1.0f * importedEntityCount / sumEntityCount, sumEntityCount, importedEntityCount);
                        }

                        if (entry.IsDirectory)
                        {
                            dirCount++;
                            OnDir?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                        }
                        else
                        {
                            fileCount++;
                            OnFile?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                        }
                    }
                }
                return;


                //(法.2)
                foreach (var entry in archive.Entries)
                {
                    entry.WriteToDirectory(outputPath, extractionOptions);

                    importedEntityCount++;

                    if (importedEntityCount >= nextStep)
                    {
                        nextStep = Math.Min(sumEntityCount, nextStep + step);
                        OnProgress?.Invoke(1.0f * importedEntityCount / sumEntityCount, sumEntityCount, importedEntityCount);
                    }

                    if (entry.IsDirectory)
                    {
                        dirCount++;
                        OnDir?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                    }
                    else
                    {
                        fileCount++;
                        OnFile?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                    }
                }
            }
        }



        /// <summary>
        /// Supported Reader Formats: Zip, GZip, BZip2, Tar, Rar, LZip, XZ
        /// </summary>
        public void UnpackByReader()
        {
            dirCount = 0;
            fileCount = 0;
            sumEntityCount = 0;

            #region (x.1)获取文件（夹）总个数
            using (Stream stream = File.OpenRead(inputPath))
            using (var reader = ReaderFactory.Open(stream, readOptions))
            {               
                while (reader.MoveToNextEntry())
                {
                    sumEntityCount++;
                }   
            }

            if (sumEntityCount == 0)
            {
                OnProgress?.Invoke(1, 0, 0);
                return;
            }
            #endregion

            #region (x.2)解压
            using (Stream stream = File.OpenRead(inputPath))
            using (var reader = ReaderFactory.Open(stream, readOptions))
            {
                int importedEntityCount = 0;
                int step = Math.Min((int)(sumEntityCount * progressStep), stepMaxEntityCount);
                int nextStep = step;

                while (reader.MoveToNextEntry())
                {
                    var entry = reader.Entry;

                    //if (entry.IsDirectory)
                    //{
                    //    reader.WriteEntryToDirectory(outputDirPath, extractionOptions);
                    //}
                    //else
                    {
                        reader.WriteEntryToDirectory(outputPath, extractionOptions);
                    }

                    importedEntityCount++;

                    if (entry.IsDirectory)
                    {
                        dirCount++;
                        OnDir?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                    }
                    else
                    {
                        fileCount++;
                        OnFile?.Invoke(sumEntityCount, importedEntityCount, entry.Key);
                    }

                    if (importedEntityCount >= nextStep)
                    {
                        nextStep = Math.Min(sumEntityCount, nextStep + step);
                        OnProgress?.Invoke(1.0f * importedEntityCount / sumEntityCount, sumEntityCount, importedEntityCount);
                    }
                }
            }
            #endregion


        }

    }
}

 