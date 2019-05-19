﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Autoclicker {
    class KeyboardHook {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public event EventHandler<int> OnKeyPressed;
        public event EventHandler<int> OnKeyUnpressed;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public KeyboardHook() {
            _proc = HookCallback;
        }

        public void HookKeyboard() {
            _hookID = SetHook(_proc);
        }

        public void UnHookKeyboard() {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN) {
                int vkCode = Marshal.ReadInt32(lParam);

                OnKeyPressed.Invoke(this, (vkCode));
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP) {
                int vkCode = Marshal.ReadInt32(lParam);

                OnKeyUnpressed.Invoke(this, (vkCode));
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
