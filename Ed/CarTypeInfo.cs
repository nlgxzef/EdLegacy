namespace Ed
{
    public partial class EdTypes
    {
        public class CarTypeInfo
        {
            /* 0x0000 */ public byte[] CarTypeName = new byte[16];
            /* 0x0010 */ public byte[] BaseModelName = new byte[16];
            /* 0x0020 */ public byte[] GeometryFilename = new byte[32];
            /* 0x0040 */ public byte[] ManufacturerName = new byte[16];
            /* 0x0050 */ public uint CarTypeNameHash;
            /* 0x0054 */ public float HeadlightFOV;
            /* 0x0058 */ public byte padHighPerformance;
            /* 0x0059 */ public byte NumAvailableSkinNumbers;
            /* 0x005a */ public byte WhatGame;
            /* 0x005b */ public byte ConvertableFlag;
            /* 0x005c */ public byte WheelOuterRadius;
            /* 0x005d */ public byte WheelInnerRadiusMin;
            /* 0x005e */ public byte WheelInnerRadiusMax;
            /* 0x005f */ public byte pad0;
            /* 0x0060 */ public bVector3 HeadlightPosition = new bVector3();
            /* 0x0070 */ public bVector3 DriverRenderingOffset = new bVector3();
            /* 0x0080 */ public bVector3 InCarSteeringWheelRenderingOffset = new bVector3();
            /* 0x0090 */ public int Type;
            /* 0x0094 */ public int UsageType; // 0 = Racer, 1 = Cop, 2 = Traffic, 3 = Wheels, 4 = Universal
            /* 0x0098 */ public uint CarMemTypeHash; // Player = EC0C7C75, Racing = 73266079, Cop = 619F0000, Traffic = 7E29DB66, BigTraffic = 90B300AE, HugeTraffic = 87631B91
            /* 0x009c */ public byte[] MaxInstances = new byte[5];
            /* 0x00a1 */ public byte[] WantToKeepLoaded = new byte[5];
            /* 0x00a6 */ public byte[] pad4 = new byte[2];
            /* 0x00a8 */ public float[] MinTimeBetweenUses = new float[5];
            /* 0x00bc */ public byte[] AvailableSkinNumbers = new byte[10];
            /* 0x00c6 */ public byte DefaultSkinNumber;
            /* 0x00c7 */ public byte Skinnable;
            /* 0x00c8 */ public int Padding;
            /* 0x00cc */ public uint DefaultBasePaint; // GLOSS_L1_COLOR01 (01-80) / METAL_L1_COLOR01 (01-80) / PEARL1_PAINT (1-20) / CHROME01_PAINT (01-10) / MATTE01_PAINT (01-10) / TRAFFIC_L1_COLOR01 / COP_L1_COLOR01
        }; /* size: 0x00d0 */
    }
}
