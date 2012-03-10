using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eraAPK.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace eraAPK
{
    class Program
    {
        private static eraGUI GUI;
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [STAThread]
        static void Main(string[] args)
        {
            Program.consoleOcultar();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GUI = new eraGUI();
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\temp"))
                Directory.Delete(Directory.GetCurrentDirectory() + "\\temp", true);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\temp");

            Application.Run(GUI);
        }
        public static void consoleOcultar()
        {
            IntPtr hWnd = FindWindow(null, Console.Title.ToString());
            ShowWindow(hWnd, 0);
        }
        public static void consoleMostrar()
        {
            IntPtr hWnd = FindWindow(null, Console.Title.ToString());
            ShowWindow(hWnd, 1);
        }
        public static void OcultarAlgo(string t) //Titulo
        {
            IntPtr hWnd = FindWindow(null, t);
            ShowWindow(hWnd, 0);
        }
    }
}
