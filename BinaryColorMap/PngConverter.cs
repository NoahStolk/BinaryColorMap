using System;
using System.Drawing;

namespace BinaryColorMap
{
	public static class PngConverter
	{
		public static BinaryColorMap ConvertPngToBcm(string pngPath, byte frameCount)
		{
			Bitmap bitmap = new Bitmap(pngPath);

			if (bitmap.Width / frameCount > byte.MaxValue || bitmap.Height > byte.MaxValue) // Assuming frames are displayed horizontally.
				throw new Exception($"BCM format doesn't support images with a width or height greater than {byte.MaxValue} pixels for now.");

			BinaryColorMap bcm = new BinaryColorMap(frameCount, (byte)bitmap.Width, (byte)bitmap.Height);

			for (int i = 0; i < bcm.FrameCount; i++)
			{
				for (int j = 0; j < bcm.Width; j++)
				{
					for (int k = 0; k < bcm.Height; k++)
					{
						System.Drawing.Color c = bitmap.GetPixel(j, k); // TODO: Take frameCount into account.
						Color color = new Color(c.R, c.G, c.B, c.A);
						if (!bcm.Colors.Contains(color))
							bcm.Colors.Add(color);

						if (bcm.Colors.Count > byte.MaxValue)
							throw new Exception($"BCM format doesn't support images with more than {byte.MaxValue} colors.");

						bcm.Pixels[i, j, k] = (byte)bcm.Colors.IndexOf(color);
					}
				}
			}

			return bcm;
		}
	}
}