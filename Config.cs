using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoclicker
{
    public sealed class Config : ConfigurationSection
    {

        [ConfigurationProperty("key",
            DefaultValue = 192,
            IsRequired = true)]
        public int Key
        {
            get;
            set;
        }

        [ConfigurationProperty("leftClick",
            DefaultValue = true,
            IsRequired = true)]
        public bool LeftClick
        {
            get;
            set;
        }

        [ConfigurationProperty("lowerSecond",
            DefaultValue = 0.2,
            IsRequired = true)]
        public float LowerSecond
        {
            get;
            set;
        }

        [ConfigurationProperty("upperSecond",
            DefaultValue = 0.3,
            IsRequired = true)]
        public float UpperSecond
        {
            get;
            set;
        }
    }
}
