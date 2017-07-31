using System;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Reflection;
using System.IO;
using Matrix.Server;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Service
{
    static class Program
    {
        private static ILog log = Common.Logging.LogManager.GetLogger("Matrix.Service");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Получим текущий путь запуска для службы и установим текущую директорию
            string fullpath = Assembly.GetExecutingAssembly().GetName().CodeBase;
            string path = Path.GetDirectoryName(fullpath).Substring(6);
            Directory.SetCurrentDirectory(path);

            Matrix.Server.Server server = new Matrix.Server.Server();
            RemotingConfiguration.Configure("Matrix.Service.exe.config", false);
            ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new Matrix.Service.Service(server) };
            try
            {
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }
    }
}