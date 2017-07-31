using System;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Matrix.Types;
using System.Text;
using System.Text.RegularExpressions;
using Common.Logging;

namespace Matrix.Services
{
    public class ServiceXml2MSSQL : Service
    {
        private static ILog log = LogManager.GetLogger<ServiceXml2MSSQL>();
        private string _connectionString;

        private string _insert;
        private string _update;
        private string _delete;

        private string _table;
        private string _row;
        private string _dmlStatus;
        private string _historyFiledName;
        private string _historyFiledValue;
        private string _historyFiledValueType;

        private SqlConnection _con;
        private SqlCommand _cmdInsert;
        private SqlCommand _cmdUpdate;
        private SqlCommand _cmdDelete;

        private string[,] _params;
        private string[,] _maps;
        private string[,] _iparams;
        private string[,] _uparams;
        private string[,] _dparams;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            _connectionString = xmlconf.Attributes["connectionString"].Value;
            _type = xmlconf.Attributes["type"].Value.ToLower();
            XmlNode _node = xmlconf.SelectSingleNode("insert/command");
            _insert = _node.InnerText;
            _node = xmlconf.SelectSingleNode("delete/command");
            _delete = _node.InnerText;
            if ("merge".Equals(_type))
            {
                _node = xmlconf.SelectSingleNode("update/command");
                _update = _node.InnerText;
            }

            //Параметры для insert
            XmlNodeList nodes = xmlconf.SelectNodes("insert/param");
            _iparams = new string[nodes.Count, 5];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _iparams[i, 0] = node.Attributes["name"].Value;
                _iparams[i, 1] = node.Attributes["type"].Value;
                _iparams[i, 2] = node.Attributes["typeval"].Value;
                _iparams[i, 3] = node.Attributes["value"].Value;
                try
                {
                    _iparams[i, 4] = node.Attributes["direction"].Value.ToLower();
                }
                catch (Exception e)
                {
                    _iparams[i, 4] = "in";
                }
                i++;
            }

