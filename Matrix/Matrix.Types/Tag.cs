using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Matrix.Types
{
    [SerializableAttribute]
    public class Tag
    {
        /// <summary>
        /// ���
        /// </summary>
        public string Name;

        /// <summary>
        /// ����� �����������
        /// </summary>
        public int LogMode=1;
        
        
        /// <summary>
        /// ���
        /// </summary>
        public TagType Type;

        public string Default;
        /// <summary>
        /// ������� ���������
        /// </summary>
        private object _value;

        public string Description;

        public object Value{
            get { return _value; }
            set {
                DateTime now = DateTime.Now;
                try
                {
                    switch (this.Type)
                    {
                        case TagType.Boolean:
                            value = Convert.ToBoolean(value);
                            break;
                        case TagType.Byte:
                            value = Convert.ToByte(value);
                            break;
                        case TagType.Integer:
                            value = Convert.ToInt32(value);
                            break;
                        case TagType.Float:
                            value = Convert.ToSingle(value);
                            break;
                        case TagType.String:
                            value = Convert.ToString(value);
                            break;
                        case TagType.DateTime:
                            if(!(value is DateTime))
                                value = Convert.ToDateTime(value);
                            break;
                        case TagType.XML:
                            value = Convert.ToString(value);
                            break;
                    }

                    //���� ���������� ����� ������ ������ ��
                    if (this.Delta > 0 && 
                        value != null &&
                        this.Value != null &&
                        (((this.Type == TagType.Byte || this.Type == TagType.Integer || this.Type == TagType.Float) &&
                        Math.Abs(Convert.ToSingle(this.Value) - Convert.ToSingle(value)) < this.Delta )||
                        ((this.Type == TagType.Boolean || this.Type == TagType.DateTime || this.Type == TagType.Object || this.Type == TagType.String || this.Type == TagType.XML) &&
                        this.Value.Equals(value))))
                    {

                        this.IsChange = false;
                        //��������� ����� ���������� ���������� �������� ��������=1���
                        this.cnt = (int)(DateTime.Now - this.DtChange).TotalSeconds;                                
                        //�������� �������
                        //this.cnt++;
                        //��������� ��� ������������� ��� ����� + cnt ���������� ��������
                        //����� ������� ���� ��������� ����� �� ���� �������������, �� ��� ����� �������������� ����������
                        //this.Step = (int)((now - this.DtChange).TotalMilliseconds / (this.cnt+1));                                
                    }
                    else
                    {
                        //������� ��������
                        this.IsChange = true;
                        this.cnt = 0;//������� ������� ���������� ��������
                        //this.Step = 0;
                        if (this._oldvalue == null)
                            this._oldvalue = value;
                        else
                            this._oldvalue = this.Value;
                        this._value = value;
                        this.DtChange = now;
                    }
                    this.Quality = QualityType.GOOD;
                }
                catch 
                {
                    this.Quality = QualityType.BAD;
                }
                this.Timestamp = now;
            }
        }
        /// <summary>
        /// ���������� ���������
        /// </summary>
        private object _oldvalue;

        public object OldValue
        {
            get { return _oldvalue;  }
            set
            {
                try
                {
                    switch (this.Type)
                    {
                        case TagType.Boolean:
                            value = Convert.ToBoolean(value);
                            break;
                        case TagType.Byte:
                            value = Convert.ToByte(value);
                            break;
                        case TagType.Integer:
                            value = Convert.ToInt32(value);
                            break;
                        case TagType.Float:
                            value = Convert.ToSingle(value);
                            break;
                        case TagType.String:
                            value = Convert.ToString(value);
                            break;
                        case TagType.DateTime:
                            value = Convert.ToDateTime(value);
                            break;
                        case TagType.XML:
                            value = Convert.ToString(value);
                            break;
                    }
                    _oldvalue = value;
                }
                catch { }
            }
        }
        /// <summary>
        /// ���� ���������� ����������
        /// </summary>
        public DateTime Timestamp;
        /// <summary>
        /// �������� ���������� ��� ������� ��������� ��� ��� ��������� (��� ������ ������)
        /// </summary>
        public float Delta = 0f;
        /// <summary>
        /// ��� ������������� ��� ������ ������ ���, ����� ���� ����� ������������ � ���
        /// </summary>
        public int Step = 300;
        /// <summary>
        /// ��� �������
        /// </summary>
        public bool IsChange;
        /// <summary>
        /// ���� ���������� ���������
        /// </summary>
        public DateTime DtChange;
        /// <summary>
        /// ������� ���� ���������� �������� � ������� ���������� ���������
        /// </summary>
        public int cnt;
        ///// <summary>
        ///// ����� ������� ������ ���������� � ��� ���� �������� �� ��������
        ///// ������������ ��������� � delta ��� ������ 
        ///// 300 ��� �� ���������
        ///// </summary>
        //public int LogInterval=300;
        /// <summary>
        /// ������������� �������� ��� �������� �����
        /// </summary>
        public string Service;
        /// <summary>
        /// ��� �������� ����
        /// </summary>
        public QualityType Quality;
        /// <summary>
        /// ��� ������ 
        /// </summary>
        public int ErrorCode;
        /// <summary>
        /// ������ 
        /// </summary>
        public string ErrorMessage;

        private Tag Clone()
        {
            Tag tag = new Tag();
            tag.Name = this.Name;
            tag.Description = this.Description;
            tag._oldvalue = this.OldValue;
            tag.Quality = this.Quality;
            tag.Service = this.Service;
            tag.Timestamp = this.Timestamp;
            tag.Type = this.Type;
            tag._value = this.Value;
            tag.cnt = this.cnt;
            tag.Delta = this.Delta;
            tag.Step = this.Step;
            tag.DtChange = this.DtChange;
            tag.ErrorCode = this.ErrorCode;
            tag.ErrorMessage = this.ErrorMessage;
            tag.IsChange = this.IsChange;
            tag.Default = this.Default;
            tag.LogMode = this.LogMode;
            return tag;
        }

        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//���������������� �����
        public Tag getOrUpdate(Tag tag)
        {
            if (tag != null)
            {
                //�������� ����� ��������
                this.Service = tag.Service;
                this._oldvalue = tag.OldValue;
                this.Quality = tag.Quality;
                this.Timestamp = tag.Timestamp;
                this._value = tag.Value;
                this.cnt = tag.cnt;
                this.Delta = tag.Delta;
                this.Step = tag.Step;
                this.DtChange = tag.DtChange;
                this.ErrorCode = tag.ErrorCode;
                this.ErrorMessage = tag.ErrorMessage;
                this.IsChange = tag.IsChange;
                this.Default = tag.Default;
                this.LogMode = tag.LogMode;
            }
            return Clone();
        }
        
        public string getXml()
        {
            string xml = "";
            xml = "<" + this.Name;
            xml += " ischange=\"" + Convert.ToString(this.IsChange) + "\"";
            xml += " dtchange=\"" + this.DtChange.ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) + "\"";
            xml += " cnt=\"" + this.cnt + "\"";
            xml += " timestamp=\"" + this.Timestamp.ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) + "\"";
            xml += " errorcode=\"" + this.ErrorCode + "\"";
            xml += " quality=\"" + this.Quality + "\"";
            xml += " description=\"" + this.Description + "\">";
            xml += "<default><![CDATA[" + this.Default + "]]></default>";
            xml += "<value>";
            //if (this.Type == TagType.String || this.Type == TagType.XML)
            if (this.Type == TagType.String)
                xml += "<![CDATA["+this._value+"]]>";
            else if (this.Type == TagType.DateTime)
                xml += (this._value != null && !"".Equals(this._value)) ? ((DateTime)this._value).ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) : "";
            else
                xml += this._value;
            xml += "</value>";
            xml += "<oldvalue>";
            //if (this.Type == TagType.String || this.Type == TagType.XML)
            if (this.Type == TagType.String)
                xml += "<![CDATA[" + this._oldvalue + "]]>";
            else if (this.Type == TagType.DateTime)
                xml +=  (this._oldvalue!=null && !"".Equals(this._oldvalue))?((DateTime)this._oldvalue).ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT) : "";
            else
                xml += this._oldvalue;
            xml += "</oldvalue>";
            xml += "<errormessage><![CDATA[" + this.ErrorMessage + "]]></errormessage>";
            xml += "</" + this.Name + ">";
            return xml;
        }
        
        public void setXml(XmlNode node)
        {
            foreach(XmlNode nd in node.ChildNodes){
                switch (nd.Name)
                {
                    case "value":
                        if (this.Type == TagType.XML)
                            this._value = nd.InnerXml;
                        else if (this.Type == TagType.DateTime && !"".Equals(nd.InnerText))
                            this._value = Convert.ToDateTime(nd.InnerText);
                        else if (this.Type == TagType.Integer)
                            this._value = Convert.ToInt32(nd.InnerText);
                        else if (this.Type == TagType.Float)
                            this._value = Convert.ToSingle(nd.InnerText);
                        else if (this.Type == TagType.Boolean)
                            this._value = Convert.ToBoolean(nd.InnerText);
                        else if (this.Type == TagType.Byte)
                            this._value = Convert.ToByte(nd.InnerText);
                        else
                            this._value = nd.InnerText;
                        break;
                    case "oldvalue":
                        if (this.Type == TagType.XML)
                            this._oldvalue = nd.InnerXml;
                        else if (this.Type == TagType.DateTime && !"".Equals(nd.InnerText))
                            this._oldvalue = Convert.ToDateTime(nd.InnerText);
                        else if (this.Type == TagType.Integer)
                            this._oldvalue = Convert.ToInt32(nd.InnerText);
                        else if (this.Type == TagType.Float)
                            this._oldvalue = Convert.ToSingle(nd.InnerText);
                        else if (this.Type == TagType.Boolean)
                            this._oldvalue = Convert.ToBoolean(nd.InnerText);
                        else if (this.Type == TagType.Byte)
                            this._oldvalue = Convert.ToByte(nd.InnerText);
                        else
                            this._oldvalue = nd.InnerText;
                        break;
                    case "errormessage":
                        this.ErrorMessage = nd.InnerText;
                        break;
                    case "default":
                        this.Default = nd.InnerText;
                        break;

                }
            }
            this.IsChange = Convert.ToBoolean(node.Attributes["ischange"].Value);
            this.DtChange = Convert.ToDateTime(node.Attributes["dtchange"].Value);
            this.cnt = Convert.ToInt32(node.Attributes["cnt"].Value);
            this.Timestamp = Convert.ToDateTime(node.Attributes["timestamp"].Value);
            this.ErrorCode = Convert.ToInt32(node.Attributes["errorcode"].Value);            
        }
       
    }
}
