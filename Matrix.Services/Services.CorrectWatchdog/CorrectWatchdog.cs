using System;
using System.Xml;
using System.Collections;
using Matrix.Types;

namespace ServiceCorrectWatchdog 
{
    public class CorrectWatchdog : Service
    {
        // Переменные

        private int code_oper;
        private string xml;
        private ArrayList dt, h, i;
        private Hashtable ht, htk;
        private Single xim_c;
        private DateTime dt_prod;
        private string xml_error;
        private int ep;
        private DateTime dt_spr;

        private int k_ohl;
        private int v1l, v2l, v3l, v4l;
        private int v1l_old, v2l_old, v3l_old, v4l_old;
        private int v1r, v2r, v3r, v4r;
        private int v1r_old, v2r_old, v3r_old, v4r_old;
        private Single w1l, w2l, w3l, w4l;
        private Single w1l_old, w2l_old, w3l_old, w4l_old;
        private Single w1r, w2r, w3r, w4r;
        private Single w1r_old, w2r_old, w3r_old, w4r_old;

        // Константы

        const int max_count = 1000;             // Максимум для сторожевого таймера
        const int from_end_interval = 1;        // Интервал времени от конца продувки (в минутах)
        const int max_xmlerror_count = 10000;   // Размер строки ошибок
        const int spr_interval = 60;            // Интервал опроса справочников (в секундах)

        // --------------------------------------------------------------------------------------------------
        // INIT

        public override void Init(XmlNode xmlconf)
        {
            try
            {
                base.Init(xmlconf);

                code_oper = 0;
                xim_c = 0;
                dt_prod = DateTime.MinValue;
                xml = "";
                dt = new ArrayList();
                h = new ArrayList();
                i = new ArrayList();
                ht = new Hashtable();
                htk = new Hashtable();
                xml_error = "";
                dt_spr = DateTime.MinValue;

                v1l_old = 0; v2l_old = 0; v3l_old = 0; v4l_old = 0;
                w1l_old = 0; w2l_old = 0; w3l_old = 0; w4l_old = 0;
                v1r_old = 0; v2r_old = 0; v3r_old = 0; v4r_old = 0;
                w1r_old = 0; w2r_old = 0; w3r_old = 0; w4r_old = 0;

                Save_WatchdogError("");

                _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Инициализация сторожевой службы");
            }
            catch (Exception e)
            {
                _server.LogWrite(LogType.ERROR, "[" + this._name + "] Init: " + e.ToString());
            }
        }

        // --------------------------------------------------------------------------------------------------
        // RUNONCE

