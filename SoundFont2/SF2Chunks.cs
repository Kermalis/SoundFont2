using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.SoundFont2
{
    public class SF2Chunk
    {
        readonly SF2 sf2;

        readonly char[] chunkName; // Length 4
        public string ChunkName => new string(chunkName);
        public uint Size { get; protected set; } // Size in bytes

        protected SF2Chunk(SF2 inSf2, string name)
        {
            sf2 = inSf2;
            chunkName = name.ToCharArray();
        }
        protected SF2Chunk(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            chunkName = reader.ReadChars(4);
            Size = reader.ReadUInt32();
        }
        internal virtual void Write(BinaryWriter writer)
        {
            writer.Write(chunkName);
            writer.Write(Size);
        }
    }

    public abstract class SF2ListChunk : SF2Chunk
    {
        readonly char[] listChunkName; // Length 4
        public string ListChunkName => new string(listChunkName);

        protected SF2ListChunk(SF2 inSf2, string name) : base(inSf2, "LIST")
        {
            listChunkName = name.ToCharArray();
            Size = 4;
        }
        protected SF2ListChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            listChunkName = reader.ReadChars(4);
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(listChunkName);
        }

        public abstract uint CalculateSize();
    }

    public sealed class SF2PresetHeader
    {
        public const uint Size = 38;
        readonly SF2 sf2;

        char[] presetName; // Length 20
        public string PresetName
        {
            get => new string(presetName);
            set => SF2Utils.TruncateOrNot(value, 20, ref presetName);
        }
        public ushort Preset, Bank, PresetBagIndex;
        // Reserved for future implementations
        readonly uint library = 0, genre = 0, morphology = 0;

        internal SF2PresetHeader(SF2 inSf2)
        {
            sf2 = inSf2;
            presetName = new char[20];
        }
        internal SF2PresetHeader(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            presetName = reader.ReadChars(20);
            Preset = reader.ReadUInt16();
            Bank = reader.ReadUInt16();
            PresetBagIndex = reader.ReadUInt16();
            library = reader.ReadUInt32();
            genre = reader.ReadUInt32();
            morphology = reader.ReadUInt32();
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write(presetName);
            writer.Write(Preset);
            writer.Write(Bank);
            writer.Write(PresetBagIndex);
            writer.Write(library);
            writer.Write(genre);
            writer.Write(morphology);
        }

        public override string ToString() => $"Preset Header - Bank = {Bank}, " +
            $"\nPreset = {Preset}, " +
            $"\nName = \"{PresetName}\"";
    }

    // Covers sfPresetBag and sfInstBag
    public sealed class SF2Bag
    {
        public const uint Size = 4;
        readonly SF2 sf2;

        public ushort GeneratorIndex; // Index in list of generators
        public ushort ModulatorIndex; // Index in list of modulators

        internal SF2Bag(SF2 inSf2, bool preset)
        {
            sf2 = inSf2;
            if (preset)
            {
                GeneratorIndex = (ushort)sf2.PGENCount;
                ModulatorIndex = (ushort)sf2.PMODCount;
            }
            else
            {
                GeneratorIndex = (ushort)sf2.IGENCount;
                ModulatorIndex = (ushort)sf2.IMODCount;
            }
        }
        internal SF2Bag(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            GeneratorIndex = reader.ReadUInt16();
            ModulatorIndex = reader.ReadUInt16();
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write(GeneratorIndex);
            writer.Write(ModulatorIndex);
        }

        public override string ToString() => $"Bag - Generator index = {GeneratorIndex}, " +
            $"\nModulator index = {ModulatorIndex}";
    }

    // Covers sfModList and sfInstModList
    public sealed class SF2ModulatorList
    {
        public const uint Size = 10;
        readonly SF2 sf2;

        public SF2Modulator ModulatorSource;
        public SF2Generator ModulatorDestination;
        public short ModulatorAmount;
        public SF2Modulator ModulatorAmountSource;
        public SF2Transform ModulatorTransform;

        internal SF2ModulatorList(SF2 inSf2)
        {
            sf2 = inSf2;
        }
        internal SF2ModulatorList(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            ModulatorSource = (SF2Modulator)reader.ReadUInt16();
            ModulatorDestination = (SF2Generator)reader.ReadUInt16();
            ModulatorAmount = reader.ReadInt16();
            ModulatorAmountSource = (SF2Modulator)reader.ReadUInt16();
            ModulatorTransform = (SF2Transform)reader.ReadUInt16();
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write((ushort)ModulatorSource);
            writer.Write((ushort)ModulatorDestination);
            writer.Write(ModulatorAmount);
            writer.Write((ushort)ModulatorAmountSource);
            writer.Write((ushort)ModulatorTransform);
        }

        public override string ToString() => $"Modulator List - Modulator source = {ModulatorSource}, " +
            $"\nModulator destination = {ModulatorDestination}, " +
            $"\nModulator amount = {ModulatorAmount}, " +
            $"\nModulator amount source = {ModulatorAmountSource}, " +
            $"\nModulator transform = {ModulatorTransform}";
    }

    public sealed class SF2GeneratorList
    {
        public const uint Size = 4;
        readonly SF2 sf2;

        public SF2Generator Generator;
        public SF2GeneratorAmount GeneratorAmount;

        internal SF2GeneratorList(SF2 inSf2)
        {
            sf2 = inSf2;
        }
        internal SF2GeneratorList(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            Generator = (SF2Generator)reader.ReadUInt16();
            GeneratorAmount = new SF2GeneratorAmount { UAmount = reader.ReadUInt16() };
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write((ushort)Generator);
            writer.Write(GeneratorAmount.UAmount);
        }

        public override string ToString() => $"Generator List - Generator = {Generator}, " +
            $"\nGenerator amount = \"{GeneratorAmount}\"";
    }

    public sealed class SF2Instrument
    {
        public const uint Size = 22;
        readonly SF2 sf2;

        char[] instrumentName; // Length 20
        public string InstrumentName
        {
            get => new string(instrumentName);
            set => SF2Utils.TruncateOrNot(value, 20, ref instrumentName);
        }
        public ushort InstrumentBagIndex;

        internal SF2Instrument(SF2 inSf2)
        {
            sf2 = inSf2;
            instrumentName = new char[20];
        }
        internal SF2Instrument(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            instrumentName = reader.ReadChars(20);
            InstrumentBagIndex = reader.ReadUInt16();
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write(instrumentName);
            writer.Write(InstrumentBagIndex);
        }

        public override string ToString() => $"Instrument - Name = \"{InstrumentName}\"";
    }

    public sealed class SF2SampleHeader
    {
        public const uint Size = 46;
        readonly SF2 sf2;

        char[] sampleName; // Length 20
        public string SampleName
        {
            get => new string(sampleName);
            set => SF2Utils.TruncateOrNot(value, 20, ref sampleName);
        }
        public uint Start;
        public uint End;
        public uint LoopStart;
        public uint LoopEnd;
        public uint SampleRate;
        public byte OriginalKey;
        public sbyte PitchCorrection;
        public ushort SampleLink;
        public SF2SampleLink SampleType = SF2SampleLink.MonoSample;

        internal SF2SampleHeader(SF2 inSf2)
        {
            sf2 = inSf2;
            sampleName = new char[20];
        }
        internal SF2SampleHeader(SF2 inSf2, BinaryReader reader)
        {
            sf2 = inSf2;
            sampleName = reader.ReadChars(20);
            Start = reader.ReadUInt32();
            End = reader.ReadUInt32();
            LoopStart = reader.ReadUInt32();
            LoopEnd = reader.ReadUInt32();
            SampleRate = reader.ReadUInt32();
            OriginalKey = reader.ReadByte();
            PitchCorrection = reader.ReadSByte();
            SampleLink = reader.ReadUInt16();
            SampleType = (SF2SampleLink)reader.ReadUInt16();
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write(sampleName);
            writer.Write(Start);
            writer.Write(End);
            writer.Write(LoopStart);
            writer.Write(LoopEnd);
            writer.Write(SampleRate);
            writer.Write(OriginalKey);
            writer.Write(PitchCorrection);
            writer.Write(SampleLink);
            writer.Write((ushort)SampleType);
        }

        public override string ToString() => $"Sample - Name = \"{SampleName}\", " +
            $"\nType = {SampleType}";
    }

    #region Sub-Chunks

    public sealed class VersionSubChunk : SF2Chunk
    {
        // Output format is SoundFont v2.1
        public SF2VersionTag Version;

        internal VersionSubChunk(SF2 inSf2, string subChunkName) : base(inSf2, subChunkName)
        {
            Size += SF2VersionTag.Size;
        }
        internal VersionSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            Version = new SF2VersionTag
            {
                Major = reader.ReadUInt16(),
                Minor = reader.ReadUInt16()
            };
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Version.Major);
            writer.Write(Version.Minor);
        }

        public override string ToString() => $"Version Chunk - Revision = {Version}";
    }

    public sealed class HeaderSubChunk : SF2Chunk
    {
        readonly int maxSize;

        char[] field;
        public string Field
        {
            get => new string(field);
            set
            {
                var strAsList = value.ToCharArray().ToList();
                if (strAsList.Count >= maxSize) // Input too long; cut it down
                {
                    strAsList = strAsList.Take(maxSize).ToList();
                    strAsList[maxSize - 1] = '\0';
                }
                else if (strAsList.Count % 2 == 0) // Even amount of characters
                {
                    strAsList.Add('\0'); // Add two null-terminators to keep the byte count even
                    strAsList.Add('\0');
                }
                else // Odd amount of characters
                {
                    strAsList.Add('\0'); // Add one null-terminator since that would make byte the count even
                }
                field = strAsList.ToArray();
                Size = (uint)field.Length;
            }
        }

        internal HeaderSubChunk(SF2 inSf2, string subChunkName, int maxSize = 0x100) : base(inSf2, subChunkName)
        {
            this.maxSize = maxSize;
        }
        internal HeaderSubChunk(SF2 inSf2, BinaryReader reader, int maxSize = 0x100) : base(inSf2, reader)
        {
            this.maxSize = maxSize;
            field = reader.ReadChars((int)Size);
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(field);
        }

        public override string ToString() => $"Header Chunk - Name = \"{ChunkName}\", " +
            $"\nField Max Size = {maxSize}, " +
            $"\nField = \"{Field}\"";
    }

    public sealed class SMPLSubChunk : SF2Chunk
    {
        byte[] binary; // Binary of bytes

        internal SMPLSubChunk(SF2 inSf2) : base(inSf2, "smpl")
        {
            binary = new byte[0];
        }
        internal SMPLSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            binary = reader.ReadBytes((int)Size);
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(binary.ToArray());
        }

        // Returns index of the start of the sample
        internal uint AddSample(short[] pcm16, bool bLoop, uint loopPos)
        {
            uint len = (uint)pcm16.Length;
            uint start = Size / 2;
            // 2 bytes per sample
            // 8 samples after looping
            // 46 empty samples
            uint addedSize = bLoop ? (len + 8 + 46) * 2 : Size += (len + 46) * 2;
            Size += addedSize;

            byte[] newData = new byte[addedSize];

            using (var writer = new BinaryWriter(new MemoryStream(newData)))
            {
                // Write wave
                for (int i = 0; i < pcm16.Length; i++)
                    writer.Write(pcm16[i]);

                // If looping is enabled, write 8 samples from the loop point
                if (bLoop)
                    for (int i = 0; i < 8; i++)
                        writer.Write(pcm16[loopPos + i]);

                // Write 46 empty samples
                for (int i = 0; i < 46; i++)
                    writer.Write((short)0);
            }

            binary = binary.Concat(newData).ToArray();

            return start;
        }

        public override string ToString() => $"Sample Data Chunk";
    }

    public sealed class PHDRSubChunk : SF2Chunk
    {
        readonly List<SF2PresetHeader> presets = new List<SF2PresetHeader>();
        public int Count => presets.Count;

        internal PHDRSubChunk(SF2 inSf2) : base(inSf2, "phdr") { }
        internal PHDRSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2PresetHeader.Size;
            for (int i = 0; i < amt; i++)
                presets.Add(new SF2PresetHeader(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < presets.Count; i++)
                presets[i].Write(writer);
        }

        internal void AddPreset(SF2PresetHeader preset)
        {
            presets.Add(preset);
            Size += SF2PresetHeader.Size;
        }

        public override string ToString() => $"Preset Header Chunk - Preset count = {Count}";
    }

    public sealed class INSTSubChunk : SF2Chunk
    {
        readonly List<SF2Instrument> instruments = new List<SF2Instrument>();
        public int Count => instruments.Count;

        internal INSTSubChunk(SF2 inSf2) : base(inSf2, "inst") { }
        internal INSTSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2Instrument.Size;
            for (int i = 0; i < amt; i++)
                instruments.Add(new SF2Instrument(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < instruments.Count; i++)
                instruments[i].Write(writer);
        }

        internal void AddInstrument(SF2Instrument instrument)
        {
            instruments.Add(instrument);
            Size += SF2Instrument.Size;
        }

        public override string ToString() => $"Instrument Chunk - Instrument count = {Count}";
    }

    public sealed class BAGSubChunk : SF2Chunk
    {
        readonly List<SF2Bag> bags = new List<SF2Bag>();
        public int Count => bags.Count;

        internal BAGSubChunk(SF2 inSf2, bool preset) : base(inSf2, preset ? "pbag" : "ibag") { }
        internal BAGSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2Bag.Size;
            for (int i = 0; i < amt; i++)
                bags.Add(new SF2Bag(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < bags.Count; i++)
                bags[i].Write(writer);
        }

        internal void AddBag(SF2Bag bag)
        {
            bags.Add(bag);
            Size += SF2Bag.Size;
        }

        public override string ToString() => $"Bag Chunk - Name = \"{ChunkName}\", " +
            $"\nBag count = {Count}";
    }

    public sealed class MODSubChunk : SF2Chunk
    {
        readonly List<SF2ModulatorList> modulators = new List<SF2ModulatorList>();
        public int Count => modulators.Count;

        internal MODSubChunk(SF2 inSf2, bool preset) : base(inSf2, preset ? "pmod" : "imod") { }
        internal MODSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2ModulatorList.Size;
            for (int i = 0; i < amt; i++)
                modulators.Add(new SF2ModulatorList(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < modulators.Count; i++)
                modulators[i].Write(writer);
        }

        internal void AddModulator(SF2ModulatorList modulator)
        {
            modulators.Add(modulator);
            Size += SF2ModulatorList.Size;
        }

        public override string ToString() => $"Modulator Chunk - Name = \"{ChunkName}\", " +
            $"\nModulator count = {Count}";
    }

    public sealed class GENSubChunk : SF2Chunk
    {
        readonly List<SF2GeneratorList> generators = new List<SF2GeneratorList>();
        public int Count => generators.Count;

        internal GENSubChunk(SF2 inSf2, bool preset) : base(inSf2, preset ? "pgen" : "igen") { }
        internal GENSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2GeneratorList.Size;
            for (int i = 0; i < amt; i++)
                generators.Add(new SF2GeneratorList(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < generators.Count; i++)
                generators[i].Write(writer);
        }

        internal void AddGenerator(SF2GeneratorList generator)
        {
            generators.Add(generator);
            Size += SF2GeneratorList.Size;
        }

        public override string ToString() => $"Generator Chunk - Name = \"{ChunkName}\", " +
            $"\nGenerator count = {Count}";
    }

    public sealed class SHDRSubChunk : SF2Chunk
    {
        readonly List<SF2SampleHeader> samples = new List<SF2SampleHeader>();
        public int Count => samples.Count;

        internal SHDRSubChunk(SF2 inSf2) : base(inSf2, "shdr") { }
        internal SHDRSubChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            uint amt = Size / SF2SampleHeader.Size;
            for (int i = 0; i < amt; i++)
                samples.Add(new SF2SampleHeader(inSf2, reader));
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            for (int i = 0; i < samples.Count; i++)
                samples[i].Write(writer);
        }

        internal void AddSample(SF2SampleHeader sample)
        {
            samples.Add(sample);
            Size += SF2SampleHeader.Size;
        }

        public override string ToString() => $"Sample Header Chunk - Sample header count = {Count}";
    }

    #endregion

    #region Main Chunks

    public sealed class InfoListChunk : SF2ListChunk
    {
        readonly List<SF2Chunk> subChunks = new List<SF2Chunk>();

        /*public InfoListChunk(SF2 inSf2, string engine, string bank, string rom, SF2VersionTag rom_revision, string date, string designer, string products, string copyright, string comment, string tools)
            : base(inSf2, "INFO")
        {

            // Optional sub-chunks
            if (!string.IsNullOrEmpty(rom))
                subChunks.Add(new HeaderSubChunk(inSf2, "irom", rom));
            if (rom_revision != null)
                subChunks.Add(new VersionSubChunk(inSf2, "iver", rom_revision));
            if (!string.IsNullOrEmpty(date))
                subChunks.Add(new HeaderSubChunk(inSf2, "ICRD", date));
            if (!string.IsNullOrEmpty(designer))
                subChunks.Add(new HeaderSubChunk(inSf2, "IENG", designer));
            if (!string.IsNullOrEmpty(products))
                subChunks.Add(new HeaderSubChunk(inSf2, "IPRD", products));
            if (!string.IsNullOrEmpty(copyright))
                subChunks.Add(new HeaderSubChunk(inSf2, "ICOP", copyright));
            if (!string.IsNullOrEmpty(comment))
                subChunks.Add(new HeaderSubChunk(inSf2, "ICMT", comment, 0x10000));
            if (!string.IsNullOrEmpty(tools))
                subChunks.Add(new HeaderSubChunk(inSf2, "ISFT", tools));
        }*/
        internal InfoListChunk(SF2 inSf2, string engine = "", string bank = "") : base(inSf2, "INFO")
        {
            // Mandatory sub-chunks
            subChunks.Add(new VersionSubChunk(inSf2, "ifil") { Version = new SF2VersionTag { Major = 2, Minor = 1 } });
            subChunks.Add(new HeaderSubChunk(inSf2, "isng") { Field = string.IsNullOrEmpty(engine) ? "EMU8000" : engine });
            subChunks.Add(new HeaderSubChunk(inSf2, "INAM") { Field = string.IsNullOrEmpty(bank) ? "General MIDI" : bank });

            CalculateSize();
        }
        internal InfoListChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            var startOffset = reader.BaseStream.Position;
            while (reader.BaseStream.Position < startOffset + Size - 4) // The 4 represents the INFO that was already read
            {
                char[] name = reader.ReadChars(4);
                reader.BaseStream.Position -= 4;
                string strName = new string(name);
                switch (strName)
                {
                    case "ICMT": subChunks.Add(new HeaderSubChunk(inSf2, reader, 0x10000)); break;
                    case "ifil":
                    case "iver": subChunks.Add(new VersionSubChunk(inSf2, reader)); break;
                    case "isng":
                    case "INAM":
                    case "ICRD":
                    case "IENG":
                    case "IPRD":
                    case "ICOP":
                    case "ISFT":
                    case "irom": subChunks.Add(new HeaderSubChunk(inSf2, reader)); break;
                    default:
                        throw new NotSupportedException($"Unsupported chunk name at 0x{reader.BaseStream.Position:X}: \"{strName}\"");
                }
            }
            CalculateSize();
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            foreach (var sub in subChunks)
                sub.Write(writer);
        }

        public override uint CalculateSize()
        {
            Size = 4;
            foreach (var sub in subChunks)
                Size += sub.Size + 8;
            return Size;
        }

        public override string ToString() => $"Info List Chunk - Sub-chunk count = {subChunks.Count}";
    }

    public sealed class SdtaListChunk : SF2ListChunk
    {
        public readonly SMPLSubChunk SMPLSubChunk;

        internal SdtaListChunk(SF2 inSf2) : base(inSf2, "sdta")
        {
            SMPLSubChunk = new SMPLSubChunk(inSf2);
            CalculateSize();
        }
        internal SdtaListChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            SMPLSubChunk = new SMPLSubChunk(inSf2, reader);
            CalculateSize();
        }
        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            SMPLSubChunk.Write(writer);
        }

        public override uint CalculateSize()
        {
            Size = 4;
            Size += SMPLSubChunk.Size + 8;
            return Size;
        }

        public override string ToString() => $"Sample Data List Chunk";
    }

    public sealed class PdtaListChunk : SF2ListChunk
    {
        public readonly PHDRSubChunk PHDRSubChunk;
        public readonly BAGSubChunk PBAGSubChunk;
        public readonly MODSubChunk PMODSubChunk;
        public readonly GENSubChunk PGENSubChunk;
        public readonly INSTSubChunk INSTSubChunk;
        public readonly BAGSubChunk IBAGSubChunk;
        public readonly MODSubChunk IMODSubChunk;
        public readonly GENSubChunk IGENSubChunk;
        public readonly SHDRSubChunk SHDRSubChunk;

        internal PdtaListChunk(SF2 inSf2) : base(inSf2, "pdta")
        {
            PHDRSubChunk = new PHDRSubChunk(inSf2);
            PBAGSubChunk = new BAGSubChunk(inSf2, true);
            PMODSubChunk = new MODSubChunk(inSf2, true);
            PGENSubChunk = new GENSubChunk(inSf2, true);
            INSTSubChunk = new INSTSubChunk(inSf2);
            IBAGSubChunk = new BAGSubChunk(inSf2, false);
            IMODSubChunk = new MODSubChunk(inSf2, false);
            IGENSubChunk = new GENSubChunk(inSf2, false);
            SHDRSubChunk = new SHDRSubChunk(inSf2);
            CalculateSize();
        }
        internal PdtaListChunk(SF2 inSf2, BinaryReader reader) : base(inSf2, reader)
        {
            PHDRSubChunk = new PHDRSubChunk(inSf2, reader);
            PBAGSubChunk = new BAGSubChunk(inSf2, reader);
            PMODSubChunk = new MODSubChunk(inSf2, reader);
            PGENSubChunk = new GENSubChunk(inSf2, reader);
            INSTSubChunk = new INSTSubChunk(inSf2, reader);
            IBAGSubChunk = new BAGSubChunk(inSf2, reader);
            IMODSubChunk = new MODSubChunk(inSf2, reader);
            IGENSubChunk = new GENSubChunk(inSf2, reader);
            SHDRSubChunk = new SHDRSubChunk(inSf2, reader);
            CalculateSize();
        }

        public override uint CalculateSize()
        {
            Size = 4;
            Size += PHDRSubChunk.Size + 8;
            Size += PBAGSubChunk.Size + 8;
            Size += PMODSubChunk.Size + 8;
            Size += PGENSubChunk.Size + 8;
            Size += INSTSubChunk.Size + 8;
            Size += IBAGSubChunk.Size + 8;
            Size += IMODSubChunk.Size + 8;
            Size += IGENSubChunk.Size + 8;
            Size += SHDRSubChunk.Size + 8;
            return Size;
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            PHDRSubChunk.Write(writer);
            PBAGSubChunk.Write(writer);
            PMODSubChunk.Write(writer);
            PGENSubChunk.Write(writer);
            INSTSubChunk.Write(writer);
            IBAGSubChunk.Write(writer);
            IMODSubChunk.Write(writer);
            IGENSubChunk.Write(writer);
            SHDRSubChunk.Write(writer);
        }

        public override string ToString() => $"Hydra List Chunk";
    }

    #endregion
}
