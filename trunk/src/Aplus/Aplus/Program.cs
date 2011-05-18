using System;

namespace Aplus
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * hosting: commandline,
               runtime: context,
             
             */

            AplusCore.Hosting.AplusConsoleHost console = new AplusCore.Hosting.AplusConsoleHost();

            Console.WriteLine("RES: {0}", console.Run(args));

            Console.WriteLine("exiting... (press enter)");
            Console.ReadLine();
        }
    }
}
