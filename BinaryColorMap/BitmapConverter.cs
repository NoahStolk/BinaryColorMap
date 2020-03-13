using System;
using System.Drawing;
using System.IO;

namespace BinaryColorMap
{
	public static class BitmapConverter
	{
		public const int NibbleMaxValueIncludingZero = 16;

		public static Bcm ConvertBitmapToBcmObject(string bitmapPath, byte frameCount, byte originX, byte originY)
		{
			Bitmap bitmap = new Bitmap(bitmapPath);
			return ConvertBitmapToBcmObject(bitmap, frameCount, originX, originY);
		}

		public static Bcm ConvertBitmapToBcmObject(Bitmap bitmap, byte frameCount, byte originX, byte originY)
		{
			if (bitmap.Width / frameCount > byte.MaxValue || bitmap.Height > byte.MaxValue) // Assuming frames are displayed horizontally.
				throw new Exception($"BCM format doesn't support images with a width or height greater than {byte.MaxValue} pixels for now.");

			Bcm bcm = new Bcm(frameCount, (byte)(bitmap.Width / frameCount), (byte)bitmap.Height, originX, originY);
			bcm.Palettes.Add(new Palette("Default"));

			for (int i = 0; i < bcm.FrameCount; i++)
			{
				for (int j = 0; j < bcm.Width; j++)
				{
					for (int k = 0; k < bcm.Height; k++)
					{
						Color c = bitmap.GetPixel(i * bcm.Width + j, k);
						BcmColor color = new BcmColor(c.R, c.G, c.B, c.A);
						if (!bcm.Palettes[0].Colors.Contains(color))
							bcm.Palettes[0].Colors.Add(color);

						if (bcm.Palettes[0].Colors.Count > NibbleMaxValueIncludingZero)
							throw new Exception($"BCM format doesn't support images with more than {NibbleMaxValueIncludingZero} colors.");

						bcm.PixelData[i, j, k] = (byte)bcm.Palettes[0].Colors.IndexOf(color);
					}
				}
			}

			return bcm;
		}

		public static Bitmap ConvertBcmToBitmapObject(string bcmPath, Palette palette)
		{
			Bcm bcm = Bcm.Create(File.ReadAllBytes(bcmPath));
			return ConvertBcmToBitmapObject(bcm, palette);
		}

		public static Bitmap ConvertBcmToBitmapObject(Bcm bcm, Palette palette)
		{
			Bitmap bitmap = new Bitmap(bcm.FrameCount * bcm.Width, bcm.Height);

			for (int i = 0; i < bcm.FrameCount; i++)
			{
				for (int j = 0; j < bcm.Width; j++)
				{
					for (int k = 0; k < bcm.Height; k++)
					{
						byte colorIndex = bcm.PixelData[i, j, k];
						BcmColor c = palette.Colors[colorIndex];
						bitmap.SetPixel(i * bcm.Width + j, k, Color.FromArgb(c.A, c.R, c.G, c.B));
					}
				}
			}

			return bitmap;
		}

		public static Palette ConvertBitmapToPaletteObject(string bitmapPath)
		{
			Bitmap bitmap = new Bitmap(bitmapPath);
			return ConvertBitmapToPaletteObject(bitmap, Palette.GetPaletteNameFromPath(bitmapPath));
		}

		public static Palette ConvertBitmapToPaletteObject(Bitmap bitmap, string paletteName)
		{
			if (bitmap.Width > NibbleMaxValueIncludingZero || bitmap.Height > 1)
				throw new Exception($"BCP format doesn't support palette images with more than {NibbleMaxValueIncludingZero} colors.");

			Palette palette = new Palette(paletteName);

			for (int i = 0; i < bitmap.Width; i++)
			{
				Color c = bitmap.GetPixel(i, 0);
				palette.Colors.Add(new BcmColor(c.R, c.G, c.B, c.A));
			}

			return palette;
		}

		public static Bitmap ConvertPaletteToBitmapObject(string palettePath)
		{
			Palette palette = Palette.Create(Path.GetFileNameWithoutExtension(palettePath), File.ReadAllBytes(palettePath));
			return ConvertPaletteToBitmapObject(palette);
		}

		public static Bitmap ConvertPaletteToBitmapObject(Palette palette)
		{
			Bitmap bitmap = new Bitmap(palette.ColorCount, 1);

			for (int i = 0; i < palette.ColorCount; i++)
			{
				BcmColor c = palette.Colors[i];
				bitmap.SetPixel(i, 0, Color.FromArgb(c.A, c.R, c.G, c.B));
			}

			return bitmap;
		}
	}
}