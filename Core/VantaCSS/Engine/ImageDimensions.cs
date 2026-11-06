using System;
using System.IO;

namespace VantaCSS
{
    /// <summary>
    /// Reads image pixel dimensions from file headers (PNG, GIF, BMP, JPEG)
    /// without decoding pixel data or depending on System.Drawing.
    /// </summary>
    internal static class ImageDimensions
    {
        public static bool TryGetSize(Stream stream, out int width, out int height)
        {
            width = 0;
            height = 0;
            if (stream == null || !stream.CanSeek) return false;

            var header = new byte[24];
            int read = ReadFully(stream, header, header.Length);
            if (read < 10) return false;

            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 && read >= 24)
            {
                width = ReadBigEndian32(header, 16);
                height = ReadBigEndian32(header, 20);
            }
            else if (header[0] == 'G' && header[1] == 'I' && header[2] == 'F')
            {
                width = ReadLittleEndian16(header, 6);
                height = ReadLittleEndian16(header, 8);
            }
            else if (header[0] == 'B' && header[1] == 'M' && read >= 26)
            {
                width = ReadLittleEndian32(header, 18);
                height = Math.Abs(ReadLittleEndian32(header, 22));
            }
            else if (header[0] == 0xFF && header[1] == 0xD8)
            {
                stream.Position = 2;
                return ReadJpeg(stream, out width, out height);
            }
            else
            {
                return false;
            }

            return width > 0 && height > 0;
        }

        private static bool ReadJpeg(Stream stream, out int width, out int height)
        {
            width = 0;
            height = 0;
            var segmentLength = new byte[2];
            var frame = new byte[5];

            while (true)
            {
                int b = stream.ReadByte();
                if (b < 0) return false;
                if (b != 0xFF) continue;

                int marker = stream.ReadByte();
                while (marker == 0xFF) marker = stream.ReadByte();
                if (marker < 0) return false;

                // Standalone markers carry no length field.
                if (marker == 0x01 || (marker >= 0xD0 && marker <= 0xD8)) continue;
                // Reached the scan data (or end of image) before a frame header.
                if (marker == 0xD9 || marker == 0xDA) return false;

                if (ReadFully(stream, segmentLength, 2) < 2) return false;

                bool isFrameHeader = marker >= 0xC0 && marker <= 0xCF && marker != 0xC4 && marker != 0xC8 && marker != 0xCC;
                if (isFrameHeader)
                {
                    if (ReadFully(stream, frame, frame.Length) < frame.Length) return false;
                    height = (frame[1] << 8) | frame[2];
                    width = (frame[3] << 8) | frame[4];
                    return width > 0 && height > 0;
                }

                int length = (segmentLength[0] << 8) | segmentLength[1];
                if (length < 2) return false;
                stream.Position += length - 2;
            }
        }

        private static int ReadFully(Stream stream, byte[] buffer, int count)
        {
            int total = 0;
            while (total < count)
            {
                int n = stream.Read(buffer, total, count - total);
                if (n <= 0) break;
                total += n;
            }
            return total;
        }

        private static int ReadBigEndian32(byte[] b, int o) => (b[o] << 24) | (b[o + 1] << 16) | (b[o + 2] << 8) | b[o + 3];

        private static int ReadLittleEndian32(byte[] b, int o) => b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24);

        private static int ReadLittleEndian16(byte[] b, int o) => b[o] | (b[o + 1] << 8);
    }
}
