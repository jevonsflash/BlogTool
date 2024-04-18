using BlogTool.Core.AssetsStores;
using BlogTool.Core.AssetsStores.Implements;
using BlogTool.Core.Helper;
using BlogTool.Core.Markdown;
using BlogTool.Core.Markdown.Implements;
using BlogTool.Core.Options;
using System.Diagnostics;
using System.Web;
using DirFileHelper = BlogTool.Core.Helper.DirFileHelper;

namespace BlogTool
{
    partial class Program
    {
        private static AppOption config;

        static void Main(string[] args)
        {
            if (!CliProcessor.ProcessCommandLine(args))
            {
                Console.WriteLine("缺少参数或参数不正确");

                CliProcessor.Usage();
                Environment.ExitCode = 1;
                return;
            }

            string defaultFontName = ConfigurationHelper.GetConfigValue("HeaderDefaultStyle:DefaultFontName", "宋体");
            var defaultFontColor = ConfigurationHelper.GetConfigValue("HeaderDefaultStyle:DefaultFontColor", "#FFFFFF");
            short defaultFontSize = Convert.ToInt16(ConfigurationHelper.GetConfigValue("HeaderDefaultStyle:DefaultFontSize", "10"));
            var defaultBorderColor = ConfigurationHelper.GetConfigValue("HeaderDefaultStyle:DefaultBorderColor", "#000000");
            var defaultBackColor = ConfigurationHelper.GetConfigValue("HeaderDefaultStyle:DefaultBackColor", "#888888");


            try
            {

                config = new AppOption();

                var creator = new MarkdownCreator();
                var handler = new AssetsStoreHandler();
                if (config.MarkdownProvider == "MetaWeblog")
                {
                    creator.SetMarkdownProvider(new GetMarkdownOption(), new MetaWeblogMarkdownProvider());

                }
                else if (config.MarkdownProvider == "Local")
                {
                    creator.SetMarkdownProvider(new GetMarkdownOption(), new LocalMarkdownProvider());

                }

                if (config.AssetsStoreProvider == "Embed")
                {
                    handler.SetAssetsStoreProvider(new AssetsStoreOption(), new EmbedAssetsStoreProvider());

                }
                else if (config.AssetsStoreProvider == "Local")
                {

                    handler.SetAssetsStoreProvider(new AssetsStoreOption(), new LocalAssetsStoreProvider());
                }
                var mds = creator.Create();

                string templatePath = CliProcessor.patternFilePath + "\\scaffolds\\post.md";

                string fileFullPath;


                var fileDirectory = Directory.Exists(config.DefaultOutputPath) == false
                    ? Directory.CreateDirectory(config.DefaultOutputPath).FullName
                    : new DirectoryInfo(config.DefaultOutputPath).FullName;



                if (File.Exists(templatePath))
                {
                    foreach (var md in mds)
                    {
                        var fileName = md.Title + ".md";

                        fileFullPath = Path.Combine(fileDirectory, fileName);

                        string templateMd = File.ReadAllText(templatePath);
                        templateMd = templateMd.Replace("{{ title }}", md.Title);
                        templateMd = templateMd.Replace("{{ date }}", md.DateCreated.HasValue ? md.DateCreated.Value.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        var fileContent = md.Description;

                        var imgPathDic = new Dictionary<string, string>();
                        foreach (var img in RegexUtil.ExtractorImgFromMarkdown(fileContent))
                        {
                            if (img.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"Web图片跳过：{img} ");
                                continue;
                            }

                            if (imgPathDic.ContainsKey(img))
                            {
                                Console.WriteLine($"已上传图片跳过：{img} ");
                                continue;
                            }

                            try
                            {
                                var imgPhyPath = HttpUtility.UrlDecode(Path.Combine(fileDirectory, img));
                                if (File.Exists(imgPhyPath) == false)
                                {
                                    throw new FileNotFoundException($"请检查Markdown图片路径是否正确，文件不存在：{imgPhyPath}");
                                }

                                var imgFile = new FileInfo(imgPhyPath);
                                imgPathDic[img] = handler.HandleAsync(imgFile, config.AssetsStoreOption);
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
