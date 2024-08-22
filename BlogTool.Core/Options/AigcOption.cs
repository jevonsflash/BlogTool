using System.Net;
using Newtonsoft.Json;

namespace BlogTool.Core.Options
{
    public class AigcOption
    {
        public string Provider { get; set; }
        public string Target { get; set; }
        public string ApiKey { get; set; }

        public AigcOption()
        {
        }
    }
}