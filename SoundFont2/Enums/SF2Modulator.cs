namespace Kermalis.SoundFont2
{
	/// <summary>SF2 v2.1 spec page 50</summary>
	public enum SF2Modulator : ushort
	{
		None = 0,
		NoteOnVelocity = 1,
		NoteOnKey = 2,
		PolyPressure = 10,
		ChnPressure = 13,
		PitchWheel = 14,
		PitchWheelSensivity = 16,
	}
}
