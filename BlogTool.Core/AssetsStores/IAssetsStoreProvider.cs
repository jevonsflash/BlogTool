using System.IO;

namespace BlogTool.Core.AssetsStores
{
    public interface IAssetsStoreProvider
    {
        string Store(Stream stream, string fileName, AssetsStoreOption option);
    }
}