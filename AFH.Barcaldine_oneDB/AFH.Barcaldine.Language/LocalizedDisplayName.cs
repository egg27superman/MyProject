using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace AFH.Barcaldine.Language
{
    public class LocalDisplayName : DisplayNameAttribute
    {
        private string _defaultName = "";
        public Type ResourceType
        {
            get 
            {
                return LanguageCommon.ResourceType();
            }
        }
        public string ResourceName
        {
            get;
            set;
        }
        public LocalDisplayName(string defaultName)
        {
            _defaultName = defaultName;
        }
        public override string DisplayName
        {
            get
            {
                var a = ResourceType.GetProperties();
                PropertyInfo p = ResourceType.GetProperty(ResourceName);
                if (p != null)
                    return p.GetValue(null, null).ToString();
                else
                    return _defaultName;
            }
        }
    }
}
