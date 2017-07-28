using System;
using System.Threading;
using OrigindLauncher.Resources.Client;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stat = ClientManager.Update();


            Console.WriteLine("done");
           // Console.WriteLine(result.ErrorMessage);

            Thread.CurrentThread.Join();
        }
    }
}