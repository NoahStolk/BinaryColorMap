using System.Collections.Generic;
using System.IO;

namespace BinaryColorMap
{
	public class Palette
	{
		public byte ColorCount => (byte)Colors.Count;

		public string Name { get; }
		public List<BcmColor> Colors { get; } = new List<BcmColor>();

		public Palette(string name)
		{
			Name = name;
		}

		public void WritePaletteData(string path, string baseFileName)
		{
			string directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(path);
			File.WriteAllBytes(Path.Combine(path, $"{baseFileName}-{Name}.bcp"), GetPaletteData());
		}

		public byte[] GetPaletteData()
		{
			byte[] paletteData = new byte[ColorCount * 4];

			for (int i = 0; i < ColorCount; i++)
			{
				paletteData[i * 4] = Colors[i].R;
				paletteData[i * 4 + 1] = Colors[i].G;
				paletteData[i * 4 + 2] = Colors[i].B;
				paletteData[i * 4 + 3] = Colors[i].A;
			}

			return paletteData;
		}

		public static Palette Create(string name, byte[] paletteData)
		{
			Palette palette = new Palette(name);

			for (int i = 0; i < paletteData.Length / 4; i++)
			{
				BcmColor color = new BcmColor
				{
					R = paletteData[i * 4],
					G = paletteData[i * 4 + 1],
					B = paletteData[i * 4 + 2],
					A = paletteData[i * 4 + 3]
				};
				palette.Colors.Add(color);
			}

			return palette;
		}

		public static string GetPaletteNameFromPath(string path)
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			string baseFileName = fileName.Substring(0, fileName.LastIndexOf('-'));
			return fileName.Substring(baseFileName.Length + 1, fileName.Length - baseFileName.Length - 1);
		}
	}
}