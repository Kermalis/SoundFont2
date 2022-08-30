using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public class SF2Chunk
	{
		protected readonly SF2 _sf2;

		/// <summary>Length 4</summary>
		public string ChunkName { get; }
		/// <summary>Size in bytes</summary>
		public uint Size { get; protected set; }

		protected SF2Chunk(SF2 inSf2, string name)
		{
			_sf2 = inSf2;
			ChunkName = name;
		}
		protected SF2Chunk(SF2 inSf2, EndianBinaryReader reader)
		{
			_sf2 = inSf2;
			ChunkName = reader.ReadString_Count(4);
			Size = reader.ReadUInt32();
		}

		internal virtual void Write(EndianBinaryWriter writer)
		{
			writer.WriteChars_Count(ChunkName, 4);
			writer.WriteUInt32(Size);
		}
	}
}
