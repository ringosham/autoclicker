using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using MathNet.Numerics.Distributions;

namespace Autoclicker {
    /*
     * The ClickThread.
     * Only handles clicking. The keyboard hook should reside in main class
     */
    internal class ClickThread {
        public double Mean { get; }
        public double Sigma { get; }
        public bool IsLeftClick { get; }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point {
            public int x;
            public int y;
        }

        private Point getMousePosition() {
            Win32Point point = new Win32Point();
            GetCursorPos(ref point);
            return new Point(point.x, point.y);
        }

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public ClickThread(double mean, double sigma, bool isLeftClick) {
            Mean = mean;
            Sigma = sigma;
            IsLeftClick = isLeftClick;
        }

        public void Run() {
            Normal normal = new Normal(Mean, Sigma);
            while (true) {
                Point currentPos = getMousePosition();
                if (IsLeftClick)
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint) currentPos.X, (uint) currentPos.Y, 0, 0);
                else
                    mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint) currentPos.X, (uint) currentPos.Y, 0, 0);
                //Don't care about data loss. It's only 1ms difference
                int sample = (int) normal.Sample() * 1000;
                if (sample <= 0)
                    sample = 1;
                Thread.Sleep(sample);
            }
        }
    }
}
