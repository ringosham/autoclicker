using System.Windows.Input;

namespace Autoclicker {
    public sealed class Config {

        private Config() { }

        public static Config Instance = new Config();

        public Key Key {
            get;
            set;
        }

        public bool LeftClick {
            get;
            set;
        }

        public double Mean {
            get;
            set;
        }

        public double Sigma {
            get;
            set;
        }
    }
}
