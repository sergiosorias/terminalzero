using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UpdatesManager
{
    class Program
    {
        static void Main(string[] args)
        {
            if(UpdatesManager.ExistsNewVersions)
            {
                Console.WriteLine("Hay versiones nuevas..");
                Console.WriteLine("Actualizando..");
                UpdatesManager.RunUpdateProcess(Console.WriteLine,null);
            }
            else
            {
                Console.WriteLine("No existen versiones nuevas..");
                
            }
            Console.WriteLine("Se iniciara la aplicación normalmente");
            System.Threading.Thread.Sleep(2000);
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, "Terminal Zero.exe"));
            proc.Start();
        }
    }
}
