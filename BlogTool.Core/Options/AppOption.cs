using BlogTool.Core.AssetsStores;
using BlogTool.Core.Markdown;

namespace BlogTool.Core.Options
{
    /// <summary>
    /// 博客园配置
    /// </summary>
    public class AppOption
    {
        public string HexoPath { get; set; }
        public string OutputPath { get; set; }
        public bool SkipFileWhenException { get; set; }

        public AssetsStoreOption AssetsStoreOption { get; set; }
        public GetMarkdownOption GetMarkdownOption { get; set; }
        public string MarkdownProvider { get; set; }
        public string AssetsStoreProvider { get; set; }
    }
}
