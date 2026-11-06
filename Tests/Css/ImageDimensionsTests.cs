using System;
using Rocket.Core.Components;
using Rocket.Core.DOM;
using VantaCSS;
using Xunit;

namespace Tests
{
    public class ImageDimensionsTests
    {
        private static Node<HTMLImageElementProps> ImageNode(byte[] bytes, string mime) =>
            new Node<HTMLImageElementProps>(
                typeof(HTMLImageElement),
                new HTMLImageElementProps { Src = $"data:{mime};base64,{Convert.ToBase64String(bytes)}" });

        private static byte[] Png(int width, int height)
        {
            var bytes = new byte[24];
            byte[] signature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            Array.Copy(signature, bytes, signature.Length);
            bytes[11] = 13;
            bytes[12] = (byte)'I'; bytes[13] = (byte)'H'; bytes[14] = (byte)'D'; bytes[15] = (byte)'R';
            bytes[16] = (byte)(width >> 24); bytes[17] = (byte)(width >> 16); bytes[18] = (byte)(width >> 8); bytes[19] = (byte)width;
            bytes[20] = (byte)(height >> 24); bytes[21] = (byte)(height >> 16); bytes[22] = (byte)(height >> 8); bytes[23] = (byte)height;
            return bytes;
        }

        private static byte[] Gif(int width, int height) => new byte[]
        {
            (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a',
            (byte)width, (byte)(width >> 8), (byte)height, (byte)(height >> 8)
        };

        private static byte[] Jpeg(int width, int height) => new byte[]
        {
            0xFF, 0xD8,
            0xFF, 0xE0, 0x00, 0x10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0xFF, 0xC0, 0x00, 0x11, 0x08,
            (byte)(height >> 8), (byte)height,
            (byte)(width >> 8), (byte)width,
            0x03, 0x01, 0x22, 0x00, 0x02, 0x11, 0x01, 0x03, 0x11, 0x01
        };

        [Fact]
        public void PngDataUri_SizesImageFromHeader()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = ImageNode(Png(40, 25), "image/png");
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(40, node.Layout.Width);
            Assert.Equal(25, node.Layout.Height);
        }

        [Fact]
        public void GifDataUri_SizesImageFromHeader()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = ImageNode(Gif(17, 9), "image/gif");
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(17, node.Layout.Width);
            Assert.Equal(9, node.Layout.Height);
        }

        [Fact]
        public void JpegDataUri_SizesImageFromFrameHeader()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = ImageNode(Jpeg(42, 25), "image/jpeg");
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(42, node.Layout.Width);
            Assert.Equal(25, node.Layout.Height);
        }

        [Fact]
        public void UnrecognisedData_ProducesZeroSize()
        {
            var engine = new VantaCssEngine(800, 600, "portrait");
            var node = ImageNode(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, "image/png");
            engine.CalculateLayout(node, 800, 600, Orientation.Portrait);
            Assert.Equal(0, node.Layout.Width);
            Assert.Equal(0, node.Layout.Height);
        }
    }
}
