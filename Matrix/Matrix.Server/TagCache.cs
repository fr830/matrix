using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Xml;
using System.Runtime.CompilerServices;
using System.IO;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// ���
    /// </summary>
    public class TagCache
    {
        private static ILog log = LogManager.GetLogger<TagCache>();

        private Hashtable _tags = new Hashtable();
        /// <summary>
        /// ���������� ���
        /// </summary>
        private Hashtable _tags2 = new Hashtable();
        private int _interval;
        private DateTime _dtrefresh;
        private Thread _thrd;
        private string[] _files;
        private DateTime _lastupdate;
        private Server _server = null;

        internal Hashtable Tags
        {
            get { 
                return Hashtable.Synchronized(_tags); 
            }
        }

        /// <summary>
        /// ���������� ���
        /// </summary>
        internal Hashtable Tags2
        {
            get
            {
                return Hashtable.Synchronized(_tags2);
            }
        }

        /// <summary>
        /// ����� ���������� ���������� ����������� ����
        /// </summary>        
        internal DateTime DtRefresh
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            get
            {
                return _dtrefresh;
            }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            set
            {
                _dtrefresh = value;
            }
        }

        internal DateTime LastUpdate
        {
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            get
            {
                return _lastupdate;
            }
            [MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
            set
            {
                _lastupdate = value;
            }
        }

        public TagCache(Server srv)
        {
            _server = srv;
        }
        /// <summary>
        /// ����� ������������� ����������� ����
        /// </summary>
        internal void Run()
        {
            while (true)
            {
                SynchonizeCache();
                Thread.Sleep(_interval);
          }
        }

        /// <summary>
        /// ������ ���� � ������
        /// </summary>
        internal void Start()
        {
            _thrd = new Thread(new ThreadStart(Run));
            _thrd.Start();
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        internal void Stop()
        {
            _thrd.Abort();
            _thrd.Join(1000);
        }

        /// <summary>
        /// ������������� ����
        /// </summary>
        internal void Init(XmlNode xmlconf)
        {
            //������� ��������� ���� 
            XmlNodeList nodes = xmlconf.SelectNodes("//tagmanager/tags/tag");
            InitTagCache(nodes);
            XmlNode node = xmlconf.SelectSingleNode("//tagcache");
            _interval = Convert.ToInt32(node.Attributes["interval"].Value);
            bool _cacheloaded = false;
            //���� ��������� ����
            if (node.Attributes["type"].Value.Equals("disk"))
            {
                _files = node.Attributes["path"].Value.Split(';');
                //�������������� ��������� ������ �� ����
                foreach (string _file in _files)
                {
                    if (File.Exists(_file))
                    {
                        StreamReader sr = File.OpenText(_file);
                        string xml = sr.ReadToEnd();
                        sr.Close();
                        XmlDocument xmldoc = new XmlDocument();
                        try
                        {
                            xmldoc.LoadXml(xml);
                            XmlNodeList nds = xmldoc.SelectSingleNode("//tags").ChildNodes;
                            foreach (XmlNode nd in nds)
                            {
                                try
                                {
                                    Tag tag = ((Tag)_tags[nd.Name]).getOrUpdate(null);//�������� �� ����� ������������
                                    //if (tag.Type == TagType.XML)
                                    //    tag.Value = nd.InnerXml;
                                    //else 
                                    //    tag.Value = nd.InnerText;
                                    //tag.DtChange = Convert.ToDateTime(nd.Attributes["dtchange"].Value);
                                    Tag tagcashe = ((Tag)_tags[nd.Name]).getOrUpdate(null);
                                    tagcashe.setXml(nd);//�������� �� ����
                                    
                                    //todo ������� ����� ���������� ����������
                                    //�������� ���������� �� �������� �� ���������
                                    if (!tag.Default.Equals(tagcashe.Default))
                                    {
                                        tagcashe.Default = tag.Default;
                                        tagcashe.Value = tag.Default;
                                    }
                                    ((Tag)_tags[nd.Name]).getOrUpdate(tagcashe);
                                }
                                catch (Exception e) { }
                            }
                            //���� ���������� ������ ��������� ������ �� ������������
                            _cacheloaded = true;
                            log.Info("�������� ���� ����: " + _file);
                            break;
                        }
                        catch(Exception e)
                        {
                            log.Warn("������ ������� ����� ����: " + _file + ",\r\n " + e.Message);
                        }
                    }
                }
                if(!_cacheloaded)
                    log.Warn("������� ��������� ��� ������ ����� ����");
            }

            //������������� ���
            foreach (DictionaryEntry item in Hashtable.Synchronized(_tags))
            {
                Tag tag = ((Tag)item.Value).getOrUpdate(null);
                Hashtable.Synchronized(_tags2).Add(tag.Name, tag);
                tag = null;
            }

            //�������� ���� � ���������
            nodes = xmlconf.SelectNodes("//servicemanager/services/service/init/map");
            foreach (XmlNode nd in nodes)
            {
                Tag tag = ((Tag)_tags[nd.Attributes["name"].Value]).getOrUpdate(null);
                tag.Service = nd.ParentNode.ParentNode.Attributes["name"].Value;
                ((Tag)_tags[nd.Attributes["name"].Value]).getOrUpdate(tag);
            }

        }

        /// <summary>
        /// ���������������� TagCache ������� ��������� ������������
        /// </summary>
        /// remarks 
        private void InitTagCache(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                Tag tag = new Tag();
                tag.Name = node.Attributes["name"].Value;
                if (!_tags.ContainsKey(tag.Name))//������� � ��� ���� ��� ��� ���
                {
                    //������� �������� ����
                    string _description = "";
                    try
                    {
                        _description = node.Attributes["description"].Value;
                        tag.Description = _description;
                    }
                    catch { }
                    //������� �������� �� ���������
                    string _default = "";
                    try
                    {
                        _default = node.InnerText;
                        tag.Default = _default;
                    }
                    catch { }
                    //������� �������� ���������� ��� ��������� ��������
                    float _delta = 0.0f;
                    try
                    {
                        _delta = Convert.ToSingle(node.Attributes["delta"].Value);
                        tag.Delta = _delta;
                    }
                    catch { }

                    //������� ��� ����������� 
                    int _step=0;
                    try
                    {
                        _step = Convert.ToInt32(node.Attributes["step"].Value);
                        tag.Step = _step;
                    }
                    catch { }

                    //������� ����� ����������� ���� 0-�� ����������, 1-����������
                    int _logMode = 0;
                    try
                    {
                        _logMode = Convert.ToInt32(node.Attributes["logmode"].Value);
                        tag.LogMode = _logMode;
                    }
                    catch { }

                    switch (node.Attributes["type"].Value)
                    {
                        case "bool":
                            tag.Type = TagType.Boolean;
                            try
                            {
                                tag.Value = (_default.Length > 0) ? (Convert.ToBoolean(_default)) : false;
                            }
                            catch {
                                tag.Value = false;
                            }
                            break;
                        case "byte":
                            tag.Type = TagType.Byte;
                            try
                            {
                                tag.Value  = (_default.Length > 0) ? (Convert.ToByte(_default)) : 0;
                            }
                            catch
                            {
                                tag.Value = 0;
                            }
                            tag.Delta = _delta;
                            break;
                        case "int":
                            tag.Type     = TagType.Integer;
                            try
                            {
                                tag.Value = (_default.Length > 0) ? (Convert.ToInt32(_default)) : 0;
                            }
                            catch
                            {
                                tag.Value = 0;
                            }
                            tag.Delta = _delta;
                            break;
                        case "float":
                            tag.Type     = TagType.Float;
                            try
                            {
                                tag.Value = (_default.Length > 0) ? (Convert.ToSingle(_default)) : 0.0f;
                            }
                            catch
                            {
                                tag.Value = 0.0f;
                            }
                            tag.Delta    = _delta;
                            break;
                        case "string":
                            tag.Type     = TagType.String;
                            try
                            {
                                tag.Value = (_default.Length > 0) ? (_default) : "";
                            }
                            catch
                            {
                                tag.Value = "";
                            }
                            break;
                        case "datetime":
                            tag.Type     = TagType.DateTime;
                            try
                            {
                                //todo ������� �������� �� ��������� NULL
                                //tag.Value = (_default.Length > 0) ? (Convert.ToDateTime(_default)) : null;
                                if (_default.Length > 0)
                                    tag.Value = Convert.ToDateTime(_default);
                            }
                            catch
                            {
                            }
                            break;
                        case "xml":
                            tag.Type = TagType.XML;
                            try
                            {
                                tag.Value = (_default.Length > 0) ? (_default) : "";
                            }
                            catch
                            {
                                tag.Value = "";
                            }
                            break;

                    }
                    _tags.Add(tag.Name, tag);
                }
            }
        }

        private void SynchonizeCache()
        {
            try
            {
                //if (LastUpdate > DtRefresh)
                //{
                        DtRefresh = DateTime.Now;//����� ���������� ���������� ����������� ����
                        foreach (string _file in _files)
                        {
                            //StreamWriter wr = null;
                            FileStream wr = null;
                            String buff = null;
                            try
                            {
                                    if (_file != null)
                                    {
                                        wr = new FileStream(_file, FileMode.Create, FileAccess.Write, FileShare.Read, 8192, FileOptions.WriteThrough);
                                        buff = "<cache dtchange=\"" + DtRefresh.ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) + "\">\r\n";
                                        wr.Write(UnicodeEncoding.UTF8.GetBytes(buff),0,UnicodeEncoding.UTF8.GetByteCount(buff));
                                        //wr = File.CreateText(_file);
                                        //wr.WriteLine("<tags dtchange=\"" + DtRefresh + "\">");

                                    }
                                    
                                    //�������� ��������� ���� ������� services
                                    buff = "<services>\r\n";
                                    foreach (Service svr in Server.ServiceManager.Services)
                                    {
                                        buff += "<service name=\"" + svr.Name + "\" status=\"" + svr.ServiceStatus + "\"  lastrun=\"" + svr.LastRun + "\" description=\"" +svr.Description+ "\">";
                                        if (!"".Equals(svr.ErrorMessage))
                                            buff += "<![CDATA["+svr.ErrorMessage+"]]>";
                                        buff += "</service>\r\n";
                                    }
                                    buff += "</services>\r\n";
                                    wr.Write(UnicodeEncoding.UTF8.GetBytes(buff), 0, UnicodeEncoding.UTF8.GetByteCount(buff));

                                    buff = "<tags>\r\n";
                                    wr.Write(UnicodeEncoding.UTF8.GetBytes(buff), 0, UnicodeEncoding.UTF8.GetByteCount(buff));

                                    //_tags2 = (Hashtable)Hashtable.Synchronized(_tags).Clone();
                                    foreach (DictionaryEntry item in Hashtable.Synchronized(_tags))
                                    {
                                        Tag tag = ((Tag)item.Value).getOrUpdate(null);
                                        //�������������� ����� �������� ���� c ������� � ����� �� �������
                                        if (tag.Delta > 0 && tag.Step > 0 && (DtRefresh - tag.DtChange).TotalSeconds > tag.Step)
                                            _server.SetTag(tag);
                                        //if (DtRefresh > tag.DtChange.AddMilliseconds(_interval))
                                        //{
                                        //    //������� � ���
                                        //    if (tag.cnt > 0)
                                        //    {
                                        //        //tag.Step = (int)((tag.Timestamp - tag.DtChange).TotalMilliseconds / (tag.cnt + 1));
                                        //        tag.Step = (int)((DtRefresh - tag.DtChange).TotalMilliseconds / (tag.cnt + 1));
                                        //        tag.DtChange = tag.Timestamp;
                                        //        tag.IsChange = true;
                                        //        _server.SetTag(tag);
                                        //        tag.cnt = 0;
                                        //        tag.Step = 0;
                                        //    }
                                        //    //������� ������� ����
                                        //    //tag.cnt++;
                                        //    //tag.Step = 0;
                                        //    tag.IsChange = true;
                                        //    tag.DtChange = DtRefresh;
                                        //    tag.Timestamp = DtRefresh;
                                        //    _server.SetTag(tag);
                                        //}
                                        ((Tag)Hashtable.Synchronized(_tags2)[tag.Name]).getOrUpdate(tag);
                                        if (wr != null)
                                        {
                                            buff = tag.getXml();
                                            wr.Write(UnicodeEncoding.UTF8.GetBytes(buff), 0, UnicodeEncoding.UTF8.GetByteCount(buff));
                                            //wr.WriteLine(tag.getXml());
                                        }
                                        tag = null;
                                    }
                            }
                            finally
                            {
                                if (wr != null)
                                {
                                    buff = "</tags>\r\n</cache>\r\n";
                                    wr.Write(UnicodeEncoding.UTF8.GetBytes(buff), 0, UnicodeEncoding.UTF8.GetByteCount(buff));                                    
                                    //wr.WriteLine("</tags>");
                                    wr.Flush(true);
                                    wr.Close();
                                }
                            }
                        }
                //}
            }
            catch (Exception e)
            {
                log.Warn(e.Message + e.StackTrace);
            }
            GC.Collect();
        }
    }
}
