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
    public partial class Form5 : Form
    {
        public string indClasseAlunnoMateria { get; set; }
        DataTable tabella = new DataTable();
        string riga;
        bool terzaParte = true;
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            //dataGridView1.SelectedRows.;
            tabella.Columns.Add("VOTO", typeof(double));
            tabella.Columns.Add("DATA", typeof(string));
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "*" && riga != null) //^=fine prima parte /=fine seconda parte +=fine terza parte
                {
                    if (!terzaParte)
                    {
                        if (riga.Substring(0, 15) == indClasseAlunnoMateria && riga.Length >= 15)
                        {
                            riga = riga.Substring(20, riga.Length - 20);
                            string[] rigaSplit = riga.Split(';');
                            tabella.Rows.Add(double.Parse(rigaSplit[0]), rigaSplit[1]);
                        }
                    }
                    if (riga == "+")
                        terzaParte = false;
                }
            }

            dataGridView1.DataSource = tabella;
            dataGridView1.AllowUserToAddRows = false;

        }

        private void button1_Click(object sender, EventArgs e) //elimina voto
        {
            int i = dataGridView1.CurrentCell.RowIndex;
            tabella.Rows.RemoveAt(i);
            //DataRow ciao = new DataRow(45);
            //dataGridView1.Dele
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dataGridView1.AllowUserToAddRows = true;
            //dataGridView1.AllowUserToAddRows = false;
        }

        private void button3_Click(object sender, EventArgs e)//aggiungi voto
        {
            DateTime helo = dateTimePicker1.Value;
            string heloo = $"{helo.Day}/{helo.Month}/{helo.Year}";

            tabella.Rows.Add(textBox1.Text, heloo);
            //MessageBox.Show(heloo);
        }
    }
}
