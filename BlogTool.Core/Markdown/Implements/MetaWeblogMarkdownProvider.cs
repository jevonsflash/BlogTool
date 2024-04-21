using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlogTool.Core.Markdown.Implements
{
    /// <summary>
    /// 可实现一个自定义的MarkdownProvider
    /// </summary>
    public class MetaWeblogMarkdownProvider : MarkdownProvider
    {

        public override ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option)
        {
            var markdowns = new List<IMarkdown>();

            // 使用完整url
            //var option = new ClientOption("blogurl", "metaweblogurl", "blogname", "username", "pat");
            var client = new Client(option.MetaWeblogOption);
            // 获取你的blogId
            var blogs = client.GetUsersBlogs();
            var recent = client.GetRecentPosts(5);

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
