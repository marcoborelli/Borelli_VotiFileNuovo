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
using Microsoft.VisualBasic;

namespace Borelli_VotiFileNuovo
{
    public partial class Form2 : Form
    {
        string riga, nuovaClasse="";
        public bool nuovaForm { get; set; }
        public string indClasse { get; set; }
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            nuovaForm = true;
            textBox1.Visible = false;
            button3.Visible = false;
            using (StreamWriter write = new StreamWriter(@"./tmp.txt", true)) { }

            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "^" && riga != null)
                    treeView1.Nodes.Add(riga.Substring(5, riga.Length - 5));
            }
        }

       

        private void button1_Click(object sender, EventArgs e) //rimuovi classe
        {
            EliminazioneInFile(@"./tmp.txt", @"./tmp1.txt", treeView1);
            treeView1.SelectedNode.Remove();
        }
        private void button2_Click(object sender, EventArgs e) //aggiungi classe
        {
            textBox1.Visible = true;
            button3.Visible = true;
        }
        private void button3_Click(object sender, EventArgs e)//ok in aggiunta classe
        {
            nuovaClasse = textBox1.Text.ToUpper();
            int posizionePrimaLibera = 0, nuovoIndice = 0;
            TrovaIndiceLibero(@"./tmp.txt", treeView1, ref nuovoIndice, ref posizionePrimaLibera);
            string nuovaRiga = OttieniIndiceFile(nuovoIndice);
            string posizionePrimaDellaLibera = OttieniIndiceFile(posizionePrimaLibera);
            //MessageBox.Show($"{OttieniIndiceFile(treeView1.GetNodeCount(true))}");
            //OttieniIndiceAlbero(,)
            AggiuntaClasseFile( @"./tmp1.txt", @"./tmp.txt",treeView1, nuovaRiga, nuovaClasse, posizionePrimaDellaLibera);
            treeView1.Nodes.Add(nuovaClasse);
            textBox1.Visible = false;
            button3.Visible = false;
            textBox1.Text = "";
           
        }
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            indClasse=OttieniIndiceFile(OttieniIndiceAlbero(treeView1.SelectedNode.Text, treeView1));
            nuovaForm = false;
        }

        public static void TrovaIndiceLibero(string posizioneOriginale, TreeView albero, ref int posizioneLiber, ref int posizionePrimaDellaLibera)
        {
            bool cond = true;
            string rigaa;
            int nRiga = 0, contatore = 0;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (cond)
                {
                    rigaa = read.ReadLine();
                    if (rigaa != "^")
                    {
                        rigaa = rigaa.Substring(0, 5);
                        MessageBox.Show(rigaa);
                        if ((contatore != 0) && (int.Parse(rigaa) != nRiga + 1))//se la nuova riga già convertita non è maggiore di uno allora c'è un bugo
                        {
                            posizionePrimaDellaLibera = nRiga;
                            posizioneLiber = int.Parse(rigaa) - 1;
                            return;
                        }
                        nRiga = int.Parse(rigaa);
                        contatore++;
                    }
                    else
                        cond = false;
                }
                posizionePrimaDellaLibera = nRiga;
                posizioneLiber = albero.GetNodeCount(true);
                return;//se non trova nulla ritorno il primo numero in ordine dipsonibile

            }
        }
        public static void AggiuntaClasseFile(string fileTemp, string fileOrig, TreeView albero, string nuovoNumeroRiga, string nomeClasse, string posizionePrimaDellaLibera)
        {
            string riga, rigaCompleta="";
            bool condizione = true;
            using (StreamWriter write = new StreamWriter(fileTemp)) { }
            using (StreamReader read = new StreamReader(fileOrig))
            {
                //nuovoNumeroRiga = nuovoNumeroRiga.Substring(0, 5);
                //int numRiga = Convert.ToInt16(nuovoNumeroRiga);

                while (condizione)
                {
                    rigaCompleta = read.ReadLine();
                    riga = rigaCompleta.Substring(0, 5);
                    //MessageBox.Show($"POSIZIONE PRIMA DELLA LIBERA: '{posizionePrimaDellaLibera}' MIA POSIZIONE: '{riga}'");
                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(rigaCompleta);

                    if (riga == posizionePrimaDellaLibera)
                        condizione = false;
                } //mi fermo a scrivere l'indice prima di inserire il nuovo

                using (StreamWriter write = new StreamWriter(fileTemp, true))
                {
                    //write.WriteLine(rigaCompleta);
                    write.WriteLine($"{nuovoNumeroRiga} {nomeClasse}"); //scrivo il nuovo e poi chiudo
                }
                    

                while ((riga = read.ReadLine()) != null)
                {
                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(riga);
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

        public static void EliminazioneInFile(string fileOrig, string fileTemp, TreeView albero)
        {
            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { } //creo file temporaneo

            using (StreamReader read = new StreamReader(fileOrig)) //inizio a leggere file originale
            {
                while ((riga = read.ReadLine()) !=null) //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if (riga != "^" && riga != "/" && riga != "*" && riga != "+") //se è un carattere non posso fare substring, quindi sapendo che sono minimo 5 lo copio e stop
                    {
                        if (riga.Substring(0, 5) != OttieniIndiceFile(OttieniIndiceAlbero(albero.SelectedNode.Text, albero)))
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
                //using (StreamWriter write = new StreamWriter(fileTemp, true))
                    //write.WriteLine("^");
            }
            SovrascrivereFile(fileTemp, fileOrig);

        }

        public static int OttieniIndiceAlbero(string nome, TreeView albero) //serve per trovare l'indice dell'elemento basandomi sulla posizione che hanno nel file txt
        {
            string riga;
            int indice = 0;
            bool condizione = true;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (condizione)
                {
                    riga = read.ReadLine();
                    //MessageBox.Show($"RIGA: {riga}");
                    riga = riga.Substring(5, riga.Length - 5);
                    if (riga == nome)//quando il nome è uguale esco dal ciclo e ho l'indice a cui si trova
                        condizione = false;
                    else
                        indice++;
                }
            }
            return indice;
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

