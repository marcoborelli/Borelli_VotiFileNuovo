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
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;

        }

        private void button1_Click(object sender, EventArgs e) //elimina voto
        {
            EliminazioneInFile(@"./tmp.txt", @"./tmp1.txt", dataGridView1, indClasseAlunnoMateria);
            tabella.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
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

        public static void EliminazioneInFile(string fileOrig, string fileTemp, DataGridView dati, string indClasseAlunnoMateria)
        {
            int indTabella = dati.CurrentCell.RowIndex;
            string indTabellaStringa = OttieniIndiceFile(indTabella);
            //MessageBox.Show($"{indAlbero}");
            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { } //creo file temporaneo

            using (StreamReader read = new StreamReader(fileOrig)) //inizio a leggere file originale
            {
                while ((riga = read.ReadLine()) != null) //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if (riga != "^" && riga != "/" && riga != "*" && riga != "+" && riga.Length >= 20) //se è un separatore oppure solo una classe, con indice e nome
                    {
                        if (riga.Substring(0, 20) != $"{indClasseAlunnoMateria}{indTabellaStringa}")
                        {
                            using (StreamWriter write = new StreamWriter(fileTemp, true))
                                write.WriteLine(riga);
                        }
                    }
                    else
                    {
                        using (StreamWriter write = new StreamWriter(fileTemp, true))
                            write.WriteLine(riga);
                    }
                }
            }
            SovrascrivereFile(fileTemp, fileOrig);
        }
        public static void SovrascrivereFile(string fileTemp, string fileOrig)
        {
            FileInfo fi = new FileInfo(fileTemp);
            FileInfo newFi = new FileInfo(fileOrig);
            newFi.Delete();
            newFi = fi.CopyTo(fileOrig);
            fi.Delete();
        }
        public static string OttieniIndiceFile(int posizione) //mi aggiunge tutti gli zeri per salvarlo su file
        {
            string pos = posizione.ToString();
            if (pos.Length == 5)
                return pos;
            else if (pos.Length == 4)
                return $"0{pos}";
            else if (pos.Length == 3)
                return $"00{pos}";
            else if (pos.Length == 2)
                return $"000{pos}";
            else if (pos.Length == 1)
                return $"0000{pos}";
            return "-1";
        }
    }
}
