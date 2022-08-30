using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class VersionSubChunk : SF2Chunk
	{
		public SF2VersionTag Version { get; set; }

		internal VersionSubChunk(SF2 inSf2, string subChunkName, SF2VersionTag version) : base(inSf2, subChunkName)
		{
			Size = SF2VersionTag.SIZE;
			inSf2.UpdateSize();
			Version = version;
		}
		internal VersionSubChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			Version = new SF2VersionTag(reader);
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			Version.Write(writer);
		}

		public override string ToString()
		{
			return $"Version Chunk - Revision = {Version}";
		}
	}
}
