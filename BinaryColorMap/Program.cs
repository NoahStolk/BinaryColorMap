using System.IO;

namespace BinaryColorMap
{
	public static class Program
	{
		public static void Main()
		{
			foreach (string path in Directory.GetFiles("Content"))
			{
				BinaryColorMap bcm = PngConverter.ConvertPngToBcm(path, 1);
				bcm.Write(Path.GetPathRoot(path), Path.GetFileNameWithoutExtension(path));
			}
		}
	}
}