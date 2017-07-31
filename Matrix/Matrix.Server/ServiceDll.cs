using Matrix.Types;
using System.Reflection;
using System.Xml;
using System;
using System.Collections;
using Common.Logging;

namespace Matrix.Server
{
    public class ServiceDll : Service
    {
        private static ILog log = LogManager.GetLogger<ServiceDll>();

        string _method;
        private string[,] _params;
        private string[,] _maps;
        object obj;
        MethodInfo _methodinfo;
        ParameterInfo[] _parinfo;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);
            _method = xmlconf.Attributes["method"].Value;
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
            Hashtable _ht = new Hashtable(_parinfo.GetLength(0));
            
            //получить набор параметров и сохранить в hashtable
            for (int i = 0; i < _params.GetLength(0); i++)
            {
                if (_params[i, 4].StartsWith("in"))
                {
                    if (_params[i, 2] == "tag")
                    {
                        Tag tag = _server.GetTag(_params[i, 3]);
                        _ht.Add(_params[i, 0], tag.Value);
                    }
                    else
                    {
                        switch(_params[i,1]){
                            case "int":
                                _ht.Add(_params[i,0], Convert.ToInt32(_params[i, 3]));
                                break;
                            case "float":
                                _ht.Add(_params[i, 0], Convert.ToSingle(_params[i, 3]));
                                break;
                            case "bool":
                                _ht.Add(_params[i, 0], Convert.ToBoolean(_params[i, 3]));
                                break;
                            default:
                                _ht.Add(_params[i,0], Convert.ToString(_params[i, 3]));
                                break;
                        }
                    }
                }
            }

            object[] _parvals = new object[_parinfo.GetLength(0)];
            for (int i = 0; i < _parinfo.GetLength(0); i++ )
            {
                _parvals[i] = _ht[_parinfo[i].Name];
            }
            
            //Вызов метода с параметрами
            string logbuff = (string)_methodinfo.Invoke(obj, _parvals);
            if(logbuff!=null && !"".Equals(logbuff))
                log.Info("["+this._name+"] " + logbuff);
            //Обработаем выходные параметры, если есть
            for (int i = 0; i < _params.GetLength(0); i++)
            {
                if (_params[i, 2] == "tag" && _params[i, 4].EndsWith("out")) 
                {
                    //Не писать тег если он не изменился
                    if (_params[i, 4].StartsWith("in") && _ht[_params[i, 0]].Equals(_parvals[i]))
                        continue;
                    
                    Tag tag = _server.GetTag(_params[i, 3]);
                    tag.Value = _parvals[i];
                    _server.SetTag(tag);
                    
                }
            }
        }

        public override void AfterStart()
        {
            //Загрузим библиотеку
            Assembly asm = Assembly.LoadFrom(_assembly);
            obj = asm.CreateInstance(_type, true);
            _methodinfo = obj.GetType().GetMethod(_method);
            _parinfo = _methodinfo.GetParameters();
        }

    }
}
