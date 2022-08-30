using Kermalis.EndianBinaryIO;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.SoundFont2
{
	public sealed class HeaderSubChunk : SF2Chunk
	{
		public int MaxSize { get; }
		private int _fieldTargetLength;
		private string _field;
		/// <summary>Length <see cref="MaxSize"/></summary>
		public string Field
		{
			get => _field;
			[MemberNotNull(nameof(_field))]
			set
			{
				if (value.Length >= MaxSize) // Input too long; cut it down
				{
					_fieldTargetLength = MaxSize;
				}
				else if (value.Length % 2 == 0) // Even amount of characters
				{
					_fieldTargetLength = value.Length + 2; // Add two null-terminators to keep the byte count even
				}
				else // Odd amount of characters
				{
					_fieldTargetLength = value.Length + 1; // Add one null-terminator since that would make the byte count even
				}
				_field = value;
				Size = (uint)_fieldTargetLength;
				_sf2.UpdateSize();
			}
		}

		internal HeaderSubChunk(SF2 inSf2, string subChunkName, string field, int maxSize = 0x100) : base(inSf2, subChunkName)
		{
			MaxSize = maxSize;
			Field = field;
		}
		internal HeaderSubChunk(SF2 inSf2, EndianBinaryReader reader, int maxSize = 0x100) : base(inSf2, reader)
		{
			MaxSize = maxSize;
			Field = reader.ReadString_Count_TrimNullTerminators((int)Size);
		}

		internal override void Write(EndianBinaryWriter writer)
		{
			base.Write(writer);
			writer.WriteChars_Count(_field, _fieldTargetLength);
		}

		public override string ToString()
		{
			return $"Header Chunk - Name = \"{ChunkName}\"" +
				$",\nField Max Size = {MaxSize}" +
				$",\nField = \"{Field}\"";
		}
	}
}
