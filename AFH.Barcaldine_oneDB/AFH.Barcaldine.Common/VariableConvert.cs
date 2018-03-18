using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Common
{
    public class VariableConvert
    {
        public static object ConvertStringToDBValue(string str)
        {
            if (str == null)
            {
                return DBNull.Value;
            }
            else
            {
                return str;
            }
        }

        public static object ConvertDoubleToDBValue(double? num)
        {
            if (num == null)
            {
                return DBNull.Value;
            }
            else
            {
                return num;
            }
        }

        public static object BitConverter(bool MyisShow)
        {
            int isShow1;

            if (MyisShow == true)
            {
                isShow1 = 1;
                return isShow1;
            }
            else
            {
                isShow1 = 0;
                return isShow1;
            }
        }
    }
}
