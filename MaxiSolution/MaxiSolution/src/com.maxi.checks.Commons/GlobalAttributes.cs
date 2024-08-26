using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.Data.Entities
{
    public class GlobalAttributes
    {
        private string _name;
        private string _value;
        private string _description;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
