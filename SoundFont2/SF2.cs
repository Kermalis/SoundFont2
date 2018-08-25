using System.IO;
using System.Text;

namespace Kermalis.SoundFont2
{
    public sealed class SF2
    {
        uint size;
        InfoListChunk infoChunk;
        SdtaListChunk soundChunk;
        PdtaListChunk hydraChunk;

        internal int IBAGCount => hydraChunk.IBAGSubChunk.Count;
        internal int IGENCount => hydraChunk.IGENSubChunk.Count;
        internal int IMODCount => hydraChunk.IMODSubChunk.Count;
        internal int PBAGCount => hydraChunk.PBAGSubChunk.Count;
        internal int PGENCount => hydraChunk.PGENSubChunk.Count;
        internal int PMODCount => hydraChunk.PMODSubChunk.Count;

        // For creating
        public SF2(string engine = "", string bank = "")
        {
            infoChunk = new InfoListChunk(this, engine, bank);
            soundChunk = new SdtaListChunk(this);
            hydraChunk = new PdtaListChunk(this);
        }

        // For reading
        public SF2(string path)
        {
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open), Encoding.ASCII))
            {
                char[] chars = reader.ReadChars(4);
                if (new string(chars) != "RIFF")
                    throw new InvalidDataException("RIFF header was not found at the start of the file.");

                uint size = reader.ReadUInt32();
                chars = reader.ReadChars(4);
                if (new string(chars) != "sfbk")
                    throw new InvalidDataException("sfbk header was not found at the expected offset.");

                infoChunk = new InfoListChunk(this, reader);
                soundChunk = new SdtaListChunk(this, reader);
                hydraChunk = new PdtaListChunk(this, reader);
            }
            ;
        }

        public void Save(string path)
        {
            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.ASCII))
            {
                AddTerminals();

                size = 4;
                size += infoChunk.CalculateSize() + 8;
                size += soundChunk.CalculateSize() + 8;
                size += hydraChunk.CalculateSize() + 8;

                writer.Write("RIFF".ToCharArray());
                writer.Write(size);
                writer.Write("sfbk".ToCharArray());

                infoChunk.Write(writer);
                soundChunk.Write(writer);
                hydraChunk.Write(writer);
            }
        }


        public void AddSample(short[] pcm16, string name, bool bLoop, uint loopPos, uint sampleRate, byte originalKey, sbyte pitchCorrection)
        {
            uint start = soundChunk.SMPLSubChunk.AddSample(pcm16, bLoop, loopPos);
            // If the sample is looped the standard requires us to add the 8 bytes from the start of the loop to the end
            uint end, loopEnd, loopStart;

            uint len = (uint)pcm16.Length;
            if (bLoop)
            {
                end = start + len + 8;
                loopStart = start + loopPos; loopEnd = start + len;                
            }
            else
            {
                end = start + len;
                loopStart = 0; loopEnd = 0;                
            }

            AddSampleHeader(name, start, end, loopStart, loopEnd, sampleRate, originalKey, pitchCorrection);
        }
        public void AddInstrument(string name)
        {
            hydraChunk.INSTSubChunk.AddInstrument(new SF2Instrument(this)
            {
                InstrumentName = name,
                InstrumentBagIndex = (ushort)IBAGCount
            });
        }
        public void AddINSTBag()
        {
            hydraChunk.IBAGSubChunk.AddBag(new SF2Bag(this, false));
        }
        public void AddINSTModulator()
        {
            hydraChunk.IMODSubChunk.AddModulator(new SF2ModulatorList(this));
        }
        public void AddINSTGenerator()
        {
            hydraChunk.IGENSubChunk.AddGenerator(new SF2GeneratorList(this));
        }
        public void AddINSTGenerator(SF2Generator operation, SF2GeneratorAmount genAmountType)
        {
            hydraChunk.IGENSubChunk.AddGenerator(new SF2GeneratorList(this)
            {
                Generator = operation,
                GeneratorAmount = genAmountType
            });
        }
        public void AddPreset(string name, ushort preset, ushort bank)
        {
            hydraChunk.PHDRSubChunk.AddPreset(new SF2PresetHeader(this)
            {
                PresetName = name,
                Preset = preset,
                Bank = bank,
                PresetBagIndex = (ushort)PBAGCount
            });
        }
        public void AddPresetBag()
        {
            hydraChunk.PBAGSubChunk.AddBag(new SF2Bag(this, true));
        }
        public void AddPresetModulator()
        {
            hydraChunk.PMODSubChunk.AddModulator(new SF2ModulatorList(this));
        }
        public void AddPresetGenerator()
        {
            hydraChunk.PGENSubChunk.AddGenerator(new SF2GeneratorList(this));
        }
        public void AddPresetGenerator(SF2Generator operation, SF2GeneratorAmount genAmountType)
        {
            hydraChunk.PGENSubChunk.AddGenerator(new SF2GeneratorList(this)
            {
                Generator = operation,
                GeneratorAmount = genAmountType
            });
        }

        void AddSampleHeader(string name, uint start, uint end, uint loopStart, uint loopEnd, uint sampleRate, byte originalKey, sbyte pitchCorrection)
        {
            hydraChunk.SHDRSubChunk.AddSample(new SF2SampleHeader(this)
            {
                SampleName = name,
                Start = start,
                End = end,
                LoopStart = loopStart,
                LoopEnd = loopEnd,
                SampleRate = sampleRate,
                OriginalKey = originalKey,
                PitchCorrection = pitchCorrection
            });
        }
        // Required by the standard
        void AddTerminals()
        {
            AddSampleHeader("EOS", 0, 0, 0, 0, 0, 0, 0);
            AddInstrument("EOI");
            AddINSTBag();
            AddINSTGenerator();
            AddINSTModulator();
            AddPreset("EOP", 0xFF, 0xFF);
            AddPresetBag();
            AddPresetGenerator();
            AddPresetModulator();
        }
    }
}
