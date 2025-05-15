using System;
using System.Windows.Forms;
using Transconnect.Data;
using Transconnect.UI;

namespace Transconnect
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            DataInitializer dataInitializer = new DataInitializer();
            
            Application.Run(new MenuPrincipal(dataInitializer));
        }
    }
}