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
        public List<Folder> Pastas;
        public RAW(byte[] opened)
        {
            #region Abertura e verificar se é pasta
            Data = opened;
            folderCount = (int)ReadUInt(Data, 8, Int.UInt32)/0xF;
            if(folderCount==0)
            {
                MessageBox.Show("Erro: Arquivo inválido(0 Pastas lidas)!!");
                return;
            }
            Pastas = new List<Folder>();
            for (int i = 0; i < folderCount; i++)
            {
                Pastas.Add(new Folder(Data, i));
            }
            #endregion
            
        }
        public void ListInsert(TreeView tree, RAW raw, OpenFileDialog open) 
        {
            if (tree.Nodes.Count != 0)
                tree.Nodes.Clear();
            TreeNode Raiz = new TreeNode(Path.GetFileName(open.FileName));
            int i = 0;
            foreach (var folder in raw.Pastas)
            {
                TreeNode Raiz2 = null;
                if (folder.type==Types.Texto)
                    Raiz2 = new TreeNode(Path.GetFileName(open.FileName) + "_" + i + " -Texto" + "-" + folder.texto.SeqCount);
                else if (folder.type == Types.Textura)
                    Raiz2 = new TreeNode(Path.GetFileName(open.FileName) + "_" + i + " -Textura");
                else if (folder.type==Types.Unknow)
                    Raiz2 = new TreeNode(Path.GetFileName(open.FileName) + "_" + i);
                try
                {
                    if (folder.filescount != 0)
                    {
                        int j = 0;
                        foreach (var file in folder.Arquivos)
                        {
                            if (file.type==Types.Texto)
                                Raiz2.Nodes.Add(Path.GetFileName(open.FileName) + "_" + i + "_" + j + " -Texto" + "-" + file.texto.SeqCount);
                            else if (file.type == Types.Textura)
                                Raiz2.Nodes.Add(Path.GetFileName(open.FileName) + "_" + i + "_" + j + " -Textura");
                            else if(file.type==Types.Unknow)
                                Raiz2.Nodes.Add(Path.GetFileName(open.FileName) + "_" + i + "_" + j);
                            

                            j++;
                        }
                    }
                }
                catch (NullReferenceException) { }
                Raiz.Nodes.Add(Raiz2);
                i++;
              
            }
            tree.Nodes.Add(Raiz);
        }
        public class Folder
        {
            public uint Index;
            public uint Position;
            public uint Size;
            public byte[] FileData;
            public Text texto;
            public Types type = Types.Unknow;
            public List<File> Arquivos;
            public int filescount;
            public Folder(byte[] Data, int index)
            {
                #region Pasta de arquivos
                Index = (uint)index;
                Size = (uint)ReadUInt(Data, 4 + (16 * (index)), Int.UInt32);
                Position = (uint)ReadUInt(Data, 8 + (16 * (index)), Int.UInt32);
                FileData = Bin.ReadBlock(Data, Position, Size);
                //MessageBox.Show("Índice: " + Index.ToString() + "\n" +
                //    "Posição: " + Position.ToString("X2") + "\n" +
                //    "Tamanho: " + Size.ToString("X2"));
                #endregion
                #region Arquivos dentro da Pasta
                filescount = (int)ReadUInt(FileData, 0, Int.UInt32);
                if(filescount==0|filescount>0x9f|ReadUInt(FileData, FileData.Length-2,Int.UInt16).ToString("X2")=="8000")
                {
                    if (ReadUInt(FileData, FileData.Length - 2, Int.UInt16).ToString("X2") == "8000")
                    {
                        type = Types.Texto;
                        texto = new Text(FileData, index);
                    }
                    return;
                }
                Arquivos = new List<File>();
                for (int i = 0; i < filescount; i++)
                {
                    Arquivos.Add(new File(this, i));
                }
                #endregion

            }

        }
        public class File
        {
            public int Index;
            public uint Position;
            public uint Size;
            public Types type = Types.Unknow;
            public byte[] FileData;
            public Text texto;
            public File(Folder folder, int index)
            {
                Index = index;
                Position = (uint)ReadUInt(folder.FileData, 4 + (8 * (index)) , Int.UInt32);
                Size = (uint)ReadUInt(folder.FileData, 8 + (8 * (index)), Int.UInt32);
                FileData = Bin.ReadBlock(folder.FileData, Position, Size);
                if (FileData.Length > 20)
                {
                    if (ReadUInt(FileData, FileData.Length - 2, Int.UInt16).ToString("X2") == "8000")
                    {
                        type = Types.Texto;
                        texto = new Text(FileData, index);
                    }
                    if(ReadUInt(FileData, 16, Int.UInt32).ToString("X2")=="8004")
                    {
                        type = Types.Textura;
                    }
                }
                //MessageBox.Show("Índice: " + Index.ToString() + "\n" +
                //    "Posição: " + Position.ToString("X2") + "\n" +
                //    "Tamanho: " + Size.ToString("X2"));
            }
        }
        public class Text
        {
            public byte[] Data;
            public int Index;
            public uint Position;
            public uint Size;
            public uint SeqCount;
            public List<byte[]> sequences;

            public Text(byte[] file, int index)
            {
                sequences = new List<byte[]>();
                Index = index;
                Position = 0;
                Size = (uint)file.Length;
                Data = Bin.ReadBlock(file, 0, Size);
                SeqCount = (uint)ReadUInt(Data, (int)Position, Int.UInt32);
                int pos = 4;
                for (int i = 0; i < SeqCount; i++)
                {
                    sequences.Add(ReadSequence(Data, pos + (i * 4), "8001"));
                }

            }
        }
        public class Texture
        {
            public byte[] Data;
            public int Index;
            public uint Position;
            public uint Size;
            public uint Count;

            public Texture(byte[] file, int index)
            {
                Index = index;
                Position = 0;
                Size = (uint)file.Length;
                Data = Bin.ReadBlock(file, 0, Size);
                Count = (uint)ReadUInt(Data, 0, Int.UInt32);


            }
        }
        public enum Int
        {
            Int16,
            Int32,
            Int64,
            UInt16,
            UInt32,
            UInt64
        };
        public enum Types
        {
            Texto,
            Textura,
            Unknow
        };
        public static byte[] ReadSequence(byte[] file, int offset, string breaker)
        {
            var sequence = new List<byte>();
            var memory = new MemoryStream(file);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            uint pointer = reader.ReadUInt32();
            reader.Close();
            memory.Close();
            for (uint i = pointer; file[i].ToString("X2") + file[i + 1].ToString("X2") != "0080"; i += 2)
            {
                //MessageBox.Show(file[i].ToString("X2") + file[i + 1].ToString("X2"));
                sequence.Add(file[i]);
                sequence.Add(file[i + 1]);
            }
            return sequence.ToArray();
        }
        public static ulong ReadUInt(byte[] s, int offset, Int type)
        {
            ulong retur = 0;
            var memory = new MemoryStream(s);
            var reader = new BinaryReader(memory);
            reader.BaseStream.Position = offset;
            switch(type)
            {
                case Int.UInt16:
                    retur = reader.ReadUInt16();
                    break;
                case Int.UInt32:
                    retur = reader.ReadUInt32();
                    break;
                case Int.UInt64:
                    retur = reader.ReadUInt64();
                    break;
            }
            reader.Close();
            memory.Close();
            return retur;
        }
    }
}
