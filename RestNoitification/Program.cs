using System;
using System.Windows.Forms;

namespace RestNoitification
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
            Application.Run();
        }
    }
}
