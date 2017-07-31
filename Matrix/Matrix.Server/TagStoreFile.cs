 using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Matrix.Types;

namespace Matrix.Server
{
    /// <summary>
    /// Архивый файл
    /// </summary>
    public class TagStoreFile : ITagStore
    {
        private int _id;
        private string _name = "TagStoreFile";
        private string _filename;
        private string _delimiter;
        private StreamWriter _sw;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void Init(XmlNode xmlconf)
        {
            _filename = xmlconf.Attributes["filename"].Value;
            _delimiter = xmlconf.Attributes["delimiter"].Value;
        }

        public void Open()
        {
            _sw = new StreamWriter(_filename,true);
        }

        public void Close()
        {
            _sw.Close();
        }

        public void Write(Tag tag)
        {
            _sw.WriteLine("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}", _delimiter, tag.DtChange.ToString("dd.MM.yy HH:mm:ss.fff"), tag.Name, Functions.GetTagType(tag.Type), tag.Value, tag.OldValue, tag.cnt, tag.Step, tag.Quality);
            _sw.Flush();
        }

        public ITagStore Clone()
        {
            return (ITagStore)this.MemberwiseClone();
        }    
    }
}
