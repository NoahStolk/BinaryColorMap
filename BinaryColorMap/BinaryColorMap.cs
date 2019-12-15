using System.Collections.Generic;
using System.IO;

namespace BinaryColorMap
{
	public class BinaryColorMap
	{
		public byte FrameCount { get; }
		public byte Width { get; }
		public byte Height { get; }

		public byte ColorCount => (byte)Colors.Count;

		public List<Color> Colors { get; } = new List<Color>();

		public byte[,,] Pixels { get; }

		public BinaryColorMap(byte frameCount, byte width, byte height)
		{
			FrameCount = frameCount;
			Width = width;
			Height = height;
			Pixels = new byte[FrameCount, Width, Height];
		}

		public byte[] GetPixelData()
		{
			byte[] pixelData = new byte[4 + Pixels.Length];

			pixelData[0] = FrameCount;
			pixelData[1] = Width;
			pixelData[2] = Height;
			pixelData[3] = ColorCount;

			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						pixelData[4 + i * Width * Height + j * Height + k] = Pixels[i, j, k];

			return pixelData;
		}

		public byte[] GetPaletteData()
		{
			byte[] paletteData = new byte[Colors.Count * 4];

			for (int i = 0; i < ColorCount; i++)
			{
				paletteData[i * 4] = Colors[i].R;
				paletteData[i * 4 + 1] = Colors[i].G;
				paletteData[i * 4 + 2] = Colors[i].B;
				paletteData[i * 4 + 3] = Colors[i].A;
			}

			return paletteData;
		}

		public void Write(string path, string fileName)
		{
			File.WriteAllBytes(Path.Combine(path, $"{fileName}.bcm"), GetPixelData());
			File.WriteAllBytes(Path.Combine(path, $"{fileName}.bcp"), GetPaletteData());
		}
	}
}