namespace BlogTool.Core.Options
{
    public class MetaWeblogOption
    {
        public string BlogURL { get; set; }
        public string MetaWeblogURL { get; set; }
        public string BlogID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MetaWeblogOption(string blogurl, string metaweblogurl, string blogid, string username, string password)
        {
            BlogURL = blogurl;
            BlogID = blogid;
            MetaWeblogURL = metaweblogurl;
            Username = username;
            Password = password;
        }

        public MetaWeblogOption()
        {
        }
    }
}