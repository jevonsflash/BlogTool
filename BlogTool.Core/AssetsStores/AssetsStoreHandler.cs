using System;
using System.IO;
using BlogTool.Core.Helper;

namespace BlogTool.Core.AssetsStores
{
    public class AssetsStoreHandler
    {
        private AssetsStoreOption _option;
        private IAssetsStoreProvider markdownCreatorProvider;


        public string HandleAsync(Stream imgStream, string fileName, string markdownTitle, AssetsStoreOption config)
        {


            ProcessImageResult result = null;
            try
            {
                Console.WriteLine($"\n图片开始处理：[{fileName}]");
                var (skip, stream, file) = result = ImgUtil.ProcessImage(
                    config.AddWatermark,
                    config.CompressionImage,
                    config.ImageOption,
                    imgStream, fileName);
                var length = stream.Length;

                if (skip)
                {
                    Console.WriteLine("由于文件类型不支持水印、压缩处理，已跳过");
                }
                else
                {
                    Console.WriteLine("图片水印、压缩、转换处理成功");
                }

                stream.Seek(0, SeekOrigin.Begin);

                var displayUrl = markdownCreatorProvider.Store(stream, file, markdownTitle, _option);

                if (displayUrl.Length > 256)
                {
                    displayUrl = $"路径太长，已省略中间部分 - {displayUrl.AsSpan()[..128]} ... {displayUrl.AsSpan()[^128..]}";
                }

                Console.WriteLine($"图片存储成功，存储路径：{displayUrl}");

                return displayUrl;
            }
            finally
            {
                if (result?.Stream is not null)
                {
                    result.Stream.Dispose();
                }
            }



        }

        public bool IsReplaceAllElement => markdownCreatorProvider.ReplaceMode == "Element";


        public AssetsStoreHandler(AssetsStoreOption option, IAssetsStoreProvider markdownCreatorProvider) : this()
        {
            SetAssetsStoreProvider(option, markdownCreatorProvider);

        }

        public AssetsStoreHandler()
        {

        }


        public void SetAssetsStoreProvider(AssetsStoreOption option, IAssetsStoreProvider markdownCreatorProvider)
        {
            _option = option == null ? new AssetsStoreOption() : option;
            this.markdownCreatorProvider = markdownCreatorProvider;


        }
    }
}
