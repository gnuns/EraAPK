using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace eraAPK.Core
{
    class PegarListas
    {
        public Dictionary<int, string> arquivoN;
        public Dictionary<int, string> arquivoD;
        private int T = 0;
        public PegarListas(String dirString, String ext = "")
        {
            DirectoryInfo dir = new DirectoryInfo(dirString);
            this.arquivoN = new Dictionary<int, string>();
            this.arquivoD = new Dictionary<int, string>();
            FileInfo[] Files = dir.GetFiles("*", SearchOption.AllDirectories);
            int a = 0;
            foreach (FileInfo File in Files)
            {
                string aDir = File.FullName.Replace(dir.FullName, "").Replace(File.Name, "");
                string aNom = File.Name;
                if (aNom.EndsWith(ext))
                {
                    a++;
                    arquivoD.Add(a, aDir);
                    arquivoN.Add(a, aNom);
                }
            }
            this.T = a;
        }
        public int PegarTotal()
        {
            return this.T;
        }

    }
}
