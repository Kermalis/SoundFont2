using Kermalis.EndianBinaryIO;
using System.Collections.Generic;

namespace Kermalis.SoundFont2
{
	public sealed class SHDRSubChunk : SF2Chunk
	{
		private readonly List<SF2SampleHeader> _samples = new();
		public uint Count => (uint)_samples.Count;

		internal SHDRSubChunk(SF2 inSf2) : base(inSf2, "shdr") { }
		internal SHDRSubChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			for (int i = 0; i < Size / SF2SampleHeader.SIZE; i++)
			{
				_samples.Add(new SF2SampleHeader(reader));
			}
		}

		internal uint AddSample(SF2SampleHeader sample)
		{
			_samples.Add(sample);
			Size = Count * SF2SampleHeader.SIZE;
			_sf2.UpdateSize();
			return Count - 1;
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			for (int i = 0; i < Count; i++)
			{
				_samples[i].Write(writer);
			}
		}

		public override string ToString()
		{
			return $"Sample Header Chunk - Sample header count = {Count}";
		}
	}
}
