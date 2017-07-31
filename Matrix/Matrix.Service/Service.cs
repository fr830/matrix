using System;
using System.ServiceProcess;
using Matrix.Server;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Service
{
    public partial class Service : ServiceBase
    {
        private static ILog log = Common.Logging.LogManager.GetLogger("Matrix.Service");

        Matrix.Server.Server server;
        public Service()
        {
            InitializeComponent();
        }

        public Service(Matrix.Server.Server srv)
        {
            InitializeComponent();
            server = srv;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            try
            {
                server.Stop();
            }
            catch (Exception e)
            {
                log.Warn(e.Message);
            }
        }
    }
}
