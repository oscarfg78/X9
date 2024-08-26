using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.Data.Entities
{
    public class ServiceAttribute
    {
        private string _code;
        private string _key;
        private string _value;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
