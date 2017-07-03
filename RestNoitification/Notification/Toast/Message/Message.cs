namespace RestNoitification.Notification
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 是否播放音效
        /// </summary>
        public bool IsPlayingSound { get; set; }

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public Message(string title, string subTitle, string context, string imagePath, bool isPlayingSound)
        {
            Title = title;
            SubTitle = subTitle;
            Context = context;
            ImagePath = imagePath;
            IsPlayingSound = isPlayingSound;
        }
    }
}
