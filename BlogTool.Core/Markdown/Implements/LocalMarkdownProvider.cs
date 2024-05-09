using BlogTool.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlogTool.Core.Markdown.Implements
{
    public class LocalMarkdownProvider : MarkdownProvider
    {
        public override ICollection<IMarkdown> GetMarkdowns(GetMarkdownOption option, params object[] objects)
        {
            var markdowns = new List<IMarkdown>();

            var directoryPath = option.LocalOption.Path;

            void FindAndReadMarkdownFiles(string directoryPath)
            {
                foreach (var filePath in Directory.GetFiles(directoryPath, "*.md", SearchOption.AllDirectories))
                {
                    try
                    {
                        var content = File.ReadAllText(filePath);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                        Console.WriteLine($"Found Markdown file: {Path.GetFileName(filePath)}");
                        markdowns.Add(new PostInfo()
                        {
                            Categories=new List<string>()
                            {

                            },
                            Title=fileNameWithoutExtension,
                            Description=content,
                            DateCreated=DateTime.Now,
                        });

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                    }
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
