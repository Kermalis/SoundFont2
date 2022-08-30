using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	/// <summary>SF2 v2.1 spec page 16</summary>
	public sealed class SF2VersionTag
	{
		public const uint SIZE = 4;

		public ushort Major { get; }
		public ushort Minor { get; }

		public SF2VersionTag(ushort major, ushort minor)
		{
			Major = major;
			Minor = minor;
		}
		internal SF2VersionTag(EndianBinaryReader reader)
		{
			Major = reader.ReadUInt16();
			Minor = reader.ReadUInt16();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteUInt16(Major);
			writer.WriteUInt16(Minor);
		}

		public override string ToString()
		{
			return $"v{Major}.{Minor}";
		}
	}
}
