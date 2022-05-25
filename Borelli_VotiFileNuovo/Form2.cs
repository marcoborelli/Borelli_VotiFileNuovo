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
            nuovaClasse += textBox1.Text.ToUpper();
            string nuovaRiga = OttieniIndiceFile(treeView1.GetNodeCount(true));
            //MessageBox.Show($"{OttieniIndiceFile(treeView1.GetNodeCount(true))}");
            //OttieniIndiceAlbero(,)
            AggiuntaClasseFile( @"./tmp1.txt", @"./tmp.txt",treeView1, nuovaRiga, nuovaClasse);
            treeView1.Nodes.Add(nuovaClasse);
            textBox1.Visible = false;
            button3.Visible = false;
            textBox1.Text = "";
        }
        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            indClasse=OttieniIndiceFile(OttieniIndiceAlbero(treeView1.SelectedNode.Text, treeView1));
            //MessageBox.Show(OttieniIndiceFile(OttieniIndiceAlbero(treeView1.SelectedNode.Text, treeView1)));
            nuovaForm = false;
        }
        public static void AggiuntaClasseFile(string fileTemp, string fileOrig, TreeView albero, string nuovoNumeroRiga, string nomeClasse)
        {
            string riga;
            using (StreamWriter write = new StreamWriter(fileTemp)) { }
            using (StreamReader read = new StreamReader(fileOrig))
            {
                nuovoNumeroRiga = nuovoNumeroRiga.Substring(0, 5);
                int numRiga = Convert.ToInt16(nuovoNumeroRiga);

                for (int i=0;i< numRiga; i++)
                {
                    riga = read.ReadLine();
                    using (StreamWriter write = new StreamWriter(fileTemp, true))
                        write.WriteLine(riga);
                } //mi fermo a scrivere l'indice prima di inserire il nuovo
            }
            using (StreamWriter write = new StreamWriter(fileTemp, true))
            {
                write.WriteLine($"{nuovoNumeroRiga} {nomeClasse}"); //scrivo il nuovo e poi chiudo
                write.WriteLine("^");
            }

            using (StreamReader read = new StreamReader(fileOrig)) //copio tutto il resto
            {
                while((riga=read.ReadLine())!=null)
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

            using (StreamWriter write = new StreamWriter(fileTemp)) { }

            using (StreamReader read = new StreamReader(fileOrig))
            {
                while ((riga = read.ReadLine()) != "^") //continuo a copiare finche non arrivo al ^. Poi lo aggiungo
                {
                    if (riga.Substring(0, 5) != OttieniIndiceFile(OttieniIndiceAlbero(albero.SelectedNode.Text, albero)))
                    {
                        using (StreamWriter write = new StreamWriter(fileTemp, true))
                            write.WriteLine(riga);
                    }
                }
                using (StreamWriter write = new StreamWriter(fileTemp, true))
                    write.WriteLine("^");
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
                    riga = riga.Substring(5, riga.Length - 5);
                    if (riga == nome)
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

