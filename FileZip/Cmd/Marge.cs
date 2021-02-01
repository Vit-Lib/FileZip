using System.IO;
using System.Linq;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Marge
    {

        [Command("marge")]        
        [Remarks("合并文件。参数说明：")]
        [Remarks("-i[--input] 待合并的文件夹 或 文件查询字符串。 如 /data/a/1.7z.* ")]
        [Remarks("-o[--output] 合并后文件")]
        [Remarks("示例： marge -i \"/data/a\" -o /data/a.7z  ")]
        public static void marge(string[] args)
        {
            #region (x.1) get arg 
            string input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定要合并的文件(夹)");
                return;
            }

            string output= ConsoleHelp.GetArg(args,"-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (string.IsNullOrEmpty(output))
            {
                ConsoleHelp.Log("请指定输出文件");
                return;
            }           
            #endregion

            ConsoleHelp.Log("开始合并");          
            ConsoleHelp.Log("待合并的文件(夹)：" + input);
            ConsoleHelp.Log("输出文件：" + output);


            #region (x.2)获取 要合并的文件
            string[] inFiles;
            if (input.IndexOf('*') > 0)
            {
                inFiles = new DirectoryInfo(Path.GetDirectoryName(input)).GetFiles(Path.GetFileName(input)).OrderBy(m => m.Name).Select(m => m.FullName).ToArray();
            }
            else
            {
                inFiles = new DirectoryInfo(input).GetFiles().OrderBy(m => m.Name).Select(m => m.FullName).ToArray();
            }
            var sum = inFiles.Count();
            ConsoleHelp.Log("要合并的文件总数：" + sum);

            #endregion


            #region (x.3)合并文件       
            File.Delete(output);
            using (var sw = new FileStream(output, FileMode.CreateNew))
            {               
                int finished = 0;
                foreach (var inFile in inFiles)
                {
                    ConsoleHelp.Log($"[{ ++finished }/{sum}]合并文件："+inFile);
                    using (var reader = new FileStream(inFile, FileMode.Open))
                    {
                        reader.CopyTo(sw);
                    }
                }
            }
            #endregion

            ConsoleHelp.Log("文件合并成功！！！");

        }









    }
}
