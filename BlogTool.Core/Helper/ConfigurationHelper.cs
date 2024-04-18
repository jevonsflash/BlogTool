using BlogTool.Core.Services;

namespace BlogTool.Core.Helper
{
    public class ConfigurationHelper
    {
        public static string GetConfigValue(string key, string defaultValue = default)
        {
            return AppConfigurtaionService.Configuration==null || string.IsNullOrEmpty(AppConfigurtaionService.Configuration[key]) ? defaultValue : AppConfigurtaionService.Configuration[key];
        }

    }
}
