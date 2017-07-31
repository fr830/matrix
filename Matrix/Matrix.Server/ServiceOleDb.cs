using System;
using System.Xml;
using System.Data;
using System.Data.OleDb;
using Matrix.Types;
using System.Text.RegularExpressions;
using Common.Logging;

namespace Matrix.Services
{
    public class ServiceOleDb : Service
    {
        private static ILog log = LogManager.GetLogger<ServiceOleDb>();

        private string _connectionString;

        private string _query;

        private OleDbConnection _con;
        private OleDbCommand _cmd;

        private string[,] _params;
        private string[,] _maps;
        
        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            _connectionString = xmlconf.Attributes["connectionString"].Value;
            XmlNode querynode = xmlconf.SelectSingleNode("query");
            _query = querynode.InnerText;
            _type = xmlconf.Attributes["type"].Value;
            XmlNodeList nodes = xmlconf.SelectNodes("param");
            _params = new string[nodes.Count, 5];
            int i = 0;
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
                throw new Exception("Ошибка подключения! ");
            //получить набор параметров и сохранить в параметрах
            for (int i = 0; i < _params.GetLength(0); i++)
            {
                if (_params[i, 4].StartsWith("in"))
                {
                    if (_params[i, 2] == "tag")
                    {
                        Tag tag = _server.GetTag(_params[i, 3]);
                        _cmd.Parameters[_params[i, 0]].Value = tag.Value;
                        //_cmd.Parameters[_params[i, 0]].Value = ((DateTime)tag.Value).AddSeconds(1);
                    }
                    else
                    {
                        _cmd.Parameters[_params[i, 0]].Value = _params[i, 3];
                    }
                }
            }

            if (_type == "update" || _type == "call")
            {
                _cmd.ExecuteNonQuery();
                //Обработаем выходные параметры, если есть
                for (int i = 0; i < _params.GetLength(0); i++)
                {
                    if (_params[i, 2] == "tag" && _params[i, 4].EndsWith("out"))
                    {
                        Tag tag = _server.GetTag(_params[i, 3]);
                        tag.Value = _cmd.Parameters[_params[i, 0]].Value;
                        _server.SetTag(tag);
                    } 
                }
            }
            else
            {
                OleDbDataReader dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
                try
                {
                    if (_type == "scalar")
                    {
                        while (dr.Read())
                        {
                            for (int i = 0; i < _maps.GetLength(0); i++)
                            {
                                Tag tag = _server.GetTag(_maps[i, 0]);
                                if (dr[_maps[i, 1]] == DBNull.Value)
                                    tag.Value = null;
                                else
                                    if (tag.Type == TagType.DateTime)
                                    {
                                        tag.Value = dr[_maps[i, 1]];
                                        log.Debug(((DateTime)dr[_maps[i, 1]]).ToString("dd.MM.yy HH:mm:ss.fff"));
                                    }
                                    else
                                        tag.Value = dr[_maps[i, 1]];
                                _server.SetTag(tag);
                            }
                            break;//только первую строку взять
                        }
                    }
                    else
                    {
                        string xml = "<rows>\n";
                        while (dr.Read())
                        {
                            xml += "<row>";
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                Type t = dr.GetFieldType(i);
                                if (typeof(string) == t)
                                    xml += "<" + dr.GetName(i) + "><![CDATA[" + dr.GetValue(i) + "]]></" + dr.GetName(i) + ">\n";
                                else
                                    xml += "<" + dr.GetName(i) + ">" + dr.GetValue(i) + "</" + dr.GetName(i) + ">\n";
                            }
                            xml += "</row>";
                        }
                        xml += "</rows>";
                        Tag tag = _server.GetTag(_maps[0, 0]);
                        tag.Value = xml;
                        _server.SetTag(tag);
                    }
                }
                finally
                {
                    dr.Close();
                }
            }
            //Отключим соединение если не требуется хранить его
            if(!_keepalive)
                Disconnect();
        }

        public override void AfterStart()
        {
            _con = new OleDbConnection(_connectionString);
            _cmd = _con.CreateCommand();
            _cmd.CommandText = _query;
           
            if (_type == "call")
                _cmd.CommandType = CommandType.StoredProcedure;
            else
                _cmd.CommandType = CommandType.Text;

            for (int i = 0; i < _params.GetLength(0); i++)
            {
                OleDbParameter _param = null;
                string type = _params[i, 1];
                if (Functions.GetTagType(TagType.Boolean).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.Boolean);
                else if (Functions.GetTagType(TagType.Float).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.Single);
                else if (Functions.GetTagType(TagType.Integer).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.Integer);
                else if (Functions.GetTagType(TagType.String).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.VarWChar,4000);
                else if (Functions.GetTagType(TagType.DateTime).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.DBTimeStamp);
                else if (Functions.GetTagType(TagType.XML).Equals(type))
                    _param = _cmd.Parameters.Add(_params[i, 0], OleDbType.VarChar,4000);
                if (_type == "call")
                {
                    if (_params[i, 4].Equals("inout"))
                    {
                        _param.Direction = ParameterDirection.InputOutput;
                        if (_param.OleDbType == OleDbType.VarChar)
                            _param.Size = 4000;
                    }
                    else if (_params[i, 4].Equals("out"))
                    {
                        _param.Direction = ParameterDirection.Output;
                        if (_param.OleDbType == OleDbType.VarChar)
                           _param.Size = 4000;
                    }
                    else
                            _param.Direction = ParameterDirection.Input;
                }
            }
            //создадим соединение с БД
            if(_keepalive && Connect())
                _cmd.Prepare();//подготовим запрос
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
            if (_con!=null && _con.State == ConnectionState.Open)
            {
                _cmd.Cancel();
                _con.Close();
            }
        }

        public override void BeforeStop()
        {
            Disconnect();
            _cmd = null;
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
