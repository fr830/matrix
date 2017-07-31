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
    /// Менеджер адаптеров
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
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            get { return _services; }
        }
        /// <summary>
        /// Ссылка на экземпляр сервера
        /// </summary>
        private Server _server;

        public bool IsBreak
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            get { return _isbreak; }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            set { _isbreak = value; }
        }

        public ServiceManager(Server server)
        {
            _server = server;
        }

        /// <summary>
        /// Добавить новый сервис
        /// </summary>
        /// <param name="xmlnode">секция конфигурации</param>
        public IService AddService(XmlNode xmlnode)
        {
            IService srv = null; 
            //Выберем адаптер по типу
            String _type = xmlnode.Attributes["type"].Value;
            bool _enabled = true;
            try
            {
                _enabled = Convert.ToBoolean(xmlnode.Attributes["enabled"].Value);
                //Не загружать сервис
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
                    //Взять из раздела Init
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

            //Внесём его в коллекцию
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
        /// Стартовать систему сбора
        /// </summary>
        public void Start()
        {
            log.Info("Запуск системы сбора данных...");
            //Запустим каждый поток сбора
            if (_services != null)
            {
                foreach (IService srv in _services)
                {
                    srv.Start();
                }
                //Запустим подсистему слежения за состоянием адаптеров
                IsBreak = false;
                _thrd = new Thread(new ThreadStart(Run));
                _thrd.Start();
            }
            log.Info("Система сбора данных запущена!");
        }

        /// <summary>
        /// Остановить систему сбора
        /// </summary>
        public void Stop()
        {
            log.Info("Остановка системы сбора данных...");
            //Остановим систему слежения
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
                log.Warn("Ошибка при останове системы сбора! : " + e.ToString());
            }
            //остановим сервисы
            foreach (IService srv in _services)
            {
                srv.Stop();
            }
            log.Info("Система сбора данных остановлена!");
        }

        /// <summary>
        /// Сохранить тег в источнике данных
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
        /// Сервисный поток проверки работы адаптeров
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
                                //Ошибка работы сервиса
                                srv.ServiceStatus = "ERROR";

                                if (srv.FailCount < srv.Retry)
                                {
                                    //Стандартный перезапуск
                                    if (srv.LastRun < DateTime.Now.AddMilliseconds(-srv.BeforeRetry))
                                    {
                                        //перезапуск потока
                                        try
                                        {
                                            srv.Restart();
                                            log.Warn("[" + srv.Name + "] перезапущен.");
                                        }
                                        catch (Exception e)
                                        {
                                            log.Warn("[ServiceManager] Перезапуск сервиса :" + e.ToString());
                                        }
                                    }
                                }
                                else //обработка мёртвых потоков
                                    if (srv.LastRun < DateTime.Now.AddMilliseconds(-srv.RetryPause))
                                    {
                                        srv.Restart();
                                        srv.FailCount = 0;
                                        log.Warn("[" + srv.Name + "] реанимация сервиса...");
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
            //Инициализировать адаптеры
            XmlNodeList nodes = xmlconf.SelectNodes("services/service");
            foreach (XmlNode node in nodes)
            {
                IService srv = AddService(node);
            }

        }
    }
}
