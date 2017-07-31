using System;
using System.Xml;
using Matrix.Types;

namespace Matrix.Services
{
    public class ServiceAlert : Service
    {

        private string[,] _maps;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            XmlNodeList nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 3];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i, 0] = node.Attributes["name"].Value;
                try
                {
                    _maps[i, 1] = node.Attributes["type"].Value;
                }
                catch (Exception e) { }
                try
                {
                    _maps[i, 2] = node.Attributes["value"].Value;
                }
                catch (Exception e) { }
                i++;
            }
        }


        public override void RunOnce()
        {
            //получить набор параметров и сохранить в параметрах
            Tag tag = null;

            for (int i = 0; i < _maps.GetLength(0); i++)
            {

                tag = _server.GetTag(_maps[i, 0]);
                
                if (_maps[i, 1] == "tag")
                    tag.getOrUpdate(_server.GetTag(_maps[i, 2]));
                else
                    tag.Value = _maps[i, 2];
                _server.SetTag(tag);
            }

        }

        public override void AfterStart()
        {
        }

        public override void BeforeStop()
        {
        }
    }
}
