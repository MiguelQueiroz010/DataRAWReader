using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace DataRAWReader.ArquivoRAW
{
    public class RAW
    {
        public byte[] Data;
        public int folderCount;
        public RAW(byte[] opened)
        {
            Data = opened;
            folderCount = (int)ReadUInt(Data, 8, Uint.UInt32);
            MessageBox.Show("Pastas: "+folderCount.ToString("X2"));

        }
        public class Folder
        {
            public uint Index;
            public uint Position;
            public uint Size;
            public Folder(RAW root)
            {
                

            }

        }
        public class File
        {
            public uint Index;
            public uint Position;
            public uint Size;
            public File(Folder folder, RAW raw)
            {
                
            }
        }
        public enum Uint
        {
            UInt16,
            UInt32,
            UInt64
        };
        public static ulong ReadUInt(byte[] s, int offset, Uint type)
        {
            ulong retur = 0;
            var memory = new MemoryStream(s);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            switch(type)
            {
                case Uint.UInt16:
                    retur = reader.ReadUInt16();
                    break;
                case Uint.UInt32:
                    retur = reader.ReadUInt32();
                    break;
                case Uint.UInt64:
                    retur = reader.ReadUInt64();
                    break;
            }
            reader.Close();
            memory.Close();
            return retur;
        }
    }
}
