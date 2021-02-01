using SharpCompress.Common;
using SharpCompress.Writers;
using System.IO;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Compression
    {

        #region zip
        [Command("zip")]
        [Remarks("压缩为gzip文件。参数说明：")]
        [Remarks("-i[--input] 待压缩目录 例如 \"/data/a\"")]
        [Remarks("-o[--output] 压缩后文件名(若不指定,加压到压缩文件夹所在目录),例如 \"/data/a.zip\"")]
        [Remarks("示例： zip -i \"/data/a\" -o \"/data/a.zip\" ")]
        public static void zip(string[] args)
        {
            compression(args);
        }
        #endregion
         



        #region compression
        public static void compression(string[] args)
        {

            ArchiveType archiveType;
            CompressionType compressionType;
            string fileSuffix;

            #region (x.1) get arg

            switch (args[0]) 
            {
                //case "gzip": archiveType = ArchiveType.GZip; compressionType = CompressionType.GZip; fileSuffix = "gz"; break;
                //case "rar": archiveType = ArchiveType.Rar; compressionType = CompressionType.Deflate; fileSuffix = "rar"; break;
                //case "7z": archiveType = ArchiveType.SevenZip; compressionType = CompressionType.de; fileSuffix = "7z"; break;
                //case "tar": archiveType = ArchiveType.Tar; compressionType = CompressionType.GZip; fileSuffix = "tar"; break;
                case "zip": archiveType = ArchiveType.Zip; compressionType = CompressionType.Deflate; fileSuffix = "zip"; break;
                default: return;
            }


            string input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定待压缩目录");
                return;
            }

            string output = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (string.IsNullOrEmpty(output))
            {
                output = input + "."+ fileSuffix;              
            }
            #endregion

 


            #region (x.2) 开始压缩

            ConsoleHelp.Log("开始压缩");
            ConsoleHelp.Log("待压缩文件：" + input);
            ConsoleHelp.Log("压缩后文件名：" + output);

            var writerOptions=new WriterOptions(compressionType);
            writerOptions.ArchiveEncoding.Default = System.Text.Encoding.GetEncoding("utf-8");

            using (var fileStream = File.OpenWrite(output))
            using (var writer = WriterFactory.Open(fileStream, archiveType, writerOptions))
            {               
                writer.WriteAll(input, "*", SearchOption.AllDirectories);           
            }

            #endregion

            ConsoleHelp.Log("文件压缩成功！！！");
        }

        #endregion



    }
}
