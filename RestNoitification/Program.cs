using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.IO;
using System.Diagnostics;
using RestNoitification.ShellHelpers;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Threading;

namespace RestNoitification
{
    class Program
    {
        private const String APP_ID = "Fan.RestNoitification";

        static void Main(string[] args)
        {
            int minutes = 25;
            if (!(args.Length == 0 || (args.Length == 1 && int.TryParse(args[0], out minutes))))
            {
                ShowToast("Usage:", "\t" + System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName) + " [interval]", "interval 为可选参数：表示工作时常，单位分钟，默认值25，必须为整数");
            }
            else
            {
                ShowToast("工作提醒", "现在是" + DateTime.Now.ToString("HH:mm:ss"), "开始工作吧，" + minutes + "分钟后提醒你休息");
                Thread.Sleep(1000 * 10 * minutes);
                ShowToast("休息提醒", "现在是" + DateTime.Now.ToString("HH:mm:ss"), "你已经工作了" + minutes + "分钟， 该休息了");
            }
            Environment.Exit(-1);
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. You must download and include the 
        // Windows API Code Pack for Microsoft .NET Framework for this code to function
        //
        // Included in this project is a wxs file that be used with the WiX toolkit
        // to make an installer that creates the necessary shortcut. One or the other should be used.
        private static bool TryCreateShortcut()
        {
            String shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\RestNoitification.lnk";
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
                return true;
            }
            return false;
        }

        private static void InstallShortcut(String shortcutPath)
        {
            // Find the path to the current executable
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(APP_ID))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }

        private static void SetSilent(bool useSound, XmlDocument toastXml)
        {
            var audio = toastXml.GetElementsByTagName("audio").FirstOrDefault();

            if (audio == null)
            {
                audio = toastXml.CreateElement("audio");
                var toastNode = ((XmlElement)toastXml.SelectSingleNode("/toast"));

                if (toastNode != null)
                {
                    toastNode.AppendChild(audio);
                }
            }

            var attribute = toastXml.CreateAttribute("silent");
            attribute.Value = (!useSound).ToString().ToLower();
            audio.Attributes.SetNamedItem(attribute);
        }

        // Create and show the toast.
        // See the "Toasts" sample for more detail on what can be done with toasts
        private static void ShowToast(string title, string subTitle, string context, bool sound = true)
        {
            TryCreateShortcut();

            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(subTitle));
            stringElements[2].AppendChild(toastXml.CreateTextNode(context));

            // Specify the absolute path to an image
            String imagePath = "file:///" + System.AppDomain.CurrentDomain.BaseDirectory + @"me.jpg";
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Set if silent or not
            SetSilent(sound, toastXml);

            //Console.Write(toastXml.GetXml());
            //Console.Read();

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += (sender, e) =>
            {
                Console.WriteLine("Activated");
                //Environment.Exit(0);
            };
            toast.Dismissed += (sender, e) =>
            {
                String outputText = "";
                int exitCode = -1;
                switch (e.Reason)
                {
                    case ToastDismissalReason.ApplicationHidden:
                        outputText = "Hidden";
                        exitCode = 1;
                        break;
                    case ToastDismissalReason.UserCanceled:
                        outputText = "Dismissed";
                        exitCode = 2;
                        break;
                    case ToastDismissalReason.TimedOut:
                        outputText = "Timeout";
                        exitCode = 3;
                        break;
                }
                Console.WriteLine(outputText);
                //Environment.Exit(exitCode);
            };
            toast.Failed += (sender, e) =>
            {
                Console.WriteLine("Error.");
                Environment.Exit(-1);
            };

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }
    }
}
