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

            }
        }

        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
