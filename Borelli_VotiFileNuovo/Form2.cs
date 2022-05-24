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
    public partial class Form2 : Form
    {
        string riga;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (StreamWriter write = new StreamWriter(@"./tmp.txt", true)) { }

            using (StreamReader read = new StreamReader(@"./tmp.txt"))
            {
                while ((riga = read.ReadLine()) != "^")
                    treeView1.Nodes.Add(riga.Substring(5, riga.Length - 5));
            }
        }

        private void button1_Click(object sender, EventArgs e) //rimuovi classe
        {
            EliminazioneInFile(@"./tmp.txt", @"./tmp1.txt", treeView1);
            treeView1.SelectedNode.Remove();
        }

        public static void EliminazioneInFile(string fileOrig, string fileTemp, TreeView albero)
        {
            string riga;

            using (StreamWriter write = new StreamWriter(fileTemp)) { }

            using (StreamReader read = new StreamReader(fileOrig))
            {
                while ((riga = read.ReadLine()) != "^")
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

            FileInfo fi = new FileInfo(fileTemp);
            FileInfo newFi = new FileInfo(fileOrig);
            newFi.Delete();
            newFi = fi.CopyTo(fileOrig);

            fi.Delete();
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
        public static string OttieniIndiceFile(int posizione)
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
