using System;
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
				byte[] result = bcm.ToBinary();
				File.WriteAllBytes($"{Path.GetFileNameWithoutExtension(path)}.bcm", result);

				Console.WriteLine(Path.GetFileNameWithoutExtension(path));
				Console.WriteLine($"PNG size: {new FileInfo(path).Length} bytes.");
				Console.WriteLine($"BCM size: {result.Length} bytes. ({bcm.Pixels.Length} pixels, {bcm.ColorCount} colors)");
				Console.WriteLine();
			}
		}
	}
}