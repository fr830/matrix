using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Common.Logging;

namespace Matrix.Types
{
    /// <summary>
    /// Тестовый адаптер
    /// </summary>
    public class Service : IService
    {
        private static ILog log = LogManager.GetLogger<Service>();

        /// <summary>
        /// интервал опроса, 60 сек по умолчанию
        /// </summary>
        protected int _interval = 60000;
        /// <summary>
        /// интервал проверки состояния потока, 3 минуты по умолчанию
        /// </summary>
        protected int _checkinterval = 180000;

        protected DateTime _lastrun;
        /// <summary>
        /// Условие запуска адаптера
        /// </summary>
        protected string _startif;

        /// <summary>
        /// Где сохранить результат и в каком виде
        /// </summary>
        protected string _status;
        /// <summary>
        /// название адаптера
        /// </summary>
        protected string _name;
        /// <summary>
        /// Ссылка на сервер
        /// </summary>
        protected string _description;
        protected IServer _server;
        protected int _id;

        protected int _fails = 0;
        protected int _maxcycle = 0;

        protected bool _keepalive = false;
        protected int _retry = 3;           //число попыток запуска
        protected int _beforeretry = 60000; //через минуту повторить попытку запуска
        protected int _retrypause = 600000;  //через 10 минут реанимировать поток //3600000;
        protected bool _isbreak = false;
        protected string _type;
        protected string _assembly;
        protected string _serviceStatus;   //Статус сервиса STOP, RUN, ERROR
        protected string _errorMessage;   //расшифровка ошибки

        public string ServiceStatus
        {
            get { return _serviceStatus; }
            set { _serviceStatus = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Retry
        {
            get { return _retry; }
            set { _retry = value; }
        }

        public int BeforeRetry
        {
            get { return _beforeretry; }
            set { _beforeretry = value; }
        }

        public Service()
        {
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IServer Server
        {
            set { _server = value; }
        }

        public bool IsBreak
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            get { return _isbreak; }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            set { _isbreak = value; }
        }

        public DateTime LastRun
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            get { return _lastrun; }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            set { _lastrun = value; }
        }

        public int FailCount
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            get { return _fails; }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
            set { _fails = value; }
        }

        public bool KeepAlive
        {
            get { return _keepalive; }
            set { _keepalive = value; }
        }

        public int RetryPause
        {
            get { return _retrypause; }
            set { _retrypause = value; }
        }

        /// <summary>
        /// основной метод потока
        /// </summary>
        public void Run()
        {
            try
            {
                //Установить статус работы
                _serviceStatus = "RUN";
                _errorMessage = "";

                AfterStart();
                //Постоянная работа
                int cyclecnt = 0;
                bool start = false;
                LastRun = DateTime.Now;

                while (true)
                {
                    DateTime now = DateTime.Now;
                    //Пауза для уменьшения нагрузки на процессор
                    if (_interval > 0)
                    {
                        int delta = (int)(now - LastRun).TotalMilliseconds;
                        int d = _interval - delta - 50;
                        if (d > 50)
                        {
                            start = false;
                            Thread.Sleep(50);
                        }
                        else if (d > 0)
                        {
                            //Сохраним дату и время начала прогона
                            LastRun = LastRun.AddMilliseconds(_interval);
                            //_server.LogWrite(LogType.DEBUG, "d > 0 && d < 50");
                            start = true;
                        }
                        else if (d < 0)
                        {
                            //Сохраним дату и время начала прогона с коррекцией на выход из интервала
                            LastRun = LastRun.AddMilliseconds(_interval - d);
                            //_server.LogWrite(LogType.DEBUG, "d < 0");
                            //LastRun = now;
                            start = true;
                        }
                    }
                    else
                    {
                        //Сохраним дату и время начала прогона
                        LastRun = now;
                        start = true;
                    }

                    if (start)
                    {
                        if (TestExpr(_startif))//есть условие запуска
                        {
                            //Работа на один прогон
                            RunOnce();
                            //Фиксация результата
                            RunStatus();
                        }
                        //Сбросим счётчик ошибок
                        FailCount = 0;
                        //Плановый останов
                        cyclecnt++;
                        if (_maxcycle > 0 && cyclecnt > _maxcycle) break;
                    }
                    if (IsBreak) break;//остановить поток
                }
            }
            catch (Exception e)
            {
                //Ошибка работы сервиса
                _serviceStatus = "ERROR";
                _errorMessage = e.Message;
                log.Warn("[" + this._name + "] Аварийное завершение потока : " + e.ToString());
            }
            finally
            {
                BeforeStop();
            }
        }

