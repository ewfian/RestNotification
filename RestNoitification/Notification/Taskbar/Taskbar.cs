using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestNoitification.Notification
{
    /// <summary>
    /// 任务栏通知
    /// </summary>
    public static class Taskbar
    {
        /// <summary>
        /// 图标
        /// </summary>
        private static NotifyIcon TrayIcon = new NotifyIcon();
        /// <summary>
        /// 显示图标
        /// </summary>
        public static void Show()
        {
            TrayIcon.Visible = true;
        }
        /// <summary>
        /// 隐藏图标
        /// </summary>
        public static void Hide()
        {
            TrayIcon.Visible = false;
        }
        /// <summary>
        /// 更新图标
        /// </summary>
        public static void UpdateIcon(Bitmap image, string text, bool autoShow = true)
        {
            if (autoShow)
            {
                Show();
            }
            TrayIcon.Icon = Icon.FromHandle(image.GetHicon());
            TrayIcon.Text = text;
        }
        /// <summary>
        /// 显示气泡提示文本
        /// </summary>
        public static void ShowBallonTip(string title,string text,int timeout=2000,ToolTipIcon icon=ToolTipIcon.Info)
        {
            TrayIcon.ShowBalloonTip(2000, title, text,icon);
        }

    }
}
