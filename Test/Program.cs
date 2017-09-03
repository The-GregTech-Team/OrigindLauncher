using System;
using System.Diagnostics;
using System.Threading;


namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var stat = ClientManager.Update();


            Trace.WriteLine(@"done");
           // Trace.WriteLine(result.ErrorMessage);

            Thread.CurrentThread.Join();
        }
    }
}