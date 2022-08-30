using Kermalis.EndianBinaryIO;

namespace Kermalis.SoundFont2
{
	/// <summary>Covers sfModList and sfInstModList</summary>
	public sealed class SF2ModulatorList
	{
		public const uint SIZE = 10;

		public SF2Modulator ModulatorSource { get; set; }
		public SF2Generator ModulatorDestination { get; set; }
		public short ModulatorAmount { get; set; }
		public SF2Modulator ModulatorAmountSource { get; set; }
		public SF2Transform ModulatorTransform { get; set; }

		internal SF2ModulatorList() { }
		internal SF2ModulatorList(EndianBinaryReader reader)
		{
			ModulatorSource = reader.ReadEnum<SF2Modulator>();
			ModulatorDestination = reader.ReadEnum<SF2Generator>();
			ModulatorAmount = reader.ReadInt16();
			ModulatorAmountSource = reader.ReadEnum<SF2Modulator>();
			ModulatorTransform = reader.ReadEnum<SF2Transform>();
		}

		internal void Write(EndianBinaryWriter writer)
		{
			writer.WriteEnum(ModulatorSource);
			writer.WriteEnum(ModulatorDestination);
			writer.WriteInt16(ModulatorAmount);
			writer.WriteEnum(ModulatorAmountSource);
			writer.WriteEnum(ModulatorTransform);
		}

		public override string ToString()
		{
			return $"Modulator List - Modulator source = {ModulatorSource}" +
				$",\nModulator destination = {ModulatorDestination}" +
				$",\nModulator amount = {ModulatorAmount}" +
				$",\nModulator amount source = {ModulatorAmountSource}" +
				$",\nModulator transform = {ModulatorTransform}";
		}
	}
}
