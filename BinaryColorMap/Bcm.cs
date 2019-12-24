using NetBase.Utils;
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
			byte[] pixelData = new byte[HeaderSize + PixelData.Length];

			pixelData[0] = FrameCount;
			pixelData[1] = ColorCount;
			pixelData[2] = Width;
			pixelData[3] = Height;
			pixelData[4] = OriginX;
			pixelData[5] = OriginY;

			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						pixelData[HeaderSize + i * Width * Height + j * Height + k] = PixelData[i, j, k];

			return pixelData;
		}

		public static Bcm Create(byte[] pixelData)
		{
			Bcm bcm = new Bcm(pixelData[0], pixelData[2], pixelData[3], pixelData[4], pixelData[5]);

			for (int i = 0; i < bcm.FrameCount; i++)
				for (int j = 0; j < bcm.Width; j++)
					for (int k = 0; k < bcm.Height; k++)
						bcm.PixelData[i, j, k] = pixelData[HeaderSize + i * bcm.Width * bcm.Height + j * bcm.Height + k];

			return bcm;
		}
	}
}