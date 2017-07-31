using System;
using System.Xml;
using OPC.Common;
using OPC.Data.Interface;
using OPC.Data;
using System.Runtime.InteropServices;
using Matrix.Types;



namespace Matrix.Services
{
    /// <summary>
    /// Адаптер OPC
    /// </summary>
    public class ServiceOPCSimaticNET : Service
    {
        private string _progid;
        private string _host;
        private string _grpname;
        //private bool _callback;
        private OPCDATASOURCE _opcds = OPCDATASOURCE.OPC_DS_DEVICE; //Брать из прибора
        private OpcServer _opcsrv;
        private OpcGroup _opcgrp; //IOPCGroups
        private OPCItemResult[] _opcitms;//OPCItems
        
        private int[] _serverhandles;
        private Array _vals;
        private Array _errs;

        private string[,] _maps;
        private object _quality, _val, _dstmp, _err;
        private object lockThis = new object();
        private object lockConn = new object();
        private int _cntconn = 0;

        public override void Init(XmlNode xmlconf)
        {
            base.Init(xmlconf);
            _progid = xmlconf.Attributes["progid"].Value;
            _host = xmlconf.Attributes["host"].Value;
            _grpname = xmlconf.Attributes["group"].Value;
            //_callback = Convert.ToBoolean(xmlconf.Attributes["callback"].Value);
            try
            {
                string opcds = xmlconf.Attributes["opcds"].Value;
                if("cache".Equals(opcds.ToLower()))
                    _opcds = OPCDATASOURCE.OPC_DS_CACHE;
                else if("device".Equals(opcds.ToLower()))
                    _opcds = OPCDATASOURCE.OPC_DS_DEVICE;
            }catch(Exception e){
                _opcds = OPCDATASOURCE.OPC_DS_DEVICE;
            }

            XmlNodeList nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 2];
            //_opcitms = new OPCItemDef[nodes.Count];
            int i = 0;
            foreach (XmlNode node in nodes)
            {
                _maps[i, 0] = node.Attributes["name"].Value;
                _maps[i, 1] = node.Attributes["opctag"].Value;
                i++;
            }
        }

        public override void AfterStart()
        {
            try
            {
                _opcsrv = new OpcServer();
                //Connect();
            }
            catch (Exception e)
            {
                _server.LogWrite(LogType.ERROR, e.Message);
            }
        }

        public override void RunOnce()
        {
            _server.LogWrite(LogType.DEBUG,  "[" + this._name + "] : RunOnce вход");

            //Создадим соединение
            if (!Connect())
                throw new Exception("[" + this._name + "] : Ошибка подключения! ");

            //Синхронный режим прямой вызов
            OPCItemState[] _opcitmstate; 
            //--Тестовая секция--
            lock (lockThis)
            {
                _opcgrp.Read(_opcds, _serverhandles, out _opcitmstate);
            }
            for (int i = 0; i < _maps.GetLength(0); i++)
            {
                Tag tag = _server.GetTag(_maps[i, 0]);
                if (_opcitmstate[i].Error == HRESULTS.S_OK)
                {
                    if (_opcitmstate[i].DataValue is byte[])
                        _opcitmstate[i].DataValue = ByteArrayToString((byte[])_opcitmstate[i].DataValue);
                    else
                        tag.Value = _opcitmstate[i].DataValue;
                    tag.Timestamp = new DateTime(_opcitmstate[i].TimeStamp);
                    tag.Quality = (QualityType)Convert.ToByte(_opcitmstate[i].Quality);
                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : " + _maps[i, 1] + "=" + tag.Value);
                }
                else
                {
                    tag.Quality = QualityType.BAD;
                    _server.LogWrite(LogType.ERROR, "[" + this._name + "] : tag=" + _maps[i, 0] + ", opctag=" + _maps[i, 1] + ", ошибка: " + _opcitmstate[i].Error);
                }
                _server.SetTag(tag);
            }
            //--Тестовая секция--

            //Отключим соединение если не требуется хранить его
            Disconnect();
            _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : RunOnce выход");
        }

        public override void BeforeStop()
        {
            Disconnect();
            _opcgrp = null;
            _opcsrv = null;
        }

        //private void OPCGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        //{
        //    try
        //    {
        //        Tag tag = new Tag();
        //        _lastrun = DateTime.Now;
        //        for (int i = 0; i < _maps.GetLength(0); i++)
        //        {
        //            tag.Timestamp = DateTime.Now;
        //            tag.Name = _maps[i, 0];
        //            tag.Value = _opcitms[i].Value;
        //            tag.Quality = (QualityType)_opcitms[i].Quality;
        //            _server.SetTag(tag);
        //            _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : " +_maps[i, 1] + "=" + tag.Value);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _server.LogWrite(LogType.ERROR, "[" + this.Name + "]: " + e.ToString());
        //    }
        //}

