using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace AFH.Barcaldine.Language
{
    public class LocalRequiredValidation : RequiredAttribute
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
        public LocalRequiredValidation()
        {
            
        }

        public override string FormatErrorMessage(string name)
        {
            
                PropertyInfo p = ResourceType.GetProperty(ResourceName);
                if (p != null)
                    return p.GetValue(null, null).ToString();
                else
                    return _defaultName;
        }
    }
}
