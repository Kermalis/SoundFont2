using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	/// <summary>Covers sfPresetBag and sfInstBag</summary>
	public sealed class SF2Bag
	{
		public const uint SIZE = 4;

		/// <summary>Index in list of generators</summary>
		public ushort GeneratorIndex { get; set; }
		/// <summary>Index in list of modulators</summary>
		public ushort ModulatorIndex { get; set; }

		internal SF2Bag(SF2 inSf2, bool isPresetBag)
		{
			if (isPresetBag)
			{
				GeneratorIndex = (ushort)inSf2.HydraChunk.PGENSubChunk.Count;
				ModulatorIndex = (ushort)inSf2.HydraChunk.PMODSubChunk.Count;
			}
			else
			{
				GeneratorIndex = (ushort)inSf2.HydraChunk.IGENSubChunk.Count;
				ModulatorIndex = (ushort)inSf2.HydraChunk.IMODSubChunk.Count;
			}
		}
		internal SF2Bag(EndianBinaryReader reader)
		{
			GeneratorIndex = reader.ReadUInt16();
			ModulatorIndex = reader.ReadUInt16();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt16(GeneratorIndex);
			writer.WriteUInt16(ModulatorIndex);
		}

		public override string ToString()
		{
			return $"Bag - Generator index = {GeneratorIndex}" +
				$",\nModulator index = {ModulatorIndex}";
		}
	}
}