            //Параметры для delete
            nodes = xmlconf.SelectNodes("delete/param");
            _dparams = new string[nodes.Count, 5];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _dparams[i, 0] = node.Attributes["name"].Value;
                _dparams[i, 1] = node.Attributes["type"].Value;
                _dparams[i, 2] = node.Attributes["typeval"].Value;
                _dparams[i, 3] = node.Attributes["value"].Value;
                try
                {
                    _dparams[i, 4] = node.Attributes["direction"].Value.ToLower();
                }
                catch (Exception e)
                {
                    _dparams[i, 4] = "in";
                }
                i++;
            }

            //Если merge репликация то
            if ("merge".Equals(_type))
            {
                //Параметры для update
                nodes = xmlconf.SelectNodes("update/param");
                _uparams = new string[nodes.Count, 5];
                i = 0;
                foreach (XmlNode node in nodes)
                {
                    _uparams[i, 0] = node.Attributes["name"].Value;
                    _uparams[i, 1] = node.Attributes["type"].Value;
                    _uparams[i, 2] = node.Attributes["typeval"].Value;
                    _uparams[i, 3] = node.Attributes["value"].Value;
                    try
                    {
                        _uparams[i, 4] = node.Attributes["direction"].Value.ToLower();
                    }
                    catch (Exception e)
                    {
                        _uparams[i, 4] = "in";
                    }
                    i++;
                }
            }

            //Параметры глобальные
            nodes = xmlconf.SelectNodes("param");
            _params = new string[nodes.Count, 5];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _params[i, 0] = node.Attributes["name"].Value;
                _params[i, 1] = node.Attributes["type"].Value;
                _params[i, 2] = node.Attributes["typeval"].Value;
                _params[i, 3] = node.Attributes["value"].Value;
                try
                {
                    _params[i, 4] = node.Attributes["direction"].Value.ToLower();
                }
                catch (Exception e)
                {
                    _params[i, 4] = "in";
                }
                switch (_params[i, 0].ToLower())
                {
                    case "table":
                        _table = _params[i, 3];
                        break;
                    case "row":
                        _row = _params[i, 3];
                        break;
                    case "dmlstatus":
                        _dmlStatus = _params[i, 3];
                        break;
                    case "historyfieldname":
                        _historyFiledName = _params[i, 3];
                        break;
                    case "historyfieldvalue":
                        _historyFiledValue = _params[i, 3];
                        _historyFiledValueType = _params[i, 1];
                        break;
                    default:
                        break;
                }
                i++;
            }
            nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 2];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i, 0] = node.Attributes["name"].Value;
                _maps[i, 1] = node.Attributes["column"].Value;
                i++;
            }
        }


        public override void RunOnce()
        {
            //Создадим соединение
            if (!Connect())
                throw new Exception("Ошибка подключения!");

            //получить набор параметров и сохранить в параметрах
            Tag tag = _server.GetTag(_table);
            if (tag.Value == null || "".Equals(tag.Value))
                return;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml((string)tag.Value);
            //Если есть данные в XML
            XmlNodeList nodeList = xml.SelectNodes(_row);
            if(nodeList.Count > 0)
            {
                SqlTransaction _tran = _con.BeginTransaction();
                try
                {

                    //Удалим данные в таблице
                    _cmdInsert.Transaction = _tran;
                    _cmdDelete.Transaction = _tran;
                    if ("merge".Equals(_type))
                        _cmdUpdate.Transaction = _tran;
                    //Удалим данные если это snapshot
                    if ("snapshot".Equals(_type))
                    {
                        for (int i = 0; i < _dparams.GetLength(0); i++)
                        {
                            if (_dparams[i, 4].StartsWith("in"))
                            {
                                object _val = null;
                                if ("tag".Equals(_dparams[i, 2]))
                                    _val = _server.GetTag(_dparams[i, 3]).Value;
                                else
                                    _val = _dparams[i, 3];
                                _cmdDelete.Parameters[_dparams[i, 0]].Value = (_val!=null) ? _val : DBNull.Value;
                            }
                        }
                        _cmdDelete.ExecuteNonQuery();
                    }

                    //Взять XML и для каждого узла выполнить
                    object _historyValue = null;
                    foreach (XmlNode node in nodeList)
                    {
                        if ("snapshot".Equals(_type))
                        {
                            //заполним параметры
                            for (int i = 0; i < _iparams.GetLength(0); i++)
                            {
                                object _val = node.SelectSingleNode(_iparams[i, 3]).InnerText;
                                SqlParameter _param = _cmdInsert.Parameters[_iparams[i, 0]];
                                if (_param.SqlDbType == SqlDbType.Float && _val != null && !"".Equals(_val))
                                    _val = Convert.ToSingle(((string)(_val)).Replace('.', ','));
                                else if (_param.SqlDbType == SqlDbType.DateTime && _val != null && !"".Equals(_val))
                                    _val = Convert.ToDateTime(_val);
                                _cmdInsert.Parameters[_iparams[i, 0]].Value = (_val != null && !"".Equals(_val)) ? _val : DBNull.Value;
                            }
                            _cmdInsert.ExecuteNonQuery();
                        }
                        else if ("merge".Equals(_type))
                        {
                            string dml = node.SelectSingleNode(_dmlStatus).InnerText.ToLower();
                            if ("i".Equals(dml))
                            {
                                //заполним параметры
                                for (int i = 0; i < _iparams.GetLength(0); i++)
                                {
                                    object _val = node.SelectSingleNode(_iparams[i, 3]).InnerText;
                                    _cmdInsert.Parameters[_iparams[i, 0]].Value = _val;
                                }
                                _cmdInsert.ExecuteNonQuery();
                            }
                            else if ("u".Equals(dml))
                            {
                                //заполним параметры
                                for (int i = 0; i < _uparams.GetLength(0); i++)
                                {
                                    object _val = node.SelectSingleNode(_uparams[i, 3]).InnerText;
                                    _cmdUpdate.Parameters[_uparams[i, 0]].Value = _val;
                                }
                                _cmdUpdate.ExecuteNonQuery();
                            }
                            else if ("d".Equals(dml))
                            {
                                //заполним параметры
                                for (int i = 0; i < _dparams.GetLength(0); i++)
                                {
                                    object _val = node.SelectSingleNode(_dparams[i, 3]).InnerText;
                                    _cmdDelete.Parameters[_dparams[i, 0]].Value = _val;
                                }
                                _cmdDelete.ExecuteNonQuery();
                            }
                        }

                        //запомним макс значение поля history
                        if ("datetime".Equals(_historyFiledValueType.ToLower()))
                        {
                            DateTime _val = DateTime.Parse(node.SelectSingleNode(_historyFiledName).InnerText);
                            if (_historyValue == null || _val > (DateTime)_historyValue)
                                _historyValue = _val;
                        }
                        else
                        {
                            int _val = int.Parse(node.SelectSingleNode(_historyFiledName).InnerText);
                            if (_historyValue == null || _val > (int)_historyValue)
                                _historyValue = _val;
                        }
                        
                        LastRun = DateTime.Now;
                    }
                    _tran.Commit();
                    //Сохраним поле history в теге
                    if (_historyValue != null)
                    {

                        tag = _server.GetTag(_table);
                        tag.Value = "";
                        tag.OldValue = "";
                        _server.SetTag(tag);
                        tag = _server.GetTag(_historyFiledValue);
                        tag.Value = _historyValue;
                        _server.SetTag(tag);
                    }
                    log.Info("[" + this._name + "] блок данных передан, "+_historyFiledName+"="+_historyValue.ToString());
                }
                catch (Exception e)
                {
                    log.Warn("[" + this._name + "] " + e.Message);
                    _tran.Rollback();
                    throw new Exception("Ошибка сохранения данных!"+e.Message);
                }
            }

            //Отключим соединение если не требуется хранить его
            if (!_keepalive)
                Disconnect();
        }

        public override void AfterStart()
        {
            _con = new SqlConnection(parseConnectionString(_connectionString));
            _cmdInsert = _con.CreateCommand();
            _cmdInsert.CommandText = _insert;
            _cmdInsert.CommandType = CommandType.Text;
            _cmdDelete = _con.CreateCommand();
            _cmdDelete.CommandText = _delete;
            _cmdDelete.CommandType = CommandType.Text;
            if ("merge".Equals(_type))
            {
                _cmdUpdate = _con.CreateCommand();
                _cmdUpdate.CommandText = _update;
                _cmdUpdate.CommandType = CommandType.Text;
            }
            //Insert
            for (int i = 0; i < _iparams.GetLength(0); i++)
            {
                SqlParameter _param = null;
                string type = _iparams[i, 1];
                if (Functions.GetTagType(TagType.Boolean).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.TinyInt);
                else if (Functions.GetTagType(TagType.Float).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.Float);
                else if (Functions.GetTagType(TagType.Integer).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.Int);
                else if (Functions.GetTagType(TagType.String).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.NVarChar);
                else if (Functions.GetTagType(TagType.DateTime).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.DateTime);
                else if (Functions.GetTagType(TagType.XML).Equals(type))
                    _param = _cmdInsert.Parameters.Add(_iparams[i, 0], SqlDbType.NVarChar);
            }
            //Delete
            for (int i = 0; i < _dparams.GetLength(0); i++)
            {
                SqlParameter _param = null;
                string type = _dparams[i, 1];
                if (Functions.GetTagType(TagType.Boolean).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.TinyInt);
                else if (Functions.GetTagType(TagType.Float).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.Float);
                else if (Functions.GetTagType(TagType.Integer).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.Int);
                else if (Functions.GetTagType(TagType.String).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.NVarChar);
                else if (Functions.GetTagType(TagType.DateTime).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.DateTime);
                else if (Functions.GetTagType(TagType.XML).Equals(type))
                    _param = _cmdDelete.Parameters.Add(_dparams[i, 0], SqlDbType.NVarChar);
            }

            //Update
            if ("merge".Equals(_type))
            {
                for (int i = 0; i < _uparams.GetLength(0); i++)
                {
                    SqlParameter _param = null;
                    string type = _uparams[i, 1];
                    if (Functions.GetTagType(TagType.Boolean).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.TinyInt);
                    else if (Functions.GetTagType(TagType.Float).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.Float);
                    else if (Functions.GetTagType(TagType.Integer).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.Int);
                    else if (Functions.GetTagType(TagType.String).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.NVarChar);
                    else if (Functions.GetTagType(TagType.DateTime).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.DateTime);
                    else if (Functions.GetTagType(TagType.XML).Equals(type))
                        _param = _cmdUpdate.Parameters.Add(_uparams[i, 0], SqlDbType.NVarChar);
                }
            }

            //создадим соединение с БД
            if (_keepalive && Connect())
            {
                _cmdInsert.Prepare();//подготовим запрос
                _cmdDelete.Prepare();//подготовим запрос
                if ("merge".Equals(_type))
                    _cmdUpdate.Prepare();//подготовим запрос
            }
        }

        private bool Connect()
        {
            //создадим соединение с БД
            if (_con.State != ConnectionState.Open)
            {
                _con.ConnectionString = parseConnectionString(_connectionString);
                _con.Open();
            }
            if (_con.State == ConnectionState.Open)
                return true;
            else
                return false;
        }

        private void Disconnect()
        {
            if (_con != null && _con.State == ConnectionState.Open)
            {
                if (_cmdInsert != null)
                    _cmdInsert.Cancel();
                if (_cmdUpdate != null)
                    _cmdUpdate.Cancel();
                if (_cmdDelete != null)
                    _cmdDelete.Cancel();
                _con.Close();
            }
        }

        public override void BeforeStop()
        {
            Disconnect();
            _cmdInsert = null;
            _cmdUpdate = null;
            _cmdDelete = null;
            _con = null;
        }

        private string parseConnectionString(string constr)
        {
            Regex r = new Regex("\\[\\S+\\]");
            string[] pars = constr.Split(';');
            foreach (string par in pars)
            {
                MatchCollection vars = r.Matches(par);
                foreach (Match var in vars)
                {
                    //Удалим первую и последнюю скобку
                    string tagname = var.Value.Substring(1, var.Value.Length - 2);
                    Tag tag = _server.GetTag(tagname.Trim());
                    string value = tag.Value.ToString();
                    constr = constr.Replace(var.Value, value);
                }
            }
            return constr;
        }
    }
}
