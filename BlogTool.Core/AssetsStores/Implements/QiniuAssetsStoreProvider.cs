using System;
using System.IO;
using BlogTool.Core.Helper;

namespace BlogTool.Core.AssetsStores.Implements;

public class QiniuAssetsStoreProvider : IAssetsStoreProvider
{
    public string Store(Stream stream, string fileName, string markdownTitle, AssetsStoreOption option)
    {
        //Todo:实现七牛云存储
        throw new NotImplementedException();
    }

    public string ReplaceMode => "Content";

}