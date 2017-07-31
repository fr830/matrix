using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.IO;

namespace Matrix.Config
{
    public partial class MainForm : Form
    {
        public string MatrixFile = null;
        private XmlDocument xmlDom = null;
        Hashtable tags = new Hashtable();

        private string PrintXML(string XML)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml("<root>"+XML+"</root>");

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.DocumentElement.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            
            catch (XmlException)
            {
                return XML;
            }

            finally{
                writer.Close();
                mStream.Close();
            }

            return Result;
        }



        public MainForm()
        {
            InitializeComponent();
        }

        private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Matrix config file (matrix.xml)|matrix.xml|Xml files|*.xml";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MatrixFile = dialog.FileName;
                this.statusStrip1.Items.Add(MatrixFile);
                //Загрузка тегов
                btnTagSave.Enabled = false;
                btnTagCancel.Enabled = false;
                btnTagDel.Enabled = false;
                tags.Clear();
                dataGridViewTags.Rows.Clear();
                xmlDom = new XmlDocument();
                xmlDom.Load(MatrixFile);

                LoadConfigFromXml();

            }

        }

        private void LoadConfigFromXml()
        {

            createNewToolStripMenuItem.Enabled = false;
            openConfigToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;

            tabControl1.Enabled = true;

                XmlNodeList _tags = xmlDom.SelectNodes("//tagmanager/tags/tag");
                dataGridViewTags.Rows.Clear();
                foreach (XmlNode node in _tags)
                {

                    int i = dataGridViewTags.Rows.Add();
                    dataGridViewTags.Rows[i].Cells["columnTagName"].Value = GetXmlNodeAttribute(node,"name","noname");
                    dataGridViewTags.Rows[i].Cells["columnTagType"].Value = GetXmlNodeAttribute(node,"type","");
                    dataGridViewTags.Rows[i].Cells["columnTagDesc"].Value = GetXmlNodeAttribute(node,"description","");
                    tags.Add(node.Attributes["name"].Value, node);
                }
                
                //Загрузка общей конфигурации
                btnLogAdd.Enabled = false;
                btnLogDel.Enabled = false;
                btnLogEdit.Enabled = true;
                btnLogSave.Enabled = false;
                btnLogCancel.Enabled = false;

                //Жарналирование данных
                cbLogMode.Enabled = false;
                XmlNode _logmanager = xmlDom.SelectSingleNode("//logmanager");
                cbLogMode.SelectedItem = _logmanager.Attributes["mode"].Value;
                
                dataGridViewLogs.Rows.Clear();
                XmlNodeList _logs = xmlDom.SelectNodes("//logmanager/logfiles/logfile");
                foreach (XmlNode node in _logs)
                {
                    int i = dataGridViewLogs.Rows.Add();
                    dataGridViewLogs.Rows[i].Cells["columnLogName"].Value = node.Attributes["name"].Value;
                    dataGridViewLogs.Rows[i].Cells["columnLogSize"].Value = node.Attributes["size"].Value;
                }

                tbLogQueue.Enabled = false;
                tbLogQueueDelimiter.Enabled = false;
                XmlNode _queue = xmlDom.SelectSingleNode("//logmanager/queue");
                if (_queue != null)
                {
                    tbLogQueue.Text = GetXmlNodeAttribute(_queue,"name","error");
                    tbLogQueueDelimiter.Text = GetXmlNodeAttribute(_queue, "delimiter", ";");
                }

                //Загрузка лог message
                cbMsgLogMode.Enabled = false;
                tbMsgLogName.Enabled = false;
                tbMsgLogMaxSize.Enabled = false;

                XmlNode _message = xmlDom.SelectSingleNode("//logmanager/message");
                cbMsgLogMode.SelectedItem = GetXmlNodeAttribute(_message, "level", "info");
                tbMsgLogName.Text = GetXmlNodeAttribute(_message, "path", "logs/app.log");
                tbMsgLogMaxSize.Text = GetXmlNodeAttribute(_message, "maxsize", "65536");
 

                //Tag Cache
                tbTagCacheFile.Enabled = false;
                tbTagCacheInterval.Enabled = false;
                cbTagCacheType.Enabled = false;
                cbTagCacheType.SelectedItem = "disk";
                XmlNode _tagcache = xmlDom.SelectSingleNode("//tagcache");
                tbTagCacheInterval.Text = GetXmlNodeAttribute(_tagcache,"interval","60000");
                tbTagCacheFile.Text = GetXmlNodeAttribute(_tagcache, "path", "logs/tags.xml;logs/tags.bak.xml");


                //Tag Store
                cbTagStoreMode.Enabled = false;
                tbTagStoreFile.Enabled = false;
                tbTagStoreDelimiter.Enabled = false;
                tbTagStoreRowPerTran.Enabled = false;
                tbTagStoreConStr.Enabled = false;
                tbTagStoreDelimiter.Text = "";
                tbTagStoreFile.Text = "";
                tbTagStoreRowPerTran.Text = "";
                tbTagStoreConStr.Text = "";
                XmlNode _tagstore = xmlDom.SelectSingleNode("//tagstoremanager/tagstore");
                cbTagStoreMode.SelectedItem = _tagstore.Attributes["mode"].Value;
                XmlNode _init = xmlDom.SelectSingleNode("//tagstoremanager/tagstore/init");
                if ((string)cbTagStoreMode.SelectedItem == "file")
                {
                    tbTagStoreFile.Text = GetXmlNodeAttribute(_init,"filename","logs/store.dat");
                    tbTagStoreDelimiter.Text = GetXmlNodeAttribute(_init,"delimiter",";");
                }
                else
                {
                    tbTagStoreRowPerTran.Text = GetXmlNodeAttribute(_init,"rowpertran","200");
                    tbTagStoreConStr.Text = GetXmlNodeAttribute(_init,"connectionString","");
                }            

                //Service Manager
                tbServiceCheckInterval.Enabled = false;
                XmlNode _serviceManager = xmlDom.SelectSingleNode("//servicemanager");
                tbServiceCheckInterval.Text = GetXmlNodeAttribute(_serviceManager,"checkinterval","60000");


                //Загрузка сервисов
                cbSrvType.Enabled = false;
                chkSrvKeepAlive.Enabled = false;
                chkSrvEnabled.Enabled = false;
                tbSrvBeforeRetry.Enabled = false;
                tbSrvBeforeRetry.Enabled = false;
                tbSrvCheckInterval.Enabled = false;
                tbSrvDesc.Enabled = false;
                tbSrvInit.Enabled = false;
                tbSrvInitDesc.Enabled = false;
                tbSrvInterval.Enabled = false;
                tbSrvMaxCycle.Enabled = false;
                tbSrvName.Enabled = false;
                tbSrvParams.Enabled = false;
                tbSrvRetry.Enabled = false;
                tbSrvRetryPause.Enabled = false;
                tbSrvStartIf.Enabled = false;
                tbSrvStatus.Enabled = false;

                btnSrvAdd.Enabled = true;
                btnSrvDel.Enabled = true;
                btnSrvEdit.Enabled = true;
                btnSrvSave.Enabled = false;
                btnSrvCancel.Enabled = false;

                dataGridViewServices.Rows.Clear();
                XmlNodeList _services = xmlDom.SelectNodes("//services/service");
                foreach (XmlNode _node in _services)
                {
                    int i = dataGridViewServices.Rows.Add();
                    dataGridViewServices.Rows[i].Cells["columnServiceName"].Value = GetXmlNodeAttribute(_node,"name","");
                    dataGridViewServices.Rows[i].Cells["columnServiceEnabled"].Value = GetXmlNodeAttribute(_node,"enabled","true");
                    dataGridViewServices.Rows[i].Cells["columnServiceDesc"].Value = GetXmlNodeAttribute(_node,"description","");
                }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        //Загрузка тега
        private void LoadTagItem(string tagName)
        {
            if (tagName != null && !"".Equals(tagName))
            {
                XmlNode node = (XmlNode)tags[tagName];

                tbTagName.Text = GetXmlNodeAttribute(node, "name", "");
                tbTagDesc.Text = GetXmlNodeAttribute(node, "description", "");
                tbTagDefault.Text = node.InnerText;
                cbTagType.SelectedItem = GetXmlNodeAttribute(node, "type", "");
                chkTagLogMode.Checked = GetXmlNodeAttribute(node, "logmode", "") == "1" ? true : false;

                if (chkTagLogMode.Checked)
                {
                    tbTagDelta.Text = GetXmlNodeAttribute(node, "delta", "0");
                    tbTagStep.Text = GetXmlNodeAttribute(node, "step", "300");
                }
                else
                {
                    tbTagDelta.Text = "0";
                    tbTagStep.Text = "300";
                }
            }
            else
            {
                tbTagName.Text = "";
                tbTagDesc.Text = "";
                tbTagDefault.Text = "";
                cbTagType.SelectedItem = "";
                chkTagLogMode.Checked = false;

                tbTagDelta.Text = "0";
                tbTagStep.Text = "300";
            }

            tbTagName.Enabled = false;
            tbTagDefault.Enabled = false;
            tbTagDesc.Enabled = false;
            chkTagLogMode.Enabled = false;
            cbTagType.Enabled = false;
            tbTagDelta.Enabled = false;
            tbTagStep.Enabled = false;

            btnTagAdd.Enabled = true;
            btnTagDel.Enabled = true;
            btnTagEdit.Enabled = true;
            btnTagSave.Enabled = false;
            btnTagCancel.Enabled = false;
        }

        private void SetXmlNodeAttribure(XmlDocument _xmlDom, XmlNode _node, string _attr, string _value){
            try
            {
                _node.Attributes[_attr].Value = _value;
            }
            catch
            {
                XmlAttribute attr = _xmlDom.CreateAttribute(_attr);
                _node.Attributes.Append(attr);
                _node.Attributes[_attr].Value = _value;
            }

        }

        private void RemoveXmlNodeAttribure(XmlDocument _xmlDom, XmlNode _node, string _attr)
        {
            try
            {
                _node.Attributes.Remove(_node.Attributes[_attr]);
            }
            catch
            {
            }

        }

        private string GetXmlNodeAttribute(XmlNode _node, string _attr, string _default)
        {
            try
            {
                return "".Equals(_node.Attributes[_attr].Value) ? _default : _node.Attributes[_attr].Value;
            }
            catch
            {
                return _default;
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            tbTagName.Enabled = true;
            tbTagDefault.Enabled = true;
            tbTagDesc.Enabled = true;
            chkTagLogMode.Enabled = true;
            cbTagType.Enabled = true;

            if (chkTagLogMode.Checked)
            {
                tbTagDelta.Enabled = true;
                tbTagStep.Enabled = true;
            }
            btnTagSave.Enabled = true;
            btnTagCancel.Enabled = true;
            btnTagEdit.Enabled = false;
            btnTagAdd.Enabled = false;
            btnTagDel.Enabled = false;
            dataGridViewTags.Enabled = false;
        }


        private void chkLogMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTagLogMode.Checked)
            {
                tbTagDelta.Enabled = true;
                tbTagStep.Enabled = true;
            }
            else
            {
                tbTagDelta.Enabled = false;
                tbTagStep.Enabled = false;
            }
        }

        private void btnLogAdd_Click(object sender, EventArgs e)
        {
            dataGridViewLogs.Rows.Add();
        }

        private void btnLogDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить лог файл?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                dataGridViewLogs.Rows.Remove(dataGridViewLogs.CurrentRow);
            }
        }

        private void btnLogEdit_Click(object sender, EventArgs e)
        {
            dataGridViewLogs.ReadOnly = false;
            cbLogMode.Enabled = true;
            btnLogAdd.Enabled = true;
            btnLogDel.Enabled = true;
            btnLogEdit.Enabled = false;
            btnLogSave.Enabled = true;
            btnLogCancel.Enabled = true;

            tbLogQueue.Enabled = true;
            tbLogQueueDelimiter.Enabled = true;

            cbMsgLogMode.Enabled = true;
            tbMsgLogName.Enabled = true;
            tbMsgLogMaxSize.Enabled = true;

            cbTagCacheType.Enabled = true;
            tbTagCacheFile.Enabled = true;
            tbTagCacheInterval.Enabled = true;

            cbTagStoreMode.Enabled = true;

            if ((string)cbTagStoreMode.SelectedItem == "file")
            {
                tbTagStoreFile.Enabled = true;
                tbTagStoreDelimiter.Enabled = true;
                tbTagStoreRowPerTran.Enabled = false;
                tbTagStoreConStr.Enabled = false;
            }
            else
            {
                tbTagStoreFile.Enabled = false;
                tbTagStoreDelimiter.Enabled = false;
                tbTagStoreRowPerTran.Enabled = true;
                tbTagStoreConStr.Enabled = true;
            }

            tbServiceCheckInterval.Enabled = true;


        }

        private void btnLogSave_Click(object sender, EventArgs e)
        {
            //Вернуть старые настройки
            XmlNode _logmanager = xmlDom.SelectSingleNode("//logmanager");
            SetXmlNodeAttribure(xmlDom, _logmanager, "mode", (string)cbLogMode.SelectedItem);
            SetXmlNodeAttribure(xmlDom, _logmanager, "description", "Журналирование данных, допустимые mode: nolog|log|msmq");
            
            XmlNode _logs = xmlDom.SelectSingleNode("//logmanager/logfiles");
            _logs.RemoveAll();
            foreach(DataGridViewRow row in dataGridViewLogs.Rows)
            {
                XmlNode node = xmlDom.CreateNode(XmlNodeType.Element,"logfile", null);
                XmlAttribute attrName = xmlDom.CreateAttribute("name");
                attrName.Value = (string)row.Cells["columnLogName"].Value;
                XmlAttribute attrPath = xmlDom.CreateAttribute("path");
                attrPath.Value = @"logs\" + row.Cells["columnLogName"].Value;
                XmlAttribute attrSize = xmlDom.CreateAttribute("size");
                attrSize.Value = (string)row.Cells["columnLogSize"].Value;

                node.Attributes.Append(attrName);
                node.Attributes.Append(attrPath);
                node.Attributes.Append(attrSize);

                _logs.AppendChild(node);
            }

            //Сохраним настройки журналирования в очередь
            XmlNode _queue = xmlDom.SelectSingleNode("//logmanager/queue");
            if ((string)cbLogMode.SelectedItem == "msmq")
            {
                if (_queue == null)
                {
                    _queue = xmlDom.CreateNode(XmlNodeType.Element, "queue", null);
                    _logmanager.AppendChild(_queue);
                }

                SetXmlNodeAttribure(xmlDom, _queue, "name", tbLogQueue.Text);
                SetXmlNodeAttribure(xmlDom, _queue, "delimiter", tbLogQueueDelimiter.Text);
                SetXmlNodeAttribure(xmlDom, _queue, "description", "Очередь сообщений для логирования данных");

            }
            else
            {
                if (_queue != null)
                {
                    _logmanager.RemoveChild(_queue);
                }
            }


            //Сохраним настройки журнала сообщений
            XmlNode _message = xmlDom.SelectSingleNode("//logmanager/message");
            if (_message == null)
            {
                _message = xmlDom.CreateNode(XmlNodeType.Element, "message", null);
                _logmanager.AppendChild(_message);
            }

            SetXmlNodeAttribure(xmlDom, _message, "name","message");
            SetXmlNodeAttribure(xmlDom, _message, "path", tbMsgLogName.Text);
            SetXmlNodeAttribure(xmlDom, _message, "level", (string)cbMsgLogMode.SelectedItem);
            SetXmlNodeAttribure(xmlDom, _message, "maxsize", tbMsgLogMaxSize.Text);
            SetXmlNodeAttribure(xmlDom, _message, "description", "Журнал сообщений, допустимые level: console|debug|info|warn|error");

            //Сохраним TagCache
            XmlNode _tagcache = xmlDom.SelectSingleNode("//tagcache");
            if (_tagcache == null)
            {
                _tagcache = xmlDom.CreateNode(XmlNodeType.Element, "tagcache", null);
                _logmanager.AppendChild(_tagcache);
            }

            SetXmlNodeAttribure(xmlDom, _tagcache, "type", (string)cbTagCacheType.SelectedItem);
            SetXmlNodeAttribure(xmlDom, _tagcache, "path", tbTagCacheFile.Text);
            SetXmlNodeAttribure(xmlDom, _tagcache, "interval", tbTagCacheInterval.Text);
            SetXmlNodeAttribure(xmlDom, _tagcache, "description", "время обновления кэша в мс");

            XmlNode _tagstoremanager = xmlDom.SelectSingleNode("//tagstoremanager");
            if (_tagstoremanager == null)
            {
                _tagstoremanager = xmlDom.CreateNode(XmlNodeType.Element, "tagstoremanager", null);
                _logmanager.AppendChild(_tagstoremanager);
            }
            //Сохраним TagStore
            XmlNode _tagstore = xmlDom.SelectSingleNode("//tagstoremanager/tagstore");
            if (_tagstore == null)
            {
                _tagstore = xmlDom.CreateNode(XmlNodeType.Element, "tagstore", null);
                _tagstoremanager.AppendChild(_tagstore);
            }

            XmlNode _tagstoreinit = xmlDom.SelectSingleNode("//tagstoremanager/tagstore/init");
            if (_tagstoreinit == null)
            {
                _tagstoreinit = xmlDom.CreateNode(XmlNodeType.Element, "init", null);
                _tagstore.AppendChild(_tagstoreinit);
            }
            else
                _tagstoreinit.RemoveAll();

            SetXmlNodeAttribure(xmlDom, _tagstore, "mode", (string)cbTagStoreMode.SelectedItem);
            SetXmlNodeAttribure(xmlDom, _tagstore, "description", "Настройки хранилища данных");
            if ((string)cbTagStoreMode.SelectedItem == "file")
            {
                SetXmlNodeAttribure(xmlDom, _tagstoreinit, "filename", tbTagStoreFile.Text);
                SetXmlNodeAttribure(xmlDom, _tagstoreinit, "delimiter", tbTagStoreDelimiter.Text);
            }
            else
            {
                SetXmlNodeAttribure(xmlDom, _tagstoreinit, "rowpertran", tbTagStoreRowPerTran.Text);
                SetXmlNodeAttribure(xmlDom, _tagstoreinit, "connectionString", tbTagStoreConStr.Text);
            }

            XmlNode _serviceManager = xmlDom.SelectSingleNode("//servicemanager");
            SetXmlNodeAttribure(xmlDom, _serviceManager, "checkinterval", tbServiceCheckInterval.Text);

            //xmlDom.Save(MatrixFile);

            dataGridViewLogs.ReadOnly = false;
            cbLogMode.Enabled = false;
            btnLogAdd.Enabled = false;
            btnLogDel.Enabled = false;
            btnLogEdit.Enabled = true;
            btnLogSave.Enabled = false;
            btnLogCancel.Enabled = false;

            tbLogQueue.Enabled = false;
            tbLogQueueDelimiter.Enabled = false;

            cbMsgLogMode.Enabled = false;
            tbMsgLogName.Enabled = false;
            tbMsgLogMaxSize.Enabled = false;

            cbTagCacheType.Enabled = false;
            tbTagCacheFile.Enabled = false;
            tbTagCacheInterval.Enabled = false;

            cbTagStoreMode.Enabled = false;
            tbTagStoreFile.Enabled = false;
            tbTagStoreDelimiter.Enabled = false;
            tbTagStoreRowPerTran.Enabled = false;
            tbTagStoreConStr.Enabled = false;

            tbServiceCheckInterval.Enabled = false;

        }

        private void btnLogCancel_Click(object sender, EventArgs e)
        {
            //Вернуть старые настройки
            XmlNode _logmanager = xmlDom.SelectSingleNode("//logmanager");
            cbLogMode.SelectedItem = _logmanager.Attributes["mode"].Value;

            dataGridViewLogs.Rows.Clear();
            XmlNodeList _logs = xmlDom.SelectNodes("//logmanager/logfiles/logfile");
            foreach (XmlNode node in _logs)
            {
                int i = dataGridViewLogs.Rows.Add();
                dataGridViewLogs.Rows[i].Cells["columnLogName"].Value = node.Attributes["name"].Value;
                dataGridViewLogs.Rows[i].Cells["columnLogSize"].Value = node.Attributes["size"].Value;
            }

            XmlNode _queue = xmlDom.SelectSingleNode("//logmanager/queue");
            if (_queue != null)
            {
                tbLogQueue.Text = _queue.Attributes["name"].Value;
                tbLogQueueDelimiter.Text = _queue.Attributes["delimiter"].Value;
            }

            XmlNode _message = xmlDom.SelectSingleNode("//logmanager/message");
            cbMsgLogMode.SelectedItem = _message.Attributes["level"].Value;
            tbMsgLogName.Text = _message.Attributes["path"].Value;
            tbMsgLogMaxSize.Text = _message.Attributes["maxsize"].Value;

            XmlNode _tagcache = xmlDom.SelectSingleNode("//tagcache");
            cbTagCacheType.SelectedItem = _tagcache.Attributes["type"].Value;
            tbTagCacheInterval.Text = _tagcache.Attributes["interval"].Value;
            tbTagCacheFile.Text = _tagcache.Attributes["path"].Value;

            XmlNode _tagstore = xmlDom.SelectSingleNode("//tagstoremanager/tagstore");
            if (_tagstore != null)
            {
                cbTagStoreMode.SelectedItem = _tagstore.Attributes["mode"].Value;
                XmlNode _init = xmlDom.SelectSingleNode("//tagstoremanager/tagstore/init");
                if ((string)cbTagStoreMode.SelectedItem == "file")
                {
                    try
                    {
                        tbTagStoreFile.Text = _init.Attributes["filename"].Value;
                    }
                    catch
                    {
                        tbTagCacheInterval.Text = "logs/store.dat";
                    }
                    try
                    {
                        tbTagStoreDelimiter.Text = _init.Attributes["delimiter"].Value;
                    }
                    catch
                    {
                        tbTagStoreDelimiter.Text = ";";
                    }
                }
                else
                {
                    try
                    {
                        tbTagStoreRowPerTran.Text = _init.Attributes["rowpertran"].Value;
                    }
                    catch
                    {
                        tbTagStoreRowPerTran.Text = "200";
                    }
                    try
                    {
                        tbTagStoreConStr.Text = _init.Attributes["connectionString"].Value;
                    }
                    catch
                    {
                        tbTagStoreConStr.Text = "";
                    }

                }
            }
            else
            {
                cbTagStoreMode.SelectedItem = "file";
                tbTagStoreDelimiter.Text = ";";
                tbTagStoreFile.Text = "logs/tags.xml;logs/tags.bak.xml";
                tbTagStoreRowPerTran.Text = "";
                tbTagStoreConStr.Text = "";
            }

            dataGridViewLogs.ReadOnly = false;
            cbLogMode.Enabled = false;
            btnLogAdd.Enabled = false;
            btnLogDel.Enabled = false;
            btnLogEdit.Enabled = true;
            btnLogSave.Enabled = false;
            btnLogCancel.Enabled = false;

            tbLogQueue.Enabled = false;
            tbLogQueueDelimiter.Enabled = false;


            cbMsgLogMode.Enabled = false;
            tbMsgLogName.Enabled = false;
            tbMsgLogMaxSize.Enabled = false;

            cbTagCacheType.Enabled = false;
            tbTagCacheFile.Enabled = false;
            tbTagCacheInterval.Enabled = false;

            cbTagStoreMode.Enabled = false;
            tbTagStoreFile.Enabled = false;
            tbTagStoreDelimiter.Enabled = false;
            tbTagStoreRowPerTran.Enabled = false;
            tbTagStoreConStr.Enabled = false;

            tbServiceCheckInterval.Enabled = false;

        }

        private void cbTagStoreMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if ((string)cbTagStoreMode.SelectedItem == "file")
            {
                if("".Equals(tbTagStoreFile.Text))
                    tbTagStoreFile.Text = "logs/store.dat";
                if ("".Equals(tbTagStoreDelimiter.Text))
                    tbTagStoreDelimiter.Text = ";";
                tbTagStoreFile.Enabled = true;
                tbTagStoreDelimiter.Enabled = true;
                tbTagStoreRowPerTran.Enabled = false;
                tbTagStoreConStr.Enabled = false;
            }
            else
            {
                if ("".Equals(tbTagStoreRowPerTran.Text))
                    tbTagStoreRowPerTran.Text = "200";
                tbTagStoreFile.Enabled = false;
                tbTagStoreDelimiter.Enabled = false;
                tbTagStoreRowPerTran.Enabled = true;
                tbTagStoreConStr.Enabled = true;
            }
        }

        private void btnSrvCancel_Click(object sender, EventArgs e)
        {
            string _serviceName = (string)dataGridViewServices.CurrentRow.Cells["columnServiceName"].Value;
            XmlNode _service = xmlDom.SelectSingleNode("//servicemanager/services/service[@name='" + _serviceName + "']");
            tbSrvName.Text = GetXmlNodeAttribute(_service, "name", "noname");
            tbSrvDesc.Text = GetXmlNodeAttribute(_service, "description", "");
            cbSrvType.SelectedItem = GetXmlNodeAttribute(_service, "type", "");
            chkSrvEnabled.Checked = (GetXmlNodeAttribute(_service, "enabled", "true") == "true") ? true : false;
            XmlNode _init = _service != null ? _service.ChildNodes[0] : null;
            chkSrvKeepAlive.Checked = (GetXmlNodeAttribute(_init, "keepalive", "false") == "true") ? true : false;
            tbSrvBeforeRetry.Text = GetXmlNodeAttribute(_init, "beforeretry", "60000");
            tbSrvCheckInterval.Text = GetXmlNodeAttribute(_init, "checkinterval", "180000");
            tbSrvInitDesc.Text = GetXmlNodeAttribute(_init, "description", "");
            tbSrvInterval.Text = GetXmlNodeAttribute(_init, "interval", "60000");
            tbSrvMaxCycle.Text = GetXmlNodeAttribute(_init, "maxcycle", "0");
            tbSrvRetry.Text = GetXmlNodeAttribute(_init, "retry", "3");
            tbSrvRetryPause.Text = GetXmlNodeAttribute(_init, "retrypause", "180000");
            tbSrvStartIf.Text = GetXmlNodeAttribute(_init, "startif", "");
            tbSrvStatus.Text = GetXmlNodeAttribute(_init, "status", "");
            tbSrvInit.Text = _init != null ? PrintXML(_init.InnerXml) : "";

            //Считать дополнительные параметры
            string[] stdAttrs = { "beforeretry", "checkinterval", "description", "keepalive", "interval", "maxcycle", "retry", "retrypause", "startif", "status" };
            tbSrvParams.Text = "";
            if (_service != null)
                foreach (XmlAttribute _attr in _service.ChildNodes[0].Attributes)
                {
                    if (!stdAttrs.Contains(_attr.Name))
                        tbSrvParams.Text += _attr.Name + "=" + _attr.Value + "\r\n";
                }
                      
            
            cbSrvType.Enabled = false;
            chkSrvKeepAlive.Enabled = false;
            chkSrvEnabled.Enabled = false;
            tbSrvBeforeRetry.Enabled = false;
            tbSrvBeforeRetry.Enabled = false;
            tbSrvCheckInterval.Enabled = false;
            tbSrvDesc.Enabled = false;
            tbSrvInit.Enabled = false;
            tbSrvInitDesc.Enabled = false;
            tbSrvInterval.Enabled = false;
            tbSrvMaxCycle.Enabled = false;
            tbSrvName.Enabled = false;
            tbSrvParams.Enabled = false;
            tbSrvRetry.Enabled = false;
            tbSrvRetryPause.Enabled = false;
            tbSrvStartIf.Enabled = false;
            tbSrvStatus.Enabled = false;

            btnSrvAdd.Enabled = true;
            btnSrvDel.Enabled = true;
            btnSrvEdit.Enabled = true;
            btnSrvSave.Enabled = false;
            btnSrvCancel.Enabled = false;

            dataGridViewServices.Enabled = true;

        }

        private void btnSrvSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Сохраним данные по сервису
                XmlNode _service = xmlDom.SelectSingleNode("//servicemanager/services/service[@name='" + tbSrvName.Text + "']");
                if (_service == null)
                {
                    _service = xmlDom.CreateNode(XmlNodeType.Element, "service", null);
                    XmlNode _services = xmlDom.SelectSingleNode("//servicemanager/services");
                    _services.AppendChild(_service);
                }
                else if (_service.HasChildNodes)
                    _service.RemoveAll();

                _service.AppendChild(xmlDom.CreateNode(XmlNodeType.Element, "init", null));

                SetXmlNodeAttribure(xmlDom, _service, "name", tbSrvName.Text);
                SetXmlNodeAttribure(xmlDom, _service, "description", tbSrvDesc.Text);
                SetXmlNodeAttribure(xmlDom, _service, "type", (string)cbSrvType.SelectedItem);
                SetXmlNodeAttribure(xmlDom, _service, "enabled", chkSrvEnabled.Checked ? "true" : "false");

                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "beforeretry", tbSrvBeforeRetry.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "description", tbSrvInitDesc.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "interval", tbSrvInterval.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "checkinterval", tbSrvCheckInterval.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "keepalive", chkSrvKeepAlive.Checked ? "true" : "false");
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "maxcycle", tbSrvMaxCycle.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "retry", tbSrvRetry.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "retrypause", tbSrvRetryPause.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "startif", tbSrvStartIf.Text);
                SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], "status", tbSrvStatus.Text);

                string[] _pars = tbSrvParams.Text.Split('\r');
                foreach (string _par in _pars)
                {
                    string[] _obj = _par.Split('=');
                    if (_obj.Length == 2)
                        SetXmlNodeAttribure(xmlDom, _service.ChildNodes[0], _obj[0].Trim(), _obj[1].Trim());
                }

                _service.ChildNodes[0].InnerXml = tbSrvInit.Text;

                dataGridViewServices.CurrentRow.Cells["columnServiceName"].Value = tbSrvName.Text;
                dataGridViewServices.CurrentRow.Cells["columnServiceEnabled"].Value = chkSrvEnabled.Checked ? "true" : "false";
                dataGridViewServices.CurrentRow.Cells["columnServiceDesc"].Value = tbSrvDesc.Text;


                cbSrvType.Enabled = false;
                chkSrvKeepAlive.Enabled = false;
                chkSrvEnabled.Enabled = false;
                tbSrvBeforeRetry.Enabled = false;
                tbSrvCheckInterval.Enabled = false;
                tbSrvDesc.Enabled = false;
                tbSrvInit.Enabled = false;
                tbSrvInitDesc.Enabled = false;
                tbSrvInterval.Enabled = false;
                tbSrvMaxCycle.Enabled = false;
                tbSrvName.Enabled = false;
                tbSrvParams.Enabled = false;
                tbSrvRetry.Enabled = false;
                tbSrvRetryPause.Enabled = false;
                tbSrvStartIf.Enabled = false;
                tbSrvStatus.Enabled = false;

                btnSrvAdd.Enabled = true;
                btnSrvDel.Enabled = true;
                btnSrvEdit.Enabled = true;
                btnSrvSave.Enabled = false;
                btnSrvCancel.Enabled = false;

                dataGridViewServices.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Проверьте правильность заполнения данных и формата XML!","Ошибка сохранения");
            }

        }

        private void btnSrvEdit_Click(object sender, EventArgs e)
        {
            cbSrvType.Enabled = true;
            chkSrvKeepAlive.Enabled = true;
            chkSrvEnabled.Enabled = true;
            tbSrvBeforeRetry.Enabled = true;
            tbSrvBeforeRetry.Enabled = true;
            tbSrvCheckInterval.Enabled = true;
            tbSrvDesc.Enabled = true;
            tbSrvInit.Enabled = true;
            tbSrvInitDesc.Enabled = true;
            tbSrvInterval.Enabled = true;
            tbSrvMaxCycle.Enabled = true;
            tbSrvName.Enabled = true;
            tbSrvParams.Enabled = true;
            tbSrvRetry.Enabled = true;
            tbSrvRetryPause.Enabled = true;
            tbSrvStartIf.Enabled = true;
            tbSrvStatus.Enabled = true;

            btnSrvAdd.Enabled = false;
            btnSrvDel.Enabled = false;
            btnSrvEdit.Enabled = false;
            btnSrvSave.Enabled = true;
            btnSrvCancel.Enabled = true;

            dataGridViewServices.Enabled = false;

        }

        private void dataGridViewServices_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewServices.Rows[e.RowIndex].Cells["columnServiceName"].Value != null)
                LoadServiceItem((string)dataGridViewServices.Rows[e.RowIndex].Cells["columnServiceName"].Value);
        }

        private void LoadServiceItem(string _serviceName)
        {
                XmlNode _service = xmlDom.SelectSingleNode("//servicemanager/services/service[@name='" + _serviceName + "']");
                tbSrvName.Text = GetXmlNodeAttribute(_service, "name", "");
                tbSrvDesc.Text = GetXmlNodeAttribute(_service, "description", "");
                cbSrvType.SelectedItem = GetXmlNodeAttribute(_service, "type", "");
                if (cbSrvType.SelectedItem == null)
                    cbSrvType.SelectedItem = "custom";
                chkSrvEnabled.Checked = (GetXmlNodeAttribute(_service, "enabled", "true") == "true") ? true : false;
                XmlNode _init = _service != null ? _service.ChildNodes[0] : null;
                chkSrvKeepAlive.Checked = (GetXmlNodeAttribute(_init, "keepalive", "false") == "true") ? true : false;
                tbSrvBeforeRetry.Text = GetXmlNodeAttribute(_init, "beforeretry", "60000");
                tbSrvCheckInterval.Text = GetXmlNodeAttribute(_init, "checkinterval", "180000");
                tbSrvInitDesc.Text = GetXmlNodeAttribute(_init, "description", "");
                tbSrvInterval.Text = GetXmlNodeAttribute(_init, "interval", "60000");
                tbSrvMaxCycle.Text = GetXmlNodeAttribute(_init, "maxcycle", "0");
                tbSrvRetry.Text = GetXmlNodeAttribute(_init, "retry", "3");
                tbSrvRetryPause.Text = GetXmlNodeAttribute(_init, "retrypause", "180000");
                tbSrvStartIf.Text = GetXmlNodeAttribute(_init, "startif", "");
                tbSrvStatus.Text = GetXmlNodeAttribute(_init, "status", "");
                tbSrvInit.Text = _init != null ? PrintXML(_init.InnerXml) : "";

                //Считать дополнительные параметры
                string[] stdAttrs = { "beforeretry", "checkinterval", "description", "keepalive", "interval", "maxcycle", "retry", "retrypause", "startif", "status" };
                tbSrvParams.Text = "";
                if (_service != null)
                    foreach (XmlAttribute _attr in _service.ChildNodes[0].Attributes)
                    {
                        if (!stdAttrs.Contains(_attr.Name))
                            tbSrvParams.Text += _attr.Name + "=" + _attr.Value + "\r\n";
                    }
        }
        
        private void btnSrvAdd_Click(object sender, EventArgs e)
        {
            int i = dataGridViewServices.Rows.Add();
            dataGridViewServices.CurrentCell = dataGridViewServices.Rows[i].Cells[0];

            tbSrvName.Text = "";
            tbSrvDesc.Text = "";
            cbSrvType.SelectedItem = "";
            chkSrvEnabled.Checked = false;
            chkSrvKeepAlive.Checked = false;
            tbSrvBeforeRetry.Text =  "60000";
            tbSrvCheckInterval.Text = "180000";
            tbSrvInitDesc.Text = "";
            tbSrvInterval.Text = "60000";
            tbSrvMaxCycle.Text = "0";
            tbSrvRetry.Text = "3";
            tbSrvRetryPause.Text = "180000";
            tbSrvStartIf.Text = "";
            tbSrvStatus.Text = "";
            tbSrvInit.Text = "";

            dataGridViewServices.Enabled = false;
            cbSrvType.Enabled = true;
            chkSrvKeepAlive.Enabled = true;
            chkSrvEnabled.Enabled = true;
            tbSrvBeforeRetry.Enabled = true;
            tbSrvBeforeRetry.Enabled = true;
            tbSrvCheckInterval.Enabled = true;
            tbSrvDesc.Enabled = true;
            tbSrvInit.Enabled = true;
            tbSrvInitDesc.Enabled = true;
            tbSrvInterval.Enabled = true;
            tbSrvMaxCycle.Enabled = true;
            tbSrvName.Enabled = true;
            tbSrvParams.Enabled = true;
            tbSrvRetry.Enabled = true;
            tbSrvRetryPause.Enabled = true;
            tbSrvStartIf.Enabled = true;
            tbSrvStatus.Enabled = true;

            btnSrvAdd.Enabled = false;
            btnSrvDel.Enabled = false;
            btnSrvEdit.Enabled = false;
            btnSrvSave.Enabled = true;
            btnSrvCancel.Enabled = true;
                        
        }

        private void btnSrvDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить сервис", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                XmlNode _services = xmlDom.SelectSingleNode("//servicemanager/services");
                XmlNode _service = xmlDom.SelectSingleNode("//servicemanager/services/service[@name='" + tbSrvName.Text + "']");
                _services.RemoveChild(_service);
                dataGridViewServices.Rows.Remove(dataGridViewServices.CurrentRow);

                //Очистим поля
                tbSrvName.Text = "";
                tbSrvDesc.Text = "";
                cbSrvType.SelectedItem = "";
                chkSrvEnabled.Checked = false;

                chkSrvKeepAlive.Checked = false;
                tbSrvBeforeRetry.Text = "";
                tbSrvCheckInterval.Text = "";
                tbSrvInitDesc.Text = "";
                tbSrvInterval.Text = "";
                tbSrvMaxCycle.Text = "";
                tbSrvRetry.Text = "";
                tbSrvRetryPause.Text = "";
                tbSrvStartIf.Text = "";
                tbSrvStatus.Text = "";
                tbSrvInit.Text = "";

            }
        }

        private void cbSrvType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch ((string)cbSrvType.SelectedItem)
            {
                case "exec": 
                    tbSrvParams.Text = "cmd=cscript\r\nworkdir=c:/windows/temp";
                    tbSrvInit.Text = @"
<param name=""arg0"" type=""string"" typeval=""tag|val|file"" value=""/B"" />
<param name=""arg1"" type=""string"" typeval=""file"" value=""test.js"">
<![CDATA[
WScript.StdOut.WriteLine(""Arg[0]=""+WScript.Arguments(0));
WScript.StdOut.WriteLine(""RETURN"");
WScript.StdOut.WriteLine(""p1=1.1"");
WScript.StdOut.WriteLine(""p2=2.34"");
WScript.StdOut.WriteLine(""p3=4.5"");
WScript.StdOut.WriteLine(""p4=Hello world=Hhhh=hhhh!!!!"");
]]>
</param>
<param name=""arg2"" type=""float"" typeval=""val"" value=""12.2765"" />
<map name=""tag1"" type=""var"" value=""p1"" />
<map name=""tag2"" type=""var"" value=""p2"" />
<map name=""tag3"" type=""var"" value=""p3"" />
<map name=""tag4"" type=""var"" value=""p4"" />";
                    break;
                case "mssql":
                case "oracle":
                case "oledb":
                    tbSrvParams.Text = "connectionString=строка соединения с БД\r\ntype=query|scalar|update|call";
                    tbSrvInit.Text = @"
<query description=""example for type update|call"">
<![CDATA[
    insert into dbo.test(col1,col2,col3)
    values(@par1,2,GETDATE())
]]>
</query>
<param name=""par1"" type=""bool|byte|int|float|datetime|string|xml"" typeval=""tag|const"" value=""data|120"" description=""typeval is tag|const""/>

<query description=""example for type scalar"">
<![CDATA[
    SELECT name as col1 FROM sys.databases WHERE name LIKE @par1
]]>
</query>
<param name=""par1"" type=""bool|byte|int|float|datetime|string|xml"" typeval=""tag|const"" value=""data|120"" description=""typeval is tag|const""/>
<map name=""tag"" column=""col1""/>

<query description=""example for type query"">
<![CDATA[
    SELECT ID, ULogin,UPassword FROM test
]]>
</query>
<map name=""tag"" column=""""/>";

                    break;
                case "dll": 
                    tbSrvParams.Text = "assemble=имя сборки *.dll\r\nmethod=метод для запуска";
                    tbSrvInit.Text = "";
                    break;
                case "delay": 
                    tbSrvParams.Text = "delay=60000";
                    tbSrvInit.Text = "";
                    break;
                case "http": 
                    tbSrvParams.Text = "url=адрес ресурса\r\nmethod=метод GET|POST\r\ndownloadfile=имя выгружаемого файла\r\nuploadfile=имя загружаемого файла";
                    tbSrvInit.Text = "";
                    break;
                case "custom": 
                    tbSrvParams.Text = "type=имя класса\r\nassemble=имя сборки *.dll";
                    tbSrvInit.Text = "";
                    break;
                case "snapshot":
                    tbSrvParams.Text = "";
                    tbSrvInit.Text = @"
<snapshot>
<![CDATA[
    <msg>
        <description>
        Формируйте шаблон, в квадратных [] скобках укажите имя тега как указано ниже.
        В разделе <map> укажите имя тега для сохранения результата. 
        </description>
        <type>1</type>
        <name>melt_mark_xa</name>
        <id>[id]</id>
    </msg>
]]>
</snapshot>
<map name=""WINCCDB1.CURDB""/>";
                    break;
                case "xml2mssql":
                    tbSrvParams.Text = "type=snapshot|merge";
                    tbSrvInit.Text = @"
<delete>
    <command>
    <![CDATA[
        SELECT GETDATE()
	]]>
    </command>
</delete>
<insert>
    <command>
    <![CDATA[
        IF EXISTS(SELECT ID FROM UA#USERS WHERE ID=@ID)
           UPDATE UA#USERS SET
             ULogin = @ULogin, UPassword = @UPassword        
           WHERE ID=@ID  
        ELSE
           INSERT INTO UA#USERS(ID,ULogin,UPassword)
	       VALUES (@ID,@ULogin,@UPassword) 
    ]]>
    </command>
    <param name=""@ID"" type=""int"" typeval=""const"" value=""ID""/>
    <param name=""@ULogin"" type=""string"" typeval=""const"" value=""ULogin""/>
    <param name=""@UPassword"" type=""string"" typeval=""const"" value=""UPassword""/>    
</insert>
<param name=""table"" type=""xml"" typeval=""tag"" value=""Users.1"" description=""typeval is tag|const""/>
<param name=""row"" type=""xml"" typeval=""const"" value=""//rows/row"" description=""XPath тега строки""/>
<param name=""dmlStatus"" type=""string"" typeval=""const"" value=""dmlstatus"" description=""XPath тега поля DML операции I,U,D""/>
<param name=""historyFieldName"" type=""string"" typeval=""const"" value=""LastAccess"" description=""XPath тега поля истории""/>
<param name=""historyFieldValue"" type=""datetime"" direction=""out"" typeval=""tag"" value=""UsersLastSnap.1"" description=""typeval is tag|const""/>
";
                    break;
                default:
                    tbSrvParams.Text = "";
                    tbSrvInit.Text = "";
                    break;
            }
        }

        private void dataGridViewTags_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewTags.Rows[e.RowIndex].Cells["columnTagName"].Value!=null)
                LoadTagItem((string)dataGridViewTags.Rows[e.RowIndex].Cells["columnTagName"].Value);
        }

        private void btnTagAdd_Click(object sender, EventArgs e)
        {
            int i = dataGridViewTags.Rows.Add();
            dataGridViewTags.CurrentCell = dataGridViewTags.Rows[i].Cells[0];

            tbTagName.Text = "";
            tbTagDesc.Text = "";
            tbTagDefault.Text = "";
            cbTagType.SelectedItem = "";
            chkTagLogMode.Checked = false;
            tbTagDelta.Text = "0";
            tbTagStep.Text = "300";

            dataGridViewTags.Enabled = false;
            cbTagType.Enabled = true;
            chkTagLogMode.Enabled = true;
            tbTagName.Enabled = true;
            tbTagDesc.Enabled = true;
            tbTagDefault.Enabled = true;
            tbTagDelta.Enabled = false;
            tbTagStep.Enabled = false;

            btnTagAdd.Enabled = false;
            btnTagDel.Enabled = false;
            btnTagEdit.Enabled = false;
            btnTagSave.Enabled = true;
            btnTagCancel.Enabled = true;
        }


        private void btnTagCancel_Click(object sender, EventArgs e)
        {
            string tagName = (string)dataGridViewTags.CurrentRow.Cells["columnTagName"].Value;
            LoadTagItem(tagName);
            if(tagName==null)
                dataGridViewTags.Rows.Remove(dataGridViewTags.CurrentRow);
            dataGridViewTags.Enabled = true;

        }

        private void SaveConfig()
        {
            if (xmlDom != null)
            {
                //Соберём Xml документ и сохраним его на диске
                //Теги
                XmlNode _tags = xmlDom.SelectSingleNode("//tagmanager/tags");
                _tags.RemoveAll();
                foreach (DictionaryEntry node in tags)
                {
                    _tags.AppendChild((XmlNode)node.Value);
                }
                _tags = xmlDom.SelectSingleNode("//tagmanager/tags");

                //Сохраним на диске
                if (File.Exists(MatrixFile))
                    File.Copy(MatrixFile, MatrixFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak");
                xmlDom.Save(MatrixFile);
                MessageBox.Show("Файл конфигурации успешно записан!");
            }
        }

        private void btnTagSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Сохраним данные по сервису
                XmlNode _tag = xmlDom.SelectSingleNode("//tagmanager/tags/tag[@name='" + tbTagName.Text + "']");
                if (_tag == null)
                {
                    _tag = xmlDom.CreateNode(XmlNodeType.Element, "tag", null);
                    XmlNode _tags = xmlDom.SelectSingleNode("//tagmanager/tags");
                    _tags.AppendChild(_tag);
                    if (!this.tags.ContainsKey(tbTagName.Text))
                        this.tags.Add(tbTagName.Text, _tag);
                }

                SetXmlNodeAttribure(xmlDom, _tag, "name", tbTagName.Text);
                SetXmlNodeAttribure(xmlDom, _tag, "description", tbTagDesc.Text);
                SetXmlNodeAttribure(xmlDom, _tag, "type", (string)cbTagType.SelectedItem);
                if (chkTagLogMode.Checked)
                {
                    SetXmlNodeAttribure(xmlDom, _tag, "logmode", chkTagLogMode.Checked ? "1" : "0");
                    SetXmlNodeAttribure(xmlDom, _tag, "delta", tbTagDelta.Text);
                    SetXmlNodeAttribure(xmlDom, _tag, "step", tbTagStep.Text);
                }
                else
                {
                    RemoveXmlNodeAttribure(xmlDom, _tag, "logmode");
                    RemoveXmlNodeAttribure(xmlDom, _tag, "delta");
                    RemoveXmlNodeAttribure(xmlDom, _tag, "step");
                }


                _tag.InnerXml = tbTagDefault.Text;

                dataGridViewTags.CurrentRow.Cells["columnTagName"].Value = tbTagName.Text;
                dataGridViewTags.CurrentRow.Cells["columnTagType"].Value = (string)cbTagType.SelectedItem;
                dataGridViewTags.CurrentRow.Cells["columntagDesc"].Value = tbTagDesc.Text;


                tbTagName.Enabled = false;
                cbTagType.Enabled = false;
                tbTagDesc.Enabled = false;
                chkTagLogMode.Enabled = false;
                tbTagDelta.Enabled = false;
                tbTagStep.Enabled = false;

                btnTagAdd.Enabled = true;
                btnTagDel.Enabled = true;
                btnTagEdit.Enabled = true;
                btnTagSave.Enabled = false;
                btnTagCancel.Enabled = false;

                dataGridViewTags.Enabled = true;

            }
            catch
            {
                MessageBox.Show("Проверьте правильность заполнения данных и формата XML!", "Ошибка сохранения");
            }

        }

        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewConfig();
            LoadConfigFromXml();
        }


        private void CreateNewConfig()
        {
            createNewToolStripMenuItem.Enabled = false;
            openConfigToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;

            tabControl1.Enabled = true;

            btnTagSave.Enabled = false;
            btnTagCancel.Enabled = false;
            btnTagDel.Enabled = false;
            tags.Clear();
            dataGridViewTags.Rows.Clear();
            dataGridViewServices.Rows.Clear();
            dataGridViewLogs.Rows.Clear();

            MatrixFile = "matrix.xml";
            this.statusStrip1.Items.Add(MatrixFile);

            xmlDom = new XmlDocument();

            XmlNode _rootNode = xmlDom.CreateNode(XmlNodeType.Element, "matrix", null);
            xmlDom.AppendChild(_rootNode);

            XmlNode _tagcache = xmlDom.CreateNode(XmlNodeType.Element, "tagcache", null);
            _rootNode.AppendChild(_tagcache);
            SetXmlNodeAttribure(xmlDom, _tagcache, "interval", "1000");
            SetXmlNodeAttribure(xmlDom, _tagcache, "type", "disk");
            SetXmlNodeAttribure(xmlDom, _tagcache, "path", "logs/tags.xml;logs/tags.bak.xml");
            SetXmlNodeAttribure(xmlDom, _tagcache, "description", "время обновления кэша в мсl");

            XmlNode _logmanager = xmlDom.CreateNode(XmlNodeType.Element, "logmanager", null);
            _rootNode.AppendChild(_logmanager);
            SetXmlNodeAttribure(xmlDom, _logmanager, "mode", "nolog");
            SetXmlNodeAttribure(xmlDom, _logmanager, "description", "log|nolog");

            XmlNode _logfiles = xmlDom.CreateNode(XmlNodeType.Element, "logfiles", null);
            _logmanager.AppendChild(_logfiles);

            XmlNode _logfile1 = xmlDom.CreateNode(XmlNodeType.Element, "logfile", null);
            _logfiles.AppendChild(_logfile1);
            SetXmlNodeAttribure(xmlDom, _logfile1, "name", "log1");
            SetXmlNodeAttribure(xmlDom, _logfile1, "path", "logs/log1");
            SetXmlNodeAttribure(xmlDom, _logfile1, "size", "32768");

            XmlNode _logfile2 = xmlDom.CreateNode(XmlNodeType.Element, "logfile", null);
            _logfiles.AppendChild(_logfile2);
            SetXmlNodeAttribure(xmlDom, _logfile2, "name", "log2");
            SetXmlNodeAttribure(xmlDom, _logfile2, "path", "logs/log2");
            SetXmlNodeAttribure(xmlDom, _logfile2, "size", "32768");

            XmlNode _logfile3 = xmlDom.CreateNode(XmlNodeType.Element, "logfile", null);
            _logfiles.AppendChild(_logfile3);
            SetXmlNodeAttribure(xmlDom, _logfile3, "name", "log3");
            SetXmlNodeAttribure(xmlDom, _logfile3, "path", "logs/log3");
            SetXmlNodeAttribure(xmlDom, _logfile3, "size", "32768");

            XmlNode _message = xmlDom.CreateNode(XmlNodeType.Element, "message", null);
            _logmanager.AppendChild(_message);
            SetXmlNodeAttribure(xmlDom, _message, "name", "message");
            SetXmlNodeAttribure(xmlDom, _message, "path", "logs/app.log");
            SetXmlNodeAttribure(xmlDom, _message, "level", "info");
            SetXmlNodeAttribure(xmlDom, _message, "maxsize", "524288");
            SetXmlNodeAttribure(xmlDom, _message, "decsription", "console|debug|info|warn|error");

            XmlNode _tagstoremanager = xmlDom.CreateNode(XmlNodeType.Element, "tagstoremanager", null);
            _rootNode.AppendChild(_tagstoremanager);
            XmlNode _tagstore = xmlDom.CreateNode(XmlNodeType.Element, "tagstore", null);
            _tagstoremanager.AppendChild(_tagstore);
            SetXmlNodeAttribure(xmlDom, _tagstore, "mode", "file");
            SetXmlNodeAttribure(xmlDom, _tagstore, "description", "file|mssql|oracle");

            XmlNode _tagstoreinit = xmlDom.CreateNode(XmlNodeType.Element, "init", null);
            _tagstoremanager.AppendChild(_tagstoreinit);
            SetXmlNodeAttribure(xmlDom, _tagstoreinit, "filename", "logs/store.dat");
            SetXmlNodeAttribure(xmlDom, _tagstoreinit, "delimiter", ";");

            XmlNode _servicemanager = xmlDom.CreateNode(XmlNodeType.Element, "servicemanager", null);
            _rootNode.AppendChild(_servicemanager);
            SetXmlNodeAttribure(xmlDom, _servicemanager, "checkinterval", "5000");
            XmlNode _services = xmlDom.CreateNode(XmlNodeType.Element, "services", null);
            _servicemanager.AppendChild(_services);

            XmlNode _tagmanager = xmlDom.CreateNode(XmlNodeType.Element, "tagmanager", null);
            _rootNode.AppendChild(_tagmanager);
            XmlNode _tagNodes = xmlDom.CreateNode(XmlNodeType.Element, "tags", null);
            _tagmanager.AppendChild(_tagNodes);

        }

        private void CloseConfig()
        {
            tags.Clear();
            MatrixFile = "";
            statusStrip1.Items.Clear();
            dataGridViewTags.Rows.Clear();
            dataGridViewServices.Rows.Clear();
            dataGridViewLogs.Rows.Clear();

            createNewToolStripMenuItem.Enabled = true;
            openConfigToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;

            tabControl1.Enabled = false;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            createNewToolStripMenuItem.Enabled = true;
            openConfigToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            tabControl1.Enabled = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseConfig();
        }

        private void btnTagCopy_Click(object sender, EventArgs e)
        {
            string tagName = (string)dataGridViewTags.CurrentRow.Cells["columnTagName"].Value;
            if (tagName != null && !"".Equals(tagName))
            {
                int i = dataGridViewTags.Rows.Add();
                dataGridViewTags.CurrentCell = dataGridViewTags[0, i];
                LoadTagItem(tagName);
                tbTagName.Text = tagName + "_new";
                btnEdit_Click(sender, e);
            }
        }

        private void btnSrvCopy_Click(object sender, EventArgs e)
        {
            string _serviceName = (string)dataGridViewServices.CurrentRow.Cells["columnServiceName"].Value;
            if (_serviceName != null && !"".Equals(_serviceName))
            {
                int i = dataGridViewServices.Rows.Add();
                dataGridViewServices.CurrentCell = dataGridViewServices[0, i];
                LoadServiceItem(_serviceName);
                tbSrvName.Text = _serviceName + "_new";
                btnSrvEdit_Click(sender, e);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Matrix config file (matrix.xml)|matrix.xml|Xml files|*.xml";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MatrixFile = dialog.FileName;
                this.statusStrip1.Items[0].Text = MatrixFile;
                SaveConfig();
            }

        }

        private void btnTagDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить Tag", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                XmlNode _tags = xmlDom.SelectSingleNode("//tagmanager/tags");
                XmlNode _tag = xmlDom.SelectSingleNode("//tagmanager/tags/tag[@name='" + tbTagName.Text + "']");
                _tags.RemoveChild(_tag);
                tags.Remove(tbTagName.Text);
                
                dataGridViewTags.Rows.Remove(dataGridViewTags.CurrentRow);

                //Очистим поля
                tbTagName.Text = "";
                tbTagDesc.Text = "";
                tbTagDefault.Text = "";
                cbTagType.SelectedItem = "";
                chkTagLogMode.Checked = false;

                tbTagDelta.Text = "";
                tbTagStep.Text = "";
            }
        }
      
    }
}
