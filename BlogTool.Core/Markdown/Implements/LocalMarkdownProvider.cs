﻿using BlogTool.Core.Aigc.DashScope;
using BlogTool.Core.Aigc.DashScope.TextGeneration;
using BlogTool.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlogTool.Core.Markdown.Implements
{
    public class LocalMarkdownProvider : MarkdownProvider
    {
        private IAigcClient aigcClientClient;

        private const string msg = "你是一个摘要生成工具，你需要解释我发送给你的内容，不要换行，不要超过200字，只需要介绍文章的内容，不需要提出建议和缺少的东西，只包含纯文本，不要用Markdown格式。请用中文回答，文章内容为：";

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

            var directoryPath = option.LocalOption.Path;

            void FindAndReadMarkdownFiles(string directoryPath)
            {
                foreach (var filePath in Directory.GetFiles(directoryPath, "*.md", SearchOption.AllDirectories))
                {
                    var content = "";
                    try
                    {
                        content = File.ReadAllText(filePath);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                        continue;
                    }
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    Console.WriteLine($"Found Markdown file: {Path.GetFileName(filePath)}");

                    var description = "";
                    try
                    {
                        Console.WriteLine($"Generating description with AI ...");

                        var target = option.AigcOption.Target.Split(',');
                        if (aigcClientClient!=default && !string.IsNullOrEmpty(content) && target.Contains("Description"))
                        {
                            var _textGenerationClient = aigcClientClient.TextGeneration;
                            var result = _textGenerationClient.Chat("qwen-turbo", [
    new ChatMessage("assistant", $"{msg}{content}"),
                ]).Result;
                            description = result.Output.Text;
                        }



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error generating description with AI, file {filePath}: {ex.Message}");
                    }



                    markdowns.Add(new PostInfo()
                    {
                        Categories=new List<string>()
                        {

                        },
                        Title=fileNameWithoutExtension,
                        Content=content,
                        Description=description,
                        DateCreated=DateTime.Now,
                    });


                }
                if (option.LocalOption.Recursive)
                {
                    foreach (var subDirectory in Directory.GetDirectories(directoryPath))
                    {
                        FindAndReadMarkdownFiles(subDirectory);
                    }
                }

            };

            FindAndReadMarkdownFiles(directoryPath);

            return markdowns;
        }
    }
}
