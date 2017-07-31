using System;
using System.Collections.Generic;
using System.Text;
using Matrix.Types;



namespace Matrix.Server
{
    public struct LogItem
    {
        /// <summary>
        /// ���� � ����� ���������� ����
        /// </summary>
        public DateTime dt;
        /// <summary>
        /// ��� ����
        /// </summary>
        public string tagname;
        /// <summary>
        /// �������� ����
        /// </summary>
        public object tagvalue;
        /// <summary>
        /// ��� ��������
        /// </summary>
        public TagType tagtype;


    }
}
