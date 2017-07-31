using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Threading;
using System.Runtime.CompilerServices;
using Matrix.Types;

namespace Matrix.Server
{
    /// <summary>
    /// Архивный Oracle
    /// </summary>
    public class TagStoreOracle : ITagStore
    {
        #region ITagStore Members
        private int _id;
        private string _name;
        private string _connectionString;
        public OracleConnection _conn;
        private OracleCommand _cmd;
        private OracleTransaction _transaction;
        private int _cnt = 0;
        private int _rowpertran = 0;

        
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public void Init(System.Xml.XmlNode xmlconf)
        {
            //инициализируем соединение
            _connectionString = xmlconf.Attributes["connectionString"].Value;
            _rowpertran = Convert.ToInt32(xmlconf.Attributes["rowpertran"].Value);
        }

        public void Open()
        {
            _cnt = 0;
            _conn = new OracleConnection(_connectionString);
            //_conn.ConnectionString = _connectionString;
            _cmd = _conn.CreateCommand();
            _cmd.CommandType = CommandType.Text;
            _cmd.CommandText = "insert into trends(dt,tagname,tagtype,tagvalue, quality) values(:dt,:tagname,:tagtype,:tagvalue,:quality)";
            _cmd.Parameters.Add("dt", OracleType.DateTime);
            _cmd.Parameters.Add("tagname", OracleType.VarChar,50);
            _cmd.Parameters.Add("tagtype", OracleType.VarChar,50);
            _cmd.Parameters.Add("tagvalue", OracleType.VarChar,50);
            _cmd.Parameters.Add("quality", OracleType.Int32);
            _conn.Open();
            _cmd.Prepare();
            _transaction = _conn.BeginTransaction();
            _cmd.Transaction = _transaction;
        }

        public void Close()
        {
            _transaction.Commit();
            _conn.Close();
        }

        public void Write(Tag tag)
        {
            _cmd.Parameters["dt"].Value         = tag.Timestamp;
            _cmd.Parameters["tagname"].Value    = tag.Name;
            _cmd.Parameters["tagtype"].Value    = Functions.GetTagType(tag.Type);
            _cmd.Parameters["tagvalue"].Value   = tag.Value;
            _cmd.Parameters["quality"].Value    = tag.Quality;
            _cmd.ExecuteNonQuery();
            _cnt++;
            //Выполним сохранение транзакции через каждые 100 записей
            if (_cnt > _rowpertran)
            {
                _cnt = 0;
                _transaction.Commit();
                _transaction = _conn.BeginTransaction();
                _cmd.Transaction = _transaction;
            }
        }        

        public ITagStore Clone()
        {
            return (ITagStore)this.MemberwiseClone();
        }

#endregion
    }
}
