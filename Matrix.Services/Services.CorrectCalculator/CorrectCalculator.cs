using System;
using System.Xml;
using Matrix.Types;

namespace ServiceCorrectCalculator
{
   public class CorrectCalculator : Service
    {
       // Переменные

       private string xml;
       private string xml_error;
       private int ep;

       private bool b_PLC;
       private bool b_C, b_2, b_Cycle; 
       private Single C_f, C_n, C_v, C1, C2, D_C, C_r_1, h_c;
       private Single Hf_z, Hf_b, D_H, dH;
       private Single K_oc, K_ot, K_oc_C_v, K_ot_C_v;
       private int D_Q1o, D_T, D_Q1o_old;
       private int dT, T_f, T_z, T_r_1, T_r_1_old, T_r_2;
       private int D_T_r, D_T_G, T_r_1_G, T_r_1_G_pred, D_T_dop, D_T_r_G;
       private Single dG, D_Gohl, K_ohl, K_dT;
       private Single Weight_Left, Weight_Right;
       private Single G, G1, G2, G3, G_ohl;
       private string Info_1, Info_ohl, Info_2;
       private int Step;
       private int T_r_C_n, D_Q2O_C_n, D_QOdod_C_n, D_QOdod;
       private int Flag;

       private int tst_r, tst_r1, tst_r1g, tst_rn, d_tst_r, d_tst_rg;
       private Single xim_c_r;
       private Single d_g_ohl, _g_ohl, _g1, _g2;
       private int time_doduv;
       private int d_q_r1, d_q_r2, d_q_rn, d_q_dod_rn;
       private Single h_furm_1, h_furm_2;
       private int i_o2, i_o2_1, i_o2_2, d_q_1, d_q_2;
       private bool l_bunker, r_bunker;
       private Single k_oc_cv, k_ot_cv, k_oc_cvcr, k_ot_cvcr;

       private float C_v1, C_v2, C_v3, C_v4;
       private float a1, a2, a3, a4, a5;
       private float b1, b2, b3, b4, b5, b6;

       // Константы

       const int max_watchdog_count = 1000;
       const int max_xmlerror_count = 10000;

       // --------------------------------------------------------------------------------------------------
       // INIT

