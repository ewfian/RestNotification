using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestNotification.Utilities
{
    /// <summary>
    /// 位图辅助类
    /// </summary>
    public class BitmapHelper
    {
        /// <summary>
        /// 返回指定数字的位图图像
        /// </summary>
        public static Bitmap GetTrayBitmap(int current)
        {
            Bitmap bmp = new Bitmap(30, 30);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Font drawFont = new Font("Arial Narrow", 25, FontStyle.Regular);
                SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.White);
                StringFormat drawFormat = new StringFormat();
                graphics.DrawString(current.ToString("D2"), drawFont, drawBrush, -5, -5, drawFormat);
            }
            return bmp;
        }
    }
}
