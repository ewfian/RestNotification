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
            //25分钟休息一次
            IntervalOfRest = new TimeSpan(0, 25, 0);
            //托盘图标每1分钟更新一次
            IntervalOfUpdate = new TimeSpan(0, 1, 0);
            //气泡提醒每5分钟显示一次
            IntervalOfNotify = new TimeSpan(0, 25, 0);

            TrayIcon = new TrayIcon();
            TrayIcon.NotifyIcon.ContextMenu = TrayIconHelper.GetExitContextMenu();

            Toast = new Toast();

            Start();

        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            Reset();
            Task.Run(() => RestAsync(IntervalOfRest));
            Task.Run(() => UpdateAsync(IntervalOfUpdate));
            Task.Run(() => NotifyAsync(IntervalOfNotify));
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Target = DateTime.Now + IntervalOfRest;
            TrayIcon.UpdateIcon(BitmapHelper.GetTrayBitmap((int)IntervalOfRest.TotalMinutes), "");
        }

        /// <summary>
        /// 创建一个指定时间间隔后休息的任务
        /// </summary>
        private async Task RestAsync(TimeSpan interval, int timeout = 3000)
        {
            while (true)
            {
                await Task.Delay(interval);
                await Task.Run(() => Toast.ShowToast(GetDefaultMessage(Target), () => { WorkStation.Lock(); Environment.Exit(0); }));
                //Reset();
            }
        }
        /// <summary>
        /// 创建一个指定时间间隔后更新的任务
        /// </summary>
        private async Task UpdateAsync(TimeSpan interval)
        {
            while (true)
            {
                int total = (int)Math.Round((Target - DateTime.Now).TotalMinutes);
                await Task.Run(() => TrayIcon.UpdateIcon(BitmapHelper.GetTrayBitmap(total), $"再工作{total}分钟就该休息了"));
                await Task.Delay(interval);
            }
        }
        /// <summary>
        /// 创建一个指定时间间隔后提醒的任务
        /// </summary>
        private async Task NotifyAsync(TimeSpan interval, int timeout = 3000)
        {
            while (true)
            {
                int total = (int)Math.Round((Target - DateTime.Now).TotalMinutes);
                await Task.Run(() => TrayIcon.ShowBallonTip("温馨提醒", $"再工作{total}分钟就该休息了", timeout));
                await Task.Delay(interval);
            }
        }

        /// <summary>
        /// 返回默认消息
        /// </summary>
        private static Message GetDefaultMessage(DateTime target)
        {
            string title = "休息提醒";
            string subTitle = $"现在是{DateTime.Now.ToString("HH:mm:ss")}";
            string content = $"你已经工作了{(int)(target - DateTime.Now).TotalMinutes}分钟， 该休息了!{Environment.NewLine}（点击可取消锁屏）";
            string imagePath = $"file:///{System.AppDomain.CurrentDomain.BaseDirectory}Assets\\young.jpg"; ;
            return new Message(title, subTitle, content, imagePath, true);
        }
    }
}
