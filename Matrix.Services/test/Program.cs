using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Reflection;
using System.IO;
using Matrix.Server;


namespace Matrix.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Server server = new Server.Server();
            server.Start();
            //RemotingConfiguration.Configure("Test.exe.config", false);
            Console.ReadKey();
            server.Stop();
            Console.ReadKey();
        }
    }
}
