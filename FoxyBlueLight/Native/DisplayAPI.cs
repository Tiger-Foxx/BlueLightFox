// FoxyBlueLight/Native/DisplayAPI.cs
using System;
using System.Runtime.InteropServices;

namespace FoxyBlueLight.Native
{
    public class DisplayAPI
    {
        // Cette classe nous permet d'interagir avec les API Windows pour modifier les couleurs de l'écran
        
        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP ramp);

        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDC, ref RAMP ramp);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        // Structure pour stocker les valeurs RGB de la rampe gamma
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Blue;
        }

        // Crée une nouvelle rampe vide
        public static RAMP CreateRamp()
        {
            RAMP ramp = new RAMP
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            return ramp;
        }

        // Obtient la rampe gamma actuelle du système
        public static bool GetCurrentRamp(out RAMP ramp)
        {
            ramp = CreateRamp();
            IntPtr hDC = GetDC(IntPtr.Zero); // Pour l'écran principal
            bool success = GetDeviceGammaRamp(hDC, ref ramp);
            ReleaseDC(IntPtr.Zero, hDC);
            return success;
        }

        // Définit une nouvelle rampe gamma pour modifier les couleurs de l'écran
        public static bool SetRamp(RAMP ramp)
        {
            IntPtr hDC = GetDC(IntPtr.Zero); // Pour l'écran principal
            bool success = SetDeviceGammaRamp(hDC, ref ramp);
            ReleaseDC(IntPtr.Zero, hDC);
            return success;
        }
    }
}