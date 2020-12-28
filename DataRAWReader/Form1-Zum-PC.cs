using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataRAWReader
{
    public partial class Form1 : Form
    {
        OpenFileDialog open;
        ArquivoRAW.RAW raw;
        public Form1()
        {
            InitializeComponent();
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
                raw = new ArquivoRAW.RAW(opened);
                raw.ListInsert(treeView1, raw, open);
                fecharToolStripMenuItem.Enabled = true;
                label1.Text = "Pastas: " + raw.folderCount;
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
            label1.ResetText();
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Level == 2)
            {
                exportarToolStripMenuItem.Enabled = true;
                if (raw.Pastas[treeView1.SelectedNode.Parent.Index].Arquivos[treeView1.SelectedNode.Index].type==ArquivoRAW.RAW.Types.Texto)
                {
                    exportarTextoToolStripMenuItem.Enabled = true;
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
    }
}