        /// <summary>
        /// Фиксация результата одного прогона
        /// </summary>
        private void RunStatus()
        {
            if (_status != null && _status.Length > 0)
            {
                Tag tag;
                //получим массив выражений
                string[] artagexpr = _status.Split(';');
                foreach (string tagexpr in artagexpr)
                {
                    string[] vars = tagexpr.Split('=');
                    string tagname = vars[0].Substring(1, vars[0].Length - 2);
                    tag = _server.GetTag(tagname.Trim());
                    switch (tag.Type)
                    {
                        case TagType.Boolean:
                            tag.Value = Convert.ToBoolean(vars[1]);
                            break;
                        case TagType.Byte:
                            tag.Value = Convert.ToByte(vars[1]);
                            break;
                        case TagType.Integer:
                            tag.Value = Convert.ToInt32(vars[1]);
                            break;
                        case TagType.Float:
                            tag.Value = Convert.ToSingle(vars[1]);
                            break;
                        case TagType.String:
                            tag.Value = vars[1];
                            break;
                        case TagType.DateTime:
                            tag.Value = Convert.ToDateTime(vars[1]);
                            break;
                        case TagType.XML:
                            tag.Value = vars[1];
                            break;
                    }
                    _server.SetTag(tag);
                }
            }
        }

        /// <summary>
        /// Выполнение одного прогона
        /// </summary>
        public virtual void RunOnce()
        {
        }

        /// <summary>
        /// Операции для выполнения подключения и т.п.
        /// </summary>
        public virtual void AfterStart()
        {
        }

        /// <summary>
        /// Операции для освобождения ресурсов
        /// </summary>
        public virtual void BeforeStop()
        {
        }

        /// <summary>
        /// Инициализация адаптера
        /// </summary>
        /// <param name="xmlconf">Секция конфигурации</param>
        public virtual void Init(XmlNode xmlconf)
        {
            //Инициализация сервиса
            _serviceStatus = "INIT";
            try
            {
                _type = xmlconf.Attributes["type"].Value.ToString();
            }
            catch { }
            try
            {
                _assembly = xmlconf.Attributes["assembly"].Value.ToString();
            }
            catch { }
            try
            {
                _interval = Convert.ToInt32(xmlconf.Attributes["interval"].Value);
            }
            catch { }
            try
            {
                _checkinterval = Convert.ToInt32(xmlconf.Attributes["checkinterval"].Value);
            }
            catch { }
            try
            {
                _startif = xmlconf.Attributes["startif"].Value;
            }
            catch { }
            try
            {
                _status = xmlconf.Attributes["status"].Value;
            }
            catch { }
            try
            {
                _maxcycle = Convert.ToInt32(xmlconf.Attributes["maxcycle"].Value);
            }
            catch { }
            try
            {
                _keepalive = Convert.ToBoolean(xmlconf.Attributes["keepalive"].Value);
            }
            catch { }
            try
            {
                _retry = Convert.ToInt32(xmlconf.Attributes["retry"].Value);
            }
            catch { }
            try
            {
                _beforeretry = Convert.ToInt32(xmlconf.Attributes["beforeretry"].Value);
            }
            catch { }
            try
            {
                _retrypause = Convert.ToInt32(xmlconf.Attributes["retrypause"].Value);
            }
            catch { }
        }

        /// <summary>
        /// Запуск потока
        /// </summary>
        public void Start()
        {
            try
            {
                //Инициализация сервиса
                _serviceStatus = "STARTING";

                IsBreak = false;
                _thrd = new Thread(new ThreadStart(this.Run));
                _thrd.Start();
                _serviceStatus = "STARTED";
            }
            catch (Exception e)
            {
                log.Warn("[" + this._name + "] Start: " + e.ToString());
            }
        }

        /// <summary>
        /// Ссылка на поток
        /// </summary>
        private Thread _thrd;

        /// <summary>
        /// Остановить поток
        /// </summary>
        public void Stop()
        {
            //Инициализация сервиса
            if (_thrd.IsAlive)
            {
                _serviceStatus = "STOPING";
                try
                {
                    IsBreak = true;
                    Thread.Sleep(200);
                    if (_thrd.IsAlive)
                    {
                        if (!_thrd.Join(500))
                            _thrd.Abort();
                        _thrd = null;
                    }
                    GC.Collect();
                }
                catch (Exception e)
                {
                    log.Warn("[" + this._name + "] Stop: " + e.ToString());
                }
                _serviceStatus = "STOPED";
            }
        }

        public void Restart()
        {
            //Увеличим счётчик ошибок связанный с перезапуском
            FailCount++;
            Stop();
            Start();
        }

        /// <summary>
        /// Сохранить тег в истонике данных
        /// </summary>
        public virtual void Write(Tag tag)
        {
        }

        public virtual bool IsRun()
        {
            bool retval = false;
            try
            {
                if (_thrd.IsAlive && LastRun > DateTime.Now.AddMilliseconds(-_checkinterval))
                    retval = true;
                else
                    retval = false;
                return retval;
            }
            catch (Exception e)
            {
                log.Warn("[" + this._name + "] IsRun: " + e.ToString());
                return retval;
            }
        }

