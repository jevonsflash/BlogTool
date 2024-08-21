using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdcb.DashScope;
using Sdcb.DashScope.TextGeneration;

namespace BlogTool.Core.Markdown.Implements
{
    /// <summary>
    /// 可实现一个自定义的MarkdownProvider
    /// </summary>
    public class MetaWeblogMarkdownProvider : MarkdownProvider
    {
        private readonly TextGenerationClient _textGenerationClient;

        private const string msg = "你是一个摘要生成工具，你需要解释我发送给你的内容，不要换行，不要超过200字，只需要介绍文章的内容，不需要提出建议和缺少的东西。请用中文回答，文章内容为：";
        public MetaWeblogMarkdownProvider()
        {
            _textGenerationClient = new TextGenerationClient(new DashScopeClient(
                "sk-abfb5186d29e4a0cbd6c329517b61cce"
                ));
        }
        public override ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option)
        {
            var markdowns = new List<IMarkdown>();

            // 使用完整url
            //var option = new ClientOption("blogurl", "metaweblogurl", "blogname", "username", "pat");
            var client = new MetaWeblogClient(option.MetaWeblogOption);
            // 获取你的blogId
            var blogs = client.GetUsersBlogs();
            var recent = client.GetRecentPosts(option.RecentTakeCount);

            foreach (var recentItem in recent)
            {
                var categories = new List<string>();
                foreach (var category in recentItem.Categories)
                {
                    var content = ExtractContentOutsideBrackets(category);
                    if (!string.IsNullOrEmpty(content))
                    {
                        categories.Add(content);
                    }
                }
                recentItem.Categories= categories;
                var result = _textGenerationClient.Chat("qwen-turbo", [
                     new ChatMessage("assistant", $"{msg}{recentItem.Content}"),
                ]).Result;
                var description = result.Output.Text;
                recentItem.Description=description;


            }

            markdowns.AddRange(recent);
            return markdowns;
        }

        private string ExtractContentOutsideBrackets(string input)
        {
            string pattern = @"\[.*?\](.*)";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return string.Empty;
        }

    }
}
