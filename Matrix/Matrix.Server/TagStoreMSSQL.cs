using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Matrix.Types;

namespace Matrix.Server
{
    /// <summary>
    /// Архивный MS SL сервер
    /// </summary>
    public class TagStoreMSSQL : ITagStore
    {

        private int _id;
        private string _name = "TagStoreMSSQL";
        private string _connectionString;
        private SqlConnection _conn = null;
        private DataTable _table = null;
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
        }

        public void Init(System.Xml.XmlNode xmlconf)
        {
            //инициализируем соединение
            _connectionString = xmlconf.Attributes["connectionString"].Value;
            try
            {
                _rowpertran = Convert.ToInt32(xmlconf.Attributes["rowpertran"].Value);
            }
            catch (Exception ex) 
            {
                _rowpertran = 200;
            }
        }
        
        public void Open()
        {
            _cnt = 0;
            _conn = new SqlConnection(_connectionString);
            _table = new DataTable("Trends");
            _conn.Open();

            DataColumn id = new DataColumn();
            id.DataType = typeof(int);
            id.ColumnName = "id";
            id.AutoIncrement = true;
            _table.Columns.Add(id);

            DataColumn dt = new DataColumn();
            dt.DataType = typeof(DateTime);
            dt.ColumnName = "dt";
            _table.Columns.Add(dt);

            DataColumn tagname = new DataColumn();
            tagname.DataType = typeof(string);
            tagname.ColumnName = "tagname";
            _table.Columns.Add(tagname);

            DataColumn tagtype = new DataColumn();
            tagtype.DataType = typeof(string);
            tagtype.ColumnName = "tagtype";
            _table.Columns.Add(tagtype);

            DataColumn tagvalue = new DataColumn();
            tagvalue.DataType = typeof(string);
            tagvalue.ColumnName = "tagvalue";
            _table.Columns.Add(tagvalue);

            DataColumn count = new DataColumn();
            count.DataType = typeof(int);
            count.ColumnName = "cnt";
            _table.Columns.Add(count);

            DataColumn step = new DataColumn();
            step.DataType = typeof(int);
            step.ColumnName = "step";
            _table.Columns.Add(step);

            DataColumn quality = new DataColumn();
            quality.DataType = typeof(int);
            quality.ColumnName = "quality";
            _table.Columns.Add(quality);

            // Create an array for DataColumn objects.
            //DataColumn[] keys = new DataColumn[1];
            //keys[0] = productID;
            //newProducts.PrimaryKey = keys;
        }

        public void Close()
        {
            FlushToDB();
            _conn.Close();
        }

        public void Write(Tag tag)
        {
            DataRow row     = _table.NewRow();
            row["dt"]       = tag.DtChange;
            row["tagname"]  = tag.Name;
            row["tagtype"]  = Functions.GetTagType(tag.Type);
            row["tagvalue"] = tag.Value;
            row["cnt"]      = tag.cnt;
            row["step"]     = tag.Step;
            row["quality"] = tag.Quality;
            _table.Rows.Add(row);
            _table.AcceptChanges();

            _cnt++;
            //Выполним сохранение транзакции через каждые _rowpertran записей
            if (_cnt > _rowpertran)
            {
                FlushToDB();
            }

        }
        
        //Сбросить быстрой вставкой буфер на сервер
        private void FlushToDB()
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_conn))
            {
                bulkCopy.BatchSize = _rowpertran;
                bulkCopy.DestinationTableName = "dbo.Trends";
                try
                {
                    // Write the array of rows to the destination.
                    bulkCopy.WriteToServer(_table);
                    _table.Rows.Clear();
                    _table.AcceptChanges();
                    _cnt = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public ITagStore Clone()
        {
            return (ITagStore)this.MemberwiseClone();
        }

    }
}
