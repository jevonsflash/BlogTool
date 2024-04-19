using System.IO;

namespace BlogTool.Core.AssetsStores
{
    public abstract class AssetsStoreProvider : IAssetsStoreProvider
    {
        public AssetsStoreProvider()
        {

        }

        public abstract string ReplaceMode { get; }

        public abstract string Store(Stream stream, string fileName, string markdownTitle, AssetsStoreOption option);

    }
}