        public override void RunOnce()
        {
            try
            {
                //_server.LogWrite(LogType.DEBUG, "-- WATCHDOG -- Запуск сторожевой службы");

                // Биты жизни и индикатор
                // ----------------------------------------------------

                DateTime dt1, dt2;
                TimeSpan ts2;

                ep = 1; // ***
                Tag t = _server.GetTag("WatchdogCalc");
                t.Value = false;
//                dt1 = DateTime.Now;
                _server.WriteTag(t);
//                dt2 = DateTime.Now;

//                ts2 = dt2 - dt1;
//                dt2 = new DateTime(ts2.Ticks);
//                _server.LogWrite(LogType.DEBUG, "set " + dt2.ToString("HH:mm:ss.fff") + " - WatchdogCalc");

                ep = 11; // ***
                t = _server.GetTag("WatchdogAll");
                t.Value = false;
//                dt1 = DateTime.Now;
                _server.WriteTag(t);
//                dt2 = DateTime.Now;

//                ts2 = dt2 - dt1;
//                dt2 = new DateTime(ts2.Ticks);
//                _server.LogWrite(LogType.DEBUG, "set " + dt2.ToString("HH:mm:ss.fff") + " - WatchdogAll");

                ep = 12; // ***
                t = _server.GetTag("Calc.Connected");
                t.Value = true;
//                dt1 = DateTime.Now;
                _server.WriteTag(t);
//                dt2 = DateTime.Now;

//                ts2 = dt2 - dt1;
//                dt2 = new DateTime(ts2.Ticks);
//                _server.LogWrite(LogType.DEBUG, "set " + dt2.ToString("HH:mm:ss.fff") + " - Calc.Connected");


                // Чтение кода операции
                // ----------------------------------------------------

                ep = 2; // ***
                t = _server.GetTag("OperationCode");

                if (((code_oper == 4) && (Convert.ToInt16(t.Value) == 6))
                    || (((code_oper >= 7) && (code_oper <= 13)) && (Convert.ToInt16(t.Value) == 6)))
                {
                    if (code_oper == 4)
                    {
                        // Повалка после продувки
                        // ----------------------------------------------------

                        // Запись тэга данных по продувке

                        ep = 3; // ***
                        t = _server.GetTag("k5.produvka_data");
                        t.Value = xml;
                        _server.WriteTag(t);

                        // Определение периода для высоты фурмы

                        ep = 4; // ***
                        t = _server.GetTag("k5.t_h");
                        int min_From = Convert.ToInt16(t.Value);
                        if (min_From <= 0) min_From = from_end_interval;
                        DateTime dt_Last = DateTime.MinValue;
                        DateTime dt_First = DateTime.MinValue;

                        if (dt.Count > 0)
                        {
                            ep = 5; // ***
                            dt_First = Convert.ToDateTime(dt[0]);
                            ep = 2; // ***
                            dt_Last = Convert.ToDateTime(dt[dt.Count - 1]);
                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Время окончания продувки: " + dt_Last.ToString("mm:ss"));

                            ep = 6; // ***
                            TimeSpan ts = TimeSpan.FromMinutes(min_From);
                            DateTime dt_From_End = new DateTime(ts.Ticks);
                            if (dt_Last > dt_From_End)
                            {
                                ep = 7; // ***
                                ts = dt_Last - dt_From_End;
                                dt_Last = new DateTime(ts.Ticks);
                            }
                            else
                            {
                                dt_Last = dt_First;
                            }
                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Время начала обсчета продувки: " + dt_Last.ToString("mm:ss"));
                        }

                        // Определение высоты фурмы

                        ep = 8; // ***
                        Single h_temp;
                        for (int j = 0; j <= dt.Count - 1; j++)
                        {
                            if (Convert.ToDateTime(dt[j]) >= dt_Last)
                            {
                                h_temp = Convert.ToSingle(h[j]);
                                if (ht.ContainsKey(h_temp))
                                    ht[h_temp] = Convert.ToInt16(ht[h_temp]) + 1;
                                else ht.Add(h_temp, 1);
                            }
                        }

                        ep = 9; // ***
                        int h_max_count = -1;
                        Single h_calc = -1;
                        foreach (DictionaryEntry de in ht)
                        {
                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- " + de.Key.ToString() + " " + de.Value.ToString());

                            if (Convert.ToInt16(de.Value) > h_max_count)
                            {
                                h_max_count = Convert.ToInt16(de.Value);
                                h_calc = Convert.ToSingle(de.Key);
                            }
                        }

                        if (h_calc != -1)
                        {
                            ep = 10; // ***
                            t = _server.GetTag("k5.h_furm_zadan");
                            t.Value = h_calc;
                            _server.WriteTag(t);
                        }

                        // Установка триггера для записи данных по продувке в БД

                        ep = 100; // ***
                        t = _server.GetTag("k5.produvka2db");
                        t.Value = true;
                        _server.WriteTag(t);
                    }
                    else
                    {
                        if ((code_oper >= 7) && (code_oper <= 13))
                        {
                            // Повалка после додувки
                            // ----------------------------------------------------

                            // Запись тэга данных по додувке

                            ep = 11; // ***
                            t = _server.GetTag("k5.doduvka_data");
                            t.Value = xml;
                            _server.WriteTag(t);

                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Окончание додувки: " + DateTime.Now.ToString("HH:mm:ss"));

                            // Установка триггера для записи данных по додувке в БД

                            ep = 111; // ***
                            t = _server.GetTag("k5.doduvka2db");
                            t.Value = true;
                            _server.WriteTag(t);
                        }

                    }

                    // Запись кода операции

                    ep = 12; // ***
                    t = _server.GetTag("OperationCode");
                    code_oper = Convert.ToInt16(t.Value);

                    // Очистка данных

                    ep = 13; // ***
                    xml = "";
                    dt_prod = DateTime.MinValue;

                    dt.Clear();
                    h.Clear();
                    i.Clear();
                    ht.Clear();
                }
                else
                {
                    // Начало продувки
                    // ----------------------------------------------------

                    ep = 14; // ***
                    if ((code_oper != 3) && (Convert.ToInt16(t.Value) == 3))
                    {
                        if (dt_prod == DateTime.MinValue)
                        {
                            // Начало первой продувки

                            dt_prod = DateTime.Now;
                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Начало первой продувки");
                        }
                        else
                        {

                            // Начало продолжения продувки

                            if (dt.Count > 0)
                            {
                                // Учет продолжительности предыдущей продувки

                                ep = 15; // ***
                                DateTime dt_prod_prev = Convert.ToDateTime(dt[dt.Count - 1]);
                                TimeSpan ts_prev = DateTime.Now - dt_prod_prev.AddSeconds(1);
                                dt_prod = new DateTime(ts_prev.Ticks);
                            }
                            else
                            {
                                dt_prod = DateTime.Now;
                            }

                            //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Начало продолжения продувки");
                        }
                    }

                    // Начало додувки
                    // ----------------------------------------------------


                    ep = 16; // ***
                    if (!((code_oper >= 7) && (code_oper <= 13)) 
                        && ((Convert.ToInt16(t.Value) >= 7) && (Convert.ToInt16(t.Value) <= 13)))
                    {
                        // Начало додувки

                        dt_prod = DateTime.Now;
                        //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Начало додувки");
                    }

                    // Запись кода операции

                    ep = 17; // ***
                    code_oper = Convert.ToInt16(t.Value);

                    if ((code_oper == 3) || ((code_oper >= 7) && (code_oper <= 13)))
                    {
                        // Период продувки или додувки
                        // ----------------------------------------------------

                        String s_dlit;
                        DateTime dlit;
                        int sec;
                        if (dt_prod != DateTime.MinValue)
                        {
                            ep = 18; // ***
                            TimeSpan ts = DateTime.Now - dt_prod;
                            dlit = new DateTime(ts.Ticks);
                            s_dlit = dlit.ToString("mm:ss");
                            sec = dlit.Second + dlit.Minute * 60;
                            //if (code_oper == 3)
                                //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Определяем время от начала продувки: " + s_dlit);
                            //else
                                //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Определяем время от начала додувки: " + s_dlit);
                        }
                        else
                        {
                            ep = 19; // ***
                            dlit = DateTime.MinValue;
                            s_dlit = "0";
                            sec = 0;
                            //if (code_oper == 3)
                                //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Нулевое время начала продувки ");
                            //else
                                //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Нулевое время начала додувки ");
                        }

                        ep = 20; // ***
                        t = _server.GetTag("Fact.H");
                        Single h_ = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value)*100)/100);

                        ep = 21; // ***
                        t = _server.GetTag("Fact.I");
                        int i_ = Convert.ToInt16(Math.Round(Convert.ToSingle(t.Value)));
                        DateTime dt_ = DateTime.Now;

                        string s = "<data time1='" + dt_.ToString("dd.MM.yy HH:mm:ss") 
                            + "' time2='" + s_dlit + "' sec='" + sec  
                            + "' H='" + h_.ToString() + "' I='" + i_.ToString() + "'/>";

                        ep = 22; // ***
                        h.Add(h_);
                        dt.Add(dlit);
                        i.Add(i_);

                        xml = xml + s;
                    }
                }

