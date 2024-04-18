using BlogTool.Core.Options;

namespace BlogTool.Core.AssetsStores
{
    public class AssetsStoreOption
    {

        public string SubPath { get; set; }
        public bool AddWatermark { get; internal set; }
        public bool CompressionImage { get; internal set; }
        public ImageOption ImageOption { get; internal set; }
    }
}