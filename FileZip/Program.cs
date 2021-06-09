using System.Collections.Generic;
using Vit.ConsoleUtil;

namespace Main
{
    public class Program
    {
        /// <summary> 
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {

            //var arg = new List<string>() { "help" };
            //args = arg.ToArray();


            //var arg = new List<string>() { "unzip" };
            //arg.AddRange(new[] { "-i", "T:\\temp\\tileset.zip" });
            //arg.AddRange(new[] { "-o", "T:\\temp\\tileset" });
            //args = arg.ToArray();

            //var arg = new List<string>() { "zip" };
            //arg.AddRange(new[] { "-i", "T:\\temp\\tileset" });
            ////arg.AddRange(new[] { "-o", "T:\\temp\\tileset.zip" });
            //args = arg.ToArray();

            Vit.ConsoleUtil.ConsoleHelp.Log("[FileZip] version: " + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).FileVersion);

            ConsoleHelp.Exec(args);
            return;
        }  
    }





 





}



