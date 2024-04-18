using System.Collections.Generic;

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
            // 获取分类
            var categories = client.GetCategories();

            var userblogs = client.GetUsersBlogs();
            var recent = client.GetRecentPosts(5);
            markdowns.AddRange(recent);
            return markdowns;
        }

    }
}