        private bool _TestExpr(string strexpr)
        {
            Regex r = new Regex("(and|or)");
            Regex r1 = new Regex("(!=|>=|<=|=|>|<)");
            strexpr = strexpr.ToLower();
            strexpr = strexpr.Replace("true", "1");
            strexpr = strexpr.Replace("false", "0");
            string[] sexpr = r.Split(strexpr);
            bool result = true;
            foreach (string expr in sexpr)
            {
                if (expr == "or")
                {
                    if (result) break;
                    result = true;
                }
                else if (expr != "and")
                {
                    string[] mbr = r1.Split(expr);
                    bool result1 = false;
                    switch (mbr[1].Trim())
                    {
                        case "!=":
                            result1 = Convert.ToDouble(mbr[0]) != Convert.ToDouble(mbr[2]);
                            break;
                        case ">=":
                            result1 = Convert.ToDouble(mbr[0]) >= Convert.ToDouble(mbr[2]);
                            break;
                        case "<=":
                            result1 = Convert.ToDouble(mbr[0]) <= Convert.ToDouble(mbr[2]);
                            break;
                        case "=":
                            result1 = Convert.ToDouble(mbr[0]) == Convert.ToDouble(mbr[2]);
                            break;
                        case ">":
                            result1 = Convert.ToDouble(mbr[0]) > Convert.ToDouble(mbr[2]);
                            break;
                        case "<":
                            result1 = Convert.ToDouble(mbr[0]) < Convert.ToDouble(mbr[2]);
                            break;
                    }
                    result &= result1;
                }
            }
            return result;
        }

        private bool TestExpr(string startif)
        {
            //Сделать контроль по произвольным типам!!!!!
            if (startif != null && startif.Length > 0)
            {
                //Выполним расчёт внутренней функции ischange()
                startif = ParseIsChange(startif);
                //Выполним расчёт внутренней функции time()
                startif = ParseIsTime(startif);
                //Выполним расчёт внутренней функции equal()
                startif = ParseIsEmpty(startif);
                //Выполним расчёт внутренней функции consist()
                //startif = ParseIsChange(startif);
                string value;
                Regex r = new Regex("\\[\\S+\\]");

                MatchCollection vars = r.Matches(startif);
                foreach (Match var in vars)
                {
                    //Удалим первую и последнюю скобку
                    string tagname = var.Value.Substring(1, var.Value.Length - 2);
                    Tag tag = _server.GetTag(tagname.Trim());
                    //todo сделал конвертацию даты в милисекунды
                    value = "";
                    if (tag.Type == TagType.DateTime)
                        value = ((DateTime)((tag.Value != null && !"".Equals(tag.Value)) ? tag.Value : DateTime.MinValue) - DateTime.MinValue).TotalMilliseconds.ToString();
                    else
                        value = tag.Value.ToString();
                    startif = startif.Replace(var.Value, value);
                }
                return _TestExpr(startif);
            }
            else
                return true;
        }

        private string ParseIsChange(string startif)
        {
            Regex r = new Regex("(ischange\\(\\S+\\))");

            MatchCollection vars = r.Matches(startif);
            foreach (Match var in vars)
            {
                //Удалим первую и последнюю скобку
                string tagname = var.Value.Substring(10, var.Value.Length - 12);
                Tag tag = _server.GetTag(tagname.Trim());
                if (tag.DtChange > this._lastrun)
                    startif = startif.Replace(var.Value, "true");
                else
                    startif = startif.Replace(var.Value, "false");
            }

            return startif;
        }

        private string ParseIsTime(string startif)
        {
            Regex r = new Regex("(time\\(\\S+\\))");

            MatchCollection vars = r.Matches(startif);
            foreach (Match var in vars)
            {
                //Удалим первую и последнюю скобку
                //time(20:00)
                string time = var.Value.Substring(5, 5);
                int h = Convert.ToInt32(time.Substring(0, 2));
                int m = Convert.ToInt32(time.Substring(3, 2));
                //Создадим дату запуска
                DateTime dtrun = DateTime.Today.AddHours(h).AddMinutes(m);
                //double millisec = (DateTime.Now - dtrun).TotalMilliseconds;
                if (this._lastrun < dtrun && dtrun <= DateTime.Now)
                    //if (millisec > 0 && millisec <= this._interval)
                    startif = startif.Replace(var.Value, "true");
                else
                    startif = startif.Replace(var.Value, "false");
            }

            return startif;
        }

        private string ParseIsEmpty(string startif)
        {
            Regex r = new Regex("(empty\\(\\S+\\))");

            MatchCollection vars = r.Matches(startif);
            foreach (Match var in vars)
            {
                //Удалим первую и последнюю скобку
                string tagname = var.Value.Substring(7, var.Value.Length - 9);
                Tag tag = _server.GetTag(tagname.Trim());
                if (tag.Value  == null || "".Equals(tag.Value))
                    startif = startif.Replace(var.Value, "true");
                else
                    startif = startif.Replace(var.Value, "false");
            }

            return startif;
        }

        public string ByteArrayToString(byte[] val)
        {
            char[] c = new char[((byte[])val).Length];
            int k = 0;
            foreach (byte b in (byte[])val)
            {
                c[k] = Convert.ToChar(b);
                k++;
            }
            return new String(c);
        }

    }
}
