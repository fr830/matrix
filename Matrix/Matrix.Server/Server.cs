using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections;
using Matrix.Types;
using Common.Logging;

namespace Matrix.Server
{
    /// <summary>
    /// Сервер
    /// </summary>
    public class Server : MarshalByRefObject, IServer
    {
        private static ILog log = LogManager.GetLogger<Server>();
        private static LogFileManager _logmngr;
        private static ConfigManager _confmngr;
        private static TagCache _tagcache;
        private static ServiceManager _srvcmngr;

        private static TagStoreManager _tagstoremngr;
        private static string _version = @"
---------------------------------
Matrix Server, Version 2.3.0
Author Kuzugashev V.
Copyright 2010-2016
---------------------------------";
        private bool IsRun = false;

        public static ServiceManager ServiceManager
        {
            get { return Server._srvcmngr; }
            set { Server._srvcmngr = value; }
        }


        /// <summary>
        /// Запустить сервер
        /// </summary>
        public void Start()
        {
            //Инициализация переменных         
            _confmngr = new ConfigManager();
            _logmngr = new LogFileManager(this);
            _tagcache = new TagCache(this);//основной кэш для записи данных
            _srvcmngr = new ServiceManager(this);
            _tagstoremngr = new TagStoreManager(this);
            //Инициализация
            
            _confmngr.LoadConf();

            log.Info(_version);
            log.Info( "Запуск сервера...");
            //Инициализация
            Thread.Sleep(500);
            _confmngr.InitTagStoreManager(_tagstoremngr);
            //Кэш
            _confmngr.InitTagCache(_tagcache);
            //Лог
            _confmngr.InitLogManager(_logmngr);
            //Старт системы сбора данных
            _confmngr.InitServiceManager(_srvcmngr);            
            //запуск синхронизации кеша
            _tagcache.Start();
            log.Info("Сервер запущен!");
            //проверка записис в лог
            //Установим признак работы сервера
            IsRun = true;
        }

        /// <summary>
        /// Остановить сервер
        /// </summary>
        public void Stop()
        {
            log.Info("Остановка сервера...");
            //остановить систему сбора
            _srvcmngr.Stop();
            //остановить лог
            _logmngr.Stop();
            _tagcache.Stop();
            //_tagstoremngr.Stop();
            log.Info( "Сервер остановлен!");
            //Установим признак "не работы"
            IsRun = false;
        }

