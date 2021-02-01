using SharpCompress.Archives;
using SharpCompress.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vit.ConsoleUtil;

namespace FileZip.Cmd
{
    public class Unzip
    {


        #region unzip
        
        [Command("unzip")]
        [Remarks("解压zip文件。参数说明：")]
        [Remarks("-i[--input] 待解压文件 例如 \"/data/a.zip.001\"")]
        [Remarks("-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)")]
        [Remarks("-m[--mute] 若指定，则不输出解压的文件（夹）信息")]
        [Remarks("-mf[--mutefile] 若指定，则不输出解压的文件信息")]
        [Remarks("示例： unzip -i \"/data/a.zip\" -o \"/data/out\" --mute ")]
        public static void unzip(string[] args)
        {          
            Un7z.un7z(args);
        }
        #endregion

 


    }
}
