using Kermalis.EndianBinaryIO;
using System;

namespace Kermalis.SoundFont2
{
	public sealed class SMPLSubChunk : SF2Chunk
	{
		private short[] _samples; // Block of sample data

		internal SMPLSubChunk(SF2 inSf2) : base(inSf2, "smpl")
		{
			_samples = Array.Empty<short>();
		}
		internal SMPLSubChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			_samples = new short[Size / sizeof(short)];
			reader.ReadInt16s(_samples);
		}

		// Returns index of the start of the sample
		internal uint AddSample(ReadOnlySpan<short> pcm16, bool bLoop, uint loopPos)
		{
			int start = _samples.Length;
			uint sampleIndex = (uint)start;
			int numNewSamples = start
				+ pcm16.Length
				+ (bLoop ? 8 : 0)
				+ 46;
			Array.Resize(ref _samples, numNewSamples);

			// Write wave
			pcm16.CopyTo(_samples.AsSpan(start));
			start += pcm16.Length;

			// If looping is enabled, write 8 samples from the loop point
			if (bLoop)
			{
				// In case (loopPos + i) is greater than the sample length
				uint max = (uint)pcm16.Length - loopPos;
				for (uint i = 0; i < 8; i++)
				{
					_samples[start++] = pcm16[(int)(loopPos + (i % max))];
				}
			}

			// 46 empty samples are remaining at the end

			Size = (uint)_samples.Length * sizeof(short);
			_sf2.UpdateSize();
			return sampleIndex;
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteInt16s(_samples);
		}

		public override string ToString()
		{
			return $"Sample Data Chunk";
		}
	}
}
