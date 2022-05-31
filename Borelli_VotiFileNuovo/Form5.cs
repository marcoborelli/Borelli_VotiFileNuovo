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
            //dataGridView1.Rows.Add("HELLO", "HELLO1");
            //dataGridView1.SelectedRows.;
            tabella.Columns.Add("HELO", typeof(double));
            tabella.Columns.Add("HELLOOO", typeof(string));

            dataGridView1.DataSource = tabella;
           // tabella.Select();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DataRow ciao = new DataRow(45);
            //tabella.Rows.RemoveAt(tabella.Rows.IndexOf(ciao);
            //dataGridView1.Dele
        }
    }
}
