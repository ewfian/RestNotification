using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestNotification.Notification
{
    /// <summary>
    /// 任务栏通知图标
    /// </summary>
    public class TrayIcon
    {
        /// <summary>
        /// 图标
        /// </summary>
        public NotifyIcon NotifyIcon;

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public TrayIcon()
        {
            NotifyIcon = new NotifyIcon();
        }
        /// <summary>
        /// 显示图标
        /// </summary>
        public void Show()
        {
            NotifyIcon.Visible = true;
        }
        /// <summary>
        /// 隐藏图标
        /// </summary>
        public void Hide()
        {
            NotifyIcon.Visible = false;
        }
        /// <summary>
        /// 更新图标
        /// </summary>
        public void UpdateIcon(Bitmap image, string text, bool autoShow = true)
        {
            if (autoShow)
            {
                Show();
            }
            NotifyIcon.Icon = Icon.FromHandle(image.GetHicon());
            NotifyIcon.Text = text;
        }
        /// <summary>
        /// 显示气泡提示文本
        /// </summary>
        public void ShowBallonTip(string title, string text, int timeout = 2000, ToolTipIcon icon = ToolTipIcon.Info)
        {
            NotifyIcon.ShowBalloonTip(2000, title, text, icon);
        }
    }
}
