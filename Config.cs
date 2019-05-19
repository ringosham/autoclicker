using System.Configuration;

namespace Autoclicker {
    public sealed class Config : ConfigurationSection {

        [ConfigurationProperty("key",
            DefaultValue = 192,
            IsRequired = true)]
        public int Key {
            get;
            set;
        }

        [ConfigurationProperty("leftClick",
            DefaultValue = true,
            IsRequired = true)]
        public bool LeftClick {
            get;
            set;
        }

        [ConfigurationProperty("lowerSecond",
            DefaultValue = 0.2d,
            IsRequired = true)]
        public double LowerSecond {
            get;
            set;
        }

        [ConfigurationProperty("upperSecond",
            DefaultValue = 0.3d,
            IsRequired = true)]
        public double UpperSecond {
            get;
            set;
        }
    }
}
