using Kermalis.EndianBinaryIO;
using System.Collections.Generic;

namespace Kermalis.SoundFont2
{
	public sealed class MODSubChunk : SF2Chunk
	{
		private readonly List<SF2ModulatorList> _modulators = new();
		public uint Count => (uint)_modulators.Count;

		internal MODSubChunk(SF2 inSf2, bool isPreset) : base(inSf2, isPreset ? "pmod" : "imod") { }
		internal MODSubChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			for (int i = 0; i < Size / SF2ModulatorList.SIZE; i++)
			{
				_modulators.Add(new SF2ModulatorList(reader));
			}
		}

		internal void AddModulator(SF2ModulatorList modulator)
		{
			_modulators.Add(modulator);
			Size = Count * SF2ModulatorList.SIZE;
			_sf2.UpdateSize();
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			for (int i = 0; i < Count; i++)
			{
				_modulators[i].Write(writer);
			}
		}

		public override string ToString()
		{
			return $"Modulator Chunk - Name = \"{ChunkName}\"" +
				$",\nModulator count = {Count}";
		}
	}
}
