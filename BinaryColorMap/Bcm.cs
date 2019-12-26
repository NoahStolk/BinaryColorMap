using NetBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryColorMap
{
	public class Bcm
	{
		private const int HeaderSize = 6;

		public byte FrameCount { get; private set; }
		public byte Width { get; private set; }
		public byte Height { get; private set; }
		public byte OriginX { get; private set; }
		public byte OriginY { get; private set; }

		public byte ColorCount => (byte)Palettes.Select(p => (int)p.ColorCount).Sum();

		public byte[,,] PixelData { get; }
		public List<Palette> Palettes { get; } = new List<Palette>();

		public Bcm(byte frameCount, byte width, byte height, byte originX, byte originY)
		{
			FrameCount = frameCount;
			Width = width;
			Height = height;
			OriginX = originX;
			OriginY = originY;

			PixelData = new byte[FrameCount, Width, Height];
		}

		public void WritePixelData(string path, string fileName)
		{
			FileUtils.CreateDirectoryIfNotExists(path);
			File.WriteAllBytes(Path.Combine(path, $"{fileName}.bcm"), GetPixelData());
		}

		public byte[] GetPixelData()
		{
			byte[] pixelBytes = new byte[PixelData.Length];
			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						pixelBytes[i * Width * Height + j * Height + k] = PixelData[i, j, k];

			byte[] pixelNibbles = ConvertByteArrayToNibbleArray(pixelBytes);

			byte[] data = new byte[HeaderSize + pixelNibbles.Length];

			data[0] = FrameCount;
			data[1] = ColorCount;
			data[2] = Width;
			data[3] = Height;
			data[4] = OriginX;
			data[5] = OriginY;

			Buffer.BlockCopy(pixelNibbles, 0, data, HeaderSize, pixelNibbles.Length);

			return data;
		}

		public static Bcm Create(byte[] data)
		{
			Bcm bcm = new Bcm(data[0], data[2], data[3], data[4], data[5]);

			byte[] pixelNibbles = new byte[(data.Length - HeaderSize)];
			Buffer.BlockCopy(data, HeaderSize, pixelNibbles, 0, pixelNibbles.Length);

			byte[] pixelBytes = ConvertNibbleArrayToByteArray(pixelNibbles);

			for (int i = 0; i < bcm.FrameCount; i++)
				for (int j = 0; j < bcm.Width; j++)
					for (int k = 0; k < bcm.Height; k++)
						bcm.PixelData[i, j, k] = pixelBytes[i * bcm.Width * bcm.Height + j * bcm.Height + k];

			return bcm;
		}

		private static byte[] ConvertNibbleArrayToByteArray(byte[] nibbles)
		{
			byte[] bytes = new byte[nibbles.Length * 2];
			for (int i = 0; i < nibbles.Length; i++)
			{
				byte nibble1 = (byte)((nibbles[i] & 0xF0) >> 4);
				byte nibble2 = (byte)(nibbles[i] & 0x0F);

				bytes[i * 2] = nibble1;
				bytes[i * 2 + 1] = nibble2;
			}
			return bytes;
		}

		private static byte[] ConvertByteArrayToNibbleArray(byte[] bytes)
		{
			byte[] nibbles = new byte[bytes.Length / 2];
			for (int i = 0; i < bytes.Length; i += 2)
			{
				byte byte1 = bytes[i];
				byte byte2 = bytes[i + 1];

				byte nibble1 = (byte)(byte1 & 0x0F);
				byte nibble2 = (byte)(byte2 & 0x0F);
				nibbles[i / 2] = (byte)((nibble1 << 4) | nibble2);
			}
			return nibbles;
		}

		public (int x, int y, int width, int height) GetActualSizeBasedOnTransparency(byte transparentColorIndex, byte frameIndex)
		{
			int firstX = int.MaxValue;
			int firstY = int.MaxValue;
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					if (PixelData[frameIndex, i, j] != transparentColorIndex)
					{
						if (i < firstX)
							firstX = i;
						if (j < firstY)
							firstY = j;
					}
				}
			}

			int lastX = int.MinValue;
			int lastY = int.MinValue;
			for (int i = Width - 1; i >= 0; i--)
			{
				for (int j = Height - 1; j >= 0; j--)
				{
					if (PixelData[frameIndex, i, j] != transparentColorIndex)
					{
						if (i > lastX)
							lastX = i;
						if (j > lastY)
							lastY = j;
					}
				}
			}

			return (firstX, firstY, lastX - firstX + 1, lastY - firstY + 1);
		}
	}
}