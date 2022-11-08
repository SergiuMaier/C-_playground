using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editoru
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void iesireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void despreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Editor de texte cu mai multe taburi", "Despre", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool saveFile(string fName)
        {
            bool res = false;

            try
            {
                StreamWriter sw = new StreamWriter(fName);
                sw.Write(tabControl1.Text);
                sw.Close();

                isSaved = true;

                updateInterface();

                res = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvare!\n" + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return res;
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "TextFiles (.txt)|* .txt";
            saveFileDialog1.Title = "Open a file...";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(GetRichTextBox().Text);
                    tabControl1.SelectedTab.Text = Path.GetFileName(saveFileDialog1.FileName);
                }

            }
        }
        private string fileNameInEditor = null;
        private bool isSaved = true;

        private void updateInterface()
        {
            if (!isSaved)
            {
                string s = this.Text;
                if (s[0] != '*')
                {
                    this.Text = "*" + s;
                }
            }
            else
            {
                string s = this.Text;
                if (s[0] == '*')
                {
                    this.Text = s.Replace("*", "");
                }
            }

            labelFName.Text = (fileNameInEditor == null) ? "Fisier fara nume" : fileNameInEditor;
        }

       private void salveazaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileNameInEditor == null)
            {
                saveToolStripMenuItem_Click(sender, null);
            }
            else
            {
                saveFile(fileNameInEditor);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            isSaved = false;
            updateInterface();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool isExitSure = true;

            if (!isSaved)
            {
                DialogResult resDiag = MessageBox.Show("Aveti modificari nesalvate.\nSunteti sigur ca vreti sa parasiti aplicatia?", "Confirmare", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (resDiag == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, null);
                }

                if (resDiag == DialogResult.Cancel)
                {
                    isExitSure = false;
                }
            }
            if (!isExitSure)
            {
                e.Cancel = true;
            }
        }

        private void nouToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isDoNou = true;

            TabPage tpg = new TabPage("New Document");
            RichTextBox rtb = new RichTextBox();
            rtb.Dock = DockStyle.Fill;

            tpg.Controls.Add(rtb);
            tabControl1.TabPages.Add(tpg);

            if (!isSaved)
            {
                DialogResult resDiag = MessageBox.Show("Aveti modificari nesalvate.\nVreti sa le salvati?", "Confirmare", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (resDiag == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, null);
                }

                if (resDiag == DialogResult.Cancel)
                {
                    isDoNou = false;
                }
            }

            if (isDoNou)
            {
                tabControl1.Text = "";
                fileNameInEditor = null;
                isSaved = true;

                updateInterface();
            }
        }

        private RichTextBox GetRichTextBox()
        {
            RichTextBox rtb = null;
            TabPage tpg = tabControl1.SelectedTab;

            if (tpg != null)
            {
                rtb = tpg.Controls[0] as RichTextBox;
            }

            return rtb;

        }
        
        private void deschideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isDoOpen = true;

            if (!isSaved)
            {
                DialogResult resDiag = MessageBox.Show("Aveti modificari nesalvate.\nVreti sa le salvati?", "Confirmare", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (resDiag == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, null);
                }

                if (resDiag == DialogResult.Cancel)
                {
                    isDoOpen = false;
                }
            }

            if (isDoOpen && openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new StreamReader(openDialog.FileName);

                    TabPage tp = new TabPage("New document"); 
                    RichTextBox rtb = new RichTextBox(); 
                    rtb.Dock = DockStyle.Fill; 
                    tp.Controls.Add(rtb); 
                    tabControl1.TabPages.Add(tp); 
                    rtb.Text = sr.ReadToEnd(); 
                    rtb.TextChanged += new System.EventHandler(this.textBox1_TextChanged); 
                    sr.Close();

                    fileNameInEditor = openDialog.FileName; 
                    tp.Text = Path.GetFileName(fileNameInEditor); 
                    isSaved = true;

                    updateInterface();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la deschidere!\n" + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRichTextBox().SelectAll();
        }
    }
}