                // Замер температуры
                // ----------------------------------------------------

                ep = 23; // ***
                t = _server.GetTag("Pov.bitT");
                if (Convert.ToBoolean(t.Value) == true)
                {
                    ep = 24; // ***
                    t = _server.GetTag("Pov.T");
                    int tst = Convert.ToInt16(t.Value);

                    //_server.LogWrite(LogType.INFO, "-- WATCHDOG -- Получен замер температуры: " + tst.ToString());

                    if (tst != 9999)
                    {
                        ep = 25; // ***
                        t = _server.GetTag("k5.tst");
                        t.Value = tst;
                        _server.WriteTag(t);
                    }
                }

                // Содержание углерода
                // ----------------------------------------------------

                ep = 26; // ***
                t = _server.GetTag("Pov.C.Tablo");

                if (xim_c != Convert.ToSingle(t.Value))
                {
                    // _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Получено значение углерода: " + C_New.ToString());

                    ep = 27; // ***
                    xim_c = Convert.ToSingle(t.Value);

                    ep = 28; // ***
                    t = _server.GetTag("k5.xim.c");
                    t.Value = xim_c;
                    _server.WriteTag(t);

                    //ep = 30; // ***
                    //t = _server.GetTag("Pov.C");
                    //t.Value = Math.Round(xim_c * 100);
                    //_server.WriteTag(t);
                }

                // Справочник охлаждающих эффектов
                // ----------------------------------------------------

                ep = 31; // ***
                TimeSpan ts_spr = DateTime.Now - dt_spr;

