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
    public class LocalStringLength : StringLengthAttribute
    {
        private string _defaultName = "";
        private int _maximumLength = 0;

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


        public LocalStringLength(int maximumLength)
            : base(maximumLength)
        {
           _maximumLength = maximumLength;
        }

        public override string FormatErrorMessage(string name)
        {
                PropertyInfo p = ResourceType.GetProperty(ResourceName);
                if (p != null)
                {
                    string errorMessage = p.GetValue(null, null).ToString();
                    return string.Format(errorMessage, this._maximumLength.ToString());
                }
                else
                    return _defaultName;
        }
    }
}
