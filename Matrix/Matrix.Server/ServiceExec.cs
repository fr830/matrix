using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using Matrix.Types;
using System.IO;
using Common.Logging;

namespace Matrix.Services
{
    public class ServiceExec : Service
    {
        private static ILog log = LogManager.GetLogger<ServiceExec>();

        private string _cmd;
        private string _workdir;
        private string _body;
        // These are the Win32 error code for file not found or access denied.
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_ACCESS_DENIED = 5;


        private Process _process = null;
        private string[,] _params;
        private string[,] _maps;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);

            _cmd = xmlconf.Attributes["cmd"].Value;
            _workdir = xmlconf.Attributes["workdir"].Value;

            XmlNodeList nodes = xmlconf.SelectNodes("param");
            _params = new string[nodes.Count, 4];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _params[i, 0] = node.Attributes["name"].Value;
                _params[i, 1] = node.Attributes["type"].Value;
                _params[i, 2] = node.Attributes["typeval"].Value;
                _params[i, 3] = node.Attributes["value"].Value;
                if ("file".Equals(_params[i, 2]))
                {
                    _body = node.InnerText;
                    if (!"".Equals(_body))
                    {
                        //создадим файл скрипта
                        StreamWriter _file = File.CreateText(_workdir + "\\" + _params[i, 3]);
                        _file.Write(_body);
                        _file.Flush();
                        _file.Close();
                    }
                }
                i++;
            }

            nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 3];
            i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i, 0] = node.Attributes["name"].Value;
                try
                {
                    _maps[i, 1] = node.Attributes["type"].Value;
                }
                catch (Exception e) { }
                try
                {
                    _maps[i, 2] = node.Attributes["value"].Value;
                }
                catch (Exception e) { }
                i++;
            }
        }


        public override void RunOnce()
        {
            //получить набор параметров и сохранить в параметрах
            string _args = "";
            Hashtable _ht = new Hashtable();
            Tag tag = null;

            for (int i = 0; i < _params.GetLength(0); i++)
            {
                if (_params[i, 2] == "tag")
                {
                    tag = _server.GetTag(_params[i, 3]);
                    _args += ((_args.Length > 0) ? " " : "") +"\""+ tag.Value+"\"";
                }
                else
                {
                    _args += ((_args.Length > 0) ? " " : "") +"\""+ _params[i, 3]+"\"";
                }
            }

            try
            {
                string buff = "";
                _process = new Process();
                _process.StartInfo.FileName = _cmd;
                _process.StartInfo.CreateNoWindow = false;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.WorkingDirectory = _workdir;
                //_process.EnableRaisingEvents = true;    //Не было и возможно не получали сообщения о том что процесс умер
                _process.StartInfo.Arguments = _args;
                _process.Start();
                //StreamReader  reader = _process.StandardOutput;
                _process.OutputDataReceived += ((sender, e) =>
                {
                    buff += e.Data+"\r\n";
                });
                _process.BeginOutputReadLine();
                while (!_process.HasExited)
                {
                    Thread.Sleep(100);
                    _process.Refresh();
                }
                //buff += reader.ReadToEnd();

                log.Debug("[" + this._name + "] Exit Code=" + _process.ExitCode);
                log.Debug("[" + this._name + "]\r\n"+buff);

                
                if (_process.ExitCode == 0)
                {
                    //Если есть раздел <MAP> то разберём выход переменных
                    if (_maps.Length > 0)
                    {
                        //RETURN
                        //param1 = value
                        //param2 = value
                        //...
                        int idx = buff.LastIndexOf("RETURN");
                        if (idx >= 0)
                        {
                            string _results = buff.Substring(idx + 8);
                            string[] _pars = _results.Trim().Split('\r');
                            string _returns = "";
                            foreach (string _par in _pars)
                            {
                                string[] val = _par.Split(new Char[] { '=' }, 2);
                                if (val.Length == 2)
                                {
                                    _ht.Add(val[0].Trim().ToLower(), val[1].Trim());
                                    _returns += "\r\n" + val[0].Trim().ToLower() + "=" + val[1].Trim();
                                }
                            }
                            if (!"".Equals(_returns))
                                log.Debug("[" + this._name + "]\r\nRETURN" + _returns);
                        }
                        else
                            log.Warn("[" + this._name + "] Нет блока RETURN");
                    }

                }
                _ht.Add("exitcode", _process.ExitCode);

                for (int i = 0; i < _maps.GetLength(0); i++)
                {
                    tag = _server.GetTag(_maps[i, 0]);
                    if (_maps[i, 1] == "var")
                    {
                        string _val = (string)_ht[_maps[i, 2].ToLower()];
                        if (tag.Type == TagType.Float)
                        {
                            _val = _val.Replace('.', ',');
                        }
                        tag.Value = _val;
                    }
                    else
                        tag.Value = _maps[i, 2];
                    _server.SetTag(tag);
                }

            }
            catch (Exception e)
            {
                log.Warn("[" + this._name + "]\r\n" + e.Message + "\r\n" + e.StackTrace);
                //if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
                //    log.Warn(e.Message);
                //else if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
                //    log.Warn(e.Message);
                //else
                //    log.Warn(e.Message);
                if (_maps.Length > 0)
                {
                    tag = _server.GetTag(_maps[0, 0]);
                    tag.Value = e.Message;
                    //NativeErrorCode;
                    _server.SetTag(tag);
                }
                //Сгенерим ошибку
                throw new Exception(e.Message,e);
            }

        }

        public override void AfterStart()
        {
        }

        public override void BeforeStop()
        {
            try
            {
                _process.Kill();
                _process.WaitForExit(5000);
                _process.Close();
                _process = null;
            }
            catch (Exception e) { }
        }
    }
}
