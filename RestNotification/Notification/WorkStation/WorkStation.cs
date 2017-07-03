using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RestNotification.Notification
{
    /// <summary>
    /// 工作站
    /// </summary>
    public class WorkStation
    {
        [DllImport("user32.dll")]
        static extern bool LockWorkStation();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// 立即锁定
        /// </summary>
        public static void Lock()
        {
            LockWorkStation();
        }
        /// <summary>
        /// 延迟指定毫秒数后锁定
        /// </summary>
        public static async Task LockAsync(int millisecondsDelay)
        {
            await Task.Delay(millisecondsDelay);
            await Task.Run(() => Lock());
        }

    }
}
