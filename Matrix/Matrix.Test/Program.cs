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
            //string _val = "0,95";
            //float f = Convert.ToSingle(_val);
            //Console.WriteLine(f);
            Server.Server server = new Server.Server();
            try
            {
                server.Start();
                RemotingConfiguration.Configure("Matrix.Test.exe.config", false);
                Console.ReadKey();
            }
            finally
            {
                server.Stop();
            }
            Console.ReadKey();
        }
    }
}
