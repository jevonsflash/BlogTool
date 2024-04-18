using System;
using System.IO;
using BlogTool.Core.Helper;

namespace BlogTool.Core.AssetsStores.Implements;

public class EmbedAssetsStoreProvider : IAssetsStoreProvider
{
    public string Store(Stream stream, string fileName)
    {
        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes);
        var base64Str = Convert.ToBase64String(bytes);
        return $"data:{MimeTypeUtil.GetMimeType(Path.GetExtension(fileName))};base64,{base64Str}";
    }
}