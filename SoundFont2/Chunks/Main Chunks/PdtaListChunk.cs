using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class PdtaListChunk : SF2ListChunk
	{
		public PHDRSubChunk PHDRSubChunk { get; }
		public BAGSubChunk PBAGSubChunk { get; }
		public MODSubChunk PMODSubChunk { get; }
		public GENSubChunk PGENSubChunk { get; }
		public INSTSubChunk INSTSubChunk { get; }
		public BAGSubChunk IBAGSubChunk { get; }
		public MODSubChunk IMODSubChunk { get; }
		public GENSubChunk IGENSubChunk { get; }
		public SHDRSubChunk SHDRSubChunk { get; }

		internal PdtaListChunk(SF2 inSf2) : base(inSf2, "pdta")
		{
			PHDRSubChunk = new PHDRSubChunk(inSf2);
			PBAGSubChunk = new BAGSubChunk(inSf2, true);
			PMODSubChunk = new MODSubChunk(inSf2, true);
			PGENSubChunk = new GENSubChunk(inSf2, true);
			INSTSubChunk = new INSTSubChunk(inSf2);
			IBAGSubChunk = new BAGSubChunk(inSf2, false);
			IMODSubChunk = new MODSubChunk(inSf2, false);
			IGENSubChunk = new GENSubChunk(inSf2, false);
			SHDRSubChunk = new SHDRSubChunk(inSf2);
			inSf2.UpdateSize();
		}
		internal PdtaListChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			PHDRSubChunk = new PHDRSubChunk(inSf2, reader);
			PBAGSubChunk = new BAGSubChunk(inSf2, reader);
			PMODSubChunk = new MODSubChunk(inSf2, reader);
			PGENSubChunk = new GENSubChunk(inSf2, reader);
			INSTSubChunk = new INSTSubChunk(inSf2, reader);
			IBAGSubChunk = new BAGSubChunk(inSf2, reader);
			IMODSubChunk = new MODSubChunk(inSf2, reader);
			IGENSubChunk = new GENSubChunk(inSf2, reader);
			SHDRSubChunk = new SHDRSubChunk(inSf2, reader);
		}

		internal override uint UpdateSize()
		{
			return Size = 4
				+ PHDRSubChunk.Size + 8
				+ PBAGSubChunk.Size + 8
				+ PMODSubChunk.Size + 8
				+ PGENSubChunk.Size + 8
				+ INSTSubChunk.Size + 8
				+ IBAGSubChunk.Size + 8
				+ IMODSubChunk.Size + 8
				+ IGENSubChunk.Size + 8
				+ SHDRSubChunk.Size + 8;
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			PHDRSubChunk.Write(writer);
			PBAGSubChunk.Write(writer);
			PMODSubChunk.Write(writer);
			PGENSubChunk.Write(writer);
			INSTSubChunk.Write(writer);
			IBAGSubChunk.Write(writer);
			IMODSubChunk.Write(writer);
			IGENSubChunk.Write(writer);
			SHDRSubChunk.Write(writer);
		}

		public override string ToString()
		{
			return $"Hydra List Chunk";
		}
	}
}
