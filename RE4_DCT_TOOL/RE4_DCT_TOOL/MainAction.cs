using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_DCT_TOOL
{
    internal static class MainAction
    {
        public static void Continue(string[] args, bool isPS4NS)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i], isPS4NS);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + args[i]);
                        Console.WriteLine(ex);
                    }
                }
            }

            if (args.Length == 0)
            {
                Console.WriteLine("How to use: drag the file to the executable.");
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-DCT-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Finished!!!");
            }
        }

        private static void Action(string file, bool isPS4NS)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".DCT")
            {
                Extract.ExtractFile(fileInfo, isPS4NS);
            }
            else if (Extension == ".IDXDCT")
            {
                Repack.RepackFile(fileInfo, isPS4NS);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }
    }
}
