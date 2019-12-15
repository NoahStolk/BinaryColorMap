using System.Collections.Generic;

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

		public byte[] ToBinary()
		{
			byte[] output = new byte[4 + Colors.Count * 4 + Pixels.Length];
			output[0] = FrameCount;
			output[1] = Width;
			output[2] = Height;
			output[3] = ColorCount;

			for (int i = 0; i < ColorCount; i++)
			{
				output[4 + i * 4] = Colors[i].R;
				output[4 + i * 4 + 1] = Colors[i].G;
				output[4 + i * 4 + 2] = Colors[i].B;
				output[4 + i * 4 + 3] = Colors[i].A;
			}

			for (int i = 0; i < FrameCount; i++)
				for (int j = 0; j < Width; j++)
					for (int k = 0; k < Height; k++)
						output[4 + ColorCount * 4 + i * Width * Height + j * Height + k] = Pixels[i, j, k];

			return output;
		}
	}
}