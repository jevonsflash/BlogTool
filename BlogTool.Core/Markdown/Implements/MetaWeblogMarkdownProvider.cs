﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using BlogTool.Core.Aigc.DashScope;
using BlogTool.Core.Aigc.DashScope.TextGeneration;
using BlogTool.Core.Aigc.Tuning;

namespace BlogTool.Core.Markdown.Implements
{
    /// <summary>
    /// 可实现一个自定义的MarkdownProvider
    /// </summary>
    public class MetaWeblogMarkdownProvider : MarkdownProvider
    {
        private IAigcClient aigcClientClient;

        public override ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option, params object[] objects)
        {
            if (!string.IsNullOrEmpty(option.AigcOption.Provider)&&!string.IsNullOrEmpty(option.AigcOption.ApiKey))
            {
                switch (option.AigcOption.Provider)
                {
                    case DashScopeClient.Name:
                        aigcClientClient = new DashScopeClient(option.AigcOption.ApiKey);
                        break;

                    default:
                        break;
                }
            }

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

                var description = "";
                try
                {
                    Console.WriteLine($"Generating description with AI ...");

                    var target = option.AigcOption.Target.Split(',');
                    if (aigcClientClient!=default && !string.IsNullOrEmpty(recentItem.Content) && target.Contains("Description"))
                    {
                        var _textGenerationClient = aigcClientClient.TextGeneration;
                        var result = _textGenerationClient.Chat("qwen-turbo", [
new ChatMessage("assistant", $"{PromptConsts.DescriptionGenerationMsg}{recentItem.Content}"),
                ]).Result;
                        description = result.Output.Text;
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating description with AI, post {recentItem.Title}: {ex.Message}");
                }


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
