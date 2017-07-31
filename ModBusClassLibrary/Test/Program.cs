using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Reflection;
using System.IO;
using Matrix.Server;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            RemotingConfiguration.Configure("Test.exe.config", false);
            Console.ReadLine();
            server.Stop();
            Console.ReadKey();
        }
    }
}
