using System.IO;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Zip
    {


        #region zip
        
        [Command("zip")]
        [Remarks("压缩文件(夹)")]
        [Remarks("支持格式: .zip, .gz, .tar")]
        [Remarks("参数说明：")]
        [Remarks("-i[--input] 待压缩文件(夹),例如 \"/data/a.txt\" \"/data/files\"")]
        [Remarks("-o[--output] 压缩输出文件m，使用后缀指定格式压缩，(若不指定,输出到待压缩文件所在目录)")]
        [Remarks("-p[--progress] 进度信息间隔。如0.1代表进度每增加10%，提示一次。默认0.1")]
        [Remarks("-f[--file] 若指定，则输出每一个文件信息")]
        [Remarks("示例： zip -i \"/data/files\" -o \"/data/files.zip\" -p 0.1 -f")]
        public static void zip(string[] args)
        {
            #region (x.1) get arg
            string input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定待压缩文件(夹)");
                return;
            }

            string output = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (string.IsNullOrEmpty(output))
            {
                output = Path.GetDirectoryName(input)+".zip";
            }
            Directory.CreateDirectory(Path.GetDirectoryName(output));

            float? progress = null;
            string strProgress = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (strProgress == "")
            {
                progress = 0.1f;
            } else if (strProgress != null) 
            {
                if (float.TryParse(strProgress, out var pro) && pro>0 & pro<=1) 
                {
                    progress = pro;
                }
            }

            bool printFile = !(ConsoleHelp.GetArg(args, "-f") == null && ConsoleHelp.GetArg(args, "--file") == null);
            #endregion


            #region (x.2) 开始解压

            ConsoleHelp.Log("开始压缩");
            ConsoleHelp.Log("待压缩文件(夹)：" + input);
            ConsoleHelp.Log("压缩后文件：" + output);


            var pack = new App.Logical.FilePack
            {
                inputPath = input,
                outputPath = output
            };


            if (printFile)
            {
                pack.OnFile = (int sumCount, int curCount, string fileName) => ConsoleHelp.Log($"[{ curCount }/{sumCount} f]  " + fileName);
            }

            

            if (progress != null)
            {
                pack.progressStep = progress.Value;
                pack.OnProgress = (float pro, int sumCount, int curCount) => { ConsoleHelp.Log($"[{curCount}/{sumCount}] 已完成 { pro * 100 } %"); };
            }

            pack.Pack();


            ConsoleHelp.Log("文件压缩成功！！！");

            #endregion

        }
        #endregion




    }
}
