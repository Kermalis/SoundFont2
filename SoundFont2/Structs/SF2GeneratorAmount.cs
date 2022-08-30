using System.Runtime.InteropServices;

namespace Kermalis.SoundFont2
{
	/// <summary>SF2 spec v2.1 page 19 - Two bytes that can handle either two 8-bit values or a single 16-bit value</summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct SF2GeneratorAmount
	{
		[FieldOffset(0)] public byte LowByte;
		[FieldOffset(1)] public byte HighByte;
		[FieldOffset(0)] public short Amount;
		[FieldOffset(0)] public ushort UAmount;

		public override string ToString()
		{
			return $"BLo = {LowByte}, BHi = {HighByte}, Sh = {Amount}, U = {UAmount}";
		}
	}
}
