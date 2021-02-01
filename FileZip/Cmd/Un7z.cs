using SharpCompress.Archives;
using SharpCompress.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Un7z
    {


        #region un7z
        
        [Command("un7z")]
        [Remarks("解压7z文件。参数说明：")]
        [Remarks("-i[--input] 待解压文件 例如 \"/data/a.7z.001\"")]
        [Remarks("-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)")]
        [Remarks("-m[--mute] 若指定，则不输出解压的文件（夹）信息")]
        [Remarks("-mf[--mutefile] 若指定，则不输出解压的文件信息")]
        [Remarks("示例： un7z -i \"/data/a.7z\" -o \"/data/out\" --mute ")]
        public static void un7z(string[] args)
        {
            #region (x.1) get arg
            string  input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定待解压文件");
                return;
            }

            string output = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");
            if (string.IsNullOrEmpty(output))
            {
                //var directory = Path.GetDirectoryName(input);
                //output = Path.Combine(directory,Path.GetFileNameWithoutExtension(input));
                //if (output == input) 
                //{
                //    output = directory;
                //}
                output = Path.GetDirectoryName(input);

                //ConsoleHelp.Log("请指定输出目录");
                //return;
            }
            Directory.CreateDirectory(output);

            bool printFile = (
                (ConsoleHelp.GetArg(args, "-m") == null && ConsoleHelp.GetArg(args, "--mute") == null )
             &&(ConsoleHelp.GetArg(args, "-mf") == null && ConsoleHelp.GetArg(args, "--mutefile") == null)  );

            bool printDirectory = (ConsoleHelp.GetArg(args, "-m") == null && ConsoleHelp.GetArg(args, "--mute") == null);
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

                        input= Path.Combine(Path.GetDirectoryName(input),"tmp_"+ Path.GetFileNameWithoutExtension(input) + ".tmp");
                      
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
               
                using (var archive = ArchiveFactory.Open(input))
                {
                    var sum = archive.Entries.Count();

                    if (sum == 0)
                    {
                        ConsoleHelp.Log("压缩文件中没有文件");
                        return;
                    }

                    int finished = 0;
                    int step = sum / 100;
                    int nextStep = step;
                    foreach (var entry in archive.Entries)
                    {
                        finished++;

                        if (finished >= nextStep)
                        {
                            nextStep += step;
                            ConsoleHelp.Log($"[{ finished }/{sum} ] 已完成 { finished * 100 / sum }%");
                        }

                        if (entry.IsDirectory)
                        {
                            if (printDirectory) ConsoleHelp.Log($"[{ finished }/{sum} d]  " + entry.Key);                            
                        }
                        else
                        {
                            if (printFile) ConsoleHelp.Log($"[{ finished }/{sum} f]  " + entry.Key);                          
                        }

                        entry.WriteToDirectory(output, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }

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

            ConsoleHelp.Log("文件解压成功！！！");
        }

        #endregion

 


    }
}
