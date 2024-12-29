using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace RE4_DCT_TOOL
{
    internal static class Repack
    {
        public static void RepackFile(FileInfo fileInfo, bool isPS4NS) 
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputName = Path.Combine(diretory, name + ".dct");
            var tx2Name = Path.Combine(diretory, name + ".txt2");

            if (!File.Exists(tx2Name))
            {
                Console.WriteLine("Error the file does not exist: " + Path.GetFileName(tx2Name));
                return;
            }

            //IDX
            StreamReader idx = fileInfo.OpenText();

            uint Fixed1 = 0;
            uint Fixed2 = 0;
            uint Value1 = 0;
            uint Amount = 0;

            Dictionary<int, (uint unk, bool hasOffset)> Fields = new Dictionary<int, (uint unk, bool hasOffset)>();

            while (!idx.EndOfStream)
            {
                string Line = idx.ReadLine();

                if (Line != null)
                {
                    Line = Line.Trim().ToUpperInvariant();

                    if (!(Line.Length == 0
                        || Line.StartsWith("#")
                        || Line.StartsWith("\\")
                        || Line.StartsWith("/")
                        || Line.StartsWith(":")
                        ))
                    {
                        var split = Line.Split(':');
                        if (split[0] == "FIXED1")
                        {
                            Fixed1 = uint.Parse(split[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (split[0] == "FIXED2")
                        {
                            Fixed2 = uint.Parse(split[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (split[0] == "VALUE1")
                        {
                            Value1 = uint.Parse(split[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (split[0] == "AMOUNT")
                        {
                            Amount = uint.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else if (split[0] == "ENTRY")
                        {
                            int ID = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                            uint _unk = uint.Parse(split[2], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                            bool _hasOffset = bool.Parse(split[3]);

                            if (!Fields.ContainsKey(ID))
                            {
                                Fields.Add(ID, (_unk, _hasOffset));
                            }
                        }
                    }

                }

            }
            idx.Close();

            string[] txt2Lines = File.ReadAllLines(tx2Name);

            var bw = new BinaryWriter(new FileInfo(outputName).Create(), Encoding.UTF8);

            bw.Write((uint)0x54434944);
            bw.Write(Fixed1);
            bw.Write(Fixed2);
            if (isPS4NS)
            {
                bw.Write((uint)0); // padding
            }
            bw.Write(Value1);
            if (isPS4NS)
            {
                bw.Write((uint)0); // padding
            }

            bw.Write(Amount);

            long[] OffsetToOffset = new long[Amount];
            bool[] hasOffset = new bool[Amount];

            //entry
            for (int i = 0; i < Amount; i++)
            {
                (uint unk, bool hasOffset) field = (0, false);
                if (Fields.ContainsKey(i))
                {
                    field = Fields[i];
                }

                bw.Write(field.unk);
                if (isPS4NS)
                {
                    bw.Write((uint)0); // padding
                }
                OffsetToOffset[i] = bw.BaseStream.Position;
                hasOffset[i] = field.hasOffset;
                bw.Write((uint)0); // offset here
                if (isPS4NS)
                {
                    bw.Write((uint)0); // padding
                }
            }

            long nextOffset = bw.BaseStream.Position;

            //text
            for (int i = 0; i < Amount; i++)
            {
                if (hasOffset[i])
                {
                    uint offsetToSet = (uint)(nextOffset - OffsetToOffset[i]);
                    bw.BaseStream.Position = OffsetToOffset[i];
                    bw.Write(offsetToSet);

                    bw.BaseStream.Position = nextOffset;
                    bw.Write((byte)0); //always starts with zero

                    byte[] text = Encoding.UTF8.GetBytes(txt2Lines[i]).Select(x => x == 0x40 ? (byte)0x0A : x).ToArray();
                    bw.Write(text);
                    nextOffset = bw.BaseStream.Position;
                }
            }
            bw.Write((byte)0); //ends with zero

            //alignment
            long rest = bw.BaseStream.Length % 16;
            bw.Write(new byte[16 - rest]);

            bw.Close();
        }

    }
}
