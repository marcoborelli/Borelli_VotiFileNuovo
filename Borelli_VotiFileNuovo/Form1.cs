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
    public partial class Form1 : Form
    {
        Form2 SelezioneClassi = new Form2();
        Form3 SelezioneAlunno = new Form3();
        Form4 SelezioneMateria = new Form4();
        Form5 Visualizzatore = new Form5();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show($"{Int16.MaxValue}");
            SelezioneClassi.TopLevel = false;
            SelezioneClassi.TopMost = true;
            panel1.Controls.Add(SelezioneClassi);
            SelezioneClassi.Show();

        }
    }
}
