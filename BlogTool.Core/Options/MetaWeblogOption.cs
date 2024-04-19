using System.Net;
using Newtonsoft.Json;

namespace BlogTool.Core.Options
{
    public class MetaWeblogOption
    {
        public string BlogURL { get; set; }
        public string MetaWeblogURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public CookieContainer Cookies = null;
        public string BlogID;

        public MetaWeblogOption()
        {
        }
    }
}