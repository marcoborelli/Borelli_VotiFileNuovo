using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Borelli_VotiFileNuovo
{
    public partial class Form5 : Form
    {
        public string indClasseAlunnoMateria { get; set; }
        DataTable tabella = new DataTable();
        public Form5()
        {
            InitializeComponent();
        }
        
        private void Form5_Load(object sender, EventArgs e)
        {
            //dataGridView1.SelectedRows.;
            tabella.Columns.Add("HELO", typeof(double));
            tabella.Columns.Add("HELLOOO", typeof(string));

            tabella.Rows.Add(10, "HELLO1");
            tabella.Rows.Add(101, "HELLO11");
            tabella.Rows.Add(102, "HELLO12");
            tabella.Rows.Add(103, "HELLO13");

            dataGridView1.DataSource = tabella;
            dataGridView1.AllowUserToAddRows = false;

        }

        private void button1_Click(object sender, EventArgs e) //elimina voto
        {
            int i= dataGridView1.CurrentCell.RowIndex;
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
            
            tabella.Rows.Add(textBox1.Text,heloo);
            //MessageBox.Show(heloo);
        }
    }
}
