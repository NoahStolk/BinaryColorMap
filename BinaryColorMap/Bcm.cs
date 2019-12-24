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
			byte[] pixelData = new byte[PixelData.Length];
			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						pixelData[i * Width * Height + j * Height + k] = PixelData[i, j, k];

			byte[] pixelNibbles = ConvertByteArrayToNibbleArray(pixelData);

			byte[] finalData = new byte[HeaderSize + pixelNibbles.Length];

			finalData[0] = FrameCount;
			finalData[1] = ColorCount;
			finalData[2] = Width;
			finalData[3] = Height;
			finalData[4] = OriginX;
			finalData[5] = OriginY;

			Buffer.BlockCopy(pixelNibbles, 0, finalData, HeaderSize, pixelNibbles.Length);

			return finalData;
		}

		public static Bcm Create(byte[] pixelData)
		{
			Bcm bcm = new Bcm(pixelData[0], pixelData[2], pixelData[3], pixelData[4], pixelData[5]);

			byte[] pixelNibbles = new byte[(pixelData.Length - HeaderSize)];
			Buffer.BlockCopy(pixelData, HeaderSize, pixelNibbles, 0, pixelNibbles.Length);

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
				byte nibble2 = (byte)(nibbles[i] & 0x0F);
				byte nibble1 = (byte)((nibbles[i] & 0xF0) >> 4);

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
	}
}