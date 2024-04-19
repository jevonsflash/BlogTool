using System.Diagnostics;

namespace BlogTool
{
    partial class CliProcessor
    {
        public static string outputPath;
        public static string destination;
        public static bool waitAtEnd;
        public static string hexoPath;
        public static string assetsStoreProvider;
        public static string markdownProvider;

        public static void Usage()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Environment.ProcessPath);
            Console.WriteLine();
            Console.WriteLine("Blog Tool v{0}.{1}", versionInfo.FileMajorPart, versionInfo.FileMinorPart);
            Console.WriteLine("参数列表:");
            Console.WriteLine(" -x  Hexo");
            Console.WriteLine("     指定一个Hexo的跟目录，其中必须包含scaffolds模板头, 指定后会覆盖配置");
            Console.WriteLine(" -o  Output");
            Console.WriteLine("     指定一个路径，作为markdown和图片的导出目标，指定后会覆盖配置");
            Console.WriteLine(" -m  MarkdownProvider");
            Console.WriteLine("     值为metaweblog, local, 指定后会覆盖配置");
            Console.WriteLine(" -a  AssetsStoreProvider");
            Console.WriteLine("     值为embed, local, hexo-asset-folder, hexo-tag-plugin, 指定后会覆盖配置");
            Console.WriteLine(" -w  WaitAtEnd");
            Console.WriteLine("     指定时，程序执行完成后，将等待用户输入退出");
            Console.WriteLine(" -h  Help");
            Console.WriteLine("     查看帮助");
        }


        public static bool ProcessCommandLine(string[] args)
        {

            var i = 0;
            while (i < args.Length)
            {
                var arg = args[i];
                if (arg.StartsWith("/") || arg.StartsWith("-"))
                    arg = arg.Substring(1);
                switch (arg.ToLowerInvariant())
                {

                    case "x":
                        i++;
                        if (i < args.Length)
                        {
                            if (args[i].IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                            {
                                Console.WriteLine("路径 '{0}' 不合法", args[i]);
                                return false;
                            }
                            hexoPath = args[i];
                        }
                        else
                            return false;
                        break;


                    case "o":
                        i++;
                        if (i < args.Length)
                        {
                            if (args[i].IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                            {
                                Console.WriteLine("路径 '{0}' 不合法", args[i]);
                                return false;
                            }
                            outputPath = args[i];
                        }
                        else
                            return false;
                        break;
                    case "m":
                        i++;
                        if (i < args.Length)
                        {
                            if (!new string[] { "metaweblog", "local" }.Any(c => c == args[i].ToLower()))
                            {
                                Console.WriteLine("参数值 '{0}' 不合法", args[i]);
                                return false;
                            }
                            markdownProvider = args[i];
                        }
                        else
                            return false;
                        break;
                    case "a":
                        i++;
                        if (i < args.Length)
                        {
                            if (!new string[] { "embed", "local", "hexo-asset-folder", "hexo-tag-plugin" }.Any(c => c == args[i].ToLower()))
                            {
                                Console.WriteLine("参数值 '{0}' 不合法", args[i]);
                                return false;
                            }
                            assetsStoreProvider = args[i];
                        }
                        else
                            return false;
                        break;
                    case "w":
                        waitAtEnd = true;
                        break;
                    case "h":
                        Usage();
                        return false;


                    default:
                        Console.WriteLine("无法识别的参数: {0}", args[i]);
                        break;
                }
                i++;
            }
            return true;

        }
    }
}