       public override void Init(XmlNode xmlconf)
       {
           try
           {
               base.Init(xmlconf);
               
               xml_error = "";
               Save_CalcError("");

               _server.LogWrite(LogType.INFO, "-- CALC -- Инициализация службы расчета");
           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Init: " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // RUNONCE

       public override void RunOnce()
       {
           try
           {
               _server.LogWrite(LogType.INFO, "-- CALC -- Запуск службы расчета");

               //System.Threading.Thread.Sleep(2000);

               xml = "";

               Read_Input();
               if (!Check_Input())
               {
                   Save_Error();
                   return;
               }

               Save_Input();
               Calc();
               Save_Results();
           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] RunOnce: " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // ЧТЕНИЕ ВХОДНЫХ ДАННЫХ

       public void Read_Input()
       {
           try
           {
               // Константы для кнопок
               // ----------------------------------------

               ep = -4; // ***
               C_v1 = Convert.ToSingle((_server.GetTag("k5.c_zadan_do_1").Value));

               ep = -3; // ***
               C_v2 = Convert.ToSingle((_server.GetTag("k5.c_zadan_do_2").Value));

               ep = -2; // ***
               C_v3 = Convert.ToSingle((_server.GetTag("k5.c_zadan_do_3").Value));

               ep = -1; // ***
               C_v4 = Convert.ToSingle((_server.GetTag("k5.c_zadan_do_4").Value));

               // Углерод и температура
               // ----------------------------------------

               ep = 0; // *****
               T_f = Convert.ToInt16((_server.GetTag("k5.tst").Value));

               ep = 1; // *****
               bool b = Convert.ToBoolean(_server.GetTag("k5.start_raschet").Value);

               b_PLC = false;
               if (b)
               {
                   // Запуск из АРМа
                   // ----------------------------------------

                   ep = 1; // *****
                   int i = Convert.ToInt16(_server.GetTag("k5.vid_doduvki").Value);
                   if (i == 1) b_C = true; else b_C = false;

                   ep = 2; // *****
                   C_f = Convert.ToSingle((_server.GetTag("k5.xim.c").Value));

                   ep = 3; // *****
                   C_v = Convert.ToSingle((_server.GetTag("k5.c_zadan_do").Value));

                   ep = 4; // *****
                   T_z = Convert.ToInt16((_server.GetTag("k5.tst_zadan").Value));
               }
               else
               {
                   ep = 5; // *****
                   b = Convert.ToBoolean((_server.GetTag("ButtonCalcLancing").Value));
                   b_PLC = b;

                   if (b)
                   {
                       // Запуск из PLC
                       // ----------------------------------------

                       b_C = false;

                       ep = 6; // *****
                       b = Convert.ToBoolean((_server.GetTag("ButtonLanceC").Value));
                       if (b)
                       {
                           b_C = true;
                       }
                       else
                       {
                           ep = 7; // *****
                           b = Convert.ToBoolean((_server.GetTag("ButtonLanceAll").Value));
                           if (b) b_C = true;
                       }
                   }

                   ep = 8; // *****
                   int i_C = Convert.ToInt16((_server.GetTag("Pov.C").Value));
                   C_f = Convert.ToSingle(i_C / 100.0);

                   ep = 9; // *****
                   C_v = C_v1;
                   b = Convert.ToBoolean((_server.GetTag("ButtonC12").Value));
                   if (b)
                   {
                       ep = 10; // *****
                       C_v = C_v2;
                   }
                   else
                   {
                       ep = 11; // *****
                       b = Convert.ToBoolean((_server.GetTag("ButtonC08").Value));
                       if (b)
                       {
                           ep = 12; // *****
                           C_v = C_v3;
                       }
                       else
                       {
                           ep = 13; // *****
                           b = Convert.ToBoolean((_server.GetTag("ButtonC06").Value));
                           if (b) C_v = C_v4;
                       }
                   }

                   ep = 14; // *****
                   int D_T_PLC = 10;
                   b = Convert.ToBoolean((_server.GetTag("ButtonT20").Value));
                   if (b)
                   {
                       D_T_PLC = 20;
                   }
                   else
                   {
                       ep = 14; // *****
                       b = Convert.ToBoolean((_server.GetTag("ButtonT30").Value));
                       if (b)
                       {
                           D_T_PLC = 30;
                       }
                       else
                       {
                           ep = 15; // *****
                           b = Convert.ToBoolean((_server.GetTag("ButtonT40").Value));
                           if (b)
                           {
                               D_T_PLC = 40;
                           }
                           else
                           {
                               ep = 16; // *****
                               b = Convert.ToBoolean((_server.GetTag("ButtonT50").Value));
                               if (b)
                               {
                                   D_T_PLC = 50;
                               }
                               else
                               {
                                   ep = 17; // *****
                                   b = Convert.ToBoolean((_server.GetTag("ButtonT60").Value));
                                   if (b)
                                   {
                                       D_T_PLC = 60;
                                   }
                                   else
                                   {
                                       ep = 18; // *****
                                       b = Convert.ToBoolean((_server.GetTag("ButtonT70").Value));
                                       if (b)
                                       {
                                           D_T_PLC = 70;
                                       }
                                       else
                                       {
                                           ep = 19; // *****
                                           b = Convert.ToBoolean((_server.GetTag("ButtonT80").Value));
                                           if (b)
                                           {
                                               D_T_PLC = 80;
                                           }
                                           else
                                           {
                                               ep = 20; // *****
                                               b = Convert.ToBoolean((_server.GetTag("ButtonT90").Value));
                                               if (b)
                                               {
                                                   D_T_PLC = 90;
                                               }
                                           }
                                       }
                                   }
                               }
                           }
                       }
                   }

                   ep = 21; // *****
                   b = Convert.ToBoolean((_server.GetTag("ButtonTplus5").Value));
                   if (b) D_T_PLC = D_T_PLC + 5;

                   T_z = T_f + D_T_PLC;
               }

               ep = 22; // *****
               C_n = Convert.ToSingle((_server.GetTag("k5.c_zadan_ot").Value));

               // Другие параметры
               // ----------------------------------------

               ep = 23; // *****
               h_c = Convert.ToSingle((_server.GetTag("k5.h_c").Value));

               ep = 24; // *****
               Hf_z = Convert.ToSingle((_server.GetTag("k5.h_furm_zadan").Value));

               ep = 25; // *****
               Hf_b = Convert.ToSingle((_server.GetTag("k5.h_furm_bas").Value));

               ep = 26; // ***
               dH = Convert.ToSingle((_server.GetTag("k5.d_h").Value));
               
               ep = 27; // ***
               dT = Convert.ToInt16((_server.GetTag("k5.d_t").Value));

               ep = 28; // ***
               i_o2 = Convert.ToInt16((_server.GetTag("k5.i_o2").Value)); ;

               ep = 29; // ***
               dG = Convert.ToSingle((_server.GetTag("k5.d_g").Value));

               ep = 30; // ***
               K_ohl = Convert.ToSingle((_server.GetTag("k5.k_ohl").Value));

               ep = 31; // ***
               D_T_dop = Convert.ToInt16((_server.GetTag("k5.d_t_dop").Value));
               
               ep = 32; // ***
               K_dT = Convert.ToInt16((_server.GetTag("k5.k_d_t").Value));

               ep = 33; // ***
               Weight_Left = Convert.ToSingle((_server.GetTag("k5.weight_left").Value));

               ep = 34; // ***
               Weight_Right = Convert.ToSingle((_server.GetTag("k5.weight_right").Value));

               // Коэффициенты A и Б
               // ----------------------------------------

               ep = 35; // ***
               a1 = Convert.ToSingle((_server.GetTag("k5.a1").Value));

               ep = 36; // ***
               a2 = Convert.ToSingle((_server.GetTag("k5.a2").Value));
               
               ep = 37; // ***
               a3 = Convert.ToSingle((_server.GetTag("k5.a3").Value));
               
               ep = 38; // ***
               a4 = Convert.ToSingle((_server.GetTag("k5.a4").Value));
               
               ep = 39; // ***
               a5 = Convert.ToSingle((_server.GetTag("k5.a5").Value));

               ep = 40; // ***
               b1 = Convert.ToSingle((_server.GetTag("k5.b1").Value));

               ep = 41; // ***
               b2 = Convert.ToSingle((_server.GetTag("k5.b2").Value));

               ep = 42; // ***
               b3 = Convert.ToSingle((_server.GetTag("k5.b3").Value));

               ep = 43; // ***
               b4 = Convert.ToSingle((_server.GetTag("k5.b4").Value));

               ep = 44; // ***
               b5 = Convert.ToSingle((_server.GetTag("k5.b5").Value));

               ep = 45; // ***
               b6 = Convert.ToSingle((_server.GetTag("k5.b6").Value));
           }

           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] RunOnce(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }


       // --------------------------------------------------------------------------------------------------
       // ПРОВЕРКА ВХОДНЫХ ДАННЫХ

       public bool Check_Input()
       {
           bool b_Check = true;
           bool b_Null = false;

           if (C_f <= 0)
           {
               Info_1 = "Сф";
               b_Null = true;
           }

           if (C_n <= 0)
           {
               Info_1 = "Сн";
               b_Null = true;
           }

           if (C_v <= 0)
           {
               Info_1 = "Св";
               b_Null = true;
           }

           if (Hf_b <= 0)
           {
               Info_1 = "Нфб";
               b_Null = true;
           }

           if (Hf_z <= 0)
           {
               Info_1 = "Нфз";
               b_Null = true;
           }

           if (dH <= 0)
           {
               Info_1 = "dH";
               b_Null = true;
           }

           if (T_f <= 0)
           {
               Info_1 = "Тф";
               b_Null = true;
           }

           if (T_z <= 0)
           {
               Info_1 = "Тз";
               b_Null = true;
           }

           if (dT <= 0)
           {
               Info_1 = "dТ";
               b_Null = true;
           }

           if (D_T_dop <= 0)
           {
               Info_1 = "dТдоп";
               b_Null = true;
           }

           if (h_c <= 0)
           {
               Info_1 = "hC";
               b_Null = true;
           }

           if (dG <= 0)
           {
               Info_1 = "dG";
               b_Null = true;
           }

           if (K_ohl <= 0)
           {
               Info_1 = "Кохл";
               b_Null = true;
           }

           if (K_dT <= 0)
           {
               Info_1 = "КdT";
               b_Null = true;
           }

           if (i_o2 <= 0)
           {
               Info_1 = "IO2";
               b_Null = true;
           }

           if (Weight_Left < 0)
           {
               Info_1 = "Weight_Left &lt; 0";
               Info_2 = "Weight_Left < 0";
               b_Check = false;
           }

           if (Weight_Right < 0)
           {
               Info_1 = "Weight_Right &lt; 0";
               Info_2 = "Weight_Right < 0";
               b_Check = false;
           }

           if (C_n > C_v)
           {
               Info_1 = "Cн &gt; Св";
               Info_2 = "Cн > Св";
               b_Check = false;
           }

           if (C_f > Convert.ToSingle(0.3))
           {
               Info_1 = "Cф &gt; 0,30";
               Info_2 = "Cф > 0,30";
               b_Check = false;
           }

           // Коэффициенты A и B
           // ----------------------------------------

           if (a1 <= 0)
           {
               Info_1 = "A1";
               b_Null = true;
           }

           if (a2 <= 0)
           {
               Info_1 = "A2";
               b_Null = true;
           }

           if (a3 <= 0)
           {
               Info_1 = "A3";
               b_Null = true;
           }

           if (a4 <= 0)
           {
               Info_1 = "A4";
               b_Null = true;
           }

           if (a5 <= 0)
           {
               Info_1 = "A5";
               b_Null = true;
           }

           if (b1 >= 0)
           {
               Info_1 = "B1 &gt;= 0";
               Info_2 = "B1 >= 0";
               b_Check = false;
           }

           if (b2 <= 0)
           {
               Info_1 = "B2";
               b_Null = true;
           }

           if (b3 <= 0)
           {
               Info_1 = "B3";
               b_Null = true;
           }

           if (b4 <= 0)
           {
               Info_1 = "B4";
               b_Null = true;
           }

           if (b5 <= 0)
           {
               Info_1 = "B5";
               b_Null = true;
           }

           if (b6 <= 0)
           {
               Info_1 = "B6";
               b_Null = true;
           }

           // Константы для кнопок
           // ----------------------------------------

           if (C_v1 <= 0)
           {
               Info_1 = "Св1";
               b_Null = true;
           }

           if (C_v2 <= 0)
           {
               Info_1 = "Св2";
               b_Null = true;
           }

           if (C_v3 <= 0)
           {
               Info_1 = "Св3";
               b_Null = true;
           }

           if (C_v4 <= 0)
           {
               Info_1 = "Св4";
               b_Null = true;
           }
           
           if (b_Null)
           {
               Info_2 = Info_1 + " <= 0";
               Info_1 = Info_1 + " &lt;= 0";
               b_Check = false;
           }

           return b_Check;
       }

       // --------------------------------------------------------------------------------------------------
       // РАСЧЕТ

       public void Calc()
       {
           try
           {
               // Инициализация 

               ep = 1; // ***
               D_Gohl = 0;
               G_ohl = 0;
               G1 = 0;
               G2 = 0;
               G3 = 0;
               b_2 = false;
               Step = 1;
               Flag = 0;

               C1 = C_f;
               if (b_C) C2 = C_v; else C2 = C_f;

               T_r_C_n = 0;
               D_Q2O_C_n = 0;
               D_QOdod_C_n = 0;
               T_r_1_G = 0;
               D_T_r_G = 0;
               D_Q1o = 0;
               K_oc_C_v = 0;
               K_ot_C_v = 0;

               // Расчет 1

               if (b_C) Calc_1(true); else b_2 = true;

               // Расчет 2

               if (b_2)
               {
                   D_Q1o_old = D_Q1o;

                   xml = xml + "<r name='-----------------------------------------------------------------------------------------------------------------------'/>";

                   if (b_C)
                       xml = xml + "<r name='2. Расчет дополнительного дутья, выжигающего углерод (от Св и ниже) и повышающего температуру (до Tз)'>";
                   else
                       xml = xml + "<r name='1. Расчет дополнительного дутья, выжигающего углерод (от Св и ниже) и повышающего температуру (до Tз)'>";

                   xml = xml + "</r>";
                   xml = xml + "<r name='-----------------------------------------------------------------------------------------------------------------------'/>";

                   b_Cycle = false;
                   do
                   {
                       ep = 2; // ***
                       if (b_C)
                           C1 = Convert.ToSingle((_server.GetTag("k5.c_zadan_do").Value));
                       else
                           C1 = C_f;

                       ep = 3; // ***
                       C2 = Convert.ToSingle(Math.Round(C2 - h_c, 3));

                       Calc_1(false);

                       ep = 4; // ***
                       if (T_r_2 <= T_z)
                       {
                           if ((C2 == C_n) && (b_C))
                           {
                               T_r_C_n = T_r_2;
                               D_Q2O_C_n = D_Q1o;
                               D_QOdod_C_n = D_Q1o_old + D_Q2O_C_n;
                           }
                           b_Cycle = true;
                           if (b_C)
                               Info_1 = "Тр2 &lt;= Тз. Переходим к следующему шагу";
                           else
                               Info_1 = "Тр &lt;= Тз. Переходим к следующему шагу";
                       }
                       else
                       {
                           D_T_r = T_r_2 - T_z;
                           if (b_C)
                           {
                               if (C2 >= C_n)
                               {
                                   Flag = 2;
                                   Info_1 = "Тр2 &gt; Тз. Переходим к окончательному расчету с признаком 2 (додувка закончится на Cр &gt;= Сн)";
                               }
                               else
                               {
                                   Flag = 3;
                                   Info_1 = "Тр2 &gt; Тз. Переходим к окончательному расчету с признаком 3 (додувка закончится на Cр &lt; Сн)";
                               }
                           }
                           else
                           {
                               Info_1 = "Тр &gt; Тз. Переходим к окончательному расчету";
                           }
                           b_Cycle = false;
                       }

                       ep = 5; // ***
                       Save_Calc_1_2();

                       Step = Step + 1;
                   }
                   while (b_Cycle);

                   D_QOdod = D_Q1o_old + D_Q1o;
               }
               else
               {
                   T_r_2 = T_r_1;
                   D_Q1o_old = D_Q1o;
                   D_Q1o = 0;
                   D_QOdod = D_Q1o_old;
               }

               ep = 6; // ***
               Save_Results_2();

               // Расчет 3

               Calc_3();

               ep = 7; // ***
               Save_Results_3();
           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Calc(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
        }

       // --------------------------------------------------------------------------------------------------
       // РАСЧЕТ 1

       public void Calc_1(bool b_FirstRun)
       {
           try
           {
               // Расчет D_Q1O и D_T

               ep = 1; // ***
               D_H = Convert.ToSingle(Math.Round(Hf_z - Hf_b, 5));
               ep = 2; // ***
               K_oc = Convert.ToSingle((a1 / (a2 + a3 / (C1 * C2))) * (a4 - a5 * D_H));
               ep = 3; // ***
               D_C = Convert.ToSingle(Math.Round(C1 - C2, 5));
               ep = 4; // ***
               D_Q1o = Convert.ToInt16(Math.Round((D_C / K_oc) / 10) * 10);
               ep = 5; // ***
               K_ot = Convert.ToSingle((b1 / (b2 + b3 / (C1 * C2)) + b4) * (b5 + b6 * D_H));
               ep = 6; // ***
               D_T = Convert.ToInt16(Math.Round(K_ot * D_Q1o));

               ep = 7; // ***
               if (b_FirstRun)
               {
                   T_r_1 = T_f + D_T;
                   T_r_1_old = T_r_1;
                   C_r_1 = C2;
                   K_oc_C_v = K_oc;
                   K_ot_C_v = K_ot;
               }
               else
               {
                   if (b_C)
                       T_r_2 = T_r_1 + D_T;
                   else
                       T_r_2 = T_f + D_T;
               }

               // Проверка необходимости охлаждающих добавок (только при первом запуске)

               ep = 8; // ***
               if (b_FirstRun)
               {
                   bool b_Ohl = false;

                   if ((T_r_1 >= T_z) && (T_r_1 <= (T_z + dT)))
                   {
                       D_T_r = T_r_1 - T_z;
                       Flag = 1;
                       Info_1 = "Переходим к окончательному расчету с признаком 1 (додувка закончится на Св)";
                   }
                   else
                   {
                       if (T_r_1 < T_z)
                       {
                           D_T_r = T_r_1 - T_z;
                           b_2 = true;
                           Info_1 = "Тр &lt; Tз. Переходим к расчету дополнительного дутья";
                       }
                       else
                       {
                           b_Ohl = true;
                           Info_1 = "Тр значительно больше Tз. Переходим к расчету охлаждающих добавок";
                           Calc_Ohl();
                       }
                   }

                   // Сохранение результатов

                   ep = 9; // ***
                   Save_Calc_1_1();
                   if (b_Ohl) Save_Calc_Ohl();
                   if (Flag == 0) Save_Results_1();
               }
           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Calc_1(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // РАСЧЕТ ОХЛАЖДАЮЩИХ ДОБАВОК

       public void Calc_Ohl()
       {
           try
           {
               // Инициализация

               ep = 1; // ***
               Info_ohl = "";

               // Определение меньшей или единственной порции

               if ((Weight_Left == 0) && (Weight_Right == 0))
               {
                   G1 = 0;
                   G2 = 0;
               }
               else
               {
                   if (Weight_Left == 0)
                   {
                       G1 = Weight_Right;
                       G2 = 0;
                   }
                   else
                   {
                       if (Weight_Right == 0)
                       {
                           G1 = Weight_Left;
                           G2 = 0;
                       }
                       else
                       {
                           if (Weight_Left < Weight_Right)
                           {
                               G1 = Weight_Left;
                               G2 = Weight_Right;
                           }
                           else
                           {
                               G1 = Weight_Right;
                               G2 = Weight_Left;
                           }
                       }
                   }
               }

               G3 = G1 + G2;

               // Расчет добавок

               ep = 2; // ***
               if (G3 == 0)
               {
                   G_ohl = 0;
                   Flag = 5;
                   Info_ohl = "G3 = 0 (охладители отсутствуют). Переходим к окончательному расчету с признаком 5";
               }
               else
               {
                   string s_G;
                   G = G1;
                   s_G = "G1";

                   do 
                   {
                       b_Cycle = false;

                       ep = 3; // ***
                       D_T_G = Convert.ToInt16(Math.Round(K_ohl * G));
                       T_r_1_G_pred = T_r_1_G;
				       T_r_1_G = T_r_1 - D_T_G;

                       ep = 4; // ***
                       if ((T_r_1_G >= T_z) && (T_r_1_G <= (T_z + dT)))
				       {
					       T_r_1 = T_r_1_G;
					       D_T_r = T_r_1_G - T_z;
					       G_ohl = G;
					       Flag = 1;
					       Info_ohl = "Gохл = " + s_G + ". Переходим к окончательному расчету с признаком 1 (додувка закончится на Св)";
				       }
				       else
				       {
					       if (T_r_1_G < T_z)
					       {
                               ep = 5; // ***
                               bool b_ohl_1, b_ohl_2;
                               string s = "";
                               b_ohl_1 = T_r_1_G < (T_z - D_T_dop);
                               b_ohl_2 = (T_r_1_G_pred > (T_z + dT)) && (T_r_1_G_pred <= (T_z + K_dT * dT));

                               ep = 6; // ***
                               if (b_ohl_1 || b_ohl_2) 
						       {
                                   if (b_ohl_1)
                                       s = "Tр1G значительно меньше Tз. ";
                                   else
                                       s = "Перегрев в пределах допустимого. ";

							       if (G == G1)
							       {
								       D_T_r = T_r_1 - T_z;
                                       D_T_r_G = T_r_1_G - T_z;
                                       G_ohl = 0;
								       Flag = 4;
								       Info_ohl = s + "Переходим к окончательному расчету с признаком 4 (Gохл = 0)";
							       }
							       else
							       {
								       if (G == G2)
								       {
                                           ep = 7; // ***
                                           T_r_1 = T_r_1 - Convert.ToInt16(Math.Round(G1 * K_ohl));
									       D_T_r = T_r_1 - T_z;
                                           D_T_r_G = T_r_1_G - T_z;
                                           G_ohl = G1;
									       Flag = 4;
									       Info_ohl = s + "Переходим к окончательному расчету с признаком 4 (Gохл = G1)";
								       }
								       else
								       {
									       if (G == G3)
									       {
                                               ep = 8; // ***
                                               T_r_1 = T_r_1 - Convert.ToInt16(Math.Round(G2 * K_ohl));
										       D_T_r = T_r_1 - T_z;
                                               D_T_r_G = T_r_1_G - T_z;
                                               G_ohl = G2;
										       Flag = 4;
                                               Info_ohl = s + "Переходим к окончательному расчету с признаком 4 (Gохл = G2)";
									       }
								       }
							       }
						       }
						       else
						       {
							       T_r_1 = T_r_1_G;
							       G_ohl = G;
							       b_2 = true;
							       Info_ohl = "Дефицит тепла в допустимых пределах. Переходим к расчету дополнительного дутья (Gохл = " + s_G + ")";
						       }
					       }
					       else
					       {
						       if (G == G3)
						       {
                                   ep = 9; // ***
                                   T_r_1 = T_r_1_G;
							       D_T_r = T_r_1_G - T_z;
							       G_ohl = G3;
							       Flag = 5;
							       Info_ohl = "Охладителя в бункерах недостаточно. Переходим к окончательному расчету с признаком 5 (Gохл = G3)";
						       }
						       else
						       {
							       if (G2 == 0)
							       {
								       T_r_1 = T_r_1_G;
								       D_T_r = T_r_1_G - T_z;
								       G_ohl = G;
								       Flag = 5;
								       Info_ohl = "Охладителя в бункерах недостаточно. Переходим к окончательному расчету с признаком 5 (Gохл = " + s_G + ")";
							       }
							       else
							       {
                                       if (G == G2)
                                       {
                                           G = G3;
                                           s_G = "G3";
                                           b_Cycle = true;
                                       }
                                       else
                                       {
                                           G = G2;
                                           s_G = "G2";
                                           b_Cycle = true;
                                       }
							       }
						       }
					       }
				       }
			       }
                   while (b_Cycle);
               }
           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Calc_Ohl(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // РАСЧЕТ 3

       public void Calc_3()
       {
           try
           {
               // Описание

               if (b_C)
               {
                   switch (Flag)
                   {
                       case 1:
                           if (G_ohl > 0)
                               Info_1 = "Вариант додувки с признаком 1 (с отдачей дополнительной охлаждающей добавки) предполагает выжечь углерод до верхней границы и отдать охладитель, получив при этом заданную температуру металла";
                           else
                               Info_1 = "Вариант додувки с признаком 1 (без отдачи дополнительной охлаждающей добавки) предполагает выжечь углерод до верхней границы, получив при этом заданную температуру металла";
                           break;
                       case 2:
                           if (G_ohl > 0)
                               Info_1 = "Вариант додувки с признаком 2 (с отдачей дополнительной охлаждающей добавки) предполагает выжечь углерод до значения между верхней и нижней границей и отдать охладитель, получив при этом заданную температуру металла";
                           else
                               Info_1 = "Вариант додувки с признаком 2 (без отдачи дополнительной охлаждающей добавки) предполагает выжечь углерод до значения между верхней и нижней границей, получив при этом заданную температуру металла";
                           break;
                       case 3:
                           Info_1 = "Вариант додувки с признаком 3 предполагает выжечь углерод до значения ниже нижней границы, получив при этом заданную температуру металла";
                           break;
                       case 4:
                           Info_1 = "Вариант додувки с признаком 4 предполагает выжечь углерод до значения верхней границы, "
                               + "получив при этом заданную температуру металла только при отдаче рекомендуемого количества охладителя, но добавки которого значительно превышают рекомендованное количество";
                           break;
                       case 5:
                           Info_1 = "Вариант додувки с признаком 5 предполагает выжечь углерод до значения верхней границы, получив при этом заданную температуру металла "
                               + "только при отдаче рекомендуемого охладителя, но добавок которого в промбункерах недостаточное количество";
                           break;
                       default:
                           Info_1 = "Неверный признак";
                           break;
                   }
               }
               else
               {
                   Info_1 = "";
               }

               // Расчет

               ep = 1; // ***
               tst_r = T_r_2;
               tst_r1 = T_r_1_old;
               tst_r1g = T_r_1_G;
               d_tst_rg = D_T_r_G;
               d_tst_r = D_T_r;
               d_q_r1 = D_Q1o_old;
               d_q_r2 = D_Q1o;

               d_g_ohl = D_Gohl;
               _g_ohl = G_ohl;
               _g1 = G1;
               _g2 = G2;

               tst_rn = T_r_C_n;
               d_q_rn = D_Q2O_C_n;
               d_q_dod_rn = D_QOdod_C_n;

               xim_c_r = C2;
               l_bunker = false;
               r_bunker = false;

               // Коэффициенты передачи

               k_oc_cv = K_oc_C_v;
               k_ot_cv = K_ot_C_v;

               Single C1_ = C_v;
               Single C2_ = xim_c_r;
               ep = 11; // ***
               Single D_H_ = Convert.ToSingle(Math.Round(Hf_z - Hf_b, 5));
               ep = 12; // ***
               k_oc_cvcr = Convert.ToSingle((a1 / (a2 + a3 / (C1_ * C2_))) * (a4 - a5 * D_H_));
               ep = 13; // ***
               k_ot_cvcr = Convert.ToSingle((b1 / (b2 + b3 / (C1_ * C2_)) + b4) * (b5 + b6 * D_H_));

               ep = 2; // ***
               if (G_ohl > 0)
               {
                   if (G_ohl == G3)
                   {
                       l_bunker = true;
                       r_bunker = true;
                   }
                   else
                   {
                       if (G_ohl == Weight_Left) l_bunker = true; else r_bunker = true;
                   }
               }

               ep = 3; // ***
               time_doduv = Convert.ToInt16(Math.Round(Convert.ToSingle(60 * D_QOdod / i_o2)));

               ep = 4; // ***
               h_furm_1 = Hf_z;
               h_furm_2 = Hf_z - dH;
               i_o2_1 = i_o2;
               i_o2_2 = i_o2;
               ep = 5; // ***
               d_q_1 = Convert.ToInt16(Math.Round(Convert.ToSingle(D_QOdod / 2) / 10) * 10);
               d_q_2 = D_QOdod;
           }

           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Calc_3(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ ИНФОРМАЦИИ ОБ ОШИБКЕ ВВОДА

       public void Save_Error()
       {
           try
           {
               string s_Error = "-- CALC -- Расчет отменен. Ошибка ввода: " + Info_2;
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);

               xml = xml + "<param name='Ошибка ввода' value='" + Info_1 + "'/>";

               Tag t = _server.GetTag("k5.raschet_info");
               t.Value = xml;
               _server.WriteTag(t);

               // Биты событий

               t = _server.GetTag("k5.start_raschet");
               t.Value = false;
               _server.WriteTag(t);

               if (!b_PLC)
               {
                   ep = 31; // ***
                   t = _server.GetTag("k5.raschet_to_plc");
                   b_PLC = Convert.ToBoolean(t.Value);
               }

               if (b_PLC)
               {
                   t = _server.GetTag("ButtonCalcLancing");
                   t.Value = false;
                   _server.WriteTag(t);
               }
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Error: " + e.ToString());
           }
       }
       
       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ ВХОДНОЙ ИНФОРМАЦИИ

       public void Save_Input()
       {
           try
           {
               xml = xml + "<in name='Входная информация'>";
               xml = xml + "<param name='Сф' value='" + C_f + "'/>";
               if (b_C)
               {
                   xml = xml + "<param name='Сн' value='" + C_n + "'/>";
                   xml = xml + "<param name='Св' value='" + C_v + "'/>";
               }
               xml = xml + "<param name='hС' value='" + _server.GetTag("k5.h_c").Value + "'/>";
               xml = xml + "<param name='Tф' value='" + _server.GetTag("k5.tst").Value + "'/>";
               xml = xml + "<param name='Tз' value='" + T_z + "'/>";
               if (b_C)
               {
                   xml = xml + "<param name='dT' value='" + _server.GetTag("k5.d_t").Value + "'/>";
                   xml = xml + "<param name='dTдоп' value='" + _server.GetTag("k5.d_t_dop").Value + "'/>";
                   xml = xml + "<param name='KdT' value='" + _server.GetTag("k5.k_d_t").Value + "'/>";
               }
               xml = xml + "<param name='Hф' value='" + _server.GetTag("k5.h_furm_zadan").Value + "'/>";
               xml = xml + "<param name='Hфб' value='" + _server.GetTag("k5.h_furm_bas").Value + "'/>";
               xml = xml + "<param name='dH' value='" + _server.GetTag("k5.d_h").Value + "'/>";
               xml = xml + "<param name='IO2' value='" + _server.GetTag("k5.i_o2").Value + "'/>";
               if (b_C)
               {
                   xml = xml + "<param name='Левый ПБ' value='" + _server.GetTag("k5.weight_left").Value + "'/>";
                   xml = xml + "<param name='Правый ПБ' value='" + _server.GetTag("k5.weight_right").Value + "'/>";
                   xml = xml + "<param name='dG' value='" + _server.GetTag("k5.d_g").Value + "'/>";
                   xml = xml + "<param name='Кохл' value='" + _server.GetTag("k5.k_ohl").Value + "'/>";
               }
               xml = xml + "<param name='A1' value='" + a1 + "'/>";
               xml = xml + "<param name='A2' value='" + a2 + "'/>";
               xml = xml + "<param name='A3' value='" + a3 + "'/>";
               xml = xml + "<param name='A4' value='" + a4 + "'/>";
               xml = xml + "<param name='A5' value='" + a5 + "'/>";
               xml = xml + "<param name='B1' value='" + b1 + "'/>";
               xml = xml + "<param name='B2' value='" + b2 + "'/>";
               xml = xml + "<param name='B3' value='" + b3 + "'/>";
               xml = xml + "<param name='B4' value='" + b4 + "'/>";
               xml = xml + "<param name='B5' value='" + b5 + "'/>";
               xml = xml + "<param name='B6' value='" + b6 + "'/>";
               xml = xml + "</in>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Input: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ ХОДА РАСЧЕТА 1 (ПЕРВЫЙ ЗАПУСК)

       public void Save_Calc_1_1()
       {
           try
           {
               xml = xml + "<r name='------------------------------------------------------------------------------------'/>";
               xml = xml + "<r name='1. Расчет параметров додувки для достижения верхней границы по углероду'/>";
               xml = xml + "<r name='------------------------------------------------------------------------------------'>";
               xml = xml + "<param name='С1' value='" + C1 + "'/>";
               xml = xml + "<param name='С2' value='" + C2 + "'/>";
               xml = xml + "<param name='D_H' value='" + D_H + "'/>";
               xml = xml + "<param name='Ko-c' value='" + K_oc + "'/>";
               xml = xml + "<param name='D_C' value='" + D_C + "'/>";
               xml = xml + "<param name='D_Q1O' value='" + D_Q1o + "'/>";
               xml = xml + "<param name='Ko-t' value='" + K_ot + "'/>";
               xml = xml + "<param name='D_T' value='" + D_T + "'/>";
               xml = xml + "<param name='Tр' value='" + T_r_1_old + "'/>";
               xml = xml + "<param name='----- Итог -----' value='" + Info_1 + "'/>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Calc_1_1: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ ХОДА РАСЧЕТА 1 (ПОСЛЕДУЮЩИЕ ЗАПУСКИ)

       public void Save_Calc_1_2()
       {
           try
           {
               xml = xml + "<r name='Расчет параметров додувки (Шаг " + Step + ")'>";
               xml = xml + "<param name='С1' value='" + C1 + "'/>";
               xml = xml + "<param name='С2' value='" + C2 + "'/>";
               xml = xml + "<param name='D_H' value='" + D_H + "'/>";
               xml = xml + "<param name='Ko-c' value='" + K_oc + "'/>";
               xml = xml + "<param name='D_C' value='" + D_C + "'/>";

               if (b_C)
                   xml = xml + "<param name='D_Q2O' value='" + D_Q1o + "'/>";
               else
                   xml = xml + "<param name='D_QO' value='" + D_Q1o + "'/>";

               xml = xml + "<param name='Ko-t' value='" + K_ot + "'/>";
               xml = xml + "<param name='D_T' value='" + D_T + "'/>";

               if (b_C)
                   xml = xml + "<param name='Tр2' value='" + T_r_2 + "'/>";
               else
                   xml = xml + "<param name='Tр' value='" + T_r_2 + "'/>";

               xml = xml + "<param name='----- Итог -----' value='" + Info_1 + "'/>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Calc_1_2: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ ХОДА РАСЧЕТА ОХЛАЖДАЮЩИХ ДОБАВОК

       public void Save_Calc_Ohl()
       {
           try
           {
               xml = xml + "<r name='1.7.4. Расчет охлаждающих добавок'>";
               xml = xml + "<param name='G1' value='" + G1 + "'/>";
               xml = xml + "<param name='G2' value='" + G2 + "'/>";
               xml = xml + "<param name='G3' value='" + G3 + "'/>";
               xml = xml + "<param name='Тр' value='" + T_r_1 + "'/>";
               xml = xml + "<param name='D_Тр' value='" + D_T_r + "'/>";
               if (Flag == 4) xml = xml + "<param name='D_Tр1G' value='" + D_T_r_G + "'/>";
               xml = xml + "<param name='Тр1' value='" + T_r_1_old + "'/>";
               xml = xml + "<param name='Тр1Gпред' value='" + T_r_1_G_pred + "'/>";
               xml = xml + "<param name='Тр1G' value='" + T_r_1_G + "'/>";
               xml = xml + "<param name='Gохл' value='" + G_ohl + "'/>";
               xml = xml + "<param name='----- Итог -----' value='" + Info_ohl + "'/>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Ohl: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ РЕЗУЛЬТАТОВ РАСЧЕТА 1

       public void Save_Results_1()
       {
           try
           {
               xml = xml + "<r name='Выходная информация'>";
               xml = xml + "<param name='Ср' value='" + C_r_1 + "'/>";
               xml = xml + "<param name='Tр' value='" + T_r_1 + "'/>";
               xml = xml + "<param name='D_Tр' value='" + D_T_r + "'/>";
               xml = xml + "<param name='D_Q1O' value='" + D_Q1o + "'/>";
               xml = xml + "<param name='Gохл' value='" + G_ohl + "'/>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Results_1: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ РЕЗУЛЬТАТОВ РАСЧЕТА 2

       public void Save_Results_2()
       {
           try
           {
               xml = xml + "<r name='Выходная информация'>";
               if (b_C) xml = xml + "<param name='Признак' value='" + Flag + "'/>";
               xml = xml + "<param name='Ср' value='" + C2 + "'/>";
               xml = xml + "<param name='Tр' value='" + T_r_2 + "'/>";
               xml = xml + "<param name='D_Tр' value='" + D_T_r + "'/>";
               if (Flag == 4) xml = xml + "<param name='D_Tр1G' value='" + D_T_r_G + "'/>";

               if (b_C)
               {
                   xml = xml + "<param name='D_Q1O' value='" + D_Q1o_old + "'/>";
                   xml = xml + "<param name='D_Q2O' value='" + D_Q1o + "'/>";
               }
               else
                   xml = xml + "<param name='D_QO' value='" + D_Q1o + "'/>";

               xml = xml + "<param name='D_QОдод' value='" + D_QOdod + "'/>";
               if (b_C) xml = xml + "<param name='Gохл' value='" + G_ohl + "'/>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Results_2: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ РЕЗУЛЬТАТОВ РАСЧЕТА 3

       public void Save_Results_3()
       {
           try
           {
               xml = xml + "<r name='---------------------------------------------------'/>";

               if (b_C)
                   xml = xml + "<r name='3. Окончательный расчет параметров додувки'/>";
               else
                   xml = xml + "<r name='2. Окончательный расчет параметров додувки'/>";

               xml = xml + "<r name='---------------------------------------------------'/>";
               if (b_C) xml = xml + "<param name='Описание' value='" + Info_1 + "'/>";
               xml = xml + "<r name='Выходная информация'>";
               if (b_C) xml = xml + "<param name='Признак' value='" + Flag + "'/>";
               xml = xml + "<param name='Сф' value='" + C_f + "'/>";
               xml = xml + "<param name='Ср' value='" + xim_c_r + "'/>";
               xml = xml + "<param name='Tз' value='" + T_z + "'/>";

               if (G_ohl > 0)
               {
                   xml = xml + "<param name='Tр1' value='" + tst_r1 + "'/>";
                   xml = xml + "<param name='Tр1G' value='" + tst_r1g + "'/>";
               }

               xml = xml + "<param name='Tр' value='" + tst_r + "'/>";
               xml = xml + "<param name='D_Tр' value='" + d_tst_r + "'/>";
               if (Flag == 4) xml = xml + "<param name='D_Tр1G' value='" + d_tst_rg + "'/>";

               xml = xml + "<param name='Ko-c Cв-Cр' value='" + k_oc_cvcr + "'/>";
               xml = xml + "<param name='Ko-t Cв-Cр' value='" + k_ot_cvcr + "'/>";

               if (G_ohl > 0)
               {
                   xml = xml + "<param name='D_Gохл' value='" + d_g_ohl + "'/>";
                   xml = xml + "<param name='Gохл' value='" + _g_ohl + "'/>";
                   xml = xml + "<param name='G1' value='" + _g1 + "'/>";
                   xml = xml + "<param name='G2' value='" + _g2 + "'/>";
               }

               if (b_2 && b_C) xml = xml + "<param name='D_Q1O' value='" + d_q_r1 + "'/>";

               if (Flag == 3)
               {
                   xml = xml + "<param name='ТрСн' value='" + T_r_C_n + "'/>";
                   xml = xml + "<param name='D_Q2OСн' value='" + d_q_rn + "'/>";
                   xml = xml + "<param name='D_Q2OдодСн' value='" + D_QOdod_C_n + "'/>";
               }

               xml = xml + "<param name='D_QOдод' value='" + D_QOdod + "'/>";
               xml = xml + "<param name='tдод' value='" + time_doduv + " сек'/>";

               xml = xml + "<param name='----- Шаблон додувки (Шаг 1)'>";
               xml = xml + "<param name='Hф' value='" + h_furm_1 + "'/>";
               xml = xml + "<param name='IO2' value='" + i_o2_1 + "'/>";
               xml = xml + "<param name='DQO2' value='" + d_q_1 + "'/>";

               if (l_bunker)
                   xml = xml + "<param name='Левый ПБ' value='отдать'/>";
               else
                   xml = xml + "<param name='Левый ПБ' value='не отдавать'/>";

               if (r_bunker)
                   xml = xml + "<param name='Правый ПБ' value='отдать'/>";
               else
                   xml = xml + "<param name='Правый ПБ' value='не отдавать'/>";

               xml = xml + "</param>";
               xml = xml + "<param name='----- Шаблон додувки (Шаг 2)'>";
               xml = xml + "<param name='Hф' value='" + h_furm_2 + "'/>";
               xml = xml + "<param name='IO2' value='" + i_o2_2 + "'/>";
               xml = xml + "<param name='DQO2' value='" + d_q_2 + "'/>";
               xml = xml + "</param>";
               xml = xml + "</r>";
           }
           catch (Exception e)
           {
               _server.LogWrite(LogType.ERROR, "[" + this._name + "] Save_Results_3: " + e.ToString());
           }
       }

       // --------------------------------------------------------------------------------------------------
       // СОХРАНЕНИЕ РЕЗУЛЬТАТОВ

       public void Save_Results()
       {
           try
           {
               Tag t;

               // Расчетные тэги

               ep = 1; // ***
               t = _server.GetTag("k5.num_raschet");
               t.Value = Convert.ToInt16(t.Value) + 1;
               _server.WriteTag(t);

               ep = 2; // ***
               t = _server.GetTag("k5.priznak_raschet");
               t.Value = Flag;
               _server.WriteTag(t);

               ep = 3; // ***
               t = _server.GetTag("k5.desc_raschet");
               t.Value = Info_1;
               _server.WriteTag(t);

               ep = 4; // ***
               t = _server.GetTag("k5.tst_r");
               t.Value = tst_r;
               _server.WriteTag(t);

               ep = 5; // ***
               t = _server.GetTag("k5.tst_r1");
               t.Value = tst_r1;
               _server.WriteTag(t);

               ep = 6; // ***
               t = _server.GetTag("k5.tst_r1g");
               t.Value = tst_r1g;
               _server.WriteTag(t);

               ep = 7; // ***
               t = _server.GetTag("k5.d_tst_rg");
               t.Value = d_tst_rg;
               _server.WriteTag(t);

               ep = 8; // ***
               t = _server.GetTag("k5.tst_rn");
               t.Value = tst_rn;
               _server.WriteTag(t);

               ep = 9; // ***
               t = _server.GetTag("k5.d_tst_r");
               t.Value = d_tst_r;
               _server.WriteTag(t);

               ep = 10; // ***
               t = _server.GetTag("k5.xim.c_r");
               t.Value = xim_c_r;
               _server.WriteTag(t);

               ep = 11; // ***
               t = _server.GetTag("k5.d_g_ohl");
               t.Value = d_g_ohl;
               _server.WriteTag(t);

               ep = 12; // ***
               t = _server.GetTag("k5.g_ohl");
               t.Value = _g_ohl;
               _server.WriteTag(t);

               ep = 13; // ***
               t = _server.GetTag("k5.g1");
               t.Value = _g1;
               _server.WriteTag(t);

               ep = 14; // ***
               t = _server.GetTag("k5.g2");
               t.Value = _g2;
               _server.WriteTag(t);

               ep = 15; // ***
               t = _server.GetTag("k5.time_doduv");
               t.Value = time_doduv;
               _server.WriteTag(t);

               ep = 16; // ***
               t = _server.GetTag("k5.d_q_r1");
               t.Value = d_q_r1;
               _server.WriteTag(t);

               ep = 161; // ***
               t = _server.GetTag("k5.d_q_r2");
               t.Value = d_q_r2;
               _server.WriteTag(t);

               ep = 17; // ***
               t = _server.GetTag("k5.d_q_rn");
               t.Value = d_q_rn;
               _server.WriteTag(t);

               ep = 18; // ***
               t = _server.GetTag("k5.d_q_dod_rn");
               t.Value = d_q_dod_rn;
               _server.WriteTag(t);

               ep = 19; // ***
               t = _server.GetTag("k5.h_furm_1");
               t.Value = h_furm_1;
               _server.WriteTag(t);

               ep = 20; // ***
               t = _server.GetTag("k5.h_furm_2");
               t.Value = h_furm_2;
               _server.WriteTag(t);

               ep = 21; // ***
               t = _server.GetTag("k5.i_o2_1");
               t.Value = i_o2_1;
               _server.WriteTag(t);

               ep = 22; // ***
               t = _server.GetTag("k5.i_o2_2");
               t.Value = i_o2_2;
               _server.WriteTag(t);

               ep = 23; // ***
               t = _server.GetTag("k5.d_q_1");
               t.Value = d_q_1;
               _server.WriteTag(t);

               ep = 24; // ***
               t = _server.GetTag("k5.d_q_2");
               t.Value = d_q_2;
               _server.WriteTag(t);

               ep = 25; // ***
               t = _server.GetTag("k5.l_bunker");
               t.Value = l_bunker;
               _server.WriteTag(t);

               ep = 26; // ***
               t = _server.GetTag("k5.r_bunker");
               t.Value = r_bunker;
               _server.WriteTag(t);

               ep = 261; // ***
               t = _server.GetTag("k5.k_oc_cv");
               t.Value = k_oc_cv;
               _server.WriteTag(t);

               ep = 262; // ***
               t = _server.GetTag("k5.k_ot_cv");
               t.Value = k_ot_cv;
               _server.WriteTag(t);

               ep = 263; // ***
               t = _server.GetTag("k5.k_oc_cvcr");
               t.Value = k_oc_cvcr;
               _server.WriteTag(t);

               ep = 264; // ***
               t = _server.GetTag("k5.k_ot_cvcr");
               t.Value = k_ot_cvcr;
               _server.WriteTag(t);

               // Ход расчета

               ep = 27; // ***
               string xml2 = "<r name='Номер расчета' value='" + _server.GetTag("k5.num_raschet").Value + "'/>";
               if (b_C)
               {
                   xml2 = xml2 + "<r name='----------------------------------'/>";
                   xml2 = xml2 + "<r name='Вид' value='Додувка на углерод'/>";
                   xml2 = xml2 + "<r name='----------------------------------'/>";
                   xml = xml2 + xml;
               }
               else
               {
                   xml2 = xml2 + "<r name='---------------------------------------'/>";
                   xml2 = xml2 + "<r name='Вид' value='Додувка на температуру'/>";
                   xml2 = xml2 + "<r name='---------------------------------------'/>";
                   xml = xml2 + xml;
               }

               ep = 28; // ***
               t = _server.GetTag("k5.raschet_info");
               t.Value = xml;
               _server.WriteTag(t);

               // Результаты расчета в PLC

               if (!b_PLC)
               {
                   ep = 31; // ***
                   t = _server.GetTag("k5.raschet_to_plc");
                   b_PLC = Convert.ToBoolean(t.Value);
               }
               else
               {
                   // Сохранение заданных параметров (при старте из PLC)

                   ep = 311; // ***
                   t = _server.GetTag("k5.c_zadan_do");
                   t.Value = C_v;
                   _server.WriteTag(t);

                   ep = 312; // ***
                   t = _server.GetTag("k5.tst_zadan");
                   t.Value = T_z;
                   _server.WriteTag(t);
               }
               
               if (b_PLC)
               {
                   ep = 313; // ***
                   t = _server.GetTag("WatchdogCalcCounter");
                   if (Convert.ToInt16(t.Value) == max_watchdog_count) t.Value = 1;
                   else t.Value = Convert.ToInt16(t.Value) + 1;
                   _server.WriteTag(t);

                   ep = 32; // ***
                   t = _server.GetTag("Result.T");
                   t.Value = tst_r;
                   _server.WriteTag(t);

                   ep = 33; // ***
                   t = _server.GetTag("Result.C");
                   t.Value = xim_c_r*100;
                   _server.WriteTag(t);

                   ep = 34; // ***
                   t = _server.GetTag("Result.DT");
                   t.Value = d_tst_r;
                   _server.WriteTag(t);

                   ep = 35; // ***
                   t = _server.GetTag("Result.Time.Min");
                   t.Value = time_doduv / 60;
                   _server.WriteTag(t);

                   ep = 351; // ***
                   t = _server.GetTag("Result.Time.Sec");
                   t.Value = time_doduv % 60;
                   _server.WriteTag(t);

                   ep = 36; // ***
                   t = _server.GetTag("PatternStep1.H");
                   t.Value = h_furm_1;
                   _server.WriteTag(t);

                   ep = 37; // ***
                   t = _server.GetTag("PatternStep2.H");
                   t.Value = h_furm_2;
                   _server.WriteTag(t);

                   ep = 38; // ***
                   t = _server.GetTag("PatternStep1.I");
                   t.Value = i_o2_1;
                   _server.WriteTag(t);

                   ep = 39; // ***
                   t = _server.GetTag("PatternStep2.I");
                   t.Value = i_o2_2;
                   _server.WriteTag(t);

                   ep = 40; // ***
                   t = _server.GetTag("PatternStep1.Q");
                   t.Value = d_q_1;
                   _server.WriteTag(t);

                   ep = 41; // ***
                   t = _server.GetTag("PatternStep2.Q");
                   t.Value = d_q_2;
                   _server.WriteTag(t);

                   ep = 42; // ***
                   t = _server.GetTag("PatternBinL");
                   t.Value = l_bunker;
                   _server.WriteTag(t);

                   ep = 43; // ***
                   t = _server.GetTag("PatternBinR");
                   t.Value = r_bunker;
                   _server.WriteTag(t);
               }

               // Биты событий

               t = _server.GetTag("k5.start_raschet");
               t.Value = false;
               _server.WriteTag(t);

               if (b_PLC)
               {
                   t = _server.GetTag("ButtonCalcLancing");
                   t.Value = false;
                   _server.WriteTag(t);
               }

           }
           catch (Exception e)
           {
               string s_Error = "[" + this._name + "] Save_Results(Point " + ep.ToString() + "): " + e.ToString();
               _server.LogWrite(LogType.ERROR, s_Error);
               Save_CalcError(s_Error);
           }
       }

       // --------------------------------------------------------------------------------------------------
       // ЗАПИСЬ ОШИБКИ РАСЧЕТА

       public void Save_CalcError(string s_error)
       {
           if (xml_error.Length > max_xmlerror_count) xml_error = "";

           if (s_error.Length == 0)
               xml_error = "";
           else
               xml_error = xml_error + "<error> <![CDATA[ " + DateTime.Now.ToString() + " " + s_error + " ]]></error>";

           Tag t = _server.GetTag("k5.raschet_errors");
           t.Value = xml_error;
           _server.WriteTag(t);

       }
    }
}
