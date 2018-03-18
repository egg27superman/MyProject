using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;


namespace AFH.Barcaldine.Language
{
    public class LanguageCommon
    {
        public static Type ResourceType()
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == LanguageVariable.LanguageName.English) 
            {
                return typeof(Resources.en_AU);
            }
            else if (Thread.CurrentThread.CurrentUICulture.Name == LanguageVariable.LanguageName.SimplifiedChinese) 
            {
                return typeof(Resources.zh_CN);
            }
            else if (Thread.CurrentThread.CurrentUICulture.Name == LanguageVariable.LanguageName.TraditionalChinese)
            {
                return typeof(Resources.zh_HK);
            }
            return typeof(Resources.en_AU);
        }

        public static string GetLangString(string resourceName)
        {
            Type resource = ResourceType();
            PropertyInfo p = resource.GetProperty(resourceName);
            if (p != null)
                return p.GetValue(null, null).ToString();
            else
                return string.Empty;

        }
    }
}
