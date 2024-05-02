using System.Net;
using Newtonsoft.Json;

namespace BlogTool.Core.Options
{
    public class LocalOption
    {
        public string Path { get; set; }
        public bool Recursive { get; set; }

        public LocalOption()
        {
        }
    }
}