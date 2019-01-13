using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ed
{
    public partial class EdViewCars : Form
    {
        string CarInfoArrayPath;
        List<EdTypes.CarTypeInfo> CarTypeInfoArray;

        public EdViewCars(string _CarInfoArrayPath)
        {
            CarInfoArrayPath = _CarInfoArrayPath;
            InitializeComponent();
        }

        void ParseCarsList()
        {
            try
            {
                FileStream CarInfoFileStream = File.Open(CarInfoArrayPath, FileMode.Open);
                BinaryReader CarInfoReader = new BinaryReader(CarInfoFileStream);
                
                CarInfoReader.BaseStream.Position = 0;

                while (CarInfoReader.BaseStream.Position < CarInfoReader.BaseStream.Length)
                {
                    uint ChunkID = CarInfoReader.ReadUInt32();
                    uint ChunkSize = CarInfoReader.ReadUInt32();

                    if (ChunkID == 0x00034600) // CarTypeInfoArray
                    {
                        CarTypeInfoArray = new List<EdTypes.CarTypeInfo>();

                        CarInfoReader.BaseStream.Position += 8; // To skip filler 0x11's

                        int NumberOfCars = (int)(ChunkSize - 8) / 0xD0;
                        
                        for(int i = 0; i < NumberOfCars; i++)
                        {
                            var Car = new EdTypes.CarTypeInfo();
                            
                            Car.CarTypeName = CarInfoReader.ReadBytes(16);
                            Car.BaseModelName = CarInfoReader.ReadBytes(16);
                            Car.GeometryFilename = CarInfoReader.ReadBytes(32);
                            Car.ManufacturerName = CarInfoReader.ReadBytes(16);
                            Car.CarTypeNameHash = CarInfoReader.ReadUInt32();
                            Car.HeadlightFOV = CarInfoReader.ReadSingle();
                            Car.padHighPerformance = CarInfoReader.ReadByte();
                            Car.NumAvailableSkinNumbers = CarInfoReader.ReadByte();
                            Car.WhatGame = CarInfoReader.ReadByte();
                            Car.ConvertableFlag = CarInfoReader.ReadByte();
                            Car.WheelOuterRadius = CarInfoReader.ReadByte();
                            Car.WheelInnerRadiusMin = CarInfoReader.ReadByte();
                            Car.WheelInnerRadiusMax = CarInfoReader.ReadByte();
                            Car.pad0 = CarInfoReader.ReadByte();
                            Car.HeadlightPosition.x = CarInfoReader.ReadSingle();
                            Car.HeadlightPosition.y = CarInfoReader.ReadSingle();
                            Car.HeadlightPosition.z = CarInfoReader.ReadSingle();
                            Car.HeadlightPosition.pad = CarInfoReader.ReadSingle();
                            Car.DriverRenderingOffset.x = CarInfoReader.ReadSingle();
                            Car.DriverRenderingOffset.y = CarInfoReader.ReadSingle();
                            Car.DriverRenderingOffset.z = CarInfoReader.ReadSingle();
                            Car.DriverRenderingOffset.pad = CarInfoReader.ReadSingle();
                            Car.InCarSteeringWheelRenderingOffset.x = CarInfoReader.ReadSingle();
                            Car.InCarSteeringWheelRenderingOffset.y = CarInfoReader.ReadSingle();
                            Car.InCarSteeringWheelRenderingOffset.z = CarInfoReader.ReadSingle();
                            Car.InCarSteeringWheelRenderingOffset.pad = CarInfoReader.ReadSingle();
                            Car.Type = CarInfoReader.ReadInt32();
                            Car.UsageType = CarInfoReader.ReadInt32();
                            Car.CarMemTypeHash = CarInfoReader.ReadUInt32();
                            Car.MaxInstances = CarInfoReader.ReadBytes(5);
                            Car.WantToKeepLoaded = CarInfoReader.ReadBytes(5);
                            Car.pad4 = CarInfoReader.ReadBytes(2);
                            Car.MinTimeBetweenUses[0] = CarInfoReader.ReadSingle();
                            Car.MinTimeBetweenUses[1] = CarInfoReader.ReadSingle();
                            Car.MinTimeBetweenUses[2] = CarInfoReader.ReadSingle();
                            Car.MinTimeBetweenUses[3] = CarInfoReader.ReadSingle();
                            Car.MinTimeBetweenUses[4] = CarInfoReader.ReadSingle();
                            Car.AvailableSkinNumbers = CarInfoReader.ReadBytes(10);
                            Car.DefaultSkinNumber = CarInfoReader.ReadByte();
                            Car.Skinnable = CarInfoReader.ReadByte();
                            Car.Padding = CarInfoReader.ReadInt32();
                            Car.DefaultBasePaint = CarInfoReader.ReadUInt32();
                            
                            CarTypeInfoArray.Add(Car);
                        }
                        
                        break;

                    }
                    else
                    {
                        CarInfoReader.BaseStream.Position += ChunkSize;
                    }
                }

                CarInfoReader.Close();

                listView1.Items.Clear();

                foreach (EdTypes.CarTypeInfo i in CarTypeInfoArray)
                {
                    var CarToList = new ListViewItem();

                    CarToList.Text = i.Type.ToString();
                    CarToList.SubItems.Add(Encoding.Default.GetString(i.CarTypeName));
                    CarToList.SubItems.Add(Encoding.Default.GetString(i.BaseModelName));
                    CarToList.SubItems.Add(Encoding.Default.GetString(i.GeometryFilename));
                    CarToList.SubItems.Add(Encoding.Default.GetString(i.ManufacturerName));
                    CarToList.SubItems.Add(i.CarTypeNameHash.ToString("X8"));
                    CarToList.SubItems.Add(i.UsageType.ToString());
                    CarToList.SubItems.Add(i.CarMemTypeHash.ToString("X8"));
                    CarToList.SubItems.Add(i.Skinnable.ToString());
                    CarToList.SubItems.Add(i.DefaultBasePaint.ToString("X8"));
                    
                    listView1.Items.Add(CarToList);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void EdViewCars_Load(object sender, EventArgs e)
        {
            ParseCarsList();
        }
    }
}
