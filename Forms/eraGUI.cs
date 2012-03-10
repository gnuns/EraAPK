using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eraAPK.Core;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Threading;

namespace eraAPK.Forms
{
    public partial class eraGUI : Form
    {
        #region Variáveis
        public string DirAtual = Directory.GetCurrentDirectory();
        public string nomeLimpo;
        public string dirTemp;
        public string dirDestino;
        #endregion

        private bool TudoOk()
        {
            int Ok = 0;
            if (opDlg.SafeFileName != "")
                Ok++;
            else MessageBox.Show("Você esqueceu de selecionar o arquivo!");
            if (apkDir.Text != "" && destinoDir.Text != "")
                Ok++;
            else MessageBox.Show("Precisamos de um destino!");
            if (apkDir.Text.EndsWith("apk") || apkDir.Text.EndsWith("zip"))
                Ok++;
            else MessageBox.Show("O final do nome do arquivo deve se .apk ou .zip!");

            if (Ok == 3)
            {
                SetarVars();
                return true;
            }
            else return false;
        }

        private void SetarVars()
        {
            this.nomeLimpo = opDlg.SafeFileName.Replace(".apk", "").Replace(".zip", "").ToLower().Replace(" ", "_");
            this.dirTemp = DirAtual + @"\temp\";
            if (destinoDir.Text.EndsWith(@"\")) this.dirDestino = destinoDir.Text;
            else this.dirDestino = destinoDir.Text + @"\";
        }



        public eraGUI()
        {
            InitializeComponent();
        }

        private void eraGUI_Load(object sender, EventArgs e) {}

        private void button1_Click(object sender, EventArgs e)
        {
            Program.consoleMostrar();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("s");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            opDlg.ShowDialog();
        }

        private void opDlg_FileOk(object sender, CancelEventArgs e)
        {
            apkDir.Text = opDlg.FileName;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (fldrDlg.ShowDialog() == DialogResult.OK)
            {
                this.destinoDir.Text = fldrDlg.SelectedPath;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime tInic = DateTime.Now;
            if (TudoOk())
            {
                string ldData = dirDestino + "eraAPK-Data\\" + nomeLimpo + "\\";
                Clear();
                Add("Preparando arquivos...");



                if (!Directory.Exists(ldData))
                    Directory.CreateDirectory(ldData);
                if (!File.Exists(ldData + "todec.apk"))
                    File.Copy(apkDir.Text, ldData + "todec.apk");


                if (!Directory.Exists(dirTemp))
                    Directory.CreateDirectory(dirTemp);

                progressBar1.Increment(5);
                Add("Convertendo DEX...\n");
                ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(DirAtual + @"\comp\dex\dex2jar.bat", ldData + "todec.apk");
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;
                var p = Process.Start(startInfo);
                p.WaitForExit();

                progressBar1.Increment(15);
                if (!File.Exists(ldData + "todec.apk.dex2jar.jar"))
                {
                    Add("Desculpe! Não foi possível decodificar este arquivo.");
                    MessageBox.Show("Desculpe! Não foi possível decodificar este arquivo.");
                    //File.Delete(ldData + "");
                    //Directory.Delete(ldData);
                    progressBar1.Increment(-20);
                }
                else
                {
                    progressBar1.Increment(15);
                    Add("Obtendo classes...");
                    FastZip fz = new FastZip();
                    fz.ExtractZip(ldData + "todec.apk.dex2jar.jar", ldData + "AppSourceCodeC\\src", "class");
                    
                    PegarListas Z = new PegarListas(ldData + "AppSourceCodeC\\src", "class");
                    Add("Processando arquivos XML...");
                    ProcessStartInfo xmlP = new ProcessStartInfo(DirAtual + @"\comp\apktool.bat", "d -f " +ldData + "todec.apk " + ldData + "AppSourceCode\\");
                    xmlP.CreateNoWindow = true;
                    xmlP.UseShellExecute = false;
                    var xmm = Process.Start(xmlP);
                    xmm.WaitForExit();
                    progressBar1.Increment(50);
                    Add("Convertendo classes...");
                    if (!Directory.Exists(dirTemp + nomeLimpo)) Directory.CreateDirectory(dirTemp + nomeLimpo);
                    if (!File.Exists(dirTemp + nomeLimpo + "\\jad.exe")) File.Copy(DirAtual + @"\comp\jad.exe", dirTemp + nomeLimpo + "\\jad.exe");
                    for (int at = 1; at <= Z.PegarTotal(); at++)
                    {
                        ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo(dirTemp + nomeLimpo + "\\jad.exe",
                            "-o -d" + dirTemp + nomeLimpo + " "+ ldData + "AppSourceCodeC\\src" + Z.arquivoD[at] + @"\" + Z.arquivoN[at]);
                        si.CreateNoWindow = true;
                        si.UseShellExecute = false;
                        var g = Process.Start(si);
                        g.WaitForExit(5000);
                        if (!Directory.Exists(ldData + "AppSourceCode\\src" + Z.arquivoD[at])) Directory.CreateDirectory(ldData + "AppSourceCode\\src" + Z.arquivoD[at]);
                        File.Move(dirTemp + nomeLimpo +"\\"+Z.arquivoN[at].Replace("class", "jad"), ldData + "AppSourceCode\\src" + Z.arquivoD[at] + @"\" + Z.arquivoN[at].Replace("class", "java"));
                        Add(Z.arquivoN[at]);
                    }
                    Add("Finalizando...");
                    DateTime tFinal = DateTime.Now;
                     progressBar1.Increment(50);
                    File.Delete(ldData + "AppSourceCode\\apktool.yml");
                    File.Delete(ldData + "todec.apk");
                    File.Delete(ldData + "todec.apk.dex2jar.jar");
                    Creditos(ldData + "AppSourceCode\\eraAPK.txt", tInic, tFinal);
                    Creditos(ldData + "eraAPK.ini", tInic, tFinal);
                    Directory.Delete(ldData + "AppSourceCodeC", true);
                    Add("Pronto!");
                    MessageBox.Show("Código fonte extraído!");
                    Process.Start(ldData);
                   
                }
            }
        }

        /*  METH0DS  */
        public void Add(string i)
        {
            this.txtG.AppendText(Environment.NewLine + "\u00BB " + i);
        }
        public void Creditos(string a, DateTime ti, DateTime tf)
        {
            TimeSpan tempo = tf- ti;
            using (StreamWriter sw = new StreamWriter(a))
            {
                sw.WriteLine("#################################################");
                sw.WriteLine("# Codigo fonte extraído utilizando o eraAPK #");
                sw.WriteLine("Nome do arquivo original: " + opDlg.SafeFileName);
                sw.WriteLine("Descompilado em: " + tempo.Minutes +"m" +tempo.Seconds+"s");
                sw.WriteLine("##################################################");
                sw.WriteLine("eraAPK, escrito por Gabriel Nunes <bielz@msn.com>");
                sw.Dispose();
                sw.Close();
            }
        }
        public void Clear()
        {
            this.txtG.Text = "";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://code.google.com/p/eraapk");
            }
            catch (Exception) { }
        }

    }
}
