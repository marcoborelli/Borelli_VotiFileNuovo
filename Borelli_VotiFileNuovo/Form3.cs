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
        string riga;
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

            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { } //creo file temporaneo

            using (StreamReader read = new StreamReader(fileOrig)) //inizio a leggere file originale
            {
                while ((riga = read.ReadLine()) != null) //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if ((riga != "^" && riga != "/" && riga != "*") || riga.Length < 10) //se è un separatore oppure solo una classe, con indice e nome
                    {
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
            bool condizione = true;
            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while (condizione)
                {
                    riga = read.ReadLine();
                    if (riga.Length >= 10)
                    {
                        riga = riga.Substring(10, riga.Length - 10);

                        if (riga == nome)
                            condizione = false;
                        else
                            indice++;
                    }
                        
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
    }
}
