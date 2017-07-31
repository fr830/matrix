using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Matrix.Types;

namespace Matrix.Server
{
    /// <summary>
    /// Лог файл
    /// </summary>
    public class LogFile
    {
        /// <summary>
        /// физическое имя файла
        /// </summary>
        private string _filename;
        /// <summary>
        /// признак блокировки файла
        /// </summary>
        private bool _islock;

        /// <summary>
        /// Хендлер файла
        /// </summary>
        private FileStream _fs;

        /// <summary>
        /// Бинарный поток на запись
        /// </summary>
        private BinaryWriter _bw = null;

        /// <summary>
        /// Бинарный поток на чтение
        /// </summary>
        private BinaryReader _br = null;

        /// <summary>
        /// признак блокировки файла
        /// </summary>
        internal bool Islock
        {
            get { return _islock; }
            set { _islock = value; }
        }
        /// <summary>
        /// логическое имя
        /// </summary>
        private string _logname;

        /// <summary>
        /// логическое имя
        /// </summary>
        internal string Logname
        {
            get { return _logname; }
            set { _logname = value; }
        }
        /// <summary>
        /// размер файла
        /// </summary>
        private int _filesize;

        /// <summary>
        /// размер файла
        /// </summary>
        internal int Filesize
        {
            get { return _filesize; }
            set { _filesize = value; }
        }

        /// <summary>
        /// физическое имя файла
        /// </summary>
        internal string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        /// <summary>
        /// Сброс буфера данных в файл
        /// </summary>
        internal void Flush()
        {
            if(_bw != null) _bw.Flush();
            _fs.Flush();
        }

        /// <summary>
        /// Запись данных в лог
        /// </summary>
        internal void Write(Tag tag)
        {
            _bw.Write(_tick);//сохраним указатель транзакции
            _bw.Write(tag.Timestamp.ToBinary());
            _bw.Write(tag.DtChange.ToBinary());
            _bw.Write((bool)tag.IsChange);//признак изменения
            _bw.Write(tag.cnt);//Число изменений
            _bw.Write(tag.Name);//Имя
            _bw.Write((byte)tag.Type);//Тип
            switch (tag.Type)
            {
                case TagType.Boolean:
                    _bw.Write(Convert.ToBoolean(tag.Value));//Текущее значение
                    _bw.Write(Convert.ToBoolean(tag.OldValue));//Предыдущее значение
                    break;
                case TagType.Byte:
                    _bw.Write(Convert.ToByte(tag.Value));//Текущее значение
                    _bw.Write(Convert.ToByte(tag.OldValue));//Предыдущее значение
                    break;
                case TagType.Integer:
                    _bw.Write(Convert.ToInt32(tag.Value));
                    _bw.Write(Convert.ToInt32(tag.OldValue));
                    break;
                case TagType.DateTime:
                    _bw.Write(((DateTime)tag.Value).ToBinary());
                    _bw.Write(((DateTime)tag.OldValue).ToBinary());
                    break;
                case TagType.Float:
                    _bw.Write(Convert.ToSingle(tag.Value));
                    _bw.Write(Convert.ToSingle(tag.OldValue));
                    break;
                case TagType.String:
                case TagType.XML:
                    _bw.Write(Convert.ToString(tag.Value));
                    _bw.Write(Convert.ToString(tag.OldValue));
                    break;
            }
            _bw.Flush();
        }

        /// <summary>
        /// Сброс состояния в ноль
        /// </summary>
        internal void InitLog()
        {
            File.WriteAllBytes(_filename, new byte[_filesize]);
        }

        /// <summary>
        /// Получить следующую запись
        /// </summary>
        internal Tag Read()
        {
            try
            {
                if (_tick != _br.ReadInt64())
                    return null; //указатели не сопадают значит файл был записан не доконца
                Tag _tag = new Tag();
                DateTime ts = DateTime.FromBinary(_br.ReadInt64());
                DateTime dc = DateTime.FromBinary(_br.ReadInt64());
                bool ich = _br.ReadBoolean();
                int cnt = _br.ReadInt32();
                string tgname = _br.ReadString();
                TagType tt = (TagType)_br.ReadByte();
                object val=null, oldval=null;

                switch (tt)
                {
                    case TagType.Boolean:
                        val = _br.ReadBoolean();
                        oldval = _br.ReadBoolean();
                        break;
                    case TagType.Byte:
                        val = _br.ReadByte();
                        oldval = _br.ReadByte();
                        break;
                    case TagType.Integer:
                        val = _br.ReadInt32();
                        oldval = _br.ReadInt32();
                        break;
                    case TagType.DateTime:
                        val = DateTime.FromBinary(_br.ReadInt64());
                        oldval = DateTime.FromBinary(_br.ReadInt64());
                        break;
                    case TagType.Float:
                        val = _br.ReadSingle();
                        oldval = _br.ReadSingle();
                        break;
                    case TagType.String:
                    case TagType.XML:
                        val = _br.ReadString();
                        oldval = _br.ReadString();
                        break;
                }

                _tag.Name = tgname;
                _tag.Type = tt;
                _tag.Value = val;
                _tag.OldValue = oldval;
                _tag.Timestamp = ts;
                _tag.DtChange = dc;
                _tag.IsChange = ich;
                _tag.cnt = cnt;
                return _tag;
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        /// <summary>
        /// Открыть для записи
        /// </summary>
        internal void Open(FileAccess fa)
        {
            if (!File.Exists(_filename)) InitLog();
            if (fa == FileAccess.Write)
            {
                _fs = new FileStream(_filename, FileMode.Open, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough);
                _tick = DateTime.Now.Ticks;//Указатель транзакции
                _bw = new BinaryWriter(_fs);
                _bw.Write(_tick);//Сохраним указатель в начале файла
            }
            else
            {
                _fs = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.None);
                _br = new BinaryReader(_fs);
                _tick = _br.ReadInt64();//получим указатель транзакции
            }
            //получим тик когда открыт файл

        }

        /// <summary>
        /// Закрыть лог
        /// </summary>
        internal void Close()
        {
            if (_bw != null)
                _bw.Flush();
            else
                _br.Close();
            _fs.Close();
            _bw = null;
            _br = null;
        }

        /// <summary>
        /// Получить время последней модификации
        /// </summary>
        internal DateTime GetLastWriteTime()
        {
            return File.GetLastWriteTime(_filename);
        }

        /// <summary>
        /// получить размер записанного буфера
        /// </summary>
        internal long GetWriteSize()
        {
            return _bw.BaseStream.Position;
        }

        /// <summary>
        /// идентификатор лога
        /// </summary>
        private int _logid;

        /// <summary>
        /// Порядковый номер лога
        /// </summary>
        internal int Logid
        {
            get { return _logid; }
            set { _logid = value; }
        }

        /// <summary>
        /// Текущая запись из лога при чтении
        /// </summary>
        //private Tag _tag = new Tag();

        ///// <summary>
        ///// Текушая запись из лога
        ///// </summary>
        //internal Tag Tag
        //{
        //    get { return _tag; }
        //    set { _tag = value; }
        //}

        /// <summary>
        /// указатель транзакции
        /// </summary>
        private long _tick;


    }
}
