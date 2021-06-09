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
using SharpCompress.Common;
using System.IO;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;
using SharpCompress.Writers.GZip;

namespace App.Logical
{
    /*    
     
 

     */
    public class FilePack
    {

        const int stepMaxEntityCount = 10000;



        /// <summary>
        ///  是否可压缩。支持格式: .zip, .gz, .tar
        /// </summary>
        /// <param name="filePath"> 如  "a.zip"</param>
        /// <returns></returns>
        public static bool Supported(string filePath)
        {
            var format = Path.GetExtension(filePath)?.ToLower();
            return new[] { ".zip", ".gz", ".tar" }.Any(f => f == format);
        }


        public FilePack()
        {
           
        }


        /// <summary>
        /// 可为文件 或 文件夹
        /// </summary>
        public string inputPath;
       
        /// <summary>
        /// 输出文件，通过文件后缀绝对压缩方式
        /// </summary>
        public string outputPath;

 

        
       
        public void Pack()
        {     

            var format = Path.GetExtension(outputPath)?.ToLower();


            if (new[] { ".zip", ".gz", ".tar" }.Any(f => f == format))
            {
                PackByWriter();
                return;
            }           
            throw new InvalidOperationException("Cannot Support file format.  Supported Formats: .zip, .gz, .tar ");

        }

        public int sumEntityCount;
        public int fileCount;


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

        ///// <summary>
        /////  OnDir = (int sumCount,int curCount,string fileName)=>{ };
        ///// </summary>
        //public Action<int, int, string> OnDir;
 



        /// <summary>
        /// Supported Reader Formats: Zip, GZip, Tar
        /// </summary>
        public void PackByWriter()
        {
            fileCount = 0;
            sumEntityCount = 0;

            ArchiveType archiveType;
            WriterOptions writerOptions; 
    
            switch (Path.GetExtension(outputPath)?.ToLower())
            {
                case ".gz":
                    archiveType = ArchiveType.GZip; 
                    writerOptions = new GZipWriterOptions(); 
                    break;
                case ".tar":
                    archiveType = ArchiveType.Tar;
                    writerOptions = new TarWriterOptions(CompressionType.None, true); 
                    break;
                case ".zip": 
                    archiveType = ArchiveType.Zip; 
                    writerOptions = new WriterOptions(CompressionType.Deflate);  //CompressionType.BZip2  CompressionType.None CompressionType.LZMA CompressionType.PPMd
                    break;
                default: throw new InvalidOperationException("Cannot Support file format.  Supported Formats: .zip, .gz, .tar ");
            }


            //writerOptions.ArchiveEncoding.Default = System.Text.Encoding.UTF8;
            //readOptions.ArchiveEncoding.Default = System.Text.Encoding.Default;
            writerOptions.ArchiveEncoding.Default = System.Text.Encoding.GetEncoding("GB2312");



            if (File.Exists(inputPath))
            {
                //压缩单个文件
                sumEntityCount = 1;

                #region (x.x.2)压缩
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                using (var fileStream = File.OpenWrite(outputPath))
                using (var writer = WriterFactory.Open(fileStream, archiveType, writerOptions))
                {               
                    var directoryLength = inputPath.Length;

                    var relativePath = Path.GetFileName(inputPath);
                    writer.Write(relativePath, inputPath);

                    OnFile?.Invoke(1, 1, relativePath);
                    OnProgress?.Invoke(1.0f, 1, 1);
                }
                #endregion
            }
            else 
            {
                //压缩文件夹

                #region (x.x.1)获取文件（夹）总个数
                var enumerable = Directory.EnumerateFiles(inputPath, "*", SearchOption.AllDirectories);
                sumEntityCount = enumerable.Count();
                #endregion

                #region (x.x.2)压缩
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                using (var fileStream = File.OpenWrite(outputPath))
                using (var writer = WriterFactory.Open(fileStream, archiveType, writerOptions))
                {
                    int importedEntityCount = 0;
                    int step = Math.Min((int)(sumEntityCount * progressStep), stepMaxEntityCount);
                    int nextStep = step;


                    var directoryLength = inputPath.Length;
                    foreach (var file in enumerable)
                    {
                        var relativePath = file.Substring(directoryLength);
                        writer.Write(relativePath, file);

                        importedEntityCount++;


                        //if (entry.IsDirectory)
                        //{
                        //    dirCount++;
                        //    OnDir?.Invoke(sumEntityCount, importedEntityCount, relativePath);
                        //}
                        //else
                        {
                            fileCount++;
                            OnFile?.Invoke(sumEntityCount, importedEntityCount, relativePath);
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
}

 