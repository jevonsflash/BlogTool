using BlogTool.Core.Options;

namespace BlogTool.Core.AssetsStores
{
    public class AssetsStoreOption
    {

        public string SubPath { get; set; }
        public bool AddWatermark { get; set; }
        public bool CompressionImage { get; set; }
        public ImageOption ImageOption { get; set; }

        public string OutputPath;

    }
}