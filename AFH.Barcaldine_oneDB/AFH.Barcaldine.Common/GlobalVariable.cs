using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Common
{
    public class GlobalVariable
    {
        public struct LanguageName
        {
            public const string English = "en-AU";
            public const string SimplifiedChinese = "zh-CN";
            public const string TraditionalChinese = "zh-HK";
        }

        public struct UpdateStatus
        {
            public const string View = "View";
            public const string Add = "ADD";
            public const string Update = "Update";
        }

        public struct Log
        {
            public const string Online = "Online";
            public const string Offline = "Offline";
        }

        public struct WineImageType
        {
            public const int List = 0;
            public const int Detail = 1;
        }

        public struct RoomRateType
        {
            public const int Basic = 0;
            public const int Discount = 1;

        }

        public struct RoomType
        {
            public const int King = 0;
            public const int Queen = 1;
            public const int Princess = 2;

        }

        public enum OrderStatus
        {
            Init = 0,
            Success =1,
            Fail=2,
            Exception=3
        }

        public enum OrderType
        {
            Room=0,
            Wine=1
        }

        public enum ProcessType
        {
            Online = 0,
            Offline = 1
        }

    }
}
