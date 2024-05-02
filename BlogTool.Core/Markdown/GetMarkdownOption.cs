using BlogTool.Core.Options;

namespace BlogTool.Core.Markdown
{
    public class GetMarkdownOption
    {

        public MetaWeblogOption MetaWeblogOption { get; set; }
        public LocalOption LocalOption { get; set; }
        public int RecentTakeCount { get; set; }
        public int ReadMorePosition { get; set; }
    }
}