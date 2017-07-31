using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Matrix.Types;
using System.Messaging;

namespace Matrix.Server
{
    /// <summary>
    /// ��� � MSMQ
    /// </summary>
    public class LogMSMQ
    {
        /// <summary>
        /// ��� �������
        /// </summary>
        public string QueueName;
        public string Delimiter;
        private DateTime _unixtime = new DateTime(1970, 1, 1);


        /// <summary>
        /// �������
        /// </summary>
        private MessageQueue _queue=null;


        /// <summary>
        /// ���������� ���
        /// </summary>
        public string Name = "LogMSMQ";

        /// <summary>
        /// ������ ������ � ���
        /// </summary>
        internal void Write(Tag tag)
        {
            string _csv = "";
            _csv = "tag=" + tag.Name + Delimiter;
            _csv += "type=" + Functions.GetTagType(tag.Type) + Delimiter;
            _csv += "time=" + (int)(tag.DtChange.ToUniversalTime() - _unixtime).TotalSeconds + Delimiter;
            _csv += "cnt=" + tag.cnt + Delimiter;
            _csv += "step=" + tag.Step + Delimiter;
            _csv += "quality=" + tag.Quality + Delimiter;
            _csv += "value=";
            if (tag.Type == TagType.String)
                _csv += tag.Value + Delimiter;
            else if (tag.Type == TagType.DateTime)
                _csv += ((tag.Value != null && !"".Equals(tag.Value)) ? ((DateTime)tag.Value).ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) : "") + Delimiter;
            else
                _csv += tag.Value + Delimiter;

            Message msg = new Message(_csv,new ActiveXMessageFormatter());
            msg.Label = "MxTag";
            _queue.Send(msg);
        }


        /// <summary>
        /// ������� ������� ��� ������
        /// </summary>
        internal void Open()
        {
            _queue = new MessageQueue(QueueName);
        }

        /// <summary>
        /// ������� ���
        /// </summary>
        internal void Close()
        {
            _queue.Close();
        }

    }
}
