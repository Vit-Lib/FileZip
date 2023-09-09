using System;
using System.IO;
using System.Linq;

using SharpCompress;

using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Copy
    {
        #region copy
        [Command("copy")]
        [Remarks("复制文件(夹)")]
        [Remarks("参数说明：")]
        [Remarks("-i[--input] 源文件(夹),例如 \"/data/a.txt\" \"/data/files\"")]
        [Remarks("-o[--output] 目标文件(夹)")]
        [Remarks("-f[--file] 若指定，则输出每一个文件信息")]
        [Remarks("-d[--dir] 若指定，则输出每一个文件夹信息")]
        [Remarks("-r[--remove] 若指定，则在copy完后删除源文件(夹)")]
        [Remarks("--overwrite 若指定，则强制覆盖已存在文件，默认:false")]
        [Remarks("示例： copy -i \"/data/files\" -o \"/data/files2\" --file --dir --overwrite ")]
        public static void copy(string[] args)
        {
            #region (x.1) get arg
            string input = ConsoleHelp.GetArg(args, "-i") ?? ConsoleHelp.GetArg(args, "--input");
            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelp.Log("请指定源文件(夹)");
                return;
            }

            string output = ConsoleHelp.GetArg(args, "-o") ?? ConsoleHelp.GetArg(args, "--output");

            file = !(ConsoleHelp.GetArg(args, "-f") == null && ConsoleHelp.GetArg(args, "--file") == null);
            dir = !(ConsoleHelp.GetArg(args, "-d") == null && ConsoleHelp.GetArg(args, "--dir") == null);
            remove = !(ConsoleHelp.GetArg(args, "-r") == null && ConsoleHelp.GetArg(args, "--remove") == null);
            overwrite = !(ConsoleHelp.GetArg(args, "--overwrite") == null);

            #endregion


            #region (x.2) 开始copy

            ConsoleHelp.Log("开始copy");
            ConsoleHelp.Log("源文件(夹)：" + input);
            ConsoleHelp.Log("目标文件(夹)：" + output);

            file_sumCount = 0;
            file_curCount = 0;
            dir_sumCount = 0;
            dir_curCount = 0;


            var sourceDir = new DirectoryInfo(input);
            if (sourceDir.Exists)
            {
                if (file || dir)
                {
                    GetDirCount(sourceDir);
                    ConsoleHelp.Log($"[{dir_curCount}/{dir_sumCount} d] [{file_curCount}/{file_sumCount} f]");
                }
                CopyDir(sourceDir, output);
            }
            else if (File.Exists(input))
            {
                file_curCount++;
                CopyFile(input, output);
            }
            else
            {
                ConsoleHelp.Log("文件(夹)不存在");
                return;
            }

            ConsoleHelp.Log($"[{dir_curCount}/{dir_sumCount} d] [{file_curCount}/{file_sumCount} f]");
            ConsoleHelp.Log("文件copy成功！！！");

            #endregion

        }

        static bool remove = false;

        static bool file = false;
        static int file_sumCount = 0;
        static int file_curCount = 0;

        static bool dir = false;
        static int dir_sumCount = 0;
        static int dir_curCount = 0;
        static bool overwrite = true;
        static void GetDirCount(DirectoryInfo sourceDir)
        {
            dir_sumCount++;
            sourceDir.EnumerateDirectories().ForEach(GetDirCount);
            if (file) file_sumCount += sourceDir.EnumerateFiles().Count();
        }


        static void OnFile(string fileName)
        {
            if (file)
                ConsoleHelp.Log($"[{DateTime.Now.ToString("HH:mm:ss")}][{dir_curCount}/{dir_sumCount} d] [{file_curCount}/{file_sumCount} f] file   " + fileName);
        }
        static void OnDir(string fileName)
        {
            if (dir) ConsoleHelp.Log($"[{DateTime.Now.ToString("HH:mm:ss")}][{dir_curCount}/{dir_sumCount} d] [{file_curCount}/{file_sumCount} f] dir    " + fileName);
        }
        static void CopyFile(string source, string dest)
        {
            file_curCount++;
            if (overwrite || !File.Exists(dest))
            {
                File.Copy(source, dest, overwrite);
            }
            if (remove) File.Delete(source);
            OnFile(source);
        }


        static bool CopyDir(DirectoryInfo sourceDir, string destDir)
        {
            dir_curCount++;
            if (!sourceDir.Exists) return false;

            Directory.CreateDirectory(destDir);

            var dirs = sourceDir.GetDirectories();
            if (dirs.Any())
            {
                foreach (var child in dirs)
                {
                    CopyDir(child, Path.Combine(destDir, child.Name));
                }
            }

            var files = sourceDir.GetFiles();
            if (files.Any())
            {
                foreach (var child in files)
                {
                    CopyFile(child.FullName, Path.Combine(destDir, child.Name));        
                }
            }
            OnDir(sourceDir.FullName);
            if (remove) sourceDir.Delete();
            return true;
        }

        #endregion




    }
}
