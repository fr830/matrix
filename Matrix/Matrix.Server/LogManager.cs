using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using Matrix.Types;
using System.Messaging;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// Менеджер лог файлов
    /// </summary>
    public class LogFileManager
    {
        private static ILog log = LogManager.GetLogger<LogManager>();
        private Server _server;
        private LogFile _activelog;
        private LogMSMQ _logMsmq = null;

        private LogFile[] _logfiles;
        private string _logmode;
        private object lockThis = new object();

        internal LogFileManager(Server server)
        {
            _server = server;
        }

        internal LogFile[] Logfiles
        {
            get { return _logfiles; }
            set { _logfiles = value; }
        }

        internal LogFile Activelog
        {
            get { return _activelog; }
            set { _activelog = value; }
        }
        /// <summary>
        /// Переключить лог файл по циклу
        /// </summary>
        internal void SwithLogFile()
        {
            //todo сделать переключение по циклу с резервированием переписываемого файла
            log.Info("Переключение файла журнала.");
            _activelog.Close();
            _server.LogToStore(_activelog);
            int id = _activelog.Logid+1;
            if(id == _logfiles.Length) id = 0;
            _activelog = _logfiles[id];
            while (_activelog.Islock)
            {
                log.Warn("Ожидание файла журнала.");
                Thread.Sleep(100);
            }
            _activelog.Open(FileAccess.Write);
        }

        /// <summary>
        /// Сохранить тэг в логе
        /// </summary>
        internal void WriteToLog(Tag tag)
        {
            if (!"nolog".Equals(_logmode))
            {
                //Записать в лог
                if (_logmode.Contains("log"))
                {
                    //Проверим размер текущего файла
                    lock (lockThis)
                    {
                        CheckLogSize();
                        _activelog.Write(tag);
                    }
                }

                //Записать в MSMQ
                if (_logmode.Contains("msmq"))
                {
                    _logMsmq.Write(tag);
                }
            }
        }

        /// <summary>
        /// Добавить лог файл
        /// </summary>
        internal LogFile AddLogFile()
        {
            LogFile lf = new LogFile();
            if (_logfiles == null)
                _logfiles = new LogFile[1];
            else
            {
                int cnt = _logfiles.Length;
                LogFile[] old = _logfiles;
                _logfiles = new LogFile[cnt+1];
                old.CopyTo(_logfiles, 0);
            }
            lf.Logid = _logfiles.Length - 1;
            _logfiles[_logfiles.GetUpperBound(0)] = lf;
            return lf;
        }

        /// <summary>
        /// Запустить лог манеджер
        /// </summary>
        internal void Start()
        {
            if (!"nolog".Equals(_logmode))
            {
                if (_logmode.Contains("log"))
                {
                    log.Info("Запуск системы журналирования...");
                    //Установить текущий лог и открыть
                    DateTime dt = DateTime.MinValue;
                    int logid = 0;
                    for (int i = 0; i < _logfiles.Length; i++)
                    {
                        DateTime dt2 = _logfiles[i].GetLastWriteTime();
                        if (dt == null || dt < dt2)
                        {
                            logid = i;
                            dt = dt2;//Максимальное время время
                        }
                    }
                    //logid++;
                    //if (logid == _logfiles.Length) logid = 0;                
                    _activelog = _logfiles[logid];
                    _activelog.Open(FileAccess.Write);
                    //Переключим лог на новый
                    SwithLogFile();
                    log.Info("Система журналирования запущена!");
                }

                if (_logmode.Contains("msmq"))
                {
                    log.Info("Запуск очереди журналирования...");
                    //Открыит очередь
                    _logMsmq.Open();
                    log.Info("Очередь журналирования запущена!");
                }
            }
        }

        /// <summary>
        /// Остановить лог менежер
        /// </summary>
        internal void Stop()
        {
            if (!"nolog".Equals(_logmode))
            {
                if (_logmode.Contains("log"))
                {
                    log.Info("Останов системы журналирования...");
                    //Заархивировать последний лог
                    SwithLogFile();
                    _activelog.Close();
                }

                if (_logmode.Contains("msmq"))
                {
                    log.Info("Закрытие очереди журналирования...");
                    _logMsmq.Close();
                }

                log.Info("Система журналирования остановлена!");
            }
        }

        /// <summary>
        /// проверка размера лога и переключение его при необходимости
        /// </summary>
        internal void CheckLogSize()
        {
            if (_activelog.Filesize < _activelog.GetWriteSize()) 
                SwithLogFile();
        }

        internal void Init(XmlNode xmlconf)
        {
            //Создать логи
            _logmode = xmlconf.Attributes["mode"].Value;
            if (!"nolog".Equals(_logmode))
            {

                if (_logmode.Contains("log"))
                {
                    XmlNodeList nodes = xmlconf.SelectNodes("logfiles/logfile");
                    foreach (XmlNode node in nodes)
                    {
                        LogFile logfile = AddLogFile();
                        logfile.Logname = node.Attributes["name"].Value;
                        logfile.Filename = node.Attributes["path"].Value;
                        logfile.Filesize = Convert.ToInt32(node.Attributes["size"].Value);
                    }
                }

                if (_logmode.Contains("msmq"))
                {
                    XmlNode node = xmlconf.SelectSingleNode("queue");
                    _logMsmq = new LogMSMQ();
                    _logMsmq.QueueName = node.Attributes["name"].Value;
                    _logMsmq.Delimiter = node.Attributes["delimiter"].Value;
                }
            }
        }
    }
}
