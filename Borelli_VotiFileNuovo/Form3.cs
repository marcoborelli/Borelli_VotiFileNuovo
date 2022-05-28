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
        public string indClasse3 { get; set; }
        public bool nuovaForm3 { get; set; }
        string riga, nuovoAlunno;
        bool primaParte = true;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            nuovaForm3 = true;

            //MessageBox.Show("SONO NELLA 3" + indiceClasse);
            textBox1.Visible = false;
            button3.Visible = false;

            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "/" && riga != null) //^=fine prima parte /=fine seconda parte
                {
                    if (!primaParte)
                    {
                        if (riga.Substring(0, 5) == indiceClasse && riga.Length >= 5)
                            treeView1.Nodes.Add(riga.Substring(10, riga.Length - 10));
                    }
                    if (riga == "^")
                        primaParte = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//elimina alunno
        {
            EliminazioneInFile(@"./tmp.txt", @"./tmp1.txt", treeView1, indiceClasse);
            treeView1.SelectedNode.Remove();
        }
        private void button3_Click(object sender, EventArgs e) //ok in aggiungi alunno
        {
            nuovoAlunno = textBox1.Text.ToUpper();
            string indiceCLassePrima = "";
            int posizionePrimaLibera = 0, nuovoIndice = 0;
            TrovaIndiceLibero(@"./tmp.txt", treeView1, ref nuovoIndice, ref posizionePrimaLibera, indiceClasse, ref indiceCLassePrima);

            string nuovaRiga = indiceClasse, posizionePrimaDellaLibera = indiceCLassePrima;

            nuovaRiga += OttieniIndiceFile(nuovoIndice);
            posizionePrimaDellaLibera += OttieniIndiceFile(posizionePrimaLibera);
            MessageBox.Show($"INDICE LIBERO: '{nuovaRiga}'\nINDICE CHE LO PRECEDE: {posizionePrimaDellaLibera}'");

            AggiuntaAlunnoFile(@"./tmp1.txt", @"./tmp.txt", treeView1, nuovaRiga, nuovoAlunno, posizionePrimaDellaLibera);

            treeView1.Nodes.Add(nuovoAlunno);
            textBox1.Visible = false;
            button3.Visible = false;
            textBox1.Text = "";
        }

        public static void AggiuntaAlunnoFile(string fileTemp, string fileOrig, TreeView albero, string nuovoNumeroRiga, string nomeClasse, string posizionePrimaDellaLibera)
        {
            string riga, rigaCompleta = "";
            bool condizione = true, superateClassi = true;
            using (StreamWriter write = new StreamWriter(fileTemp)) { }
            using (StreamReader read = new StreamReader(fileOrig))
            {
                while (condizione)
                {
                    rigaCompleta = read.ReadLine();
                    if (!superateClassi)
                    {
                        riga = rigaCompleta.Substring(0, 10);
                        if (riga == posizionePrimaDellaLibera)
                            condizione = false;
                    }

                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(rigaCompleta);

                    if (rigaCompleta == "^")
                        superateClassi = false;

                } //mi fermo a scrivere l'indice prima di inserire il nuovo

                using (StreamWriter write = new StreamWriter(fileTemp, true))
                    write.WriteLine($"{nuovoNumeroRiga} {nomeClasse}"); //scrivo il nuovo e poi chiudo

                while ((riga = read.ReadLine()) != null)
                {
                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(riga);
                }
            }

            SovrascrivereFile(fileTemp, fileOrig);
        }

        public static void EliminazioneInFile(string fileOrig, string fileTemp, TreeView albero, string indiceClasse)
        {
            int indAlbero = OttieniIndiceAlbero(albero.SelectedNode.Text, albero);
            MessageBox.Show($"{indAlbero}");
            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { } //creo file temporaneo

            using (StreamReader read = new StreamReader(fileOrig)) //inizio a leggere file originale
            {
                while ((riga = read.ReadLine()) != null) //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if (riga != "^" && riga != "/" && riga != "*" && riga != "+" && riga.Length >= 10) //se è un separatore oppure solo una classe, con indice e nome
                    {
                        //MessageBox.Show($"RIGA ATTUALE: '{riga}' DA CERCARE: '{$"{indiceClasse}{OttieniIndiceFile(indAlbero)}"}'");
                        if (riga.Substring(0, 10) != $"{indiceClasse}{OttieniIndiceFile(indAlbero)}")
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

        public static int OttieniIndiceAlbero(string nome, TreeView albero)
        {
            string riga, rigaClasse;
            int indice = 0, indClasse = 0;
            bool condizione = true, superateClassi = true;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (condizione)
                {
                    riga = read.ReadLine();
                    if (!superateClassi)
                    {
                        rigaClasse = riga.Substring(0, 5);
                        riga = riga.Substring(10, riga.Length - 10);

                        //MessageBox.Show($"RIGA CLASSE: {rigaClasse}");

                        if ((indice > 0) && int.Parse(rigaClasse) != indClasse) //quando cambio classe l'indice mi torna pari a 0
                            indice = 0;

                        if (riga == nome)
                            condizione = false;
                        else
                            indice++;

                        indClasse = int.Parse(rigaClasse);
                    }
                    if (riga == "^")
                        superateClassi = false;
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
        public static void SovrascrivereFile(string fileTemp, string fileOrig)
        {
            FileInfo fi = new FileInfo(fileTemp);
            FileInfo newFi = new FileInfo(fileOrig);
            newFi.Delete();
            newFi = fi.CopyTo(fileOrig);
            fi.Delete();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
            button3.Visible = true;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            indClasse3 = OttieniIndiceFile(OttieniIndiceAlbero(treeView1.SelectedNode.Text, treeView1));
            //MessageBox.Show(OttieniIndiceFile(OttieniIndiceAlbero(treeView1.SelectedNode.Text, treeView1)));
            nuovaForm3 = false;
        }

        public static void TrovaIndiceLibero(string posizioneOriginale, TreeView albero, ref int posizioneLiber, ref int posizionePrimaDellaLibera, string indiceClasse, ref string indiceClassePrima)
        {
            bool cond = true, superateClassi = true, entratoInCiclo = true;
            string rigaa = "", individuaClasse, cicloClasseNuova, cicloInternoClasseNuova = "";
            int nRiga = 0, contatore = 0;
            indiceClassePrima = indiceClasse;
            using (StreamReader read = new StreamReader(posizioneOriginale))
            {
                while (cond)
                {
                    rigaa = read.ReadLine();
                    if (!superateClassi)
                    {
                        if (rigaa != "/")//separatore tra alunni e materie
                        {
                            individuaClasse = rigaa.Substring(0, 5);
                            if (individuaClasse == indiceClasse) //se appartiene alla stessa classe
                            {
                                rigaa = rigaa.Substring(5, 5); //parto dal 5 (quelli prima sono ola classe) e prendo i 5 che lo seguono
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

                    if (rigaa == "^")
                        superateClassi = false;
                }
            }

            if (!entratoInCiclo)
                posizionePrimaDellaLibera = nRiga;
            else
            {
                using (StreamReader read = new StreamReader(posizioneOriginale))
                {
                    while ((cicloClasseNuova = read.ReadLine()) != "/") //predo stringa prima della fine degli alunni 
                        cicloInternoClasseNuova = cicloClasseNuova;
                }

                indiceClassePrima = cicloInternoClasseNuova.Substring(0, 5); //la nuova classe che precede
                posizionePrimaDellaLibera = int.Parse(cicloInternoClasseNuova.Substring(5, 5)); //l'indice della classe
            }


            posizioneLiber = albero.GetNodeCount(true);
            return;//se non trova nulla ritorno il primo numero in ordine dipsonibile
        }
    }
}
