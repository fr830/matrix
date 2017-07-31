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
    public class ServiceOPCSimaticNETX : Service
    {
        private string _progid;
        private string _host;
        private string _grpname;
        private bool _callback;
        private OPCServer _opcsrv;
        private OPCGroup _opcgrp; //IOPCGroups
        private OPCItem[] _opcitms;//OPCItems
        
        private Array _serverhandles;
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
            _callback = Convert.ToBoolean(xmlconf.Attributes["callback"].Value);

            XmlNodeList nodes = xmlconf.SelectNodes("map");
            _maps = new string[nodes.Count, 2];
            _opcitms = new OPCItem[nodes.Count];
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
                _opcsrv = new OPCServer();
                if (_keepalive)
                    Connect();
                //GC.KeepAlive(_opcsrv);
                //Thread.Sleep(500);
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
            //--Тестовая секция--
            lock (lockThis)
            {
                _opcgrp.SyncRead((short)OPCDataSource.OPCDevice, _opcgrp.OPCItems.Count, _serverhandles, out _vals, out _errs, out _quality, out _dstmp);
            }
            for (int i = 0; i < _maps.GetLength(0); i++)
            {
                Tag tag = _server.GetTag(_maps[i, 0]);
                try
                {
                    if (_vals.GetValue(i) is byte[])
                        _vals.SetValue(ByteArrayToString((byte[])_vals.GetValue(i)),i);
                    else
                        tag.Value = _vals.GetValue(i);
                    tag.Timestamp = (DateTime)_dstmp;
                    tag.Quality = (QualityType)Convert.ToInt32(_quality);
                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : " + _maps[i, 1] + "=" + tag.Value);
                }
                catch (Exception e)
                {
                    tag.Quality = QualityType.BAD;
                    _server.LogWrite(LogType.ERROR, "[" + this._name + "] : tag=" + _maps[i, 0] + ", opctag=" + _maps[i, 1] + ", ошибка: " + e.Message);
                }
                _server.SetTag(tag);
            }
            //--Тестовая секция--

            //--Рабочая секция--
            //for (int i = 0; i < _maps.GetLength(0); i++)
            //{

            //    Tag tag = _server.GetTag(_maps[i, 0]);
            //    try
            //    {
            //        lock (lockThis)
            //        {
            //            _opcitms[i].Read((short)OPCDataSource.OPCDevice, out _val, out _quality, out _dstmp);
            //        }
            //        if (_val is byte[])
            //            _val = ByteArrayToString((byte[])_val);
            //        else
            //            tag.Value = _val;
            //        tag.Timestamp = (DateTime)_dstmp;
            //        tag.Quality = (QualityType)Convert.ToInt32(_quality);
            //        _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : " + _maps[i, 1] + "=" + tag.Value);
            //    }
            //    catch (Exception e)
            //    {
            //        tag.Quality = QualityType.BAD;
            //        _server.LogWrite(LogType.ERROR, "[" + this._name + "] : tag=" + _maps[i, 0] + ", opctag=" + _maps[i, 1] + ", ошибка: " + e.Message);
            //    }
            //    _server.SetTag(tag);
            //}
            //--Рабочая секция--

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

        private void OPCGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            try
            {
                Tag tag = new Tag();
                _lastrun = DateTime.Now;
                for (int i = 0; i < _maps.GetLength(0); i++)
                {
                    tag.Timestamp = DateTime.Now;
                    tag.Name = _maps[i, 0];
                    tag.Value = _opcitms[i].Value;
                    tag.Quality = (QualityType)_opcitms[i].Quality;
                    _server.SetTag(tag);
                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : " +_maps[i, 1] + "=" + tag.Value);
                }
            }
            catch (Exception e)
            {
                _server.LogWrite(LogType.ERROR, "[" + this.Name + "]: " + e.ToString());
            }
        }

        public override void Write(Tag tag)
        {
            DateTime dt1 = DateTime.Now;
            _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Write вход - tag="+tag.Name);
            //Создадим соединение
            if (!Connect())
                throw new Exception("[" + this._name + "] : Ошибка подключения! ");

            for (int i = 0; i < _maps.GetLength(0); i++)
            {
                if (_maps[i, 0] == tag.Name)
                {
                    lock (lockThis)
                    {
                        _opcitms[i].Write(tag.Value);
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
                        //if (_opcsrv.ServerState != 1)
                        if (_cntconn == 0 || _opcsrv.ServerState != 1)
                        {
                            _opcsrv.Connect(_progid, _host);
                            if (_opcsrv.ServerState == 1)
                            {
                                try
                                {
                                    _opcgrp = _opcsrv.OPCGroups.Add(_grpname);
                                    //GC.KeepAlive(_opcgrp);
                                    _opcgrp.ClientHandle = 0;
                                    int _cnt = _maps.GetLength(0);
                                    _serverhandles = new object[_cnt];
                                    _vals = new object[_cnt];
                                    _errs = new object[_cnt];

                                    for (int i = 0; i < _cnt; i++)
                                    {
                                        OPCItem opcitm = _opcgrp.OPCItems.AddItem(_maps[i, 1], i + 1);
                                        //GC.KeepAlive(opcitm);
                                        _serverhandles.SetValue(opcitm.ServerHandle,i);
                                        _opcitms[i] = opcitm;
                                    }
                                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : T(сек)=" + (DateTime.Now - dt1).Seconds + ", нап. груп. - " + _maps.GetLength(0));
                                    //GC.KeepAlive(_opcgrp.OPCItems);
                                    //GC.KeepAlive(_opcitms);
                                    //if (_callback)
                                    //{
                                    //Асинхронный режим через обратный вызов
                                    //_opcgrp.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(this.OPCGroup_DataChange);
                                    //_opcgrp.IsActive = true;
                                    //_opcgrp.UpdateRate = _interval;  //Как часто обновлять
                                    //_opcgrp.IsSubscribed = true; // разрешить подписку
                                    //}
                                    _cntconn++; //Увеличим счётчик активных соединений
                                    _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Connect - Активных соединений: " + _cntconn);
                                    //return true;
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
                if (_opcsrv.ServerState == 1 && _cntconn == 0 && !_keepalive)
                    _opcsrv.Disconnect();
                _server.LogWrite(LogType.DEBUG, "[" + this._name + "] : Disconnect - Активных соединений: " + _cntconn);
            }
        }

    }
}
