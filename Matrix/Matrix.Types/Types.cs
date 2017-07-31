namespace Matrix.Types
{
    //Форматы дат
    public class DateTimeFormat
    {
        public static string RUSSIANLONGFORMAT = "dd.MM.yy HH:mm:ss.fff";
    }

    //Тип тэга
    public enum TagType: byte
    {
        Object=0,Boolean=1, Byte=2, Integer=3, Float=4, DateTime=5, String=6, XML=7
    }

    //Тип качества
    public enum QualityType : byte
    {
        //GOOD = 192, BAD = 0
        BAD = 0x0000,	// STATUS_MASK Values for Quality = BAD 
        CONFIG_ERROR = 0x0004,
        NOT_CONNECTED = 0x0008,
        DEVICE_FAILURE = 0x000c,
        SENSOR_FAILURE = 0x0010,
        LAST_KNOWN = 0x0014,
        COMM_FAILURE = 0x0018,
        OUT_OF_SERVICE = 0x001C,

        UNCERTAIN = 0x0040,	// STATUS_MASK Values for Quality = UNCERTAIN 
        LAST_USABLE = 0x0044,
        SENSOR_CAL = 0x0050,
        EGU_EXCEEDED = 0x0054,
        SUB_NORMAL = 0x0058,

        GOOD = 0x00C0,	// STATUS_MASK Value for Quality = GOOD 
        LOCAL_OVERRIDE = 0x00D8
    }

    //Тип уровня вывода сообщений
    public enum LogType : byte
    {
        ERROR = 0, WARN = 1, INFO = 2, DEBUG = 3, CONSOLE = 4 
    }

    public class Functions
    {
        public static string GetTagType(TagType tagtype)
        {
            string type = "";
            switch (tagtype)
            {
                case TagType.Byte:
                    type = "byte";
                    break;
                case TagType.Boolean:
                    type = "bool";
                    break;
                case TagType.Integer:
                    type = "int";
                    break;
                case TagType.Float:
                    type = "float";
                    break;
                case TagType.String:
                    type = "string";
                    break;
                case TagType.DateTime:
                    type = "datetime";
                    break;
                case TagType.XML:
                    type = "xml";
                    break;
            }
            return type;
        }

        public static string GetLogType(LogType logtype)
        {
            string type = "";
            switch (logtype)
            {
                case LogType.CONSOLE:
                    type = "CONSOLE";
                    break;
                case LogType.DEBUG:
                    type = "DEBUG";
                    break;
                case LogType.ERROR:
                    type = "ERROR";
                    break;
                case LogType.INFO:
                    type = "INFO";
                    break;
                case LogType.WARN:
                    type = "WARN";
                    break;
            }
            return type;
        }
    }
}