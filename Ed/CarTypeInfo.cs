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
            
            /* for nfsu and nfsu2*/
            public string CopyFrom;
            public byte[] GeometryLZCFilename = new byte[32];

            public bVector3 Unknown1 = new bVector3();
            /* 0x0120 */ public float WheelFrontLeftX; // 1,36584
            /* 0x0124 */ public float WheelFrontLeftSprings; // 0,78
            /* 0x0128 */ public float WheelFrontLeftRideHeight; // 0,09754    
            /* 0x012C */ public float WheelFrontLeftUnk3; // 0,0
            /* 0x0130 */ public float WheelFrontLeftDiameter; // 0,29775
            /* 0x0134 */ public float WheelFrontLeftTireSkidWidth; // 0,195
            /* 0x0138 */ public int WheelFrontLeftID; // 0x00
            /* 0x013C */ public float WheelFrontLeftY; // 0,80742
            /* 0x0140 */ public float WheelFrontLeftWideBodyY; // 0,90202
            /* 0x0144 */ public float WheelFrontLeftUnk5; // 0,0
            /* 0x0148 */ public float WheelFrontLeftUnk6; // 0,0
            /* 0x014C */ public float WheelFrontLeftUnk7; // 0,0
            /* 0x0150 */ public float WheelFrontRightX; // 1,36584
            /* 0x0154 */ public float WheelFrontRightSprings; // -0,78
            /* 0x0158 */ public float WheelFrontRightRideHeight; // 0,09754
            /* 0x015C */ public float WheelFrontRightUnk3; // 0,0
            /* 0x0160 */ public float WheelFrontRightDiameter; // 0,29775
            /* 0x0164 */ public float WheelFrontRightTireSkidWidth; // 0,195
            /* 0x0168 */ public int WheelFrontRightID; // 0x01
            /* 0x016C */ public float WheelFrontRightY; // -0,80742
            /* 0x0170 */ public float WheelFrontRightWideBodyY; // -0,90202
            /* 0x0174 */ public float WheelFrontRightUnk5; // 0,0
            /* 0x0178 */ public float WheelFrontRightUnk6; // 0,0
            /* 0x017C */ public float WheelFrontRightUnk7; // 0,0
            /* 0x0180 */ public float WheelRearRightX; // -1,26832
            /* 0x0184 */ public float WheelRearRightSprings; // -0,78
            /* 0x0188 */ public float WheelRearRightRideHeight; // 0,09754
            /* 0x018C */ public float WheelRearRightUnk3; // 0,0
            /* 0x0190 */ public float WheelRearRightDiameter; // 0,29775
            /* 0x0194 */ public float WheelRearRightTireSkidWidth; // 0,195
            /* 0x0198 */ public int WheelRearRightID; // 0x02
            /* 0x019C */ public float WheelRearRightY; // -0,83783
            /* 0x01A0 */ public float WheelRearRightWideBodyY; // -0,90877
            /* 0x01A4 */ public float WheelRearRightUnk5; // 0,0
            /* 0x01A8 */ public float WheelRearRightUnk6; // 0,0
            /* 0x01AC */ public float WheelRearRightUnk7; // 0,0
            /* 0x01B0 */ public float WheelRearLeftX; // -1,26832
            /* 0x01B4 */ public float WheelRearLeftSprings; // 0,78
            /* 0x01B8 */ public float WheelRearLeftRideHeight; // 0,09754
            /* 0x01BC */ public float WheelRearLeftUnk3; // 0,0
            /* 0x01C0 */ public float WheelRearLeftDiameter; // 0,29775
            /* 0x01C4 */ public float WheelRearLeftTireSkidWidth; // 0,195
            /* 0x01C8 */ public int WheelRearLeftID; // 0x03
            /* 0x01CC */ public float WheelRearLeftY; // 0,83783
            /* 0x01D0 */ public float WheelRearLeftWideBodyY; // 0,90877
            /* 0x01D4 */ public float WheelRearLeftUnk5; // 0,0
            /* 0x01D8 */ public float WheelRearLeftUnk6; // 0,0
            /* 0x01DC */ public float WheelRearLeftUnk7; // 0,0

            /* 0x0280 */ public float SuspensionUnk1F;   // 5,40
            /* 0x0284 */ public float FrontSprings;      // 38
            /* 0x0288 */ public float FrontShocks1;      // 5,20
            /* 0x028C */ public float FrontShocks2;      // 5,40
            /* 0x0290 */ public float FrontSwayBar;      // 33
            /* 0x0294 */ public float SuspensionUnk2F;   // 0,145
            /* 0x0298 */ public float SuspensionUnk3F;   // -0,145
            /* 0x029C */ public float SuspensionUnk4F;   // 0,125
            /* 0x02A0 */ public float SuspensionUnk1R;   // 5,40
            /* 0x02A4 */ public float RearSprings;       // 40
            /* 0x02A8 */ public float RearShocks1;       // 5,70
            /* 0x02AC */ public float RearShocks2;       // 5,40
            /* 0x02B0 */ public float RearSwayBar;       // 33
            /* 0x02B4 */ public float SuspensionUnk2R;   // 0,145
            /* 0x02B8 */ public float SuspensionUnk3R;   // -0,145
            /* 0x02BC */ public float SuspensionUnk4R;   // 0,125
            /* 0x02C0 */ public float Unknown0pt8;       // 0,8
            /* 0x02C4 */ public float Unknown500;        // 500
            /* 0x02C8 */ public float FinalDriveRatio;   // 4,40
            /* 0x02CC */ public float FinalDriveRatio2;  // 4,40

            /* 0x02D8 */ public int NumberOfGears;   // 5
  
            /* 0x02E0 */ public float GearRatioR;    // -3,23
            /* 0x02E4 */ public float GearRatioN;    // 0
            /* 0x02E8 */ public float GearRatio1;    // 3,23
            /* 0x02EC */ public float GearRatio2;    // 2,11
            /* 0x02F0 */ public float GearRatio3;    // 1,46
            /* 0x02F4 */ public float GearRatio4;    // 1,11
            /* 0x02F8 */ public float GearRatio5;    // 0,82
            /* 0x02FC */ public float GearRatio6;    // 0
            /* 0x0300 */ public float IdleRPM;       // 800
            /* 0x0304 */ public float RedLineRPM;    // 8000
            /* 0x0308 */ public float MAXRPM;        // 8500
  
            /* 0x0310 */ public float ECUx1000; // 0,1
            /* 0x0314 */ public float ECUx2000; // 0,12
            /* 0x0318 */ public float ECUx3000; // 0,13
            /* 0x031C */ public float ECUx4000; // 0,14
            /* 0x0320 */ public float ECUx5000; // 0,143
            /* 0x0324 */ public float ECUx6000; // 0,146
            /* 0x0328 */ public float ECUx7000; // 0,151
            /* 0x032C */ public float ECUx8000; // 0,15
            /* 0x0330 */ public float ECUx9000; // 0,125
            /* 0x0334 */ public float ECUx10000; // 0
            /* 0x0338 */ public float ECUx11000; // 0
            /* 0x033C */ public float ECUx12000; // 0
  
            /* 0x0340 */ public float Turbox1000; // 0
            /* 0x0344 */ public float Turbox2000; // 0
            /* 0x0348 */ public float Turbox3000; // 0
            /* 0x034C */ public float Turbox4000; // 0
            /* 0x0350 */ public float Turbox5000; // 0
            /* 0x0354 */ public float Turbox6000; // 0
            /* 0x0358 */ public float Turbox7000; // 0,08
            /* 0x035C */ public float Turbox8000; // 0,11
            /* 0x0360 */ public float Turbox9000; // 0,1
            /* 0x0364 */ public float Turbox10000; // 0
            /* 0x0368 */ public float Turbox11000; // 0
            /* 0x036C */ public float Turbox12000; // 0
  
            /* 0x0374 */ public float FrontDownForce;    // 0,14
            /* 0x0378 */ public float RearDownForce;     // 0,14
            /* 0x037C */ public float unk0x37c;          // 0,02
            /* 0x0380 */ public float SteeringRatio;     // 1,1
            /* 0x0384 */ public float FrontBrakeStrength;     // 0,5
            /* 0x0388 */ public float RearBrakeStrength; // 0,475
            /* 0x038C */ public float BrakeBias;         // 0,56

            /* 0x0468 */ public float FinalDriveRatio_StreetPackage;   // 4,40
            /* 0x046C */ public float FinalDriveRatio2_StreetPackage;  // 4,40

            /* 0x0478 */ public int NumberOfGears_StreetPackage;   // 5

            /* 0x0480 */ public float GearRatioR_StreetPackage;    // -3,23
            /* 0x0484 */ public float GearRatioN_StreetPackage;    // 0
            /* 0x0488 */ public float GearRatio1_StreetPackage;    // 3,23
            /* 0x048C */ public float GearRatio2_StreetPackage;    // 2,11
            /* 0x0490 */ public float GearRatio3_StreetPackage;    // 1,46
            /* 0x0494 */ public float GearRatio4_StreetPackage;    // 1,11
            /* 0x0498 */ public float GearRatio5_StreetPackage;    // 0,82
            /* 0x049C */ public float GearRatio6_StreetPackage;    // 0

            /* 0x04A8 */ public float FinalDriveRatio_ProPackage;   // 4,40
            /* 0x04AC */ public float FinalDriveRatio2_ProPackage;  // 4,40

            /* 0x04B8 */ public int NumberOfGears_ProPackage;   // 5

            /* 0x04C0 */ public float GearRatioR_ProPackage;    // -3,23
            /* 0x04C4 */ public float GearRatioN_ProPackage;    // 0
            /* 0x04C8 */ public float GearRatio1_ProPackage;    // 3,23
            /* 0x04CC */ public float GearRatio2_ProPackage;    // 2,11
            /* 0x04D0 */ public float GearRatio3_ProPackage;    // 1,46
            /* 0x04D4 */ public float GearRatio4_ProPackage;    // 1,11
            /* 0x04D8 */ public float GearRatio5_ProPackage;    // 0,82
            /* 0x04DC */ public float GearRatio6_ProPackage;    // 0

            /* 0x04E8 */ public float FinalDriveRatio_ExtremePackage;   // 4,40
            /* 0x04EC */ public float FinalDriveRatio2_ExtremePackage;  // 4,40

            /* 0x04F8 */ public int NumberOfGears_ExtremePackage;   // 5

            /* 0x0500 */ public float GearRatioR_ExtremePackage;    // -3,23
            /* 0x0504 */ public float GearRatioN_ExtremePackage;    // 0
            /* 0x0508 */ public float GearRatio1_ExtremePackage;    // 3,23
            /* 0x050C */ public float GearRatio2_ExtremePackage;    // 2,11
            /* 0x0510 */ public float GearRatio3_ExtremePackage;    // 1,46
            /* 0x0514 */ public float GearRatio4_ExtremePackage;    // 1,11
            /* 0x0518 */ public float GearRatio5_ExtremePackage;    // 0,82
            /* 0x051C */ public float GearRatio6_ExtremePackage;    // 0

            /* 0x0560 */ public float IdleRPMAdd_StreetPackage;       // 800
            /* 0x0564 */ public float RedLineRPMAdd_StreetPackage;    // 8000
            /* 0x0568 */ public float MAXRPMAdd_StreetPackage;        // 8500

            /* 0x0570 */ public float ECUx1000Add_StreetPackage; // 0,1
            /* 0x0574 */ public float ECUx2000Add_StreetPackage; // 0,12
            /* 0x0578 */ public float ECUx3000Add_StreetPackage; // 0,13
            /* 0x057C */ public float ECUx4000Add_StreetPackage; // 0,14
            /* 0x0580 */ public float ECUx5000Add_StreetPackage; // 0,143
            /* 0x0584 */ public float ECUx6000Add_StreetPackage; // 0,146
            /* 0x0588 */ public float ECUx7000Add_StreetPackage; // 0,151
            /* 0x058C */ public float ECUx8000Add_StreetPackage; // 0,15
            /* 0x0590 */ public float ECUx9000Add_StreetPackage; // 0,125
            /* 0x0594 */ public float ECUx10000Add_StreetPackage; // 0
            /* 0x0598 */ public float ECUx11000Add_StreetPackage; // 0
            /* 0x059C */ public float ECUx12000Add_StreetPackage; // 0

            /* 0x05A0 */ public float IdleRPMAdd_ProPackage;       // 800
            /* 0x05A4 */ public float RedLineRPMAdd_ProPackage;    // 8000
            /* 0x05A8 */ public float MAXRPMAdd_ProPackage;        // 8500

            /* 0x05B0 */ public float ECUx1000Add_ProPackage; // 0,1
            /* 0x05B4 */ public float ECUx2000Add_ProPackage; // 0,12
            /* 0x05B8 */ public float ECUx3000Add_ProPackage; // 0,13
            /* 0x05BC */ public float ECUx4000Add_ProPackage; // 0,14
            /* 0x05C0 */ public float ECUx5000Add_ProPackage; // 0,143
            /* 0x05C4 */ public float ECUx6000Add_ProPackage; // 0,146
            /* 0x05C8 */ public float ECUx7000Add_ProPackage; // 0,151
            /* 0x05CC */ public float ECUx8000Add_ProPackage; // 0,15
            /* 0x05D0 */ public float ECUx9000Add_ProPackage; // 0,125
            /* 0x05D4 */ public float ECUx10000Add_ProPackage; // 0
            /* 0x05D8 */ public float ECUx11000Add_ProPackage; // 0
            /* 0x05DC */ public float ECUx12000Add_ProPackage; // 0

            /* 0x05E0 */ public float IdleRPMAdd_ExtremePackage;       // 800
            /* 0x05E4 */ public float RedLineRPMAdd_ExtremePackage;    // 8000
            /* 0x05E8 */ public float MAXRPMAdd_ExtremePackage;        // 8500

            /* 0x05F0 */ public float ECUx1000Add_ExtremePackage; 
            /* 0x05F4 */ public float ECUx2000Add_ExtremePackage; 
            /* 0x05F8 */ public float ECUx3000Add_ExtremePackage; 
            /* 0x05FC */ public float ECUx4000Add_ExtremePackage; 
            /* 0x0600 */ public float ECUx5000Add_ExtremePackage; 
            /* 0x0604 */ public float ECUx6000Add_ExtremePackage; 
            /* 0x0608 */ public float ECUx7000Add_ExtremePackage; 
            /* 0x060C */ public float ECUx8000Add_ExtremePackage; 
            /* 0x0610 */ public float ECUx9000Add_ExtremePackage; 
            /* 0x0614 */ public float ECUx10000Add_ExtremePackage;
            /* 0x0618 */ public float ECUx11000Add_ExtremePackage;
            /* 0x061C */ public float ECUx12000Add_ExtremePackage;

            /* 0x0620 */ public float Turbox1000Add_ExtremePackage; 
            /* 0x0624 */ public float Turbox2000Add_ExtremePackage; 
            /* 0x0628 */ public float Turbox3000Add_ExtremePackage; 
            /* 0x062C */ public float Turbox4000Add_ExtremePackage; 
            /* 0x0630 */ public float Turbox5000Add_ExtremePackage; 
            /* 0x0634 */ public float Turbox6000Add_ExtremePackage; 
            /* 0x0638 */ public float Turbox7000Add_ExtremePackage; 
            /* 0x063C */ public float Turbox8000Add_ExtremePackage; 
            /* 0x0640 */ public float Turbox9000Add_ExtremePackage; 
            /* 0x0644 */ public float Turbox10000Add_ExtremePackage;
            /* 0x0648 */ public float Turbox11000Add_ExtremePackage;
            /* 0x064C */ public float Turbox12000Add_ExtremePackage;

            /* 0x0850 */ public uint DefaultBasePaint2;
            /* 0x087C */ public int Cost;
            /* 0x088A */ public short IsSUV; // 0

        }; /* size: 0x00d0 */
    }
}
