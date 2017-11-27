using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RestNotification
{
    class Program
    {
        /// <summary>
        /// 需要定义为类变量，而非局部变量
        /// </summary>
        static System.Threading.Mutex _mutex;

        static Notification.Notification Current;

        /// <summary>
        /// 程序入口
        /// </summary>
        static void Main(string[] args)
        {
            //是否可以打开新进程
            bool createNew;
            string guid = ((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value;
            _mutex = new System.Threading.Mutex(true, guid, out createNew);
            if (false == createNew)
            {
                //发现重复进程
                Application.Exit();
            }
            else
            {
                _mutex.ReleaseMutex();

                Current = new Notification.Notification();
                int minutes = 25;
                if (args.Length == 1)
                {
                    int.TryParse(args[0], out minutes);
                }
                Current.Start(new TimeSpan(0, minutes, 0));
                Application.Run();
            }
        }
    }
}