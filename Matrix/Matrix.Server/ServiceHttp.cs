using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using Matrix.Types;


namespace Matrix.Services
{
    public class ServiceHttp : Service
    {
        private string _url;

        private WebClient _client = null;
        private HttpWebRequest _myreq = null;
        private string _method = "GET";
        private string _downloadfile = null;
        private string _uploadfile = null;
        private string[,] _params;
        private string[,] _maps;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            _url = xmlconf.Attributes["url"].Value;

            try{
                _method = xmlconf.Attributes["method"].Value;
            }catch(Exception e){}

            try
            {
                _downloadfile = xmlconf.Attributes["downloadfile"].Value;
            }
            catch (Exception e) { }

            try
            {
                _uploadfile = xmlconf.Attributes["uploadfile"].Value;
            }
            catch (Exception e) { }

            XmlNodeList nodes = xmlconf.SelectNodes("param");
            _params = new string[nodes.Count, 5];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _params[i, 0] = node.Attributes["name"].Value;
                _params[i, 1] = node.Attributes["type"].Value;
                _params[i, 2] = node.Attributes["typeval"].Value;
                _params[i, 3] = node.Attributes["value"].Value;
                i++;
            }
            nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 2];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i, 0] = node.Attributes["name"].Value;
                _maps[i, 1] = node.Attributes["column"].Value;
                i++;
            }
        }


        public override void RunOnce()
        {
            //Создадим соединение
            if (!Connect())
                throw new Exception("Ошибка подключения! ");

            if (_downloadfile == null)
            {
                //Если это не загрузка файла то
                //получить набор параметров и сохранить в параметрах
                string _postData = "";
                Tag tag = null;
                for (int i = 0; i < _params.GetLength(0); i++)
                {
                    if (_params[i, 2] == "tag")
                    {
                        tag = _server.GetTag(_params[i, 3]);
                        _postData += ((_postData.Length > 0) ? "&" : "") + _params[i, 0] + "=" + tag.Value;
                    }
                    else
                    {
                        _postData += ((_postData.Length > 0) ? "&" : "") + _params[i, 0] + "=" + _params[i, 3];
                    }
                }

                _myreq = (HttpWebRequest)WebRequest.Create(_url);
                _myreq.Timeout = 15000;
                _myreq.KeepAlive = _keepalive;
                _myreq.Method = _method;
                if (_method.ToUpper() == "POST")
                    _myreq.ContentType = "application/x-www-form-urlencoded";

                //string postData = "firstone=" + inputData;
                if (_postData.Length > 0)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] byte1 = encoding.GetBytes(_postData);

                    // Set the content type of the data being posted.

                    // Set the content length of the string being posted.
                    _myreq.ContentLength = byte1.Length;

                    Stream newStream = _myreq.GetRequestStream();

                    newStream.Write(byte1, 0, byte1.Length);
                    newStream.Close();
                }

                //request.Credentials = CredentialCache.DefaultCredentials;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)_myreq.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    tag = _server.GetTag(_maps[0, 0]);
                    tag.Value = response.StatusCode;
                    _server.SetTag(tag);

                    response.Close();
                }
                catch (WebException e)
                {
                    tag = _server.GetTag(_maps[0, 0]);
                    if (e.Response != null)
                        tag.Value = ((HttpWebResponse)e.Response).StatusCode;
                    else
                        tag.Value = e.Status;
                    _server.SetTag(tag);
                }
            }
            else if (_downloadfile != null)
            {
                //Загрузим файл в локальную директорию
                _client.DownloadFile(_url,_downloadfile);
            }
            //Отключим соединение если не требуется хранить его
            if (!_keepalive)
                Disconnect();

        }

        public override void AfterStart()
        {

        }

        public override void BeforeStop()
        {
            Disconnect();
        }

        private bool Connect()
        {
            //создадим соединение с БД
            if (_client == null)
            {
                try
                {
                    _client = new WebClient();
                    _client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; Matrix;)");
                }
                catch (Exception e)
                {
                    _client = null;
                }
            }
            if (_client!=null)
                return true;
            else
                return false;
        }

        private void Disconnect()
        {
            if (_client != null)
            {
                _myreq  = null;
                _client = null;
            }
        }
    }
}
