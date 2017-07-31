using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Matrix.Types
{
    public interface ITagStore
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Наименование
        /// </summary>
        String Name
        {
            get;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        void Init(XmlNode xmlconf);

        /// <summary>
        /// окрыть
        /// </summary>
        void Open();

        /// <summary>
        /// Закрыть
        /// </summary>
        void Close();

        /// <summary>
        /// Добавить запись
        /// </summary>
        void Write(Tag tag);

        /// <summary>
        /// Получить копию объекта
        /// </summary>        
        ITagStore Clone();
    }
}
