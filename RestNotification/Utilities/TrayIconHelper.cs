using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestNotification.Utilities
{
    public class TrayIconHelper
    {
        public static ContextMenu GetExitContextMenu()
        {
            return new ContextMenu(new MenuItem[] { new MenuItem("退出", (object sender, EventArgs e) => { Environment.Exit(0); }) });
        }
    }
}
