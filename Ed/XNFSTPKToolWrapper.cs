using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ed
{
    public class XNFSTPKToolWrapper
    {
        public class TPKSection
        {
            public string TypeName;
            public int TypeVal; // Dec
            public string Path;
            public uint Hash; // Hex
            public int Animations; // Dec
        }

        public class AnimationSection
        {
            public string Name;
            public uint Hash; // Hex
            public byte Frames; // Dec
            public byte Framerate; // Dec
            public uint Unknown1; // Hex
            public uint Unknown2; // Hex
            public ushort Unknown3; // Hex
            public uint Unknown4; // Hex
            public uint Unknown5; // Hex
            public uint Unknown6; // Hex
            public List<uint> FrameList = new List<uint>();
        }

        public class TextureSectionTPKv3 // Carbon & ProStreet
        {
            public uint Hash; // [(HashValue)]
            public string File;
            public string Name;
            public uint Hash2; // hex
            public byte UnkByte1; // hex
            public byte UnkByte2; // hex
            public byte UnkByte3; // hex
            public uint Unknown1; // hex
            public ushort Unknown3; // hex 
            public uint Unknown4;  // hex
            public uint Unknown5;  // hex
            public uint Unknown6;  // hex
            public uint Unknown7;  // hex
            public uint Unknown8;  // hex
            public uint Unknown9;  // hex
            public uint Unknown10; // hex
            public uint Unknown11; // hex
            public uint Unknown12; // hex
            public byte Unknown17; // hex
            public byte Unknown18; // hex
        }
    }
}
