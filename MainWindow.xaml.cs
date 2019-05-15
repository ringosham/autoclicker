using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace Autoclicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            var stringBuilder = new StringBuilder(2);

            switch (scanCode)
            {
                case 0x0f:
                    return "[Tab]";
                case 0x3a:
                    return "[Caps lock]";
                case 0x1c:
                    return "[Enter]";
                case 0x0e:
                    return "[Backspace]";
                case 0x2a:
                    return "[Left shift]";
                case 0x36:
                    return "[Right shift]";
                case 0x39:
                    return "[Space]";
            }

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                {
                    ch = stringBuilder[0];
                    break;
                }
                default:
                {
                    ch = stringBuilder[0];
                    break;
                }
            }
            return ch.ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ToUnicode(
            uint virtualKeyCode,
            uint scanCode,
            byte[] keyboardState,
            StringBuilder receivingBuffer,
            int bufferSize,
            uint flags
        );

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnNewKeybind(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt))
                ((TextBox) sender).Text = "[Left alt]";
            else if (Keyboard.IsKeyDown(Key.RightAlt))
                ((TextBox) sender).Text = "[Right alt]";
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                ((TextBox)sender).Text = "[Left ctrl]";
            else if (Keyboard.IsKeyDown(Key.RightCtrl))
                ((TextBox)sender).Text = "[Right ctrl]";
            else
                ((TextBox)sender).Text = GetCharFromKey(e.Key);
            e.Handled = true;
        }
    }
}
