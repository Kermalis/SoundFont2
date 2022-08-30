using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class SdtaListChunk : SF2ListChunk
	{
		public SMPLSubChunk SMPLSubChunk { get; }

		internal SdtaListChunk(SF2 inSf2) : base(inSf2, "sdta")
		{
			SMPLSubChunk = new SMPLSubChunk(inSf2);
			inSf2.UpdateSize();
		}
		internal SdtaListChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			SMPLSubChunk = new SMPLSubChunk(inSf2, reader);
		}

		internal override uint UpdateSize()
		{
			return Size = 4
				+ SMPLSubChunk.Size + 8;
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			SMPLSubChunk.Write(writer);
		}

		public override string ToString()
		{
			return $"Sample Data List Chunk";
		}
	}
}
