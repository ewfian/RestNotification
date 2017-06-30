using RestNoitification.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestNoitification.Utilities
{
    /// <summary>
    /// 消息辅助类
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// 返回默认消息
        /// </summary>
        public static Message GetDefaultMessage(DateTime target)
        {
            string title = "休息提醒";
            string subTitle = $"现在是{DateTime.Now.ToString("HH:mm:ss")}";
            string content = $"你已经工作了{(int)(target - DateTime.Now).TotalMinutes}分钟， 该休息了";
            string imagePath = $"file:///{System.AppDomain.CurrentDomain.BaseDirectory}Assets\\young.jpg"; ;
            return new Message(title,subTitle,content,imagePath,true);
        }
    }
}