        /// <summary>
        /// Временная остановка
        /// </summary>
        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Получить тэг из основного кеща
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public Tag GetTag(string tagName)
        {
            try
            {
                //return ((Tag)Hashtable.Synchronized(_tagcache.Tags)[tagName]).getOrUpdate(null);
                return ((Tag)(_tagcache.Tags)[tagName]).getOrUpdate(null);
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Получить тэг из зеркального кеша
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public Tag ReadTag(string tagName)
        {
            try{
                //Читаем данные из зеркального кэша Tags2
                //return ((Tag)Hashtable.Synchronized(_tagcache.Tags2)[tagName]).getOrUpdate(null);
                return ((Tag)(_tagcache.Tags2)[tagName]).getOrUpdate(null);
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
                return null;
            }

        }

        /// <summary>
        /// Получить массив тэгов
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public Tag[] ReadTags(string[] tagNames)
        {
            try{
                //Читаем данные из зеркального кэша Tags2
                Tag[] tags = new Tag[tagNames.Length];
                for(int i=0; i< tagNames.Length; i++)
                {
                    //tags[i] = ((Tag)Hashtable.Synchronized(_tagcache.Tags2)[tagNames[i]]).getOrUpdate(null);
                    tags[i] = ((Tag)(_tagcache.Tags2)[tagNames[i]]).getOrUpdate(null);
                }
                return tags;
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
                return null;
            }

        }

        /// <summary>
        /// получить изменённые теги с момента времени ч
        /// </summary>
        ///         
        /// 
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public Tag[] ReadTags(ref DateTime dt, string[] tagnames)
        {
            try{
                Tag[] tags = null;
                //Читаем данные из зеркального кэша Tags2
                if (dt != _tagcache.DtRefresh)//если кэш обновился разрешим запрос 
                {
                    int cnt = 0;
                    Tag[] buff = new Tag[tagnames.Length];
                    for (int i = 0; i < tagnames.Length; i++)
                    {
                        Tag tag = ((Tag)Hashtable.Synchronized(_tagcache.Tags2)[tagnames[i]]).getOrUpdate(null);
                        if (dt != null && tag.DtChange > dt)
                        {
                            buff[cnt] = tag;
                            cnt++;
                        }
                    }
                    if (cnt > 0)
                    {
                        tags = new Tag[cnt];
                        for (int i = 0; i < cnt; i++)
                        {
                            tags[i] = buff[i];
                        }
                    }
                    dt = _tagcache.DtRefresh;
                }
                return tags;
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
                return null;
            }

        }

        /// <summary>
        /// Установить тэг в кеше для адаптеров сбора данных
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public void SetTag(Tag tag)
        {
            try{
                _tagcache.LastUpdate = DateTime.Now;
                //проверим изменение значения
                //if (tag.IsChange)
                //{
                //    //Значение изменилось сохраним старое значение если было сжатие
                //    //Получим оригинал тега из кеша
                //    Tag oldtag = ((Tag)Hashtable.Synchronized(_tagcache.Tags)[tag.Name]).getOrUpdate(null);
                //    //Сбросим в лог, если тэг имеет дублированные значения до сохранения новых значений
                //    if (oldtag.cnt > 0)
                //    {
                //        //Закроем изменение реальным временем последнего изменения тэга
                //        //oldtag.DtChange = oldtag.Timestamp;
                //        oldtag.Timestamp = tag.DtChange;
                //        //oldtag.IsChange = true;
                //        //Сохраним старое значение в логе
                //        _logmngr.WriteToLog(oldtag);
                //    }
                //}

                if (tag.LogMode!=0)
                {
                    //Сохраним изменения в логе                
                    if (tag.IsChange && tag.Delta == 0)
                    {
                        //сохраним в логе если есть изменения и нет сжатия
                        _logmngr.WriteToLog(tag);
                    }
                    else if (tag.Delta > 0 && (tag.IsChange || (_tagcache.LastUpdate - tag.DtChange).TotalSeconds > tag.Step))
                    {
                        //Если есть сжатие но превышен интервал логирования
                        //Получим оригинал тега из кеша
                        Tag oldtag = ((Tag)Hashtable.Synchronized(_tagcache.Tags)[tag.Name]).getOrUpdate(null);
                        //установим время последнего обновления и расчитаем cnt и прочее
                        oldtag.Value = oldtag.Value;
                        //сохраним значение из кеша в логе
                        _logmngr.WriteToLog(oldtag);
                        //Установим новую дату отсчёта и сбросим счётчик если сработал LogInterval
                        if (!tag.IsChange)
                        {
                            tag.DtChange = _tagcache.LastUpdate;
                            tag.cnt = 0;
                        }
                    }
                }
                //Сохраним изменения в теге в рабочем кеше
                ((Tag)Hashtable.Synchronized(_tagcache.Tags)[tag.Name]).getOrUpdate(tag);
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
            }


        }

        /// <summary>
        /// Установить тэг для внешних программ
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        public void WriteTag(Tag tag)
        {
            try{
                //Сохраним изменения в сервисе
                Tag tg = ((Tag)Hashtable.Synchronized(_tagcache.Tags)[tag.Name]).getOrUpdate(null);
                tg.Value = tag.Value;
                if (tg.Service != null)
                {
                    //сохраним изменения в источнике
                    _srvcmngr.Write(tg);
                }
                //Изменения только в кеше
                SetTag(tg);
            }
            catch (Exception e)
            {
                log.Warn(e.Message + "\r\n " + e.StackTrace);
            }

        }

        /// <summary>
        /// Установить тэги из массива
        /// </summary>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод       
        public void WriteTags(Tag[] tags)
        {
            foreach (Tag tag in tags)
            {
                WriteTag(tag);
            }
        }

        /// <summary>
        /// Архивировать лог
        /// </summary>
        public void LogToStore(LogFile logfile)
        {
            _tagstoremngr.WriteToStore(logfile);
        }

        /// <summary>
        /// Сохранить сообщение в логфайле
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]//Синхронизировать метод
        //public void LogWrite(LogType lt, string message)
        //{
        //    switch(lt){
        //        case LogType.DEBUG:
        //            log.Debug(message);
        //            break;
        //        case LogType.ERROR:
        //            log.Warn(message);
        //            break;
        //        case LogType.INFO:
        //            log.Info(message);
        //            break;
        //        case LogType.WARN:
        //            log.Warn(message);
        //            break;
        //        case LogType.CONSOLE:
        //            log.Debug(message);
        //            break;
        //    }
        //}

        ~Server()
        {
            //остановить сервер
            if(IsRun)
                Stop();
        }
    }
}
