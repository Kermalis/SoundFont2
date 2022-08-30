using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class SF2Instrument
	{
		public const uint SIZE = 22;

		/// <summary>Length 20</summary>
		public string InstrumentName { get; set; }
		public ushort InstrumentBagIndex { get; set; }

		internal SF2Instrument(string name, ushort index)
		{
			InstrumentName = name;
			InstrumentBagIndex = index;
		}
		internal SF2Instrument(EndianBinaryReader reader)
		{
			InstrumentName = reader.ReadString_Count_TrimNullTerminators(20);
			InstrumentBagIndex = reader.ReadUInt16();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteChars_Count(InstrumentName, 20);
			writer.WriteUInt16(InstrumentBagIndex);
		}

		public override string ToString()
		{
			return $"Instrument - Name = \"{InstrumentName}\"";
		}
	}
}
