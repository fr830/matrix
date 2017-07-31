using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Runtime.CompilerServices;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// �������� ������
    /// </summary>
    public class TagStoreManager
    {
        private static ILog log = LogManager.GetLogger<TagStoreManager>();

        private Server _server;
        /// <summary>
        /// ����� ��������
        /// </summary>
        private ITagStore _tagstore;

        public TagStoreManager(Server server)
        {
            _server = server;
        }

        /// <summary>
        /// ��������� ������� ���� � �����
        /// </summary>
        internal void WriteToStore(LogFile lf)
        {
            Thread thrd = new Thread(new ParameterizedThreadStart(this.WriteToStoreAsync));
            thrd.Start(lf);
        }

        internal void WriteToStoreAsync(object o)
        {
            LogFile lf = (LogFile)o;
            ITagStore tagstore = _tagstore.Clone();
            try
            {
                //����������� ���� �� ������ �� ����� ��������
                lf.Islock = true;
                //������� ����                
                tagstore.Open();
                lf.Open(FileAccess.Read);
                Tag tag = null;
                while ((tag = lf.Read())!=null)
                {
                    if (tag.IsChange)
                    {
                        try
                        {
                            tagstore.Write(tag);
                        }
                        catch (Exception e)
                        {
                            //�������� ������ � �����
                            log.Warn("[" + tagstore.Name + "] " + e.Message);
                            break;
                        }
                    }
                }
            }
            finally
            {
                lf.Close();
                tagstore.Close();
                //��������� ��� ����
                lf.Islock = false;
            }
        }

        /// <summary>
        /// �������� ������� ���������
        /// </summary>
        internal ITagStore AddTagStore(XmlNode xmlnode)
        {
            //������� ��������� �� ����
            switch (xmlnode.Attributes["mode"].Value)
            {
                case "file":
                    _tagstore = new TagStoreFile();
                    //_tagstore.Name = xmlnode.Attributes["name"].Value;
                    _tagstore.Init(xmlnode.SelectSingleNode("init"));
                    break;
                case "oracle":
                    _tagstore = new TagStoreOracle();
                    //_tagstore.Name = xmlnode.Attributes["name"].Value;
                    _tagstore.Init(xmlnode.SelectSingleNode("init"));
                    break;
                case "mssql":
                    _tagstore = new TagStoreMSSQL();
                    //_tagstore.Name = xmlnode.Attributes["name"].Value;
                    _tagstore.Init(xmlnode.SelectSingleNode("init"));
                    break;
            }
            return _tagstore;
        }

        internal void Init(XmlNode xmlconf)
        {
            XmlNodeList nodes = xmlconf.SelectNodes("tagstore");
            foreach (XmlNode node in nodes)
            {
                ITagStore ts = AddTagStore(node);
            }
        }

        //internal void Start()
        //{
        //    _tagstore.Open();
        //}

        //internal void Stop()
        //{
        //    _tagstore.Close();
        //}
    }
}
