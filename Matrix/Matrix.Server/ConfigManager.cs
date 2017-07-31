using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// �������� ���������� �������������
    /// </summary>
    public class ConfigManager
    {
        private static ILog log = LogManager.GetLogger<ConfigManager>();

        private XmlDocument _mtrxconf;

        public ConfigManager()
        {
            _mtrxconf = new XmlDocument();
        }
        
        /// <summary>
        /// ��������� ������������
        /// </summary>
        /// <remarks>����������� ����� ������������ � ���������������� TagCache</remarks>
        internal void LoadConf()
        {
            //�������� ���� ������������
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string xml = File.ReadAllText(config.AppSettings.Settings["matrix_config"].Value, Encoding.GetEncoding("windows-1251"));
            _mtrxconf.LoadXml(xml);
            //_mtrxconf.LoadXml(((ConfigurationSection)config.GetSection("matrix")).SectionInformation.GetRawXml());         
            log.Info("������������ ���������!");
        }

        /// <summary>
        /// ���������������� TagCache
        /// </summary>
        internal void InitTagCache(TagCache tagcache)
        {
            log.Info("������������� ������ �����...");
            //������������ ���
            tagcache.Init(_mtrxconf);
            log.Info("����� ����� ���������������!");
        }
        /// <summary>
        /// ���������������� LogManager
        /// </summary>
        internal void InitLogManager(LogFileManager logmngr)
        {
            log.Info("������������� ������� ��������������...");
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//logmanager");
            logmngr.Init(xmlconf);
            log.Info("������� �������������� ����������������!");
            logmngr.Start();
        }

        /// <param name="adptmngr">������������� ��������� ����� ������</param>
        internal void InitServiceManager(ServiceManager srvcmngr)
        {
            log.Info("������������� ������� ����� ������...");
            //�������������� ��������
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//servicemanager");
            srvcmngr.Init(xmlconf);
            log.Info("������� ����� ������ ����������������!");
            srvcmngr.Start();
        }

        /// <param name="tagstoremangr">���������������� ������� �������������</param>
        internal void InitTagStoreManager(TagStoreManager tagstoremngr)
        {
            log.Info("������������� ������� �������������...");
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//tagstoremanager");
            tagstoremngr.Init(xmlconf);
            log.Info("������� ������������� ����������������!");
            //tagstoremngr.Start();
        }
    }
}
