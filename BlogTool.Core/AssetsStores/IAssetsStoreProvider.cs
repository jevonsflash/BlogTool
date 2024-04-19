using System.IO;

namespace BlogTool.Core.AssetsStores
{
    public interface IAssetsStoreProvider
    {
        public string ReplaceMode { get; }
        string Store(Stream stream, string fileName, string markdownTitle, AssetsStoreOption option);
    }
}