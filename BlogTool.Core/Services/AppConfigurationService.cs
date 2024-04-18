﻿using BlogTool.Core.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BlogTool.Core.Services
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class AppConfigurtaionService
    {
        private static string jsonPath = "appsettings.json";

        public static IConfiguration Configuration { get; set; }
        static AppConfigurtaionService()
        {
            if (!DirFileHelper.IsExistFile(jsonPath))
            {
                return;
            }
            var builder = new ConfigurationBuilder();
            builder.Add(new JsonConfigurationSource { Path = jsonPath, ReloadOnChange = true });
            Configuration = builder.Build();
        }
    }
}
