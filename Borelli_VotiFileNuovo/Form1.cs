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
        bool nuovaForm3, nuovaForm4, NuovaForm5;
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            nuovaForm3 = SelezioneClassi.nuovaForm; //bool che permette di caricare la finestra degli alunni
            if (!nuovaForm3)
            {
                SelezioneAlunno.TopLevel = false;
                SelezioneAlunno.TopMost = true;
                panel2.Controls.Add(SelezioneAlunno);
                SelezioneAlunno.indiceClasse = SelezioneClassi.indClasse; //mi passo l'indice della classe
                SelezioneAlunno.Show();

                nuovaForm4 = SelezioneAlunno.nuovaForm3;
                if (!nuovaForm4)
                {
                    SelezioneMateria.TopLevel = false;
                    SelezioneMateria.TopMost = true;
                    panel3.Controls.Add(SelezioneMateria);
                    SelezioneMateria.indClasseAlunno = SelezioneClassi.indClasse + SelezioneAlunno.indClasse3;
                    SelezioneMateria.Show();

                    NuovaForm5 = SelezioneMateria.passatoDa4;
                    if (!NuovaForm5)
                    {
                        Visualizzatore.TopLevel = false;
                        Visualizzatore.TopMost = true;
                        panel4.Controls.Add(Visualizzatore);
                        Visualizzatore.indClasseAlunnoMateria = SelezioneClassi.indClasse + SelezioneAlunno.indClasse3+SelezioneMateria.indAlunno4;
                        Visualizzatore.Show();
                    }
                }
            }

        }
    }
}
