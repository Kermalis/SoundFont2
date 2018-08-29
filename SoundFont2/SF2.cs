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

        internal uint IBAGCount => hydraChunk.IBAGSubChunk.Count;
        internal uint IGENCount => hydraChunk.IGENSubChunk.Count;
        internal uint IMODCount => hydraChunk.IMODSubChunk.Count;
        internal uint PBAGCount => hydraChunk.PBAGSubChunk.Count;
        internal uint PGENCount => hydraChunk.PGENSubChunk.Count;
        internal uint PMODCount => hydraChunk.PMODSubChunk.Count;

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

                size = reader.ReadUInt32();
                chars = reader.ReadChars(4);
                if (new string(chars) != "sfbk")
                    throw new InvalidDataException("sfbk header was not found at the expected offset.");

                infoChunk = new InfoListChunk(this, reader);
                soundChunk = new SdtaListChunk(this, reader);
                hydraChunk = new PdtaListChunk(this, reader);
            }
        }

        public void Save(string path)
        {
            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create), Encoding.ASCII))
            {
                AddTerminals();

                writer.Write("RIFF".ToCharArray());
                writer.Write(size);
                writer.Write("sfbk".ToCharArray());

                infoChunk.Write(writer);
                soundChunk.Write(writer);
                hydraChunk.Write(writer);
            }
        }


        // Returns sample index
        public uint AddSample(short[] pcm16, string name, bool bLoop, uint loopPos, uint sampleRate, byte originalKey, sbyte pitchCorrection)
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

            return AddSampleHeader(name, start, end, loopStart, loopEnd, sampleRate, originalKey, pitchCorrection);
        }
        public void AddInstrument(string name)
        {
            hydraChunk.INSTSubChunk.AddInstrument(new SF2Instrument(this)
            {
                InstrumentName = name,
                InstrumentBagIndex = (ushort)IBAGCount
            });
        }
        public void AddInstrumentBag()
        {
            hydraChunk.IBAGSubChunk.AddBag(new SF2Bag(this, false));
        }
        public void AddInstrumentModulator()
        {
            hydraChunk.IMODSubChunk.AddModulator(new SF2ModulatorList(this));
        }
        public void AddInstrumentGenerator()
        {
            hydraChunk.IGENSubChunk.AddGenerator(new SF2GeneratorList(this));
        }
        public void AddInstrumentGenerator(SF2Generator operation, SF2GeneratorAmount genAmountType)
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

        uint AddSampleHeader(string name, uint start, uint end, uint loopStart, uint loopEnd, uint sampleRate, byte originalKey, sbyte pitchCorrection)
        {
            return hydraChunk.SHDRSubChunk.AddSample(new SF2SampleHeader(this)
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
        void AddTerminals()
        {
            AddSampleHeader("EOS", 0, 0, 0, 0, 0, 0, 0);
            AddInstrument("EOI");
            AddInstrumentBag();
            AddInstrumentGenerator();
            AddInstrumentModulator();
            AddPreset("EOP", 0xFF, 0xFF);
            AddPresetBag();
            AddPresetGenerator();
            AddPresetModulator();
        }

        internal void UpdateSize()
        {
            if (infoChunk == null || soundChunk == null || hydraChunk == null)
                return;
            size = 4
                + infoChunk.UpdateSize() + 8
                + soundChunk.UpdateSize() + 8
                + hydraChunk.UpdateSize() + 8;
        }
    }
}
