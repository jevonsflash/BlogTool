﻿
using System;
using System.IO;
using BlogTool.Core.Options;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace BlogTool.Core;

public class ImageProcessor : IDisposable
{
    private class ImageRawData
    {
        public ImageRawData(Image image, IImageFormat format)
        {
            Image = image;
            Format = format;
        }

        public Image Image { get; set; }
        public IImageFormat Format { get; set; }
    }


    private readonly Lazy<ImageRawData> _imageLazy;
    private readonly Stream _rawStream;
    private Image _newImage;
    private string _newFormatExtension;


    public ImageProcessor(Stream imgStream)
    {
        _rawStream = imgStream;
        _imageLazy = new Lazy<ImageRawData>(() =>
        {
            MemoryStream memoryStream = new MemoryStream();
            _rawStream.CopyTo(memoryStream); // 将流的内容复制到MemoryStream中  
            byte[] buffer = memoryStream.ToArray(); // 将MemoryStream转换为字节数组（buffer）  

            var image =  Image.Load(_rawStream, out var format);
            return new ImageRawData(image, format);
        });
    }

    public ImageProcessor AddWatermark(Font font, string text, Color color, float padding = 5)
    {
        var oldImage = _newImage ?? _imageLazy.Value.Image;
        _newImage = oldImage.Clone(ctx => ApplyScalingWaterMarkSimple(ctx, font, text, color, padding));
        oldImage.Dispose();
        return this;
    }

    public IImageProcessingContext ApplyScalingWaterMarkSimple(
        IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color,
        float padding)
    {
        var imgSize = processingContext.GetCurrentSize();

        // measure the text size
        var useFont = font;
        var fontSize = TextMeasurer.MeasureSize(text, new TextOptions(font));
        if (fontSize.Width + padding > imgSize.Width || fontSize.Height + padding > imgSize.Height)
        {
            //find out how much we need to scale the text to fill the space (up or down)
            var scalingFactor = Math.Min(
                imgSize.Width / (fontSize.Width + padding),
                imgSize.Height / (fontSize.Height + padding));
            useFont = new Font(font, scalingFactor * font.Size);
            fontSize = TextMeasurer.MeasureSize(text, new TextOptions(useFont));
        }


        var location = new PointF(
            x: imgSize.Width - fontSize.Width - padding,
            y: imgSize.Height - fontSize.Height - padding);

        var shadow = new PointF(location.X + 2, location.Y + 2);
        processingContext.DrawText(text, useFont, Color.Gray, shadow);
        return processingContext.DrawText(text, useFont, color, location);
    }

    public ImageProcessor ConvertFormat(string format)
    {
        _newFormatExtension = format;
        return this;
    }

    public Stream GetCompressedResult(CompressionLevel level)
    {
        var stream = new MemoryStream();
        var image = _newImage ?? _imageLazy.Value.Image;
        IImageFormat format;
        if (_newFormatExtension is null)
        {
            format = _imageLazy.Value.Format;
        }
        else
        {
            _newImage.Configuration.ImageFormatsManager.TryFindFormatByFileExtension(_newFormatExtension, out format);
        }
        var encoder = GetEncoder(format, level);
        if (encoder is null)
        {
            image.Save(stream, _imageLazy.Value.Format);
        }
        else
        {
            image.Save(stream, encoder);
        }

        return stream;
    }

    public Stream GetResult()
    {
        if (_newImage == null && _newFormatExtension is null)
        {
            return _rawStream;
        }

        var stream = new MemoryStream();

        IImageFormat format;
        if (_newFormatExtension is null)
        {
            format = _imageLazy.Value.Format;
        }
        else
        {
            _newImage.Configuration.ImageFormatsManager.TryFindFormatByFileExtension(_newFormatExtension, out format);
        }
        _newImage.Save(stream,
            format);

        return stream;
    }


    private static IImageEncoder GetEncoder(IImageFormat format, CompressionLevel level)
    {
        return format.Name switch
        {
            // JPEG
            "JPEG" when level == CompressionLevel.Low => new JpegEncoder { Quality = 55 },
            "JPEG" when level == CompressionLevel.Medium => new JpegEncoder { Quality = 35 },
            "JPEG" when level == CompressionLevel.High => new JpegEncoder { Quality = 15 },

            // PNG
            "PNG" when level == CompressionLevel.Low => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level7
            },
            "PNG" when level == CompressionLevel.Medium => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level8
            },
            "PNG" when level == CompressionLevel.High => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level9
            },

            // Webp
            "Webp" when level == CompressionLevel.Low => new WebpEncoder { Quality = 55 },
            "Webp" when level == CompressionLevel.Medium => new WebpEncoder { Quality = 35 },
            "Webp" when level == CompressionLevel.High => new WebpEncoder { Quality = 15 },

            // TIFF
            "TIFF" when level == CompressionLevel.Low => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level7
            },
            "TIFF" when level == CompressionLevel.Medium => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level8
            },
            "TIFF" when level == CompressionLevel.High => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level9
            },
            _ => null
        };
    }

    public void Dispose()
    {
        _imageLazy.Value.Image.Dispose();
        _newImage?.Dispose();
    }
}