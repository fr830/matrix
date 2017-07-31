using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.Runtime.CompilerServices;
using Matrix.Types;
using Matrix.Services;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// �������� ���������
    /// </summary>
    public class ServiceManager
    {
        private static ILog log = LogManager.GetLogger<Server>();

        private Thread _thrd;
        private int _checkinterval;
        private bool _isbreak;

        private IService[] _services;

        public IService[] Services
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            get { return _services; }
        }
        /// <summary>
        /// ������ �� ��������� �������
        /// </summary>
        private Server _server;

        public bool IsBreak
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            get { return _isbreak; }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            set { _isbreak = value; }
        }

        public ServiceManager(Server server)
        {
            _server = server;
        }

        /// <summary>
        /// �������� ����� ������
        /// </summary>
        /// <param name="xmlnode">������ ������������</param>
        public IService AddService(XmlNode xmlnode)
        {
            IService srv = null; 
            //������� ������� �� ����
            String _type = xmlnode.Attributes["type"].Value;
            bool _enabled = true;
            try
            {
                _enabled = Convert.ToBoolean(xmlnode.Attributes["enabled"].Value);
                //�� ��������� ������
                if (!_enabled) return null;
            }
            catch (Exception e) { }
            
            String _description = "";
            try
            {
                _description = xmlnode.Attributes["description"].Value;
            }
            catch { }
            switch (_type.ToLower())
            {
                case "mssql":
                    srv = new ServiceMSSQL();
                    break;
                case "oracle":
                    srv = new ServiceOracle();
                    break;
                case "oledb":
                    srv = new ServiceOleDb();
                    break;
                case "snapshot":
                    srv = new ServiceSnapshot();
                    break;
                case "alert":
                    srv = new ServiceAlert();
                    break;
                case "http":
                    srv = new ServiceHttp();
                    break;
                case "exec":
                    srv = new ServiceExec();
                    break;
                case "timer":
                    srv = new ServiceDelay();
                    break;
                case "dll":
                    srv = new ServiceDll();
                    break;
                case "xml2mssql":
                    srv = new ServiceXml2MSSQL();
                    break;
                case "custom":
                    String _assembly = "";            
                    //_assembly = xmlnode.Attributes["assembly"].Value;
                    //����� �� ������� Init
                    _type = _assembly = xmlnode.ChildNodes[0].Attributes["type"].Value;
                    _assembly = xmlnode.ChildNodes[0].Attributes["assembly"].Value;
                    Assembly asmbl = Assembly.LoadFrom(_assembly);
                    srv = (IService)asmbl.CreateInstance(_type, true);
                    break;
                default:
                    return null;
            }
            srv.Server = _server;
            srv.Name = xmlnode.Attributes["name"].Value;
            srv.Description = _description;
            srv.Init(xmlnode.SelectSingleNode("init"));

            //����� ��� � ���������
            if (_services == null)
                _services = new IService[1];
            else
            {
                int cnt = _services.Length;
                IService[] old = _services;
                _services = new IService[cnt + 1];
                old.CopyTo(_services, 0);
            }
            srv.Id = _services.Length - 1;
            _services[_services.GetUpperBound(0)] = srv;
            return srv;
        }

        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        public void Start()
        {
            log.Info("������ ������� ����� ������...");
            //�������� ������ ����� �����
            if (_services != null)
            {
                foreach (IService srv in _services)
                {
                    srv.Start();
                }
                //�������� ���������� �������� �� ���������� ���������
                IsBreak = false;
                _thrd = new Thread(new ThreadStart(Run));
                _thrd.Start();
            }
            log.Info("������� ����� ������ ��������!");
        }

        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        public void Stop()
        {
            log.Info("��������� ������� ����� ������...");
            //��������� ������� ��������
            try
            {
                IsBreak = true;
                Thread.Sleep(1000);
                if (_thrd.IsAlive)
                {
                    if(!_thrd.Join(1000))
                        _thrd.Abort();
                }
            }
            catch (Exception e)
            {
                log.Warn("������ ��� �������� ������� �����! : " + e.ToString());
            }
            //��������� �������
            foreach (IService srv in _services)
            {
                srv.Stop();
            }
            log.Info("������� ����� ������ �����������!");
        }

        /// <summary>
        /// ��������� ��� � ��������� ������
        /// </summary>
        public void Write(Tag tag)
        {
            foreach (IService srv in _services)
            {
                if (srv.Name == tag.Service)
                {
                    srv.Write(tag);
                    break;
                }
            }
        }

        /// <summary>
        /// ��������� ����� �������� ������ �����e���
        /// </summary>
        public void Run()
        {
            DateTime _lastrun = DateTime.Now;
            try
            {
                while (true)
                {
                    if (DateTime.Now > _lastrun.AddMilliseconds(_checkinterval))
                    {
                        foreach (IService srv in _services)
                        {
                            if (!srv.IsRun())
                            {
                                //������ ������ �������
                                srv.ServiceStatus = "ERROR";

                                if (srv.FailCount < srv.Retry)
                                {
                                    //����������� ����������
                                    if (srv.LastRun < DateTime.Now.AddMilliseconds(-srv.BeforeRetry))
                                    {
                                        //���������� ������
                                        try
                                        {
                                            srv.Restart();
                                            log.Warn("[" + srv.Name + "] �����������.");
                                        }
                                        catch (Exception e)
                                        {
                                            log.Warn("[ServiceManager] ���������� ������� :" + e.ToString());
                                        }
                                    }
                                }
                                else //��������� ������ �������
                                    if (srv.LastRun < DateTime.Now.AddMilliseconds(-srv.RetryPause))
                                    {
                                        srv.Restart();
                                        srv.FailCount = 0;
                                        log.Warn("[" + srv.Name + "] ���������� �������...");
                                    }
                                    else
                                    {
                                        srv.ServiceStatus = "WAIT";
                                    }
                            }
                            else
                                srv.ServiceStatus = "RUN";
                        }
                        _lastrun = DateTime.Now;
                    }
                    else
                    {
                        if (IsBreak) break;
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception e)
            {
                log.Warn("[ServiceManager] Run: " + e.ToString());
            }
        }

        public void Init(XmlNode xmlconf)
        {
            _checkinterval = Convert.ToInt32(xmlconf.Attributes["checkinterval"].Value);
            //���������������� ��������
            XmlNodeList nodes = xmlconf.SelectNodes("services/service");
            foreach (XmlNode node in nodes)
            {
                IService srv = AddService(node);
            }

        }
    }
}
