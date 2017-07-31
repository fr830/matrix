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
    /// �������� ��� ������
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
        /// ����������� ��� ���� �� �����
        /// </summary>
        internal void SwithLogFile()
        {
            //todo ������� ������������ �� ����� � ��������������� ��������������� �����
            log.Info("������������ ����� �������.");
            _activelog.Close();
            _server.LogToStore(_activelog);
            int id = _activelog.Logid+1;
            if(id == _logfiles.Length) id = 0;
            _activelog = _logfiles[id];
            while (_activelog.Islock)
            {
                log.Warn("�������� ����� �������.");
                Thread.Sleep(100);
            }
            _activelog.Open(FileAccess.Write);
        }

        /// <summary>
        /// ��������� ��� � ����
        /// </summary>
        internal void WriteToLog(Tag tag)
        {
            if (!"nolog".Equals(_logmode))
            {
                //�������� � ���
                if (_logmode.Contains("log"))
                {
                    //�������� ������ �������� �����
                    lock (lockThis)
                    {
                        CheckLogSize();
                        _activelog.Write(tag);
                    }
                }

                //�������� � MSMQ
                if (_logmode.Contains("msmq"))
                {
                    _logMsmq.Write(tag);
                }
            }
        }

        /// <summary>
        /// �������� ��� ����
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
        /// ��������� ��� ��������
        /// </summary>
        internal void Start()
        {
            if (!"nolog".Equals(_logmode))
            {
                if (_logmode.Contains("log"))
                {
                    log.Info("������ ������� ��������������...");
                    //���������� ������� ��� � �������
                    DateTime dt = DateTime.MinValue;
                    int logid = 0;
                    for (int i = 0; i < _logfiles.Length; i++)
                    {
                        DateTime dt2 = _logfiles[i].GetLastWriteTime();
                        if (dt == null || dt < dt2)
                        {
                            logid = i;
                            dt = dt2;//������������ ����� �����
                        }
                    }
                    //logid++;
                    //if (logid == _logfiles.Length) logid = 0;                
                    _activelog = _logfiles[logid];
                    _activelog.Open(FileAccess.Write);
                    //���������� ��� �� �����
                    SwithLogFile();
                    log.Info("������� �������������� ��������!");
                }

                if (_logmode.Contains("msmq"))
                {
                    log.Info("������ ������� ��������������...");
                    //������� �������
                    _logMsmq.Open();
                    log.Info("������� �������������� ��������!");
                }
            }
        }

        /// <summary>
        /// ���������� ��� �������
        /// </summary>
        internal void Stop()
        {
            if (!"nolog".Equals(_logmode))
            {
                if (_logmode.Contains("log"))
                {
                    log.Info("������� ������� ��������������...");
                    //�������������� ��������� ���
                    SwithLogFile();
                    _activelog.Close();
                }

                if (_logmode.Contains("msmq"))
                {
                    log.Info("�������� ������� ��������������...");
                    _logMsmq.Close();
                }

                log.Info("������� �������������� �����������!");
            }
        }

        /// <summary>
        /// �������� ������� ���� � ������������ ��� ��� �������������
        /// </summary>
        internal void CheckLogSize()
        {
            if (_activelog.Filesize < _activelog.GetWriteSize()) 
                SwithLogFile();
        }

        internal void Init(XmlNode xmlconf)
        {
            //������� ����
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
