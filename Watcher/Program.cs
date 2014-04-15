using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Watcher
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex mutexMyapplication = new System.Threading.Mutex(false, "Watcher");
            if (!mutexMyapplication.WaitOne(100, false))
                return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new  KeyRecord());
        }
    }
}
