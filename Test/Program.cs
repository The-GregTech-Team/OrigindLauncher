using System;
using System.Threading;
using OrigindLauncher.Resources.Client;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stat = ClientManager.Update(e => { Console.WriteLine($"Done: {e.FileLocation}"); },
                e =>
                {
                    Console.WriteLine(
                        $"Running: {e.FileLocation}: {e.BytesReceived / (double) e.TotalBytesToReceive:P}");
                }, e => { Console.WriteLine($"Error: {e.FileLocation}, {e.Exception}"); }, () => { });


            Console.WriteLine("done");
           // Console.WriteLine(result.ErrorMessage);

            Thread.CurrentThread.Join();
        }
    }
}