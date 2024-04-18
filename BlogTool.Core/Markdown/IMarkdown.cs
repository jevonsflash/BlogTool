using System;
using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public interface IMarkdown
    {
        List<string> Categories { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime? DateCreated { get; set; }
    }
}