        public override void Write(Tag tag)
        {
            DateTime dt1 = DateTime.Now;
            _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Write вход - tag=" + tag.Name);
            //Создадим соединение
            if (!Connect())
                throw new Exception("[" + this._name + "] : Ошибка подключения! ");

            for (int i = 0; i < _maps.GetLength(0); i++)
            {
                if (_maps[i, 0] == tag.Name)
                {
                    lock (lockThis)
                    {
                        int[] arrErr;
                        _opcgrp.Write(new int[] { _opcitms[i].HandleServer }, new object[] { tag.Value }, out arrErr);
                        if (arrErr.GetLength(0) > 0 && arrErr[0]!=0)
                        {
                            string message = "[" + this._name + "] : Ошибка записи! Tag[" + tag.Name + "]=" + tag.Value + ", error: " + arrErr[0].ToString();
                            _server.LogWrite(LogType.ERROR, message);
                            throw new Exception(message);
                        }
                    }
                    break;
                }
            }
            //Отключим соединение если не требуется хранить его
            Disconnect();
            _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : T(сек)=" + (DateTime.Now - dt1).Seconds + ", Write выход  - tag=" + tag.Name);
        }

        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод       
        private bool Connect()
        {
            lock(lockConn)
            {
                DateTime dt1 = DateTime.Now;
                try
                {
                        bool result;
                        //создадим соединение с БД
                        SERVERSTATUS status;
                        _opcsrv.GetStatus(out status);
                        if (_cntconn == 0 || status==null || status.eServerState != OPCSERVERSTATE.OPC_STATUS_RUNNING)
                        {
                            _opcsrv.Connect(_progid, _host);
                            _opcsrv.GetStatus(out status);
                            if (status != null && status.eServerState == OPCSERVERSTATE.OPC_STATUS_RUNNING)
                            {
                                try
                                {
                                    _opcgrp = _opcsrv.AddGroup(_grpname,false,_interval);
                                    _opcgrp.HandleClient = 0;
                                    int _cnt = _maps.GetLength(0);
                                    _serverhandles = new int[_cnt];
                                    _vals = new object[_cnt];
                                    _errs = new object[_cnt];
                                    OPCItemDef[] _opcitmsdef = new OPCItemDef[_cnt];

                                    for (int i = 0; i < _cnt; i++)
                                    {
                                        _opcitmsdef[i] = new OPCItemDef(_maps[i, 1], true, i, VarEnum.VT_EMPTY);
                                    }
                                    _opcgrp.AddItems(_opcitmsdef, out _opcitms);
                                    for (int i = 0; i < _cnt; i++)
                                    {
                                        _serverhandles[i] = _opcitms[i].HandleServer;
                                    }
                                    _opcgrp.SetEnable(true);
                                    _opcgrp.Active = true;
                                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : T(сек)=" + (DateTime.Now - dt1).Seconds + ", нап. груп. - " + _maps.GetLength(0));
                                    _cntconn++; //Увеличим счётчик активных соединений
                                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Connect - Активных соединений: " + _cntconn);
                                    result = true;
                                }
                                catch (Exception e)
                                {
                                    _server.LogWrite(LogType.ERROR, e.Message);
                                    result = false;
                                }
                            }
                            else
                                result = false;
                        }
                        else
                        {
                            _cntconn++; //Увеличим счётчик активных соединений
                            result = true;
                        }
                        return result;
                    }
                finally
                {
                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : T(сек)=" + (DateTime.Now - dt1).Seconds + ", время на откытие соединения");
                }
            }
        }

        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод       
        private void Disconnect()
        {
            lock (lockConn)
            {
                DateTime dt1 = DateTime.Now;
                _cntconn--;
                if (_cntconn < 0)
                    _cntconn = 0;
                //Отключить если подключено и 
                SERVERSTATUS status;
                if (_opcsrv != null)
                {
                    _opcsrv.GetStatus(out status);
                    if (status != null && status.eServerState == OPCSERVERSTATE.OPC_STATUS_RUNNING && _cntconn == 0 && !_keepalive)
                        _opcsrv.Disconnect();
                }
                //Проверим нет должно ли остаться открытым соединение
                if (_cntconn == 0 && _keepalive)
                    _cntconn++;
                _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Disconnect - Активных соединений: " + _cntconn);
            }
        }

    }
}
