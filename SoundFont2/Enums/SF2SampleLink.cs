namespace Kermalis.SoundFont2
{
	/// <summary>SF2 v2.1 spec page 20</summary>
	public enum SF2SampleLink : ushort
	{
		MonoSample = 1,
		RightSample = 2,
		LeftSample = 4,
		LinkedSample = 8,
		RomMonoSample = 0x8001,
		RomRightSample = 0x8002,
		RomLeftSample = 0x8004,
		RomLinkedSample = 0x8008,
	}
}
