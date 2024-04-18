using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public abstract class MarkdownProvider : IMarkdownProvider
    {
        public MarkdownProvider()
        {

        }

        public abstract ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option);

    }
}
