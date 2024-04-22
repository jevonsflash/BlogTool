using BlogTool.Core.AssetsStores;
using BlogTool.Core.AssetsStores.Implements;
using BlogTool.Core.Helper;
using BlogTool.Core.Markdown;
using BlogTool.Core.Markdown.Implements;
using BlogTool.Core.Options;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using DirFileHelper = BlogTool.Core.Helper.DirFileHelper;

namespace BlogTool
{
    partial class Program
    {
        private static AppOption config;





        static string InsertLine(string inputString, int lineNumber, string contentToInsert)
        {
            int indexToInsert = 0;
            for (int i = 0; i < lineNumber - 1; i++)
            {
                indexToInsert = inputString.IndexOf('\n', indexToInsert) + 1;
                if (indexToInsert == 0)
                {
                    return inputString;
                }
            }

            return inputString.Insert(indexToInsert, contentToInsert + "\n");
        }

        static void Main(string[] args)
        {
            if (!CliProcessor.ProcessCommandLine(args))
            {
                Console.WriteLine("缺少参数或参数不正确");

                CliProcessor.Usage();
                Environment.ExitCode = 1;
                return;
            }

            config = new AppOption();

            config.HexoPath = string.IsNullOrEmpty(CliProcessor.hexoPath) ? ConfigurationHelper.GetConfigValue("HexoPath", "./") : CliProcessor.hexoPath;
            config.OutputPath = string.IsNullOrEmpty(CliProcessor.outputPath) ? ConfigurationHelper.GetConfigValue("OutputPath", "./source/_posts") : CliProcessor.outputPath;
            config.SkipFileWhenException = Convert.ToBoolean(ConfigurationHelper.GetConfigValue("SkipFileWhenException", "true"));
            config.AssetsStoreOption = new AssetsStoreOption()
            {
                AddWatermark = Convert.ToBoolean(ConfigurationHelper.GetConfigValue("AssetsStore:AddWatermark", "false")),
                CompressionImage = Convert.ToBoolean(ConfigurationHelper.GetConfigValue("AssetsStore:CompressionImage", "false")),
                SubPath = ConfigurationHelper.GetConfigValue("AssetsStore:SubPath", "assets"),
                OutputPath = config.OutputPath,
                ImageOption = new ImageOption()
                {


                }
            };
            config.GetMarkdownOption = new GetMarkdownOption()
            {
                MetaWeblogOption = new MetaWeblogOption()
                {
                    //BlogURL = ConfigurationHelper.GetConfigValue("GetMarkdown:MetaWeblog:BlogURL", ""),
                    MetaWeblogURL = ConfigurationHelper.GetConfigValue("GetMarkdown:MetaWeblog:MetaWeblogURL", ""),
                    Username = ConfigurationHelper.GetConfigValue("GetMarkdown:MetaWeblog:Username", ""),
                    Password = ConfigurationHelper.GetConfigValue("GetMarkdown:MetaWeblog:Password", "")
                },
                ReadMorePosition = Convert.ToInt32(ConfigurationHelper.GetConfigValue("GetMarkdown:ReadMorePosition", "-1")),
                RecentTakeCount = CliProcessor.recentTakeCount <= 0 ? Convert.ToInt32(ConfigurationHelper.GetConfigValue("GetMarkdown:RecentTakeCount", "1")) : CliProcessor.recentTakeCount
            };
            config.MarkdownProvider = string.IsNullOrEmpty(CliProcessor.markdownProvider) ? ConfigurationHelper.GetConfigValue("MarkdownProvider", "MetaWeblog") : CliProcessor.markdownProvider;
            config.AssetsStoreProvider = string.IsNullOrEmpty(CliProcessor.assetsStoreProvider) ? ConfigurationHelper.GetConfigValue("AssetsStoreProvider", "Local") : CliProcessor.assetsStoreProvider;


            try
            {

                var client = new HttpClient();

                var creator = new MarkdownCreator();
                var handler = new AssetsStoreHandler();
                if (config.MarkdownProvider.ToUpper() == "METAWEBLOG")
                {
                    creator.SetMarkdownProvider(config.GetMarkdownOption, new MetaWeblogMarkdownProvider());

                }
                else if (config.MarkdownProvider.ToUpper() == "LOCAL")
                {
                    creator.SetMarkdownProvider(config.GetMarkdownOption, new LocalMarkdownProvider());

                }

                if (config.AssetsStoreProvider.ToUpper() == "EMBED")
                {
                    handler.SetAssetsStoreProvider(config.AssetsStoreOption, new EmbedAssetsStoreProvider());

                }
                else if (config.AssetsStoreProvider.ToUpper() == "LOCAL")
                {

                    handler.SetAssetsStoreProvider(config.AssetsStoreOption, new LocalAssetsStoreProvider());
                }

                else if (config.AssetsStoreProvider.ToUpper() == "HEXO-ASSET-FOLDER")
                {

                    handler.SetAssetsStoreProvider(config.AssetsStoreOption, new HexoAssetFolderAssetsStoreProvider());
                }

                else if (config.AssetsStoreProvider.ToUpper() == "HEXO-TAG-PLUGIN")
                {

                    handler.SetAssetsStoreProvider(config.AssetsStoreOption, new HexoTagPluginAssetsStoreProvider());
                }






                var mds = creator.Create();

                string templatePath = Path.Combine(config.HexoPath, "scaffolds", "post.md");

                string fileFullPath;


                var fileDirectory = Directory.Exists(config.OutputPath) == false
                    ? Directory.CreateDirectory(config.OutputPath).FullName
                    : new DirectoryInfo(config.OutputPath).FullName;



                if (File.Exists(templatePath))
                {
                    foreach (var md in mds)
                    {
                        var fileName = md.Title + ".md";

                        fileFullPath = Path.Combine(fileDirectory, fileName);

                        string templateMd = File.ReadAllText(templatePath);
                        templateMd = templateMd.Replace("{{ title }}", md.Title);
                        templateMd = templateMd.Replace("{{ date }}", md.DateCreated.HasValue ? md.DateCreated.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        string categoriesNode = "categories:\n";
                        foreach (var category in md.Categories)
                        {
                            categoriesNode += $"  - {category}\n";
                        }

                        templateMd = templateMd.Replace("categories:", categoriesNode);

                        string keywordsNode = "tags:\n";
                        foreach (var keyword in md.Keywords.Split(","))
                        {
                            keywordsNode += $"  - {keyword}\n";
                        }

                        templateMd = templateMd.Replace("tags:", keywordsNode);


                        var fileContent = md.Description;

                        int lineNumberToInsert = config.GetMarkdownOption.ReadMorePosition;

                        if (lineNumberToInsert > 0)
                        {
                            fileContent = InsertLine(fileContent, lineNumberToInsert, "<!-- more -->");

                        }
                        fileContent = fileContent.Replace("@[toc]", "<!-- toc -->");

                        var imgPathDic = new Dictionary<string, string>();
                        foreach (var imgContent in RegexUtil.ExtractorImgFromMarkdown(fileContent))
                        {
                            var img = imgContent.Item2;
                            var imgElement = imgContent.Item1;
                            if (imgPathDic.ContainsKey(handler.IsReplaceAllElement ? imgElement : img))
                            {
                                Console.WriteLine($"已上传图片跳过：{img} ");
                                continue;
                            }

                            try
                            {
                                string imgFileName;
                                Stream imgStream;
                                if (img.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                {
                                    var sourceStream = client.GetStreamAsync(img).Result;

                                    imgStream = new MemoryStream();
                                    sourceStream.CopyTo(imgStream);

                                    int lastIndex = img.LastIndexOf('/');
                                    if (lastIndex != -1 && lastIndex < img.Length - 1)
                                    {
                                        imgFileName = img.Substring(lastIndex + 1);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"无法解析图片名称：{img} ");

                                        continue;
                                    }
                                }
                                else
                                {

                                    var imgPhyPath = HttpUtility.UrlDecode(Path.Combine(fileDirectory, img));
                                    if (File.Exists(imgPhyPath) == false)
                                    {
                                        throw new FileNotFoundException($"请检查Markdown图片路径是否正确，文件不存在：{imgPhyPath}");
                                    }

                                    var imgFile = new FileInfo(imgPhyPath);
                                    imgFileName = Path.GetFileName(imgFile.FullName);
                                    imgStream = imgFile.OpenRead();
                                }


                                imgPathDic[handler.IsReplaceAllElement ? imgElement : img] = handler.HandleAsync(imgStream, imgFileName, md.Title, config.AssetsStoreOption);
                                imgStream.Close();

                            }
                            catch (Exception ex) when (config.SkipFileWhenException)
                            {
                                Console.WriteLine($"跳过图片[{img}]，异常原因：处理失败-{ex.Message}");
                            }

                        }

                        //替换
                        fileContent = imgPathDic.Keys.Aggregate(
                            fileContent, (current, key) => current.Replace(key, imgPathDic[key]));


                        var content = string.Concat(templateMd, fileContent);
                        DirFileHelper.WriteText(fileFullPath, content);
                        Console.WriteLine($"Markdown文件处理完成，文件保存在：{fileFullPath}");
                    }

                }
                else
                {
                    Console.WriteLine($"找不到Hexo目录：{templatePath} ");

                }


                var sw = Stopwatch.StartNew();

                sw.Stop();


                Console.WriteLine("Time taken: {0}", sw.Elapsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}未知错误:{0}{1}", Environment.NewLine, ex);
                Environment.ExitCode = 2;
            }

            if (CliProcessor.waitAtEnd)
            {
                Console.WriteLine("{0}{0}敲击回车退出程序", Environment.NewLine);
                Console.ReadLine();
            }
        }


    }
}
