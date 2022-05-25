using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Borelli_VotiFileNuovo
{
    public partial class Form3 : Form
    {
        public string indiceClasse { get; set; }
        string riga;
        bool primaParte;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            primaParte = true;
            //MessageBox.Show("SONO NELLA 3" + indiceClasse);
            textBox1.Visible = false;
            button3.Visible = false;

            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "/" && riga != null) //^=fine prima parte /=fine seconda parte
                {
                    if (!primaParte)
                        treeView1.Nodes.Add(riga.Substring(10, riga.Length - 10));
                    if (riga == "^")
                        primaParte = false;
                }
            }
        }
    }
}
