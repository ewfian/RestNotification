using System;
using RestNoitification.Utilities;
using System.Threading.Tasks;

namespace RestNoitification.Notification
{
    /// <summary>
    /// 提醒
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// 休息间隔
        /// </summary>
        public TimeSpan IntervalOfRest;
        /// <summary>
        /// 图标更新间隔
        /// </summary>
        public TimeSpan IntervalOfUpdate;
        /// <summary>
        /// 气泡提醒间隔
        /// </summary>
        public TimeSpan IntervalOfNotify;
        /// <summary>
        /// 目标时间
        /// </summary>
        public DateTime Target;
        /// <summary>
        /// 任务栏通知
        /// </summary>
        public TrayIcon TrayIcon;
        /// <summary>
        /// Toast通知
        /// </summary>
        public Toast Toast;

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public Notification()
        {


            TrayIcon = new TrayIcon();
            TrayIcon.NotifyIcon.ContextMenu = TrayIconHelper.GetExitContextMenu();

            Toast = new Toast();



        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start(TimeSpan interval)
        {
            IntervalOfRest = interval;
            //默认托盘图标每1分钟更新一次
            IntervalOfUpdate = new TimeSpan(0, 1, 0);
            Reset();
            Task.Run(() => UpdateAsync(IntervalOfUpdate));
        }
        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            Target = DateTime.Now + IntervalOfRest;
            TrayIcon.UpdateIcon(BitmapHelper.GetTrayBitmap((int)IntervalOfRest.TotalMinutes), "");
            TrayIcon.ShowBallonTip("温馨提醒", GetDefaultNotify(Target), 3000);
            Task.Run(() => RestAsync(IntervalOfRest));
        }

        /// <summary>
        /// 创建一个指定时间间隔后休息的任务
        /// </summary>
        private async Task RestAsync(TimeSpan interval)
        {
            Action activated = () => { WorkStation.Lock(); Environment.Exit(0); };
            Action userCanceled = () => { Reset(); };
            Action timeOut = () => { Reset(); };
            DateTime startTime = DateTime.Now;
            await Task.Delay(interval);
            Message message = GetDefaultMessage(DateTime.Now - startTime);
            await Task.Run(() => Toast.ShowToast(message, activated, userCanceled, timeOut));
        }
        /// <summary>
        /// 创建一个指定时间间隔后更新的任务
        /// </summary>
        private async Task UpdateAsync(TimeSpan interval)
        {
            while (true)
            {
                string content = string.Empty;
                int total = (int)Math.Round((Target - DateTime.Now).TotalMinutes);
                if (total > 0)
                {
                    content = $"再工作{total}分钟就该休息了";
                }
                else
                {
                    int totalSeconds = (int)Math.Round((Target - DateTime.Now).TotalSeconds);
                    content = $"再工作{totalSeconds}秒钟就该休息了";
                }
                await Task.Run(() => TrayIcon.UpdateIcon(BitmapHelper.GetTrayBitmap(total), content));
                await Task.Delay(interval);
            }
        }
        /// <summary>
        /// 返回默认气泡消息
        /// </summary>
        private string GetDefaultNotify(DateTime target, int timeout = 3000)
        {
            string content = string.Empty;
            int total = (int)Math.Round((target - DateTime.Now).TotalMinutes);
            if (total > 0)
            {
                content = $"再工作{total}分钟就该休息了";
            }
            else
            {
                int totalSeconds = (int)Math.Round((target - DateTime.Now).TotalSeconds);
                content = $"再工作{totalSeconds}秒钟就该休息了";
            }
            return content;
        }
        /// <summary>
        /// 返回默认Toast消息
        /// </summary>
        private Message GetDefaultMessage(TimeSpan remainning)
        {
            string title = "休息提醒";
            string subTitle = $"现在是{DateTime.Now.ToString("hh:mm:ss")}";
            string content = string.Empty;
            int total = (int)Math.Round(remainning.TotalMinutes);
            if (total > 0)
            {
                content = $"你已经工作了{total}分钟,该休息了!{Environment.NewLine}（点击此处锁屏）";
            }
            else
            {
                int totalSeconds = (int)Math.Round(remainning.TotalSeconds);
                content = $"你已经工作了{totalSeconds}秒钟,该休息了!{Environment.NewLine}（点击此处锁屏）";
            }
            string imagePath = $"file:///{System.AppDomain.CurrentDomain.BaseDirectory}Assets\\young.jpg"; ;
            return new Message(title, subTitle, content, imagePath, true);
        }
    }
}
