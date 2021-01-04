using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Rainbow.ImgLib.Common;
using System.Drawing.Imaging;

namespace DataRAWReader
{
    public partial class Form1 : Form
    {
        OpenFileDialog open;
        ArquivoRAW.RAW raw;
        byte alfa = 128;
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(343, 431);
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                byte[] opened = File.ReadAllBytes(open.FileName);
                raw = new ArquivoRAW.RAW(opened,alfa);
                raw.ListInsert(treeView1, raw, open);
                fecharToolStripMenuItem.Enabled = true;
                label1.Text = "Pastas: " + raw.folderCount;
                alfaToolStripMenuItem.Enabled = false;
            }
        }

        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Nodes.Count != 0)
                treeView1.Nodes.Clear();
            raw = null;
            open.Dispose();
            fecharToolStripMenuItem.Enabled = false;
            exportarToolStripMenuItem.Enabled = false;
            exportarTextoToolStripMenuItem.Enabled = false;
            exportarTexturaToolStripMenuItem.Enabled = false;
            label1.ResetText();
            pictureBox1.Image = null;
            this.Size = new Size(343, 431);
            alfaToolStripMenuItem.Enabled = true;
        }

        private void exportarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save;
            if (treeView1.SelectedNode.Level == 2)
            {
                save = new SaveFileDialog();
                save.Filter = "Arquivo RAW|*.raw";
                save.FileName = treeView1.SelectedNode.Text;
                if (save.ShowDialog()==DialogResult.OK)
                {
                    File.WriteAllBytes(save.FileName, raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].FileData);
                    MessageBox.Show("Concluído!!");
                }
            }
            if (treeView1.SelectedNode.Level == 1)
            {
                save = new SaveFileDialog();
                save.Filter = "Arquivo RAW|*.raw";
                save.FileName = treeView1.SelectedNode.Text;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(save.FileName, raw.Pastas[treeView1.SelectedNode.Index].FileData);
                    MessageBox.Show("Concluído!!");
                }
            }
        }

        int lv2 = 0;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            if (treeView1.SelectedNode.Level == 3)
            {
                exportarToolStripMenuItem.Enabled = false;
                this.Size = new Size(707, 431);
                pictureBox1.Image = raw.Pastas[lv2].Arquivos[treeView1.SelectedNode.Parent.Index].textura.images[treeView1.SelectedNode.Index];
                exportarTexturaToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.Size = new Size(343, 431);
                pictureBox1.Image = null;
                exportarTexturaToolStripMenuItem.Enabled = false;
            }
            if (treeView1.SelectedNode.Level == 2)
            {
                lv2 = treeView1.SelectedNode.Parent.Index;
                exportarToolStripMenuItem.Enabled = true;
                if (raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].type==ArquivoRAW.RAW.Types.Texto)
                {
                    exportarTextoToolStripMenuItem.Enabled = true;
                    //foreach (var s in raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].texto.sequences)
                    //MessageBox.Show(Encodings.Naruto.UzumakiChronicles2.GetString(s));
                }
                else if (raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].type == ArquivoRAW.RAW.Types.Textura)
                {
                    //pictureBox1.Image = raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].textura.image;
                    //foreach (var s in raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].texto.sequences)
                    //MessageBox.Show(Encodings.Naruto.UzumakiChronicles2.GetString(s));
                }
                else
                    exportarTextoToolStripMenuItem.Enabled = false;
            }
            if (treeView1.SelectedNode.Level == 1)
            {
                exportarToolStripMenuItem.Enabled = true;
                if (raw.Pastas[treeView1.SelectedNode.Index].type==ArquivoRAW.RAW.Types.Texto)
                {
                    exportarTextoToolStripMenuItem.Enabled = true;
                    //foreach (var s in raw.Pastas[treeView1.SelectedNode.Index].texto.sequences)
                    //    MessageBox.Show(Encodings.Naruto.UzumakiChronicles2.GetString(s));
                }
                else
                    exportarTextoToolStripMenuItem.Enabled = false;
            }
            //else
            //    exportarToolStripMenuItem.Enabled = false;

        }

        private void exportarTextoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save;
            if (treeView1.SelectedNode.Level == 2)
            {
                save = new SaveFileDialog();
                save.Filter = "Arquivo TXT|*.txt";
                save.FileName = treeView1.SelectedNode.Text;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    string seqs = "";
                    foreach (var text in raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].texto.sequences)
                    {
                        seqs += "\n"+Encodings.Naruto.UzumakiChronicles2.GetString(text) + "\n";
                    }
                    File.WriteAllText(save.FileName, seqs);
                    MessageBox.Show("Concluído!!");
                }
            }
            if (treeView1.SelectedNode.Level == 1)
            {
                save = new SaveFileDialog();
                save.Filter = "Arquivo TXT|*.txt";
                save.FileName = treeView1.SelectedNode.Text;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    string seqs = "";
                    foreach(var text in raw.Pastas[treeView1.SelectedNode.Index].texto.sequences)
                    {
                        seqs+= "\n" + Encodings.Naruto.UzumakiChronicles2.GetString(text)+"\n";
                    }
                    File.WriteAllText(save.FileName, seqs);
                    MessageBox.Show("Concluído!!");
                }
            }
        }

        private void exportarTexturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save;
            if (treeView1.SelectedNode.Level == 3)
            {
                save = new SaveFileDialog();
                save.Filter = "PortableNetworkGraphics (.png)|*.png|Bitmap|*.bmp";
                save.FileName = treeView1.SelectedNode.Text;
                save.DefaultExt = "png";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    switch(save.FilterIndex)
                    {
                        case 1:
                            pictureBox1.Image.Save(save.FileName, ImageFormat.Png);
                            break;
                        default:
                            pictureBox1.Image.Save(save.FileName, ImageFormat.Bmp);
                            break;
                    }
                    MessageBox.Show("Concluído!!");
                }
            }
        }

        private void meio0x80128ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meio0x80128ToolStripMenuItem.Checked = true;
            máximo0xFF255ToolStripMenuItem.Checked = false;
            alfa = 128;
        }

        private void máximo0xFF255ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meio0x80128ToolStripMenuItem.Checked = false;
            máximo0xFF255ToolStripMenuItem.Checked = true;
            alfa = 255;
        }
    }
}
