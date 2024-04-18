using System.IO;

namespace BlogTool.Core.AssetsStores
{
    public abstract class AssetsStoreProvider : IAssetsStoreProvider
    {
        public AssetsStoreProvider()
        {

        }

        public abstract string Store(Stream stream, string fileName, AssetsStoreOption option);

    }
}
