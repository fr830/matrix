using System;
using System.Collections.Generic;
using System.Text;
using Matrix.Types;



namespace Matrix.Server
{
    public struct LogItem
    {
        /// <summary>
        /// Дата и время измененеия Тега
        /// </summary>
        public DateTime dt;
        /// <summary>
        /// Имя тега
        /// </summary>
        public string tagname;
        /// <summary>
        /// значение тега
        /// </summary>
        public object tagvalue;
        /// <summary>
        /// Тип значения
        /// </summary>
        public TagType tagtype;


    }
}
