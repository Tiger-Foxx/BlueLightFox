using System;
using System.Runtime.InteropServices;

namespace FoxyBlueLight.Native
{
    public class DisplayAPI
    {
        // Constantes Win32
        public const int SPI_SETDESKWALLPAPER = 0x0014;
        public const int SPIF_UPDATEINIFILE = 0x01;
        public const int SPIF_SENDCHANGE = 0x02;
        
        // Structure pour les rampes de couleur
        [StructLayout(LayoutKind.Sequential)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Red;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Green;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Blue;
        }
        
        // Méthodes natives
        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
        
        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDC, out RAMP lpRamp);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        
        // Crée une rampe vide
        public static RAMP CreateRamp()
        {
            return new RAMP
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };
        }
        
        // Obtient la rampe gamma actuelle
        public static bool GetCurrentRamp(out RAMP ramp)
        {
            ramp = CreateRamp();
            IntPtr hdc = GetDC(IntPtr.Zero);
            bool result = GetDeviceGammaRamp(hdc, out ramp);
            ReleaseDC(IntPtr.Zero, hdc);
            return result;
        }
        
        // Définit une nouvelle rampe gamma
        public static bool SetRamp(RAMP ramp)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            bool result = SetDeviceGammaRamp(hdc, ref ramp);
            ReleaseDC(IntPtr.Zero, hdc);
            return result;
        }
    }
    
    // Classe pour passer les événements à travers la fenêtre overlay
    public static class NativeMethods
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);
        
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
    }
}