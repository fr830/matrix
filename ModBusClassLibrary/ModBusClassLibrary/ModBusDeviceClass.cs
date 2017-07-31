using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Modbus_Poll_CS;

namespace ModBusDevice
{
    public class ModBusDeviceClass
    {
        private modbus mb = new modbus();

       
        ~ModBusDeviceClass()
        {
            mb.Close();
        }        

        public string ReadVariables(string comport,
                                    string NameService,
                                    string numkonv,
                                    int addr1, 
                                    int addr2, 
                                    int addr3,
                                    int addr4,
                                    out string dataXML)
        {

            string _log = null;

            dataXML = GetTag("NUMKONV", Convert.ToString(numkonv));

            int error = 1;
            ushort CountReg = 18;
            ushort pollStart = 0;
                        
            CultureInfo myCI = new CultureInfo("ru-RU", false);
            myCI.NumberFormat.NumberDecimalSeparator = ".";

            //Открытие com-порта

            string[] IniComPort = comport.Split(';');

            if (mb.Open(IniComPort[0],
                        Convert.ToInt16(IniComPort[1]),
                        Convert.ToInt16(IniComPort[2]),
                        Convert.ToInt16(IniComPort[3]),
                        Convert.ToInt16(IniComPort[4]),
                        Convert.ToInt16(IniComPort[5]),
                        ref _log)
                )
            {

                //Create array to accept read values:
                short[] values = new short[CountReg];

            
                //Read registers and display data in desired format:
                try
                {
                    if (mb.SendFc3(1, pollStart, CountReg, ref values, ref _log))
                    {                        
                        
                            
                        int intValue = (int)values[addr1];
                        intValue <<= 16;
                        intValue += (int)values[addr1 + 1];

                        dataXML = dataXML + GetTag("LOADING", Convert.ToString(BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0), myCI));

                        intValue = (int)values[addr2];
                        intValue <<= 16;
                        intValue += (int)values[addr2 + 1];

                        dataXML = dataXML + GetTag("PRODUCTIVITY", Convert.ToString(BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0), myCI));

                        intValue = (int)values[addr3];
                        intValue <<= 16;
                        intValue += (int)values[addr3 + 1];

                        dataXML = dataXML + GetTag("SPEED", Convert.ToString(BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0), myCI));

                        intValue = (int)values[addr4];
                        intValue <<= 16;
                        intValue += (int)values[addr4 + 1];

                        dataXML = dataXML + GetTag("COUNTER", Convert.ToString(BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0), myCI));

                        error = 0;



                        
                    }
                }
                catch (Exception err)
                {
                    _log = _log + " Error in modbus read: " + err.Message;                    
                }

            }

            dataXML = dataXML + GetTag("ERROR", Convert.ToString(error)) +
                                GetTag("DTREAD", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));

            dataXML = "<BODY>" + dataXML + "</BODY>";

//            _log = _log + " VAR1_XML: " + dataXML;
            if (_log != null)
                _log = NameService + ":  " + _log;
        
            return _log;
        }

        //Получить Tag XML
        private string GetTag(string tagname, string value)
        {
            return string.Format("<{0}>{1}</{0}>", tagname, value);
        }    
    
    }


}
