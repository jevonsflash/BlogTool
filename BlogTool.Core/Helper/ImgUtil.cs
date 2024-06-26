﻿using System;
using System.IO;
using BlogTool.Core.Options;

namespace BlogTool.Core.Helper;

public static class ImgUtil
{
    public static ProcessImageResult ProcessImage(
        bool addWatermark,
        bool compressImg,
        ImageOption imageConfig,
        Stream imgStream, string fileName)
    {
        if (addWatermark == false && compressImg == false)
            return new ProcessImageResult(false, imgStream, fileName);

        try
        {
            var processor = new ImageProcessor(imgStream);
            if (addWatermark)
                processor.AddWatermark(imageConfig.GetFont(),
                    imageConfig.WatermarkText ?? throw new ArgumentException("水印文字为空，请配置[ImageOption:WatermarkText]"),
                    imageConfig.GetWatermarkFontColor());

            if (imageConfig.ConvertFormatTo is not null)
            {
                processor.ConvertFormat(imageConfig.ConvertFormatTo);
                fileName = Path.ChangeExtension(fileName, imageConfig.ConvertFormatTo);
            }

            return new ProcessImageResult(
                false,
                compressImg
                ? processor.GetCompressedResult(imageConfig.GetCompressionLevel())
                : processor.GetResult(),
                fileName);
        }
        catch (SixLabors.ImageSharp.UnknownImageFormatException) when (imageConfig.SkipNotSupportFormat)
        {
            return new ProcessImageResult(true, imgStream, fileName);
        }
    }
}

public record ProcessImageResult(bool SkipNotSupport, Stream Stream, string FileName);