using System.Collections.Generic;
using System.IO;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Unzip
    {


        #region unzip

        [Command("unzip")]
        [Remarks("解压zip文件。若为分卷（后缀为数字 如.001）则先合并文件再解压")]
        [Remarks("支持格式: zip,gz,bzip2,tar,rar,lzip,xz,7zip,7z")]
        [Remarks("参数说明：")]
        [Remarks("-i[--input] 待解压文件 例如 \"/data/a.zip.001\"")]
        [Remarks("-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)")]
        [Remarks("-p[--progress] 进度信息间隔。如0.1代表进度每增加10%，提示一次。默认0.1")]
        [Remarks("-f[--file] 若指定，则输出每一个文件信息")]
        [Remarks("-d[--dir] 若指定，则输出每一个文件夹信息")]
        [Remarks("示例： unzip -i \"/data/a.zip\" -o \"/data/out\" -p 0.1 -f -d ")]
        public static void unzip(string[] args)
        {
            #region (x.1) get arg
            string input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定待解压文件");
                return;
            }

            string output = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (string.IsNullOrEmpty(output))
            {
                output = Path.GetDirectoryName(input);
            }
            Directory.CreateDirectory(output);

            float? progress = null;
            string strProgress = ConsoleHelp.GetArg(args, "-p") ?? ConsoleHelp.GetArg(args, "--progress");
            if (strProgress == "")
            {
                progress = 0.1f;
            }
            else if (strProgress != null)
            {
                if (float.TryParse(strProgress, out var pro) && pro > 0 & pro <= 1)
                {
                    progress = pro;
                }
            }

            bool printFile = !(ConsoleHelp.GetArg(args, "-f") == null && ConsoleHelp.GetArg(args, "--file") == null);

            bool printDirectory = !(ConsoleHelp.GetArg(args, "-d") == null && ConsoleHelp.GetArg(args, "--dir") == null);
            #endregion

            bool inputFileIsTemp = false;


            try
            {
                #region (x.2)若为压缩分卷文件则合并到临时文件
                {
                    var extension = Path.GetExtension(input).TrimStart('.');
                    if (int.TryParse(extension, out _))
                    {
                        var fileSearch = Path.Combine(Path.GetDirectoryName(input), Path.GetFileNameWithoutExtension(input) + ".*");

                        input = Path.Combine(Path.GetDirectoryName(input), "tmp_" + Path.GetFileNameWithoutExtension(input) + ".tmp");

                        inputFileIsTemp = true;
                        var cmd = new List<string>() { "marge" };
                        cmd.AddRange(new[] { "-i", fileSearch });
                        cmd.AddRange(new[] { "-o", input });
                        Marge.marge(cmd.ToArray());
                    }
                }
                #endregion



                #region (x.3) 开始解压

                ConsoleHelp.Log("开始解压");
                ConsoleHelp.Log("待解压文件：" + input);
                ConsoleHelp.Log("输出目录：" + output);


                var unpack = new App.Logical.FileUnpack
                {
                    inputPath = input,
                    outputPath = output
                };


                if (printFile)
                {
                    unpack.OnFile = (int sumCount, int curCount, string fileName) => ConsoleHelp.Log($"[{ curCount }/{sumCount} f]  " + fileName);
                }

                if (printDirectory)
                {
                    unpack.OnDir = (int sumCount, int curCount, string fileName) => ConsoleHelp.Log($"[{ curCount }/{sumCount} d]  " + fileName);
                }

                if (progress != null)
                {
                    unpack.progressStep = progress.Value;
                    unpack.OnProgress = (float pro, int sumCount, int curCount) => { ConsoleHelp.Log($"[{curCount}/{sumCount}] 已完成 { pro * 100 } %"); };
                }

                unpack.Unpack();


                ConsoleHelp.Log("文件解压成功！！！");

                #endregion
            }
            finally
            {
                if (inputFileIsTemp)
                {
                    ConsoleHelp.Log("删除临时文件：" + input);
                    File.Delete(input);
                }
            }
        }
        #endregion




    }
}
