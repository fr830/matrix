using System;
using System.Messaging;
using DataTransfer.Properties;
using System.Data;
using System.Data.SqlClient;


namespace DataTransferClassLibrary
{
    public class DataTransfer
    {
        MessageQueue queue = null;
        string _log = null;

        SqlConnection myConn = new SqlConnection();
        SqlCommand MyComm = new SqlCommand();


        public DataTransfer()
        {
            queue = new MessageQueue(Settings.Default.MessageQueue);        
        }

        ~DataTransfer()
        {
            queue.Close();
        }
        
        //public string SendMessage(string data, out int error)
        public string SendMessage(string data,
                                  string NameService)
        {
            _log = null;

            try
            {
                Message msg = new Message((string)data);
                queue.Send(msg);
            }
            catch (Exception e)
            {
                if (_log == null)
                    _log = NameService + ": ";
                _log = _log + e.Message;
            }
            return _log;
        }

        //Обработать все сообщения в очереди
        public string RouteMessages(string StrConnection,
                                    string NameService)
        {
            Message msg = null;
            string message = null;
            _log = null;

            try
            {
                msg = queue.Peek(new TimeSpan(0, 0, 1));
                
                if (msg == null) return null;

                msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                message = (string)msg.Body;

                if (myConn.State == ConnectionState.Closed)
                {

                    myConn.ConnectionString = StrConnection;
                    myConn.Open();
                }

                try
                {                                        
                    MyComm.Connection = myConn;                    
                    MyComm.CommandType = CommandType.StoredProcedure;
                    MyComm.CommandText = "Scales.Save_Inf_NewConveyor";
                    MyComm.Parameters.Clear();
                    MyComm.Parameters.Add("@InfData", SqlDbType.Xml).Value = message;
                    MyComm.ExecuteNonQuery();
                }

                catch (Exception e)
                {
                    if (_log == null)
                        _log = NameService + ": ";
                    _log = _log + e.Message;
                    _log = _log + "XML : " + message;
                }

                //Очистим сообщение
                msg = queue.Receive();
            
            }

            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    if (_log == null)
                        _log = NameService + ": ";
                    _log = _log + e.Message;
                }

            }

            
            catch (Exception e)
            {
                if (_log == null)
                    _log = NameService + ": ";
                _log = _log + e.Message;
            }

            return _log;
        }



    }
}
