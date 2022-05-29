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
        string riga, nuovaMateria;
        bool secondaParte = true;
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
            EliminazioneInFile(@"./tmp.txt", @"./tmp1.txt", treeView1, indClasseAlunno);
            treeView1.SelectedNode.Remove();
        }

        private void button2_Click(object sender, EventArgs e)//aggiungi materia
        {
            textBox1.Visible = true;
            button3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)//tasto ok in aggiungi materia
        {
            nuovaMateria = textBox1.Text.ToUpper();
            string indiceCLasseAlunnoPrima = "";
            int posizionePrimaLibera = 0, nuovoIndice = 0;
            //MessageBox.Show($"TOTALE: '{indClasseAlunno}'\n'{indClasseAlunno.Substring(0, 5)}'\n'{indClasseAlunno.Substring(5, indClasseAlunno.Length-5)}' ");
            TrovaIndiceLibero(@"./tmp.txt", treeView1, ref nuovoIndice, ref posizionePrimaLibera, indClasseAlunno.Substring(0, 5), indClasseAlunno.Substring(5, indClasseAlunno.Length - 5), ref indiceCLasseAlunnoPrima);

            string nuovaRiga = indClasseAlunno, posizionePrimaDellaLibera = indiceCLasseAlunnoPrima;

            nuovaRiga += OttieniIndiceFile(nuovoIndice);
            posizionePrimaDellaLibera += OttieniIndiceFile(posizionePrimaLibera);
            MessageBox.Show($"INDICE LIBERO: '{nuovaRiga}'\nINDICE CHE LO PRECEDE: {posizionePrimaDellaLibera}'");

            AggiuntaMateriaFile(@"./tmp1.txt", @"./tmp.txt", treeView1, nuovaRiga, nuovaMateria, posizionePrimaDellaLibera);

            treeView1.Nodes.Add(nuovaMateria);
            textBox1.Visible = false;
            button3.Visible = false;
            textBox1.Text = "";
        }

        public static void AggiuntaMateriaFile(string fileTemp, string fileOrig, TreeView albero, string nuovoNumeroRiga, string nomeClasse, string posizionePrimaDellaLibera)
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
                        riga = rigaCompleta.Substring(0, 15);
                        if (riga == posizionePrimaDellaLibera)
                            condizione = false;
                    }

                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(rigaCompleta);

                    if (rigaCompleta == "/")
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
        public static void TrovaIndiceLibero(string posizioneOriginale, TreeView albero, ref int posizioneLiber, ref int posizionePrimaDellaLibera, string indiceClasse, string indiceAlunno, ref string indiceClasseAlunnoPrima)
        {
            bool cond = true, superateClassi = true, entratoInCiclo = true;
            string rigaa = "", individuaClasse, cicloClasseNuova, cicloInternoClasseNuova = "", individuaAlunno;
            int nRiga = 0, contatore = 0;
            indiceClasseAlunnoPrima = indiceClasse + indiceAlunno; //di default sono questi
            using (StreamReader read = new StreamReader(posizioneOriginale))
            {
                while (cond)
                {
                    rigaa = read.ReadLine();
                    if (!superateClassi)
                    {
                        if (rigaa != "+")//separatore tra alunni e materie
                        {
                            individuaClasse = rigaa.Substring(0, 5);
                            individuaAlunno = rigaa.Substring(5, 5); //parto dal 5 (quelli prima sono ola classe) e prendo i 5 che lo seguono
                            MessageBox.Show($"INDICE CLASSE: '{individuaClasse}'\nINDICE ALUNNO: '{individuaAlunno}'");
                            if (individuaClasse == indiceClasse && individuaAlunno == indiceAlunno) //se appartiene alla stessa classe ed è lo stesso alunno
                            {
                                rigaa = rigaa.Substring(10, 5);
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

                    if (rigaa == "/")
                        superateClassi = false;
                }
            }

            if (!entratoInCiclo)
                posizionePrimaDellaLibera = nRiga;
            else
            {
                using (StreamReader read = new StreamReader(posizioneOriginale))
                {
                    while ((cicloClasseNuova = read.ReadLine()) != "+") //predo stringa prima della fine delle materie 
                        cicloInternoClasseNuova = cicloClasseNuova;
                }

                indiceClasseAlunnoPrima = cicloInternoClasseNuova.Substring(0, 10); //la nuova classe che precede
                posizionePrimaDellaLibera = int.Parse(cicloInternoClasseNuova.Substring(10, 5)); //l'indice della classe
            }


            posizioneLiber = albero.GetNodeCount(true);
            return;//se non trova nulla ritorno il primo numero in ordine dipsonibile
        }

        public static void EliminazioneInFile(string fileOrig, string fileTemp, TreeView albero, string indiceClasseAlunno)
        {
            int indAlbero = OttieniIndiceAlbero(albero.SelectedNode.Text, albero);
            MessageBox.Show($"{indAlbero}");
            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { } //creo file temporaneo

            using (StreamReader read = new StreamReader(fileOrig)) //inizio a leggere file originale
            {
                while ((riga = read.ReadLine()) != null) //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if (riga != "^" && riga != "/" && riga != "*" && riga != "+" && riga.Length >= 15) //se è un separatore oppure solo una classe, con indice e nome
                    {
                        //MessageBox.Show($"RIGA ATTUALE: '{riga}' DA CERCARE: '{$"{indiceClasse}{OttieniIndiceFile(indAlbero)}"}'");
                        if (riga.Substring(0, 15) != $"{indiceClasseAlunno}{OttieniIndiceFile(indAlbero)}")
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
            string riga, rigaClasse, rigaAlunno;
            int indice = 0, indClasseAlunno = 0;
            bool condizione = true, superateClassi = true;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (condizione)
                {
                    riga = read.ReadLine();

                    if (!superateClassi)
                    {
                        rigaClasse = riga.Substring(0, 5);
                        rigaAlunno = riga.Substring(5, 5);
                        riga = riga.Substring(15, riga.Length - 15);

                        MessageBox.Show($"RIGA CLASSE: '{rigaClasse}'\nRIGA ALUNNO: '{rigaAlunno}'");

                        if ((indice > 0) && (int.Parse(rigaClasse) + int.Parse(rigaAlunno)) != indClasseAlunno) //quando cambio classe l'indice mi torna pari a 0
                            indice = 0;

                        if (riga == nome)
                            condizione = false;
                        else
                            indice++;

                        indClasseAlunno = int.Parse(rigaClasse) + int.Parse(rigaAlunno);
                    }
                    if (riga == "/")
                        superateClassi = false;
                }
            }
            return indice;
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
