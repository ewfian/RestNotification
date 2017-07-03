using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using RestNoitification.ShellHelpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace RestNoitification.Notification
{
    /// <summary>
    /// Toast通知
    /// </summary>
    public  class Toast
    {
        /// <summary>
        /// 应用程序标识符
        /// </summary>
        public  string AppId = "Fan.RestNoitification";
        /// <summary>
        /// 快捷方式附加路径
        /// </summary>
        public  string InkPath = "\\Microsoft\\Windows\\Start Menu\\Programs\\RestNoitification.lnk";

        /// <summary>
        /// 创建并显示Toast通知
        /// </summary>
        public  void ShowToast(Message message,Action activated=null,Action userCanceled=null,Action timeOut=null)
        {
            TryCreateShortcut(AppId);

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

            stringElements[0].AppendChild(toastXml.CreateTextNode(message.Title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message.SubTitle));
            stringElements[2].AppendChild(toastXml.CreateTextNode(message.Context));

            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = message.ImagePath;

            CreateAudio(message.IsPlayingSound, toastXml);

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += (sender, e) =>
            {
                Console.WriteLine("Activated");
                activated?.Invoke();
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
                        outputText = "UserCanceled";
                        exitCode = 2;
                        userCanceled?.Invoke();
                        break;
                    case ToastDismissalReason.TimedOut:
                        outputText = "Timeout";
                        exitCode = 3;
                        timeOut?.Invoke();
                        break;
                }
                Console.WriteLine($"code:{exitCode}description:{outputText}");
            };
            toast.Failed += (sender, e) =>
            {
                Console.WriteLine("Error.");
                Environment.Exit(-1);
            };
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
        }

        /// <summary>
        /// 尝试创建快捷方式
        /// </summary>
        private  bool TryCreateShortcut(string appId)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + InkPath;
            if (!File.Exists(path))
            {
                CreateShortcut(path, appId);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        private  void CreateShortcut(String shortcutPath, string appId)
        {
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant id = new PropVariant(appId))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, id));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }
        /// <summary>
        /// 创建附加音效
        /// </summary>
        private  void CreateAudio(bool useSound, XmlDocument toastXml)
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

    }
}
