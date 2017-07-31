using System;
using System.IO;
using System.Text;
using Matrix.Types;

namespace Matrix.Server
{
    class Log
    {
        public static string _logfile;
        public static int _maxfilesize=10240000;//10ћб

        public static LogType _level = LogType.INFO;
        private static object _lockThis = new object();
        public static void Write(LogType lt, string logMessage)
        {
            if (_level >= lt)
            {
                lock (_lockThis)
                {
                    //string mess = string.Format("{0} {1}.{2}: {3}", DateTime.Now.ToShortDateString(),DateTime.Now.ToLongTimeString(),DateTime.Now.Millisecond,logMessage);
                    string mess = string.Format("[{0}]\t[{1}]\t[{2}]", DateTime.Now.ToString(Types.DateTimeFormat.RUSSIANLONGFORMAT), Functions.GetLogType(lt), logMessage);
                    if (_level < LogType.CONSOLE)
                    {
                        FileInfo fi = new FileInfo(_logfile);
                        if (fi.Exists)
                        {
                            if(fi.Length > _maxfilesize)
                            {
                                //—оздать копию файла
                                File.Move(_logfile,string.Format("{0}-{1}.bak",_logfile, DateTime.Now.ToString("yyyyMMdd-HHmmss")));
                            }
                        }
                        File.AppendAllText(_logfile, mess + "\r\n", Encoding.Default);
                    }
                    Console.WriteLine(mess);
                }
            }
        }
    }

}
