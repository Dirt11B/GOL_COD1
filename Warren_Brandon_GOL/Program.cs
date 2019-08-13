using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warren_Brandon_GOL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); // enabling shell styles
            Application.SetCompatibleTextRenderingDefault(false); 
            Application.Run(new Form1()); //form is instantiated here***
        }
    }
}
