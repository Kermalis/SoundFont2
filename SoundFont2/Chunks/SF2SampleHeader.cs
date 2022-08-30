using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class SF2SampleHeader
	{
		public const uint SIZE = 46;

		/// <summary>Length 20</summary>
		public string SampleName { get; set; }
		public uint Start { get; set; }
		public uint End { get; set; }
		public uint LoopStart { get; set; }
		public uint LoopEnd { get; set; }
		public uint SampleRate { get; set; }
		public byte OriginalKey { get; set; }
		public sbyte PitchCorrection { get; set; }
		public ushort SampleLink { get; set; }
		public SF2SampleLink SampleType { get; set; }

		internal SF2SampleHeader(string name, uint start, uint end, uint loopStart, uint loopEnd, uint sampleRate, byte originalKey, sbyte pitchCorrection)
		{
			SampleName = name;
			Start = start;
			End = end;
			LoopStart = loopStart;
			LoopEnd = loopEnd;
			SampleRate = sampleRate;
			OriginalKey = originalKey;
			PitchCorrection = pitchCorrection;
			SampleType = SF2SampleLink.MonoSample;
		}
		internal SF2SampleHeader(EndianBinaryReader reader)
		{
			SampleName = reader.ReadString_Count_TrimNullTerminators(20);
			Start = reader.ReadUInt32();
			End = reader.ReadUInt32();
			LoopStart = reader.ReadUInt32();
			LoopEnd = reader.ReadUInt32();
			SampleRate = reader.ReadUInt32();
			OriginalKey = reader.ReadByte();
			PitchCorrection = reader.ReadSByte();
			SampleLink = reader.ReadUInt16();
			SampleType = reader.ReadEnum<SF2SampleLink>();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteChars_Count(SampleName, 20);
			writer.WriteUInt32(Start);
			writer.WriteUInt32(End);
			writer.WriteUInt32(LoopStart);
			writer.WriteUInt32(LoopEnd);
			writer.WriteUInt32(SampleRate);
			writer.WriteByte(OriginalKey);
			writer.WriteSByte(PitchCorrection);
			writer.WriteUInt16(SampleLink);
			writer.WriteEnum(SampleType);
		}

		public override string ToString()
		{
			return $"Sample - Name = \"{SampleName}\"" +
				$",\nType = {SampleType}";
		}
	}
}
