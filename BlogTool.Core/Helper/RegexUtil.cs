using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlogTool.Core.Helper;

public static class RegexUtil
{
    private static readonly Regex ImgRegex = new(@"!\[.*?\]\((.*?)\)", RegexOptions.Compiled);

    public static IEnumerable<string> ExtractorImgFromMarkdown(string content)
    {
        foreach (Match match in ImgRegex.Matches(content))
        {
            yield return match.Groups[1].Value;
        }
    }
}