using Kermalis.EndianBinaryIO;
using System.Collections.Generic;

namespace Kermalis.SoundFont2
{
	public sealed class GENSubChunk : SF2Chunk
	{
		private readonly List<SF2GeneratorList> _generators = new();
		public uint Count => (uint)_generators.Count;

		internal GENSubChunk(SF2 inSf2, bool isPreset) : base(inSf2, isPreset ? "pgen" : "igen") { }
		internal GENSubChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			for (int i = 0; i < Size / SF2GeneratorList.SIZE; i++)
			{
				_generators.Add(new SF2GeneratorList(reader));
			}
		}

		internal void AddGenerator(SF2GeneratorList generator)
		{
			_generators.Add(generator);
			Size = Count * SF2GeneratorList.SIZE;
			_sf2.UpdateSize();
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			for (int i = 0; i < Count; i++)
			{
				_generators[i].Write(writer);
			}
		}

		public override string ToString()
		{
			return $"Generator Chunk - Name = \"{ChunkName}\"" +
				$",\nGenerator count = {Count}";
		}
	}
}
