using System.IO;


namespace BlogTool.Core.AssetsStores.Implements;

public class HexoAssetFolderAssetsStoreProvider : IAssetsStoreProvider
{

    public string Store(Stream stream, string fileName, string markdownTitle, AssetsStoreOption option)
    {
        var directoryPath = Path.Combine(option.OutputPath,
             markdownTitle);

        if (Directory.Exists(directoryPath) == false)
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, fileName);
        using (var fileStream = File.OpenWrite(filePath))
        {
            stream.CopyTo(fileStream);
        }

        // return access path
        return  fileName;
    }

    public string ReplaceMode => "Content";

}