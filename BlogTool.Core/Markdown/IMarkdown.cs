using System;
using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public interface IMarkdown
    {
        List<string> Categories { get; set; }
        string Title { get; set; }
        string Keywords { get; set; }
        string Content { get; set; }
        string Description { get; set; }
        DateTime? DateCreated { get; set; }
    }
}