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
    /// Менеджер управления конфигурацией
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
        /// загрузить конфигурацию
        /// </summary>
        /// <remarks>Загружается новая конфигурация и инициализируется TagCache</remarks>
        internal void LoadConf()
        {
            //Загрузим файл конфигурации
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string xml = File.ReadAllText(config.AppSettings.Settings["matrix_config"].Value, Encoding.GetEncoding("windows-1251"));
            _mtrxconf.LoadXml(xml);
            //_mtrxconf.LoadXml(((ConfigurationSection)config.GetSection("matrix")).SectionInformation.GetRawXml());         
            log.Info("Конфигурация загружена!");
        }

        /// <summary>
        /// Инициализировать TagCache
        /// </summary>
        internal void InitTagCache(TagCache tagcache)
        {
            log.Info("Инициализация буфера тегов...");
            //Сформировать кеш
            tagcache.Init(_mtrxconf);
            log.Info("Буфер тегов инициализирован!");
        }
        /// <summary>
        /// Инициализировать LogManager
        /// </summary>
        internal void InitLogManager(LogFileManager logmngr)
        {
            log.Info("Инициализания системы журналирования...");
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//logmanager");
            logmngr.Init(xmlconf);
            log.Info("Система журналирования инициализирована!");
            logmngr.Start();
        }

        /// <param name="adptmngr">Инициализация адаптеров сбора данных</param>
        internal void InitServiceManager(ServiceManager srvcmngr)
        {
            log.Info("Инициализация системы сбора данных...");
            //инициализируем менеджер
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//servicemanager");
            srvcmngr.Init(xmlconf);
            log.Info("Система сбора данных инициализирована!");
            srvcmngr.Start();
        }

        /// <param name="tagstoremangr">Инициализировать систему архивирования</param>
        internal void InitTagStoreManager(TagStoreManager tagstoremngr)
        {
            log.Info("Инициализация системы архивирования...");
            XmlNode xmlconf = _mtrxconf.SelectSingleNode("//tagstoremanager");
            tagstoremngr.Init(xmlconf);
            log.Info("Системы архивирования инициализирована!");
            //tagstoremngr.Start();
        }
    }
}
