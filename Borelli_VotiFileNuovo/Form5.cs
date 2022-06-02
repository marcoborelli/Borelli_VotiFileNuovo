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
            dateTimePicker1.Visible = false;
            textBox1.Visible = false;
            button3.Visible = false;
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

        private void button2_Click(object sender, EventArgs e) //aggiungi voto
        {
            dateTimePicker1.Visible = true;
            textBox1.Visible = true;
            button3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)//ok aggiungi voto
        {
            DateTime helo = dateTimePicker1.Value;
            string heloo = $"{helo.Day}/{helo.Month}/{helo.Year}";
            string helooo = textBox1.Text.ToUpper();
            tabella.Rows.Add(textBox1.Text, heloo);

            helooo += $";{heloo}";

            int posizionePrimaLibera = 0, nuovoIndice = 0;
            string indiceCLasseAlunnoPrima = "";
            //MessageBox.Show($"INDICE CLASSE: '{indClasseAlunnoMateria.Substring(0, 5)}'\nINDICE ALUNNO:'{indClasseAlunnoMateria.Substring(5, 5)}'\nINDICE MATERIA:'{indClasseAlunnoMateria.Substring(10, indClasseAlunnoMateria.Length - 10)}'");
            TrovaIndiceLibero(@"./tmp.txt", dataGridView1, ref nuovoIndice, ref posizionePrimaLibera, indClasseAlunnoMateria.Substring(0, 5), indClasseAlunnoMateria.Substring(5, 5), indClasseAlunnoMateria.Substring(10, 5), ref indiceCLasseAlunnoPrima);
            //MessageBox.Show(indiceCLasseAlunnoPrima);

            string nuovaRiga = indClasseAlunnoMateria, posizionePrimaDellaLibera = indiceCLasseAlunnoPrima;
            nuovaRiga += OttieniIndiceFile(nuovoIndice);
            posizionePrimaDellaLibera += OttieniIndiceFile(posizionePrimaLibera);
            MessageBox.Show($"POSIZIONE PRIMA: '{posizionePrimaDellaLibera}'\nNUOVO INDICE: '{nuovaRiga}'");

            AggiuntaVotiDateFile(@"./tmp1.txt", @"./tmp.txt", dataGridView1, nuovaRiga, helooo, posizionePrimaDellaLibera);

            //MessageBox.Show(heloo);
            dateTimePicker1.Visible = false;
            textBox1.Visible = false;
            button3.Visible = false;
        }


        public static void AggiuntaVotiDateFile(string fileTemp, string fileOrig, DataGridView dati, string nuovoNumeroRiga, string dataVoto, string posizionePrimaDellaLibera)
        {
            string riga, rigaCompleta = "";
            bool condizione = true, superateMaterie = true;
            using (StreamWriter write = new StreamWriter(fileTemp)) { }
            using (StreamReader read = new StreamReader(fileOrig))
            {
                while (condizione)
                {
                    rigaCompleta = read.ReadLine();
                    if (!superateMaterie)
                    {
                        riga = rigaCompleta.Substring(0, 20);
                        //MessageBox.Show($"RIGA MIA: '{posizionePrimaDellaLibera}'\nRIGA DA CERCARE: '{riga}'");
                        if (riga == posizionePrimaDellaLibera)
                            condizione = false;
                    }

                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(rigaCompleta);

                    if (rigaCompleta == "+")
                        superateMaterie = false;

                } //mi fermo a scrivere l'indice prima di inserire il nuovo

                using (StreamWriter write = new StreamWriter(fileTemp, true))
                    write.WriteLine($"{nuovoNumeroRiga} {dataVoto}"); //scrivo il nuovo e poi chiudo

                while ((riga = read.ReadLine()) != null)
                {
                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(riga);
                }
            }

            SovrascrivereFile(fileTemp, fileOrig);
        }
        public static void TrovaIndiceLibero(string posizioneOriginale, DataGridView dati, ref int posizioneLiber, ref int posizionePrimaDellaLibera, string indiceClasse, string indiceAlunno, string indiceMateria, ref string indiceClasseAlunnoMateriaPrima)
        {
            bool cond = true, superatiAlunni = true, entratoInCiclo = true;
            string rigaa = "", individuaClasse, cicloMateriaNuova, cicloInternoMateriaNuova = "", individuaAlunno, individuaMateria;
            int nRiga = 0, contatore = 0;
            indiceClasseAlunnoMateriaPrima = indiceClasse + indiceAlunno+indiceMateria; //di default sono questi
            using (StreamReader read = new StreamReader(posizioneOriginale))
            {
                while (cond)
                {
                    rigaa = read.ReadLine();
                    if (!superatiAlunni)
                    {
                        if (rigaa != "*")//separatore tra materie e fine
                        {
                            individuaClasse = rigaa.Substring(0, 5);
                            individuaAlunno = rigaa.Substring(5, 5); //parto dal 5 (quelli prima sono ola classe) e prendo i 5 che lo seguono
                            individuaMateria = rigaa.Substring(10, 5);
                            //MessageBox.Show($"INDICE CLASSE: '{individuaClasse}' '{indiceClasse}'\nINDICE ALUNNO: '{individuaAlunno}' '{indiceAlunno}'\nINDICE MATERIA: '{individuaMateria}' '{indiceMateria}'");
                            if (individuaClasse == indiceClasse && individuaAlunno == indiceAlunno && individuaMateria == indiceMateria) //se appartiene alla stessa classe ed è lo stesso alunno
                            {
                                rigaa = rigaa.Substring(15, 5);
                                entratoInCiclo = false;
                                if ((contatore != 0) && (int.Parse(rigaa) != nRiga + 1))//se la nuova riga già convertita non è maggiore di uno allora c'è un bugo
                                {
                                    posizionePrimaDellaLibera = nRiga;
                                    posizioneLiber = int.Parse(rigaa) - 1;
                                    return;
                                }
                                nRiga = int.Parse(rigaa);
                                contatore++;
                            }
                        }
                        else
                            cond = false;
                    }

                    if (rigaa == "+")
                        superatiAlunni = false;
                }
            }

            if (!entratoInCiclo)
                posizionePrimaDellaLibera = nRiga;
            else
            {
                using (StreamReader read = new StreamReader(posizioneOriginale))
                {
                    while ((cicloMateriaNuova = read.ReadLine()) != "*") //predo stringa prima della fine dei voti 
                        cicloInternoMateriaNuova = cicloMateriaNuova;
                }

                indiceClasseAlunnoMateriaPrima = cicloInternoMateriaNuova.Substring(0, 15); //la nuova classe che precede
                posizionePrimaDellaLibera = int.Parse(cicloInternoMateriaNuova.Substring(15, 5)); //l'indice della classe
            }


            posizioneLiber = dati.RowCount-1;
            return;
        }
        public static void EliminazioneInFile(string fileOrig, string fileTemp, DataGridView dati, string indClasseAlunnoMateria)
        {
            int indTabella = dati.CurrentCell.RowIndex;
            string indTabellaStringa = OttieniIndiceFile(indTabella);
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
