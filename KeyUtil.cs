using System;
using System.Windows.Input;

namespace Autoclicker {
    internal class KeyUtil {
        public static string ConvertKeyToString(Key key) {
            switch (key) {
                case Key.Oem1:
                    return "SemiColon";
                case Key.Oem3:
                    // The ` symbol
                    return "Acute";
                case Key.Oem5:
                    return "Backslash";
                case Key.Oem6:
                    return "CloseSquareBracket";
                case Key.OemMinus:
                    return "Minus";
                case Key.OemPlus:
                    return "Plus";
                case Key.OemOpenBrackets:
                    return "OpenSquareBracket";
                case Key.OemQuotes:
                    return "Quote";
                case Key.OemComma:
                    return "Comma";
                case Key.OemPeriod:
                    return "Period";
                case Key.OemQuestion:
                    return "ForwardSlash";
                default:
                    return key.ToString();
            }
        }

        public static Key ConvertStringToKey(string keyString) {
            switch (keyString) {
                case "SemiColon":
                    return Key.Oem1;
                case "Acute":
                    // The ` symbol
                    return Key.Oem3;
                case "Backslash":
                    return Key.Oem5;
                case "CloseSquareBracket":
                    return Key.Oem6;
                case "Minus":
                    return Key.OemMinus;
                case "Plus":
                    return Key.OemPlus;
                case "OpenSquareBracket":
                    return Key.OemOpenBrackets;
                case "Quote":
                    return Key.OemQuotes;
                case "Comma":
                    return Key.OemComma;
                case "Period":
                    return Key.OemPeriod;
                case "ForwardSlash":
                    return Key.OemQuestion;
                default:
                    bool parseResult = Enum.TryParse(keyString, out Key parseKey);
                    return parseResult ? parseKey : Key.None;
            }

        }
    }
}
