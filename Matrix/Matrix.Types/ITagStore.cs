using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Matrix.Types
{
    public interface ITagStore
    {
        /// <summary>
        /// ���������� �������������
        /// </summary>
        int Id
        {
            get;
            set;
        }

        /// <summary>
        /// ������������
        /// </summary>
        String Name
        {
            get;
        }

        /// <summary>
        /// �������������
        /// </summary>
        void Init(XmlNode xmlconf);

        /// <summary>
        /// ������
        /// </summary>
        void Open();

        /// <summary>
        /// �������
        /// </summary>
        void Close();

        /// <summary>
        /// �������� ������
        /// </summary>
        void Write(Tag tag);

        /// <summary>
        /// �������� ����� �������
        /// </summary>        
        ITagStore Clone();
    }
}
