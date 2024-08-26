using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.Data.Entities
{
    public class ServiceConfiguration
    {
        private string _code;
        private string _description;
        private bool _isEnabled;
        private DateTime? _lastTick;
        private DateTime? _nextTick;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public DateTime? LastTick
        {
            get { return _lastTick; }
            set { _lastTick = value; }
        }

        public DateTime? NextTick
        {
            get { return _nextTick; }
            set { _nextTick = value; }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public virtual ICollection<ServiceAttribute> Attributes { get; set; }


        public bool Equals(ServiceConfiguration other)
        {
            return other?._isEnabled.Equals(_isEnabled) ?? false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals((ServiceConfiguration)obj);
        }

        public override int GetHashCode()
        {
            return _isEnabled.GetHashCode();
        }
    }
}
