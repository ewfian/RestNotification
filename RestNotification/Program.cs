using System;
using System.Windows.Forms;

namespace RestNotification
{
    class Program
    {

        static Notification.Notification Current;

        /// <summary>
        /// 程序入口
        /// </summary>
        static void Main(string[] args)
        {
            Current = new Notification.Notification();
            int minutes = 25;
            if (args.Length == 1 && int.TryParse(args[0], out minutes))
            {
                if (minutes < 1 || minutes > 720)
                {
                    minutes = 25;
                }

            }
            Current.Start(new TimeSpan(0, minutes, 0));
            Application.Run();
        }
    }
}
