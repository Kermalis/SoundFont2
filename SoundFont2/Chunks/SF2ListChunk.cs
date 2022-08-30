using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public abstract class SF2ListChunk : SF2Chunk
	{
		///<summary>Length 4</summary>
		public string ListChunkName { get; }

		protected SF2ListChunk(SF2 inSf2, string name) : base(inSf2, "LIST")
		{
			ListChunkName = name;
			Size = 4;
		}
		protected SF2ListChunk(SF2 inSf2, EndianBinaryReader reader) : base(inSf2, reader)
		{
			ListChunkName = reader.ReadString_Count(4);
		}

		internal abstract uint UpdateSize();

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteChars_Count(ListChunkName, 4);
		}
	}
}
