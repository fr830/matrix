using System;
using System.Xml;
using Matrix.Types;

namespace Matrix.Services
{
    public class ServiceSnapshot : Service
    {

        private string[] _tags;
        private string[] _maps;
        
        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            XmlNodeList nodes = xmlconf.SelectNodes("snapshot/map");
            _tags = new string[nodes.Count];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _tags[i] = node.Attributes["name"].Value;
                i++;
            }
            nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i] = node.Attributes["name"].Value;
                i++;
            }            
        }


        public override void RunOnce()
        {
            //получить набор параметров и сохранить в параметрах
            Tag tag = _server.GetTag(_maps[0]);
            string xml = "";
            for (int i = 0; i < _tags.GetLength(0); i++)
            {
                Tag tg = _server.GetTag(_tags[i]);
                if (tg.Type == TagType.String)
                    xml += "<" + tg.Name + " description=\"" + tg.Description + "\"" + " dtchange=\"" + tg.DtChange + "\" timestamp=\"" + tg.Timestamp + "\"" + " quality=\"" + tg.Quality + "\"><![CDATA[" + tg.Value + "]]></" + tg.Name + ">";
                else
                    xml += "<" + tg.Name + " description=\"" + tg.Description + "\"" + " dtchange=\"" + tg.DtChange + "\" timestamp=\"" + tg.Timestamp + "\"" + " quality=\"" + tag.Quality + "\">" + tg.Value + "</" + tg.Name + ">";
            }
            tag.Timestamp = DateTime.Now;
            tag.Value = xml;
            _server.SetTag(tag);
        }

    }
}
