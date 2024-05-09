using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public interface IMarkdownProvider
    {
        ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option, params object[] objects);
    }
}