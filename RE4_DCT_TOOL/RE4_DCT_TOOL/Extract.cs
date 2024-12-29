using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_DCT_TOOL
{
    internal static class Extract
    {
        public static void ExtractFile(FileInfo fileInfo, bool isPS4NS) 
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var br = new BinaryReader(fileInfo.OpenRead());

            uint magic = br.ReadUInt32();
            if (magic != 0x54434944) //44494354 DICT
            {
                Console.WriteLine("Invalid file!");
                br.Close();
                return;
            }

            uint Fixed1 = br.ReadUInt32();
            uint Fixed2 = br.ReadUInt32();

            if (isPS4NS)
            {
                br.ReadUInt32(); // padding
            }

            uint Value1 = br.ReadUInt32();

            if (isPS4NS)
            {
                br.ReadUInt32(); // padding
            }

            uint Amount = br.ReadUInt32();

            (uint offset, uint unk, long relativeAddr)[] Fields = new (uint offset, uint unk, long relativeAddr)[Amount];

            for (int i = 0; i < Amount; i++)
            {
                Fields[i].unk = br.ReadUInt32();
                if (isPS4NS)
                {
                    br.ReadUInt32(); // padding
                }
                Fields[i].relativeAddr = br.BaseStream.Position;
                Fields[i].offset = br.ReadUInt32();
                if (isPS4NS)
                {
                    br.ReadUInt32(); // padding
                }
            }

            //idx
            var idx = new FileInfo(Path.Combine(diretory, name + ".idxdct")).CreateText();
            idx.WriteLine("# RE4_DCT_TOOL");
            idx.WriteLine("# by: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# github.com/JADERLINK");
            idx.WriteLine("");
            idx.WriteLine("Fixed1:" + Fixed1.ToString("X8"));
            idx.WriteLine("Fixed2:" + Fixed2.ToString("X8"));
            idx.WriteLine("Value1:" + Value1.ToString("X8"));
            idx.WriteLine("");
            idx.WriteLine("Amount:" + Amount.ToString("D3"));
            for (int i = 0; i < Amount; i++)
            {
                idx.WriteLine("Entry:"+ i.ToString("D3") + ":" + Fields[i].unk.ToString("X8") + ":" + (Fields[i].offset != 0).ToString());
            }
            idx.Close();

            //txt2
            var txt2 = new FileInfo(Path.Combine(diretory, name + ".txt2")).CreateText();
            for (int i = 0; i < Amount; i++)
            {
                long offsetReal = Fields[i].relativeAddr + Fields[i].offset;
                br.BaseStream.Position = offsetReal;
                List<byte> bytes = new List<byte>();
                int zeroCount = 0;
                while (true)
                {
                    byte b = br.ReadByte();

                    if (b == 0)
                    {
                        zeroCount++;

                        if (zeroCount >= 2)
                        {
                            break;
                        }
                    }
                    else if (b == 0x0A)
                    {
                        bytes.Add(0x40);
                    }
                    else if (b != 0x0D)
                    {
                        bytes.Add(b);
                    }
                }

                txt2.WriteLine(Encoding.UTF8.GetString(bytes.ToArray()));
            }

            txt2.Close();

        }

    }
}
