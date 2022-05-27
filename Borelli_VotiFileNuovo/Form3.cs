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
        string riga, nuovoAlunno;
        bool primaParte;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            primaParte = true;
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
                    if (riga != "^" && riga != "/" && riga != "*" && riga.Length >= 10) //se è un separatore oppure solo una classe, con indice e nome
                    {
                        MessageBox.Show($"RIGA ATTUALE: '{riga}' DA CERCARE: '{$"{indiceClasse}{OttieniIndiceFile(indAlbero)}"}'");
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
            string riga;
            int indice = 0;
            bool condizione = true, superateClassi = true;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (condizione)
                {
                    riga = read.ReadLine();
                    if (!superateClassi)
                    {
                        riga = riga.Substring(10, riga.Length - 10);

                        if (riga == nome)
                            condizione = false;
                        else
                            indice++;
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

        private void button3_Click(object sender, EventArgs e)
        {
            nuovoAlunno = textBox1.Text.ToUpper();
            string nuovaRiga = indiceClasse, posizionePrimaDellaLibera = indiceClasse;
            int posizionePrimaLibera = 0, nuovoIndice = 0;
            TrovaIndiceLibero(@"./tmp.txt", treeView1, ref nuovoIndice, ref posizionePrimaLibera, indiceClasse);

            nuovaRiga += OttieniIndiceFile(nuovoIndice);
            posizionePrimaDellaLibera += OttieniIndiceFile(posizionePrimaLibera);
            MessageBox.Show($"INDICE LIBERO: '{nuovaRiga}'\nINDICE CHE LO PRECEDE: {posizionePrimaDellaLibera}'");

            //AggiuntaAlunnoFile(@"./tmp1.txt", @"./tmp.txt", treeView1, nuovaRiga, nuovoAlunno, posizionePrimaDellaLibera);
        }

        public static void TrovaIndiceLibero(string posizioneOriginale, TreeView albero, ref int posizioneLiber, ref int posizionePrimaDellaLibera, string indiceClasse)
        {
            bool cond = true, superateClassi = true;
            string rigaa, individuaClasse;
            int nRiga = 0, contatore = 0;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
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
                        }
                        else
                            cond = false;

                    }

                    if (rigaa == "^")
                        superateClassi = false;

                }
                posizionePrimaDellaLibera = nRiga;
                posizioneLiber = albero.GetNodeCount(true);
                return;//se non trova nulla ritorno il primo numero in ordine dipsonibile

            }
        }
    }
}
