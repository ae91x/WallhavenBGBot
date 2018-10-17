using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WallhavenBGBot.Helpers
{
    public static class BackgroundHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDWININICHANGE = 0x02;

        public static void UpdateBackground(string filepath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filepath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
