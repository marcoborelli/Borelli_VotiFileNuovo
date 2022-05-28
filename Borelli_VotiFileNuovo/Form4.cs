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
    public partial class Form4 : Form
    {
        public string indClasseAlunno { get; set; }
        string riga;
        bool secondaParte=true;
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(indClasseAlunno);
            textBox1.Visible = false;
            button3.Visible = false;

            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "+" && riga != null) //^=fine prima parte /=fine seconda parte +=fine terza parte
                {
                    if (!secondaParte)
                    {
                        if (riga.Substring(0, 10) == indClasseAlunno && riga.Length >= 10)
                            treeView1.Nodes.Add(riga.Substring(15, riga.Length - 15));
                    }
                    if (riga == "/")
                        secondaParte = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) //elimina materia
        {

        }

        private void button2_Click(object sender, EventArgs e)//aggiungi materia
        {

        }

        private void button3_Click(object sender, EventArgs e)//tasto ok in aggiungi materia
        {

        }
    }
}