                if (ts_spr >= TimeSpan.FromSeconds(spr_interval))
                {
                    ep = 32; // ***
                    dt_spr = DateTime.Now;
                    t = _server.GetTag("k5.k_ohl_spr");

                    ep = 33; // ***
                    XmlDocument xd = new XmlDocument();
                    string xml_spr = Convert.ToString(t.Value);

                    if (xml_spr.Contains("rows"))
                    {
                        ep = 34; // ***
                        htk.Clear();
                        xd.LoadXml(Convert.ToString(t.Value));
                        XmlNodeList nodes = xd.SelectNodes("rows");

                        ep = 35; // ***
                        string k_id, k_k;
                        foreach (XmlNode node in nodes[0].ChildNodes)
                        {
                            k_id = "";
                            k_k = "";
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.ToUpper() == "ID") k_id = node2.InnerText;
                                if (node2.Name.ToUpper() == "K") k_k = node2.InnerText;
                            }
                            if (k_id != "")
                            {
                                if (k_id == "65")
                                {
                                    ep = 351; // ***
                                    t = _server.GetTag("k5.k_ohl");
                                    k_k = Convert.ToString(t.Value);
                                }

                                if (!htk.ContainsKey(k_id))
                                    htk.Add(k_id, k_k);
                            }
                        }
                    }
                }

                // Состояние промбункеров
                // ----------------------------------------------------

                // Левый

                ep = 36; // ***
                t = _server.GetTag("Bin1.Mat1");
                v1l = Convert.ToInt16(t.Value);

                ep = 37; // ***
                t = _server.GetTag("Bin1.Mat2");
                v2l = Convert.ToInt16(t.Value);

                ep = 38; // ***
                t = _server.GetTag("Bin1.Mat3");
                v3l = Convert.ToInt16(t.Value);

                ep = 39; // ***
                t = _server.GetTag("Bin1.Mat4");
                v4l = Convert.ToInt16(t.Value);

                ep = 40; // ***
                t = _server.GetTag("Bin1.Ves1");
                w1l = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 41; // ***
                t = _server.GetTag("Bin1.Ves2");
                w2l = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 42; // ***
                t = _server.GetTag("Bin1.Ves3");
                w3l = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 43; // ***
                t = _server.GetTag("Bin1.Ves4");
                w4l = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                // Правый

                ep = 44; // ***
                t = _server.GetTag("Bin2.Mat1");
                v1r = Convert.ToInt16(t.Value);

                ep = 45; // ***
                t = _server.GetTag("Bin2.Mat2");
                v2r = Convert.ToInt16(t.Value);

                ep = 46; // ***
                t = _server.GetTag("Bin2.Mat3");
                v3r = Convert.ToInt16(t.Value);

                ep = 47; // ***
                t = _server.GetTag("Bin2.Mat4");
                v4r = Convert.ToInt16(t.Value);

                ep = 48; // ***
                t = _server.GetTag("Bin2.Ves1");
                w1r = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 49; // ***
                t = _server.GetTag("Bin2.Ves2");
                w2r = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 50; // ***
                t = _server.GetTag("Bin2.Ves3");
                w3r = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                ep = 51; // ***
                t = _server.GetTag("Bin2.Ves4");
                w4r = Convert.ToSingle(Math.Round(Convert.ToSingle(t.Value) * 100) / 100);

                if (htk.Count > 0)
                {
                    ep = 52; // ***
                    bool b_l, b_r;
                    b_l = (v1l != v1l_old) || (v2l != v2l_old) || (v3l != v3l_old) || (v4l != v4l_old)
                        || (w1l != w1l_old) || (w2l != w2l_old) || (w3l != w3l_old) || (w4l != w4l_old);
                    b_r = (v1r != v1r_old) || (v2r != v2r_old) || (v3r != v3r_old) || (v4r != v4r_old)
                        || (w1r != w1r_old) || (w2r != w2r_old) || (w3r != w3r_old) || (w4r != w4r_old);

                    if (b_l || b_r)
                    {
                        // Кохл

                        ep = 53; // ***
                        t = _server.GetTag("k5.k_ohl");
                        k_ohl = Convert.ToInt16(t.Value);

                        // Обработка изменения состояния бункеров

                        _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Изменилось состояние промбункеров");
                        if (b_l)
                        {
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Левый промбункер");
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v1l.ToString() + " Вес " + w1l.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v2l.ToString() + " Вес " + w2l.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v3l.ToString() + " Вес " + w3l.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v4l.ToString() + " Вес " + w4l.ToString());
                        }

                        if (b_r)
                        {
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Правый промбункер");
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v1r.ToString() + " Вес " + w1r.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v2r.ToString() + " Вес " + w2r.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v3r.ToString() + " Вес " + w3r.ToString());
                            _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Вид " + v4r.ToString() + " Вес " + w4r.ToString());
                        }

                        int v;
                        Single k, w, sum;
                        k = 0;
                        v = 0;
                        w = 0;
                        sum = 0;

                        ep = 54; // ***
                        for (int j = 0; j <= 7; j++)
                        {
                            if ((j >= 0) && (j <= 3) && (!b_l)) continue;
                            if ((j >= 4) && (j <= 7) && (!b_r)) continue;

                            switch (j)
                            {
                                // Левый промбункер

                                case 0:
                                    v = v1l;
                                    w = w1l;
                                    break;
                                case 1:
                                    v = v2l;
                                    w = w2l;
                                    break;
                                case 2:
                                    v = v3l;
                                    w = w3l;
                                    break;
                                case 3:
                                    v = v4l;
                                    w = w4l;
                                    break;

                                // Правый промбункер

                                case 4:
                                    v = v1r;
                                    w = w1r;
                                    break;
                                case 5:
                                    v = v2r;
                                    w = w2r;
                                    break;
                                case 6:
                                    v = v3r;
                                    w = w3r;
                                    break;
                                case 7:
                                    v = v4r;
                                    w = w4r;
                                    break;
                            }


                            if (v != 0)
                            {
                                if (!htk.ContainsKey(v.ToString()))
                                {
                                    //string s_NotFound = "-- WATCHDOG -- Материал не найден в справочнике (Код " + v.ToString() + ")";
                                    //_server.LogWrite(LogType.INFO, s_NotFound);
                                    //Save_WatchdogError(s_NotFound);
                                }
                                else
                                {
                                    ep = 55; // ***
                                    k = Convert.ToSingle(htk[v.ToString()]);
                                    sum = sum + k * w;
                                }
                            }

                            // Запись в тэги

                            if (j == 3)
                            {
                                ep = 56; // ***
                                sum = Convert.ToSingle(Math.Round((sum / k_ohl * 100)) / 100);
                                t = _server.GetTag("k5.weight_left");
                                t.Value = sum;
                                _server.WriteTag(t);
                                _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Условная известь в левом: " + sum.ToString());
                                sum = 0;
                            }

                            if (j == 7)
                            {
                                ep = 57; // ***
                                sum = Convert.ToSingle(Math.Round((sum / k_ohl * 100)) / 100);
                                t = _server.GetTag("k5.weight_right");
                                t.Value = sum;
                                _server.WriteTag(t);
                                _server.LogWrite(LogType.INFO, "-- WATCHDOG -- Условная известь в правом: " + sum.ToString());
                                sum = 0;
                            }
                        }

                        // Запись старого состояния

                        ep = 58; // ***
                        v1l_old = v1l;
                        v2l_old = v2l;
                        v3l_old = v3l;
                        v4l_old = v4l;
                        w1l_old = w1l;
                        w2l_old = w2l;
                        w3l_old = w3l;
                        w4l_old = w4l;
                        v1r_old = v1r;
                        v2r_old = v2r;
                        v3r_old = v3r;
                        v4r_old = v4r;
                        w1r_old = w1r;
                        w2r_old = w2r;
                        w3r_old = w3r;
                        w4r_old = w4r;
                    }
                }

            }
            catch (Exception e)
            {
                string s_Error = "[" + this._name + "] RunOnce(Point " + ep.ToString() + "): " + e.ToString();
                _server.LogWrite(LogType.ERROR, s_Error);
                Save_WatchdogError(s_Error);
            }
        }

        // --------------------------------------------------------------------------------------------------
        // ЗАПИСЬ ОШИБКИ СТОРОЖЕВОЙ СЛУЖБЫ

        public void Save_WatchdogError(string s_error)
        {
            if (xml_error.Length > max_xmlerror_count) xml_error = "";

            if (s_error.Length == 0)
                xml_error = "";
            else
                xml_error = xml_error + "<error> <![CDATA[ " + DateTime.Now.ToString() + " " + s_error + " ]]></error>";

            Tag t = _server.GetTag("k5.watchdog_errors");
            t.Value = xml_error;
            _server.WriteTag(t);

        }

    }
}
