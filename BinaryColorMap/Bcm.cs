using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryColorMap
{
	public class Bcm
	{
		public byte FrameCount { get; private set; }
		public byte Width { get; private set; }
		public byte Height { get; private set; }

		public byte ColorCount => (byte)Palettes.Select(p => (int)p.ColorCount).Sum();

		public byte[,,] PixelData { get; }
		public List<Palette> Palettes { get; } = new List<Palette>();

		public Bcm(byte frameCount, byte width, byte height)
		{
			FrameCount = frameCount;
			Width = width;
			Height = height;
			PixelData = new byte[FrameCount, Width, Height];
		}

		public void WritePixelData(string path, string fileName)
		{
			File.WriteAllBytes(Path.Combine(path, $"{fileName}.bcm"), GetPixelData());
		}

		public byte[] GetPixelData()
		{
			byte[] pixelData = new byte[4 + PixelData.Length];

			pixelData[0] = FrameCount;
			pixelData[1] = Width;
			pixelData[2] = Height;
			pixelData[3] = ColorCount;

			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						pixelData[4 + i * Width * Height + j * Height + k] = PixelData[i, j, k];

			return pixelData;
		}

		public static Bcm Create(byte[] pixelData)
		{
			Bcm bcm = new Bcm(pixelData[0], pixelData[1], pixelData[2]);

			for (int i = 0; i < bcm.FrameCount; i++)
				for (int j = 0; j < bcm.Width; j++)
					for (int k = 0; k < bcm.Height; k++)
						bcm.PixelData[i, j, k] = pixelData[4 + i * bcm.Width * bcm.Height + j * bcm.Height + k];

			return bcm;
		}
	}
}