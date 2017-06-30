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
        /// 创建并初始化一个实例
        /// </summary>
        public Notification()
        {
            //25分钟休息一次
            IntervalOfRest = new TimeSpan(0, 25, 0);
            //托盘图标每1分钟更新一次
            IntervalOfUpdate = new TimeSpan(0, 1, 0);
            //气泡提醒每5分钟显示一次
            IntervalOfNotify = new TimeSpan(0, 5, 20);

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
            Taskbar.UpdateIcon(BitmapHelper.GetTrayBitmap((int)IntervalOfRest.TotalMinutes), "");
        }

        /// <summary>
        /// 创建一个指定时间间隔后休息的任务
        /// </summary>
        private async Task RestAsync(TimeSpan interval, int timeout = 3000)
        {
            while (true)
            {
                await Task.Delay(interval);
                await Task.Run(() => Toast.ShowToast(MessageHelper.GetDefaultMessage(Target)));
                await WorkStation.LockAsync(timeout);
                Reset();
            }
        }
        /// <summary>
        /// 创建一个指定时间间隔后更新的任务
        /// </summary>
        private async Task UpdateAsync(TimeSpan interval)
        {      
            while (true)
            {
                int total =(int)Math.Round((Target - DateTime.Now).TotalMinutes);
                await Task.Run(() => Taskbar.UpdateIcon(BitmapHelper.GetTrayBitmap(total), $"再工作{total}分钟就该休息了"));
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
                await Task.Run(() => Taskbar.ShowBallonTip("温馨提醒", $"哈哈哈，再工作{total}分钟就该休息了", timeout));
                await Task.Delay(interval);
            }
        }
    }
}
