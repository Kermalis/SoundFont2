using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	public sealed class SF2GeneratorList
	{
		public const uint SIZE = 4;

		public SF2Generator Generator { get; set; }
		public SF2GeneratorAmount GeneratorAmount { get; set; }

		internal SF2GeneratorList() { }
		internal SF2GeneratorList(SF2Generator generator, SF2GeneratorAmount amount)
		{
			Generator = generator;
			GeneratorAmount = amount;
		}
		internal SF2GeneratorList(EndianBinaryReader reader)
		{
			Generator = reader.ReadEnum<SF2Generator>();
			GeneratorAmount = new SF2GeneratorAmount { Amount = reader.ReadInt16() };
		}

		public void Write(EndianBinaryWriter writer)
		{
			writer.WriteEnum(Generator);
			writer.WriteInt16(GeneratorAmount.Amount);
		}

		public override string ToString()
		{
			return $"Generator List - Generator = {Generator}" +
				$",\nGenerator amount = \"{GeneratorAmount}\"";
		}
	}
}
