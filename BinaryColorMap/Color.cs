using System.Runtime.InteropServices;

namespace BinaryColorMap
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Color
	{
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }

		public Color(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public override bool Equals(object obj) =>
			obj is Color color
			&& color.R == R
			&& color.G == G
			&& color.B == B
			&& color.A == A;

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 1960784236;
				hashCode = hashCode * -1521134295 + R.GetHashCode();
				hashCode = hashCode * -1521134295 + G.GetHashCode();
				hashCode = hashCode * -1521134295 + B.GetHashCode();
				hashCode = hashCode * -1521134295 + A.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Color a, Color b) => Equals(a, b);

		public static bool operator !=(Color a, Color b) => !(a == b);
	}
}