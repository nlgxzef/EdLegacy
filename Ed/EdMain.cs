using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace Ed
{
    public partial class EdMain : Form
    {
        public EdMain()
        {
            InitializeComponent();
        }

        bool DisableLogs = false;
        bool DisableBackups = false;
        bool KeepTempFiles = false;
        int CurrentGame;
        string WorkingFolder = "";
        uint FileSizeDifference = 0;

        [ConditionalAttribute("DEBUG")]
        public void ToggleDebugStuff()
        {
            ButtonViewCars.Visible = true;
        }

        private void SetTitleText()
        {
            String[] VersionInfo = ProductVersion.Split('.');
            String VersionText = ProductName + " - " + "The Car Dealer!" + " | " + "v" + ProductVersion;
            int VersionDelta = Int32.Parse(VersionInfo[3]);

            VersionText += " " + "(" + "Build" + " " + VersionInfo[0] + (Int32.Parse(VersionInfo[1]) != 0 ? VersionInfo[1] + "." : "") + ";" + " " + "Rev." + (Int32.Parse(VersionInfo[2]) < 10 ? "0" : "") + VersionInfo[2];

            if (VersionDelta >= 100 && VersionDelta < 200) VersionText += " " + "Milestone";
            else if (VersionDelta >= 200 && VersionDelta < 400) VersionText += " " + "ALPHA";
            else if (VersionDelta >= 400 && VersionDelta < 800) VersionText += " " + "BETA";
            else if (VersionDelta >= 800 && VersionDelta < 1337) VersionText += " " + "Release Candidate";
            else if (VersionDelta >= 1338 && VersionDelta < 2000) VersionText += " " + "Hotfix";
            else if (VersionDelta >= 2000 && VersionDelta < 9999) VersionText += " " + "Post-Release Test";

            VersionText += ")";
            
            Text = VersionText;
        }

        public void CreateLogFile()
        {
            try
            {
                FileStream LogStream;
                StreamWriter LogFile;

                LogStream = File.Open("Ed.log", FileMode.Create);
                LogFile = new StreamWriter(LogStream);
                LogFile.WriteLine("# Ed Log File");
                LogFile.WriteLine("# File created on: " + DateTime.Now.ToString());
                LogFile.WriteLine("# ------------------------------------------------------------------------------");
                LogFile.Close();
            }
            catch (Exception)
            {
                DisableLogs = true;
                MenuItemLog.Checked = false;
            }
        }

        public void Log(String LogEntry, bool WriteToStatusBar = false)
        {
            if (!DisableLogs) try
            {
                FileStream LogStream;
                StreamWriter LogFile;

                LogStream = File.Open("Ed.log", FileMode.Append);
                LogFile = new StreamWriter(LogStream);
                LogFile.WriteLine("[" + DateTime.Now.ToString() + "]" + " : " + LogEntry);
                LogFile.Close();
            }
            catch (Exception)
            {
                DisableLogs = true;
                MenuItemLog.Checked = false;
            }

            if (WriteToStatusBar)
            {
                StatusTextEd.Text = LogEntry;
            }

        }

        public void CloneDirectory(string root, string dest, bool overwrite)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, dirName)))
                {
                    Directory.CreateDirectory(Path.Combine(dest, dirName));
                }
                CloneDirectory(directory, Path.Combine(dest, dirName), overwrite);
            }

            foreach (var file in Directory.GetFiles(root))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), overwrite);
            }
        }

        public string GetConfigPath()
        {
            string ConfigPath = "";

            switch(CurrentGame)
            {
                case (int)EdTypes.Game.Carbon:
                default:
                    ConfigPath = @"Config\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ConfigPath = @"Config\NFSMW";
                    break;
            }
            
            bool exists = Directory.Exists(ConfigPath);

            if (!exists)
                Directory.CreateDirectory(ConfigPath);

            return ConfigPath;
        }

        public string GetResourcesPath()
        {
            string ResourcesPath = "";

            switch (CurrentGame)
            {
                case (int)EdTypes.Game.Carbon:
                default:
                    ResourcesPath = @"Resources\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ResourcesPath = @"Resources\NFSMW";
                    break;
            }

            bool exists = Directory.Exists(ResourcesPath);

            if (!exists)
                Directory.CreateDirectory(ResourcesPath);
            
            return ResourcesPath;
        }

        public string GetTempPath()
        {
            string TempPath = "";

            switch (CurrentGame)
            {
                case (int)EdTypes.Game.Carbon:
                default:
                    TempPath = @"Temp\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    TempPath = @"Temp\NFSMW";
                    break;
            }

            bool exists = Directory.Exists(TempPath);

            if (!exists)
            {
                Directory.CreateDirectory(TempPath);
                Directory.CreateDirectory(Path.Combine(TempPath, @"Frontend"));
                Directory.CreateDirectory(Path.Combine(TempPath, @"Global"));
                Directory.CreateDirectory(Path.Combine(TempPath, @"Languages"));
            }

            return TempPath;
        }

        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }

        public void DeleteTempPath()
        {
            string TempPath = @"Temp";

            if (!KeepTempFiles)
            {
                bool exists = Directory.Exists(TempPath);

                if (exists) EmptyFolder(new DirectoryInfo(TempPath));
            }
        }

        public void RefreshConfigView()
        {
            string AddCarsPath = GetConfigPath();

            ListCarsToAdd.Items.Clear();

            try
            {
                DirectoryInfo di = new DirectoryInfo(AddCarsPath);
                FileInfo[] INIFiles = di.GetFiles("*.ini");
                if (INIFiles.Length == 0)
                {
                    Log("No config files found.", true);
                    return;
                }
                else
                {
                    foreach (FileInfo i in INIFiles)
                    {
                        string INIFilePath = Path.Combine(AddCarsPath, i.ToString());

                        if (i.Name == "config.ini")
                        {
                            Log("WARNING! Config files from old versions of Texture Compiler aren't supported.", true);
                        }
                        else
                        {
                            var ConfigCar = new ListViewItem();

                            var IniReader = new IniFile(INIFilePath);

                            string XName = Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false));
                            ConfigCar.Text = XName;

                            if (XName.Length > 14)
                            {
                                ConfigCar.BackColor = Color.LightYellow;
                                ConfigCar.ToolTipText = "Car names cannot be longer than 14 characters. Check your .ini file.";
                            }

                            string ManuName = IniReader.GetValue("INFO", "Manufacturer");

                            if (ManuName.Length > 15)
                            {
                                ConfigCar.BackColor = Color.LightYellow;
                                ConfigCar.ToolTipText = "Car manufacturer names cannot be longer than 14 characters. Check your .ini file.";
                            }

                            string CarName = IniReader.GetValue("RESOURCES", "Description", IniReader.GetValue("RESOURCES", "Name", "N/A"));
                            ConfigCar.SubItems.Add(CarName);

                            ListCarsToAdd.Items.Add(ConfigCar);
                        }
                    }
                }

                Log("Ready.", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ed was unable to list config files." + Environment.NewLine + "Make sure you have enough permissions (Try running Ed as administrator.)." + Environment.NewLine + Environment.NewLine + "Exception code:" + Environment.NewLine + ex.ToString(), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("ERROR! Ed was unable to list config files. Try running Ed as administrator.", true);
                Log("Exception: " + ex.ToString());
            }
        }

        public void ToggleButtons(bool _IsEnabled)
        {
            MenuItemBrowseConfig.Enabled = _IsEnabled;
            MenuItemBrowseResources.Enabled = _IsEnabled;
            MenuTools.Enabled = _IsEnabled;
            MenuItemRefresh.Enabled = _IsEnabled;
            ButtonViewCars.Enabled = _IsEnabled;
            ButtonAddCars.Enabled = _IsEnabled;
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }

        private void ButtonSelectWorkingDir_Click(object sender, EventArgs e)
        {
            ToggleButtons(false);

            if (DialogWorkingFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LabelWorkingDir.Text = DialogWorkingFolder.SelectedPath;
                WorkingFolder = DialogWorkingFolder.SelectedPath;

                try
                {
                    DirectoryInfo di = new DirectoryInfo(DialogWorkingFolder.SelectedPath);
                    FileInfo[] EXEFiles = di.GetFiles("*.exe");
                    if (EXEFiles.Length == 0)
                    {
                        MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "The directory doesn't contain any game executables.");
                        Log("ERROR! Ed was unable to detect any game installations in this directory.");
                        Log("The directory doesn't contain any game executables.");
                        return;
                    }
                    else
                    {
                        foreach (var i in EXEFiles)
                        {
                            if (i.ToString() == "NFSC.exe")
                            {
                                Log("NFS Carbon detected.");
                                CurrentGame = (int)EdTypes.Game.Carbon;
                                ToggleButtons(true);
                                break;
                            }

                            if (i.ToString() == "speed.exe")
                            {
                                Log("NFS Most Wanted detected.");
                                CurrentGame = (int)EdTypes.Game.MostWanted;
                                ToggleButtons(true);
                                break;
                            }
                        }

                        if (CurrentGame == 0)
                        {
                            MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "The directory doesn't contain any game executables.");
                            Log("ERROR! Ed was unable to detect any game installations in this directory.");
                            Log("The directory doesn't contain any game executables.");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "Make sure you have enough permissions (Try running Ed as administrator.)." + Environment.NewLine + Environment.NewLine + "Exception code:" + Environment.NewLine + ex.ToString(), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log("ERROR! Ed was unable to detect any game installations in this directory.");
                    Log("Exception: " + ex.ToString());
                }
            }
        }

        public int GetCarTypeIDFromResources(string _XName)
        {
            int CarTypeID = -1;
            int i;

            try
            {
                var CarInfoArrayFile = new FileStream(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), FileMode.Open, FileAccess.Read);
                var CarInfoArrayFileReader = new BinaryReader(CarInfoArrayFile);


                if (CarInfoArrayFileReader.ReadUInt32() == 0x00034600)
                {
                    int CarInfoCount = (CarInfoArrayFileReader.ReadInt32() - 8) / 0xD0;
                    CarTypeID = CarInfoCount;

                    for (i = 0; i< CarInfoCount; i++)
                    {
                        CarInfoArrayFileReader.BaseStream.Position = 16 + (i * 0xD0);

                        string XName = Encoding.ASCII.GetString(CarInfoArrayFileReader.ReadBytes(16));

                        if (XName == _XName)
                        {
                            CarTypeID = i;
                            break;
                        }
                    }
                }

                CarInfoArrayFileReader.Close();
                CarInfoArrayFile.Close();
            }
            catch (Exception ex)
            {
                Log("ERROR! Ed was unable to read a resource file.");
                Log("Exception: " + ex.ToString());
            }

            return CarTypeID;
            
        }

        public int GetCarPartIDFromResources(string _XName)
        {
            int CarPartID = -1;
            int i;

            try
            {
                var CarInfoCarPart5File = new FileStream(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), FileMode.Open, FileAccess.Read);
                var CarInfoCarPart5FileReader = new BinaryReader(CarInfoCarPart5File);


                if (CarInfoCarPart5FileReader.ReadUInt32() == 0x0003460B)
                {
                    int CarInfoCount = (CarInfoCarPart5FileReader.ReadInt32() / 4);
                    CarPartID = CarInfoCount;

                    for (i = 0; i < CarInfoCount; i++)
                    {
                        CarInfoCarPart5FileReader.BaseStream.Position = 8 + (i * 4);

                        int CarTypeNameHash = CarInfoCarPart5FileReader.ReadInt32();

                        if (CarTypeNameHash == BinHash.Hash(_XName))
                        {
                            CarPartID = i;
                            break;
                        }
                    }
                }

                CarInfoCarPart5FileReader.Close();
                CarInfoCarPart5File.Close();
            }
            catch (Exception ex)
            {
                Log("ERROR! Ed was unable to read a resource file.");
                Log("Exception: " + ex.ToString());
            }

            return CarPartID;

        }

        private void ButtonBrowseConfigFolder_Click(object sender, EventArgs e)
        {
            string ConfigPath = GetConfigPath();
            System.Diagnostics.Process.Start("explorer", ConfigPath);
        }

        private void ButtonViewCars_Click(object sender, EventArgs e)
        {
            var CarView = new EdViewCars(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
            CarView.Show();
        }

        private void EdMain_Load(object sender, EventArgs e)
        {
            SetTitleText();
            ToggleButtons(false);
            ToggleDebugStuff(); // will only work with Debug config
            CreateLogFile();
            Log("Ready.", true);
        }
        
        private void ButtonRestoreBackups_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(WorkingFolder);
            FileInfo[] Backups = di.GetFiles("*.edbackup", SearchOption.AllDirectories);

            foreach(FileInfo BackedUpFile in Backups)
            {
                File.Copy(BackedUpFile.FullName, BackedUpFile.FullName.Substring(0, BackedUpFile.FullName.LastIndexOf(".edbackup")), true);
                File.Delete(BackedUpFile.FullName);
            }

            MessageBox.Show("Successfully restored backups.");
            Log("Successfully restored backups.", true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleButtons(false);

            if (DialogWorkingFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LabelWorkingDir.Text = DialogWorkingFolder.SelectedPath;
                WorkingFolder = DialogWorkingFolder.SelectedPath;

                try
                {
                    DirectoryInfo di = new DirectoryInfo(DialogWorkingFolder.SelectedPath);
                    FileInfo[] EXEFiles = di.GetFiles("*.exe");
                    if (EXEFiles.Length == 0)
                    {
                        MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "The directory doesn't contain any game executables.");
                        Log("ERROR! Ed was unable to detect any game installations in this directory.");
                        Log("The directory doesn't contain any game executables.", true);
                        return;
                    }
                    else
                    {
                        foreach (var i in EXEFiles)
                        {
                            if (i.ToString() == "NFSC.exe")
                            {
                                Log("NFS Carbon detected.", true);
                                CurrentGame = (int)EdTypes.Game.Carbon;
                                RefreshConfigView();
                                ToggleButtons(true);
                                break;
                            }

                            if (i.ToString() == "speed.exe")
                            {
                                Log("NFS Most Wanted detected.", true);
                                CurrentGame = (int)EdTypes.Game.MostWanted;
                                RefreshConfigView();
                                ToggleButtons(true);
                                break;
                            }
                        }

                        if (CurrentGame == 0)
                        {
                            MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "The directory doesn't contain any game executables.");
                            Log("ERROR! Ed was unable to detect any game installations in this directory.");
                            Log("The directory doesn't contain any game executables.", true);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "Make sure you have enough permissions (Try running Ed as administrator.)." + Environment.NewLine + Environment.NewLine + "Exception code:" + Environment.NewLine + ex.ToString(), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log("ERROR! Ed was unable to detect any game installations in this directory.");
                    Log("Exception: " + ex.ToString());
                }
            }
        }

        private void MenuItemLog_Click(object sender, EventArgs e)
        {
            MenuItemLog.Checked = !MenuItemLog.Checked;
            DisableLogs = !MenuItemLog.Checked;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            MenuItemBackup.Checked = !MenuItemBackup.Checked;
            DisableBackups = !MenuItemBackup.Checked;
        }

        private void MenuItemBrowseResources_Click(object sender, EventArgs e)
        {
            string Resources = GetResourcesPath();
            System.Diagnostics.Process.Start("explorer", Resources);
        }

        private void MenuItemRefresh_Click(object sender, EventArgs e)
        {
            RefreshConfigView();
        }

        private void ListCarsToAdd_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = ListCarsToAdd.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                string Config = GetConfigPath();
                System.Diagnostics.Process.Start(Path.Combine(Config, item.Text + ".ini"));
            }
        }

        private void addCarsFromConfigFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ConfigPath = GetConfigPath();
            
            try
            {
                DirectoryInfo di = new DirectoryInfo(ConfigPath);
                FileInfo[] INIFiles = di.GetFiles("*.ini");
                if (INIFiles.Length == 0)
                {
                    MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "The directory doesn't contain any config files.");
                    Log("ERROR! Ed was unable to add cars into the game.");
                    Log("The directory doesn't contain any config files.", true);
                    return;
                }
                else
                {
                    var NewCarTypeInfoArray = new List<EdTypes.CarTypeInfo>();
                    var NewSpoilerTypes = new List<EdTypes.SlotType>();
                    var NewCollisionList = new List<EdTypes.CarCollision>();
                    var NewResourcesList = new List<EdTypes.CarResource>();

                    foreach (FileInfo i in INIFiles)
                    {
                        string INIFilePath = Path.Combine(ConfigPath, i.ToString());

                        if (i.Name == "config.ini")
                        {
                            MessageBox.Show("Config files from old versions of Texture Compiler aren't supported.", "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("WARNING! Config files from old versions of Texture Compiler aren't supported.");
                        }
                        else
                        {
                            var Car = new EdTypes.CarTypeInfo();
                            var SpoilerType = new EdTypes.SlotType();
                            var Collision = new EdTypes.CarCollision();
                            var Resource = new EdTypes.CarResource();

                            var IniReader = new IniFile(INIFilePath);

                            string XName = Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false));

                            if (XName.Length > 14)
                            {
                                MessageBox.Show("Car names cannot be longer than 14 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("Car names cannot be longer than 14 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)));
                                continue;
                            }

                            string ManuName = IniReader.GetValue("INFO", "Manufacturer");

                            if (ManuName.Length > 15)
                            {
                                MessageBox.Show("Car manufacturer names cannot be longer than 15 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("Car manufacturer names cannot be longer than 15 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)));
                                continue;
                            }

                            // Start reading and calculating required values
                            Car.CarTypeName = Encoding.ASCII.GetBytes(XName);
                            Car.BaseModelName = Encoding.ASCII.GetBytes(XName);
                            Car.GeometryFilename = Encoding.ASCII.GetBytes("CARS\\" + XName + "\\GEOMETRY.BIN");
                            Car.ManufacturerName = Encoding.ASCII.GetBytes(IniReader.GetValue("INFO", "Manufacturer"));

                            // Do some byte array magic to avoid squishing
                            Array.Resize(ref Car.CarTypeName, 16);
                            Array.Resize(ref Car.BaseModelName, 16);
                            Array.Resize(ref Car.GeometryFilename, 32);
                            Array.Resize(ref Car.ManufacturerName, 16);

                            // Continue reading
                            Car.CarTypeNameHash = (uint)BinHash.Hash(XName);
                            Car.HeadlightFOV = ToSingle(IniReader.GetDouble("INFO", "HeadlightFOV", 0.0f));
                            Car.padHighPerformance = (byte)IniReader.GetInteger("INFO", "padHighPerformance", 0, byte.MinValue, byte.MaxValue);
                            Car.NumAvailableSkinNumbers = (byte)IniReader.GetInteger("INFO", "NumAvailableSkinNumbers", 0, byte.MinValue, byte.MaxValue);
                            Car.WhatGame = (byte)IniReader.GetInteger("INFO", "WhatGame", 1, byte.MinValue, byte.MaxValue);
                            Car.ConvertableFlag = (byte)IniReader.GetInteger("INFO", "ConvertableFlag", 0, byte.MinValue, byte.MaxValue);
                            Car.WheelOuterRadius = (byte)IniReader.GetInteger("INFO", "WheelOuterRadius", 26, byte.MinValue, byte.MaxValue);
                            Car.WheelInnerRadiusMin = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMin", 17, byte.MinValue, byte.MaxValue);
                            Car.WheelInnerRadiusMax = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMax", 20, byte.MinValue, byte.MaxValue);
                            Car.pad0 = (byte)IniReader.GetInteger("INFO", "pad0", 0, byte.MinValue, byte.MaxValue);
                            Car.HeadlightPosition.x = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionX", 0.0f));
                            Car.HeadlightPosition.y = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionY", 0.0f));
                            Car.HeadlightPosition.z = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionZ", 0.0f));
                            Car.HeadlightPosition.pad = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionPad", 0.0f));
                            Car.DriverRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetX", 0.0f));
                            Car.DriverRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetY", 0.0f));
                            Car.DriverRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetZ", 0.0f));
                            Car.DriverRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetPad", 0.0f));
                            Car.InCarSteeringWheelRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetX", 0.0f));
                            Car.InCarSteeringWheelRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetY", 0.0f));
                            Car.InCarSteeringWheelRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetZ", 0.0f));
                            Car.InCarSteeringWheelRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetPad", 0.0f));

                            // Will be set while adding to GlobalB
                            Car.Type = GetCarTypeIDFromResources(XName);

                            // Backwards support for MWInside's ReCompiler
                            string CarClass = IniReader.GetValue("INFO", "Class");
                            if (!string.IsNullOrEmpty(CarClass))
                            {
                                switch (CarClass)
                                {
                                    case "Racers":
                                    case "Racing":
                                    default:
                                        Car.UsageType = (int)EdTypes.CarUsageType.Racer;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("Racing");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("GLOSS_L1_COLOR17");
                                        break;
                                    case "Player":
                                        Car.UsageType = (int)EdTypes.CarUsageType.Racer;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("Player");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("GLOSS_L1_COLOR17");
                                        break;
                                    case "Cops":
                                    case "Cop":
                                        Car.UsageType = (int)EdTypes.CarUsageType.Cop;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("Cop");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("COP_L1_COLOR01");
                                        break;
                                    case "Traffic":
                                        Car.UsageType = (int)EdTypes.CarUsageType.Traffic;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("Traffic");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("TRAFFIC_L1_COLOR01");
                                        break;
                                    case "BigTraffic":
                                        Car.UsageType = (int)EdTypes.CarUsageType.Traffic;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("BigTraffic");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("TRAFFIC_L1_COLOR01");
                                        break;
                                    case "HugeTraffic":
                                        Car.UsageType = (int)EdTypes.CarUsageType.Traffic;
                                        Car.CarMemTypeHash = (uint)BinHash.Hash("HugeTraffic");
                                        Car.DefaultBasePaint = (uint)BinHash.Hash("TRAFFIC_L1_COLOR01");
                                        break;
                                }
                            }
                            else // Read from UsageType
                            {
                                Car.UsageType = IniReader.GetInteger("INFO", "UsageType", (int)EdTypes.CarUsageType.Racer, (int)EdTypes.CarUsageType.Racer, (int)EdTypes.CarUsageType.Universal);
                                Car.CarMemTypeHash = (uint)BinHash.Hash(IniReader.GetValue("INFO", "CarMemType", "Racing"));
                                Car.DefaultBasePaint = (uint)BinHash.Hash("GLOSS_L1_COLOR17");
                            }

                            Car.MaxInstances[0] = (byte)IniReader.GetInteger("INFO", "MaxInstances1", 0, byte.MinValue, byte.MaxValue);
                            Car.MaxInstances[1] = (byte)IniReader.GetInteger("INFO", "MaxInstances2", 0, byte.MinValue, byte.MaxValue);
                            Car.MaxInstances[2] = (byte)IniReader.GetInteger("INFO", "MaxInstances3", 0, byte.MinValue, byte.MaxValue);
                            Car.MaxInstances[3] = (byte)IniReader.GetInteger("INFO", "MaxInstances4", 0, byte.MinValue, byte.MaxValue);
                            Car.MaxInstances[4] = (byte)IniReader.GetInteger("INFO", "MaxInstances5", 0, byte.MinValue, byte.MaxValue);

                            Car.WantToKeepLoaded[0] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded1", 0, byte.MinValue, byte.MaxValue);
                            Car.WantToKeepLoaded[1] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded2", 0, byte.MinValue, byte.MaxValue);
                            Car.WantToKeepLoaded[2] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded3", 0, byte.MinValue, byte.MaxValue);
                            Car.WantToKeepLoaded[3] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded4", 0, byte.MinValue, byte.MaxValue);
                            Car.WantToKeepLoaded[4] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded5", 0, byte.MinValue, byte.MaxValue);

                            Car.pad4[0] = (byte)IniReader.GetInteger("INFO", "pad41", 0, byte.MinValue, byte.MaxValue);
                            Car.pad4[1] = (byte)IniReader.GetInteger("INFO", "pad42", 0, byte.MinValue, byte.MaxValue);

                            Car.MinTimeBetweenUses[0] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses1", 0.0f));
                            Car.MinTimeBetweenUses[1] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses2", 0.0f));
                            Car.MinTimeBetweenUses[2] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses3", 0.0f));
                            Car.MinTimeBetweenUses[3] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses4", 0.0f));
                            Car.MinTimeBetweenUses[4] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses5", 0.0f));

                            Car.AvailableSkinNumbers[0] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers1", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[1] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers2", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[2] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers3", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[3] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers4", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[4] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers5", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[5] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers6", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[6] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers7", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[7] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers8", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[8] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers9", 0, byte.MinValue, byte.MaxValue);
                            Car.AvailableSkinNumbers[9] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers10", 0, byte.MinValue, byte.MaxValue);

                            Car.DefaultSkinNumber = (byte)IniReader.GetInteger("INFO", "DefaultSkinNumber", 1, byte.MinValue, byte.MaxValue);
                            Car.Skinnable = (byte)IniReader.GetInteger("INFO", "Skinnable", Car.UsageType == (int)EdTypes.CarUsageType.Racer ? 1 : 0, byte.MinValue, byte.MaxValue); // Checks usage type if not set in ini
                            Car.Padding = IniReader.GetInteger("INFO", "Padding");
                            // Car.DefaultBasePaint = IniReader.GetInteger("INFO", "DefaultBasePaint");

                            NewCarTypeInfoArray.Add(Car);

                            // ----------------------------------------------------------------------
                            // SlotTypes (Spoilers)

                            SpoilerType.CarTypeNameHash = (uint)BinHash.Hash(XName);
                            SpoilerType.Unk1 = 0x30;
                            SpoilerType.CarTypeNameHash2 = (uint)BinHash.Hash(XName);
                            SpoilerType.SpoilerHash = (uint)BinHash.Hash(IniReader.GetValue("SPOILER", "SpoilerSet", "SPOILER"));
                            SpoilerType.SpoilerAutoSculptHash = (uint)BinHash.Hash(IniReader.GetValue("SPOILER", "AutosculptSpoilerSet", "SPOILER_AS2"));
                            SpoilerType.Unk2 = 0xC2F6EBB0;
                            SpoilerType.Unk3Zero = 0;
                            SpoilerType.Unk4Zero = 0;
                            SpoilerType.Unk5Zero = 0;

                            NewSpoilerTypes.Add(SpoilerType);

                            // ----------------------------------------------------------------------
                            // Collision
                            
                            Collision.CopyFrom = IniReader.GetValue("COLLISION", "CopyFrom", "911TURBO");
                            Collision.CopyTo = XName;

                            NewCollisionList.Add(Collision);

                            // ----------------------------------------------------------------------
                            // Resources

                            Resource.Label = IniReader.GetValue("RESOURCES", "Label", "");
                            Resource.Name = IniReader.GetValue("RESOURCES", "Name", "");
                            Resource.ManufacturerLogo = IniReader.GetValue("RESOURCES", "ManufacturerLogo", "CARSELECT_MANUFACTURER_" + IniReader.GetValue("INFO", "Manufacturer"));
                            Resource.SecondaryLogo = IniReader.GetValue("RESOURCES", "SecondaryLogo", "SECONDARY_LOGO_" + XName);

                            NewResourcesList.Add(Resource);
                            
                        }
                    }

                    // ----------------------------------------------------------------------

                    // Start Working On GlobalB

                    if (CurrentGame == (int)EdTypes.Game.Carbon)
                    {
                        // Work on Car Type Array
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), true);

                        var CarInfoArray = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), FileMode.Open, FileAccess.ReadWrite);
                        var CarInfoArrayReader = new BinaryReader(CarInfoArray);
                        var CarInfoArrayWriter = new BinaryWriter(CarInfoArray);

                        // Get ID and Size to verify chunk
                        CarInfoArrayReader.BaseStream.Position = 0;
                        uint ChunkID = CarInfoArrayReader.ReadUInt32();
                        uint ChunkSize = CarInfoArrayReader.ReadUInt32();

                        if (ChunkID != 0x00034600)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                            return;
                        }

                        // Add new data
                        int NumberOfCars = (int)(ChunkSize - 8) / 0xD0; // CarTypeID for the next car
                        CarInfoArrayWriter.BaseStream.Position = CarInfoArrayWriter.BaseStream.Length;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            if (Car.Type == -1) continue;

                            CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0xD0);

                            CarInfoArrayWriter.Write(Car.CarTypeName);
                            CarInfoArrayWriter.Write(Car.BaseModelName);
                            CarInfoArrayWriter.Write(Car.GeometryFilename);
                            CarInfoArrayWriter.Write(Car.ManufacturerName);
                            CarInfoArrayWriter.Write(Car.CarTypeNameHash);
                            CarInfoArrayWriter.Write(Car.HeadlightFOV);
                            CarInfoArrayWriter.Write(Car.padHighPerformance);
                            CarInfoArrayWriter.Write(Car.NumAvailableSkinNumbers);
                            CarInfoArrayWriter.Write(Car.WhatGame);
                            CarInfoArrayWriter.Write(Car.ConvertableFlag);
                            CarInfoArrayWriter.Write(Car.WheelOuterRadius);
                            CarInfoArrayWriter.Write(Car.WheelInnerRadiusMin);
                            CarInfoArrayWriter.Write(Car.WheelInnerRadiusMax);
                            CarInfoArrayWriter.Write(Car.pad0);
                            CarInfoArrayWriter.Write(Car.HeadlightPosition.x);
                            CarInfoArrayWriter.Write(Car.HeadlightPosition.y);
                            CarInfoArrayWriter.Write(Car.HeadlightPosition.z);
                            CarInfoArrayWriter.Write(Car.HeadlightPosition.pad);
                            CarInfoArrayWriter.Write(Car.DriverRenderingOffset.x);
                            CarInfoArrayWriter.Write(Car.DriverRenderingOffset.y);
                            CarInfoArrayWriter.Write(Car.DriverRenderingOffset.z);
                            CarInfoArrayWriter.Write(Car.DriverRenderingOffset.pad);
                            CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.x);
                            CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.y);
                            CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.z);
                            CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.pad);
                            CarInfoArrayWriter.Write(Car.Type);
                            CarInfoArrayWriter.Write(Car.UsageType);
                            CarInfoArrayWriter.Write(Car.CarMemTypeHash);
                            CarInfoArrayWriter.Write(Car.MaxInstances);
                            CarInfoArrayWriter.Write(Car.WantToKeepLoaded);
                            CarInfoArrayWriter.Write(Car.pad4);
                            CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[0]);
                            CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[1]);
                            CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[2]);
                            CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[3]);
                            CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[4]);
                            CarInfoArrayWriter.Write(Car.AvailableSkinNumbers);
                            CarInfoArrayWriter.Write(Car.DefaultSkinNumber);
                            CarInfoArrayWriter.Write(Car.Skinnable);
                            CarInfoArrayWriter.Write(Car.Padding);
                            CarInfoArrayWriter.Write(Car.DefaultBasePaint);

                            CarInfoArrayWriter.BaseStream.Position = CarInfoArrayWriter.BaseStream.Length;

                            if (Car.Type < NumberOfCars)  continue;

                            // Increase car ID for the next car
                            NumberOfCars++;
                            
                        }

                        // Fix size in Chunk Header
                        uint NewChunkSize = (uint)NumberOfCars * 0xD0 + 8;

                        CarInfoArrayWriter.BaseStream.Position = 4; // Go to size
                        CarInfoArrayWriter.Write(NewChunkSize); // Write new size

                        CarInfoArrayWriter.Close();
                        CarInfoArrayReader.Close();
                        CarInfoArray.Dispose();

                        // -----------------------------------------------------

                        // SlotTypes (Spoiler Types)

                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), true);

                        var CarInfoSlotTypes = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), FileMode.Open, FileAccess.ReadWrite);
                        var CarInfoSlotTypesReader = new BinaryReader(CarInfoSlotTypes);
                        var CarInfoSlotTypesWriter = new BinaryWriter(CarInfoSlotTypes);

                        // Get ID and Size to verify chunk
                        CarInfoSlotTypesReader.BaseStream.Position = 0;
                        ChunkID = CarInfoSlotTypesReader.ReadUInt32();
                        ChunkSize = CarInfoSlotTypesReader.ReadUInt32();

                        if (ChunkID != 0x00034607)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"));
                            return;
                        }

                        // Add new data
                        int NumberOfSpoilers = 0;
                        CarInfoSlotTypesWriter.BaseStream.Position = CarInfoSlotTypesWriter.BaseStream.Length;

                        foreach (EdTypes.SlotType Spoiler in NewSpoilerTypes)
                        {
                            // Skip if types are default
                            if (Spoiler.SpoilerHash == BinHash.Hash("SPOILER") && Spoiler.SpoilerAutoSculptHash == BinHash.Hash("SPOILER_AS2")) continue;

                            CarInfoSlotTypesWriter.Write(Spoiler.CarTypeNameHash);
                            CarInfoSlotTypesWriter.Write(Spoiler.Unk1);
                            CarInfoSlotTypesWriter.Write(Spoiler.CarTypeNameHash2);
                            CarInfoSlotTypesWriter.Write(Spoiler.SpoilerHash);
                            CarInfoSlotTypesWriter.Write(Spoiler.SpoilerAutoSculptHash);
                            CarInfoSlotTypesWriter.Write(Spoiler.Unk2);
                            CarInfoSlotTypesWriter.Write(Spoiler.Unk3Zero);
                            CarInfoSlotTypesWriter.Write(Spoiler.Unk4Zero);
                            CarInfoSlotTypesWriter.Write(Spoiler.Unk5Zero);

                            // Increase spoiler count for chunk size calculation
                            NumberOfSpoilers++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfSpoilers * 0x24);

                        // Fix padding
                        int PaddingDifference = (int)(NewChunkSize % 16);

                        while (PaddingDifference != 8)
                        {
                            CarInfoSlotTypesWriter.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }

                        CarInfoSlotTypesWriter.BaseStream.Position = 4; // Go to size
                        CarInfoSlotTypesWriter.Write(NewChunkSize); // Write new size

                        CarInfoSlotTypesWriter.Close();
                        CarInfoSlotTypesReader.Close();
                        CarInfoSlotTypes.Dispose();

                        // -----------------------------------------------------

                        // CarPart Chunks: Car XName Hash List

                        Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART"));

                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), true);

                        var CarInfoCarParts5 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), FileMode.Open, FileAccess.ReadWrite);
                        var CarInfoCarParts5Reader = new BinaryReader(CarInfoCarParts5);
                        var CarInfoCarParts5Writer = new BinaryWriter(CarInfoCarParts5);

                        // Get ID and Size to verify chunk
                        CarInfoCarParts5Reader.BaseStream.Position = 0;
                        ChunkID = CarInfoCarParts5Reader.ReadUInt32();
                        ChunkSize = CarInfoCarParts5Reader.ReadUInt32();

                        if (ChunkID != 0x0003460B)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\5_0003460B.bin"));
                            return;
                        }

                        // Add new data
                        int NumberOfXNames = (int)ChunkSize / 4;
                        CarInfoCarParts5Writer.BaseStream.Position = CarInfoCarParts5Writer.BaseStream.Length;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if (GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) < NumberOfXNames) continue;

                            CarInfoCarParts5Writer.Write(Car.CarTypeNameHash);

                            // Increase spoiler count for chunk size calculation
                            NumberOfXNames++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(NumberOfXNames * 4);

                        // Fix padding
                        PaddingDifference = (int)(NewChunkSize % 16);

                        while (PaddingDifference != 8)
                        {
                            CarInfoCarParts5Writer.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }

                        CarInfoCarParts5Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts5Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts5Writer.Close();
                        CarInfoCarParts5Reader.Close();
                        CarInfoCarParts5.Dispose();

                        // -----------------------------------------------------

                        // CarPart Chunks: Car XName Hash List

                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"), true);

                        var CarInfoCarParts6 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"), FileMode.Open, FileAccess.ReadWrite);
                        var CarInfoCarParts6Reader = new BinaryReader(CarInfoCarParts6);
                        var CarInfoCarParts6Writer = new BinaryWriter(CarInfoCarParts6);

                        // Get ID and Size to verify chunk
                        CarInfoCarParts6Reader.BaseStream.Position = 0;
                        ChunkID = CarInfoCarParts6Reader.ReadUInt32();
                        ChunkSize = CarInfoCarParts6Reader.ReadUInt32();

                        if (ChunkID != 0x00034604) // Car Parts List
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\6_00034604.bin"));
                            return;
                        }

                        // Add new data
                        int NumberOfRacerPartLists = 0;
                        int NumberOfCopPartLists = 0;
                        int NumberOfTrafficPartLists = 0;

                        // Get last used id
                        CarInfoCarParts6Reader.BaseStream.Position = CarInfoCarParts6Reader.BaseStream.Length - 3;
                        byte CarID = CarInfoCarParts6Reader.ReadByte();
                        CarInfoCarParts6Reader.BaseStream.Position += 2;


                        CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        CarID++;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if (GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) < CarID) continue;

                            switch (Car.UsageType)
                            {
                                case (int)EdTypes.CarUsageType.Racer:
                                default:
                                    foreach (ushort u in CarPartIDsCarbon.Racer)
                                    {
                                        CarInfoCarParts6Writer.Write((byte)0);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(u);
                                    }

                                    NumberOfRacerPartLists++;
                                    break;

                                case (int)EdTypes.CarUsageType.Cop:
                                    foreach (ushort u in CarPartIDsCarbon.Cop)
                                    {
                                        CarInfoCarParts6Writer.Write((byte)0);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(u);
                                    }
                                    NumberOfCopPartLists++;
                                    break;

                                case (int)EdTypes.CarUsageType.Traffic:
                                    foreach (ushort u in CarPartIDsCarbon.Traffic)
                                    {
                                        CarInfoCarParts6Writer.Write((byte)0);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(u);
                                    }
                                    NumberOfTrafficPartLists++;
                                    break;
                            }

                            CarID++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfRacerPartLists * 0x454 + NumberOfCopPartLists * 0xCC + NumberOfTrafficPartLists * 0x80);

                        // Fix padding
                        PaddingDifference = (int)(NewChunkSize % 16);

                        while (PaddingDifference != 8)
                        {
                            CarInfoCarParts6Writer.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }

                        CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts6Writer.Close();
                        CarInfoCarParts6Reader.Close();
                        CarInfoCarParts6.Dispose();

                        // -----------------------------------------------------

                        // CarPart Chunks: Zero (Info) Chunk

                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"), true);

                        var CarInfoCarParts0 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"), FileMode.Open, FileAccess.ReadWrite);
                        var CarInfoCarParts0Reader = new BinaryReader(CarInfoCarParts0);
                        var CarInfoCarParts0Writer = new BinaryWriter(CarInfoCarParts0);

                        // Get ID and Size to verify chunk
                        CarInfoCarParts0Reader.BaseStream.Position = 0;
                        ChunkID = CarInfoCarParts0Reader.ReadUInt32();
                        ChunkSize = CarInfoCarParts0Reader.ReadUInt32();

                        if (ChunkID != 0x00034603) // Car Parts List
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\0_00034603.bin"));
                            return;
                        }

                        // Write new CarID here
                        CarInfoCarParts0Writer.BaseStream.Position = 0x30;
                        CarInfoCarParts0Writer.Write(CarID);

                        // Write new Parts Count here
                        CarInfoCarParts0Writer.BaseStream.Position = 0x40;
                        CarInfoCarParts0Writer.Write(NewChunkSize/4);


                        CarInfoCarParts0Writer.Close();
                        CarInfoCarParts0Reader.Close();
                        CarInfoCarParts0.Dispose();

                        // -----------------------------------------------------

                        // CarPart Chunks : Copy the rest and merge
                        
                        // 1
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\1_00034606.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\1_00034606.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\1_00034606.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\1_00034606.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\1_00034606.bin"), true);

                        // 2
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), true);

                        // 3
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), true);

                        // 4
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), true);

                        // Merge

                        DirectoryInfo TempCarPart = new DirectoryInfo(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART"));
                        FileInfo[] TempCarPartBINFiles = TempCarPart.GetFiles("*.bin");
                        if (TempCarPartBINFiles.Length == 0)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART"));
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART"));
                            return;
                        }

                        var CarPartFileMerged = File.Create(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART.bin"));
                        var CarPartFileMergedWriter = new BinaryWriter(CarPartFileMerged);

                        // Reserve first 8 bytes for chunk header and size
                        CarPartFileMergedWriter.BaseStream.Position = 0;
                        CarPartFileMergedWriter.Write(0x80034602);
                        CarPartFileMergedWriter.Write(-1);

                        int CarPartMergedFileSize = 0;

                        foreach (FileInfo BINFile in TempCarPartBINFiles)
                        {
                            byte[] buf = File.ReadAllBytes(BINFile.FullName);

                            CarPartFileMerged.Write(buf, 0, buf.Length);

                            CarPartMergedFileSize += buf.Length;
                        }

                        CarPartFileMergedWriter.BaseStream.Position = 4;
                        CarPartFileMergedWriter.Write(CarPartMergedFileSize);

                        CarPartFileMergedWriter.Close();
                        CarPartFileMerged.Close();


                        // -----------------------------------------------------

                        // Collision : Copy and merge

                        Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS"));

                        DirectoryInfo TempCollisions = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_BOUNDS"));
                        FileInfo[] TempCollisionBINFiles = TempCollisions.GetFiles("*.bin");
                        if (TempCollisionBINFiles.Length == 0)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_BOUNDS"));
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_BOUNDS"));
                            return;
                        }

                        // Copy to temp
                        foreach (FileInfo CollisionFile in TempCollisionBINFiles)
                        {
                            File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_BOUNDS", CollisionFile.Name), Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS", CollisionFile.Name), true);
                        }


                        // Copy the new ones with new names and fix their hashes
                        foreach (EdTypes.CarCollision Coll in NewCollisionList)
                        {
                            if (!File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS\" + Coll.CopyFrom + ".bin"))) // Skip adding collision if data cannot be found
                            {
                                continue;
                            }
                            File.Copy(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS\" + Coll.CopyFrom + ".bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS\" + Coll.CopyTo + ".bin"), true);

                            var NewCollisionFile = File.Open(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS\" + Coll.CopyTo + ".bin"), FileMode.Open, FileAccess.Write);
                            var NewCollisionFileWriter = new BinaryWriter(NewCollisionFile);

                            NewCollisionFileWriter.BaseStream.Position = 16;
                            NewCollisionFileWriter.Write(JenkinsHash.getHash32(Coll.CopyTo)); // Write Jenkins hash of the car folder.

                            NewCollisionFileWriter.Close();
                            NewCollisionFile.Close();
                        }


                        // Merge

                        DirectoryInfo TempCarColl = new DirectoryInfo(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS"));
                        FileInfo[] TempCarCollBINFiles = TempCarColl.GetFiles("*.bin");
                        if (TempCarCollBINFiles.Length == 0)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS"));
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS"));
                            return;
                        }

                        var CarCollFileMerged = File.Create(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS.bin"));
                        var CarCollFileMergedWriter = new BinaryWriter(CarCollFileMerged);

                        // Reserve first 8 bytes for chunk header and size
                        CarCollFileMergedWriter.BaseStream.Position = 0;
                        CarCollFileMergedWriter.Write(0x8003b900);
                        CarCollFileMergedWriter.Write(-1);

                        int CarCollMergedFileSize = 0;

                        foreach (FileInfo BINFile in TempCarCollBINFiles)
                        {
                            byte[] buf = File.ReadAllBytes(BINFile.FullName);

                            CarCollFileMerged.Write(buf, 0, buf.Length);

                            CarCollMergedFileSize += buf.Length;
                        }

                        CarCollFileMergedWriter.BaseStream.Position = 4;
                        CarCollFileMergedWriter.Write(CarCollMergedFileSize);

                        CarCollFileMergedWriter.Close();
                        CarCollFileMerged.Close();

                        // -----------------------------------------------------

                        // Finally, put all these stuff into GlobalB.lzc

                        DirectoryInfo TempGlobal = new DirectoryInfo(Path.Combine(GetTempPath(), @"Global"));
                        FileInfo[] TempGlobalFiles = TempGlobal.GetFiles("*.bin", SearchOption.TopDirectoryOnly);
                        if (TempGlobalFiles.Length == 0)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Cannot access temporary resource folder:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global"));
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Cannot access temporary resource folder:" + Environment.NewLine + Path.Combine(GetTempPath(), @"Global"));
                            return;
                        }

                        string GlobalBPath = Path.Combine(WorkingFolder, @"Global\GlobalB.lzc");
                        
                        // Make a backup (or restore if it exists)
                        if (!DisableBackups)
                        {
                            if (!File.Exists(GlobalBPath + ".edbackup")) File.Copy(GlobalBPath, GlobalBPath + ".edbackup");
                            else File.Copy(GlobalBPath + ".edbackup", GlobalBPath, true);
                        }

                        // Decompress GlobalB if it's in JDLZ format
                        byte[] GlobalBLZC = File.ReadAllBytes(GlobalBPath);
                        byte[] GlobalBBUN = JDLZ.decompress(GlobalBLZC);

                        var GlobalB = File.Open(GlobalBPath, FileMode.Open, FileAccess.ReadWrite);
                        var GlobalBReader = new BinaryReader(new MemoryStream(GlobalBBUN)); // Read from memory
                        var GlobalBWriter = new BinaryWriter(GlobalB); // Write to file
                        
                        // Write Data
                        while (GlobalBReader.BaseStream.Position < GlobalBReader.BaseStream.Length)
                        {
                            uint WriterChunkOffset = (uint)GlobalBReader.BaseStream.Position;
                            uint WriterChunkID = GlobalBReader.ReadUInt32();
                            int WriterChunkSize = GlobalBReader.ReadInt32();

                            switch (WriterChunkID)
                            {
                                case 0x00034600: // Write CarTypeInfoArray (174)
                                    byte[] bufCarInfoArray = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                                    GlobalBWriter.Write(bufCarInfoArray, 0, bufCarInfoArray.Length);

                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    break;

                                case 0x00034607:  // Write SlotTypes (177)
                                    byte[] bufSlotTypes = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"));
                                    GlobalBWriter.Write(bufSlotTypes, 0, bufSlotTypes.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    break;

                                case 0x80034602: // Write CarPart (178)
                                    byte[] bufCarPart = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART.bin"));
                                    GlobalBWriter.Write(bufCarPart, 0, bufCarPart.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    break;

                                case 0x8003b900: // Write Bounds (181)
                                    byte[] bufBounds = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS.bin"));
                                    GlobalBWriter.Write(bufBounds, 0, bufBounds.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    break;

                                case 0xb3300000:
                                    goto case 0xb0300100; // pseudo fall-through :(

                                case 0xb0300100: // Texture chunks
                                    // Fix starting position (They should start at an offset % 0x80 = 0)
                                    PaddingDifference = (int)(GlobalBWriter.BaseStream.Position % 128);

                                    while (PaddingDifference != 0)
                                    {
                                        GlobalBWriter.Write((byte)0);
                                        PaddingDifference = (PaddingDifference + 1) % 128;
                                    }

                                    goto default; // pseudo fall-through :(

                                default: // copy data
                                    GlobalBWriter.Write(WriterChunkID);
                                    GlobalBWriter.Write(WriterChunkSize);
                                    GlobalBWriter.Write(GlobalBReader.ReadBytes((WriterChunkSize)));
                                    break;
                            }
                        }

                        GlobalBWriter.Close();
                        GlobalBReader.Close();
                        GlobalB.Close();
                        Log("Successfully rebuilt GlobalB.lzc file.");

                        // -----------------------------------------------------

                        // Language : Copy and merge

                        Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Languages"));

                        DirectoryInfo TempLanguages = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Languages"));
                        FileInfo[] TempLanguageBINFiles = TempLanguages.GetFiles("*_Frontend.bin");
                        if (TempLanguageBINFiles.Length == 0)
                        {
                            goto RebuildTPK;
                        }
                        
                        // Add required strings
                        foreach (FileInfo LangFile in TempLanguageBINFiles)
                        {
                            File.Copy(Path.Combine(GetResourcesPath(), @"Languages", LangFile.Name), Path.Combine(GetTempPath(), @"Languages", LangFile.Name), true);

                            if (!File.Exists(Path.Combine(GetTempPath(), @"Languages", LangFile.Name))) // Skip adding Language if data cannot be found
                            {
                                continue;
                            }

                            byte[] LangFileBuffer = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name));
                            var LangFileMem = new MemoryStream(LangFileBuffer);
                            var LangFileReader = new BinaryReader(LangFileMem);

                            var NewLangFile = File.Open(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name), FileMode.Open, FileAccess.Write);
                            var NewLangFileWriter = new BinaryWriter(NewLangFile);

                            // Rebuild new file with added info

                            var Language = new LanguageFile();
                            LangFileReader.BaseStream.Position = 0;

                            // Read basic info
                            Language.ChunkID = LangFileReader.ReadInt32();
                            Language.ChunkSize = LangFileReader.ReadInt32();
                            Language.NumberOfEntries = LangFileReader.ReadInt32();
                            Language.HashBlockPosition = LangFileReader.ReadInt32();
                            Language.TextBlockPosition = LangFileReader.ReadInt32();
                            Language.LanguageFileType = LangFileReader.ReadBytes(16);

                            var HashList = new List<LanguageFile.HashInfo>();
                            
                            LangFileReader.BaseStream.Position = 8 + Language.HashBlockPosition;

                            for (int l=0; l<Language.NumberOfEntries; l++)
                            {
                                var AHashInfo = new LanguageFile.HashInfo();

                                AHashInfo.StringHash = LangFileReader.ReadUInt32();
                                AHashInfo.OffsetInTextBlock = LangFileReader.ReadUInt32();

                                HashList.Add(AHashInfo);
                            }

                            var ByteArrayForNewStrings = new List<byte>();
                            
                            foreach (EdTypes.CarResource CarRes in NewResourcesList)
                            {
                                if (string.IsNullOrEmpty(CarRes.Label) || string.IsNullOrEmpty(CarRes.Name)) continue;

                                var AHashInfo = new LanguageFile.HashInfo();
                                AHashInfo.StringHash = (uint)BinHash.Hash(CarRes.Label);
                                AHashInfo.OffsetInTextBlock = (uint)(Language.ChunkSize - Language.TextBlockPosition); // The address string will start from

                                HashList.Add(AHashInfo);
                                Language.NumberOfEntries++;
                                Language.TextBlockPosition += 8;
                                Language.ChunkSize += 8;

                                if (LangFile.Name.ToUpper(new CultureInfo("en-US", false)) == "LABELS_FRONTEND.BIN")
                                {
                                    ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Label)); // String files are coded as UTF-8
                                    Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Label).Count();
                                }
                                else
                                {
                                    ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Name)); // String files are coded as UTF-8
                                    Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Name).Count();
                                }
                            }

                            // Sort by hash
                            HashList.Sort((x, y) => x.StringHash.CompareTo(y.StringHash));

                            // Start Writing New File
                            NewLangFileWriter.BaseStream.Position = 0;

                            NewLangFileWriter.Write(Language.ChunkID);
                            NewLangFileWriter.Write(Language.ChunkSize);
                            NewLangFileWriter.Write(Language.NumberOfEntries);
                            NewLangFileWriter.Write(Language.HashBlockPosition);
                            NewLangFileWriter.Write(Language.TextBlockPosition);
                            NewLangFileWriter.Write(Language.LanguageFileType);

                            // Write hash block
                            foreach (LanguageFile.HashInfo i in HashList)
                            {
                                NewLangFileWriter.Write(i.StringHash);
                                NewLangFileWriter.Write(i.OffsetInTextBlock);
                            }

                            // Read original string info (Reader was waiting for us at the start of string data)
                            NewLangFileWriter.Write(LangFileReader.ReadBytes((int)(LangFileReader.BaseStream.Length - LangFileReader.BaseStream.Position)));

                            foreach (byte i in ByteArrayForNewStrings)
                            {
                                NewLangFileWriter.Write(i);
                            }

                            // Fix padding
                            PaddingDifference = (Language.ChunkSize % 4);

                            while (PaddingDifference != 0)
                            {
                                NewLangFileWriter.Write((byte)0);
                                PaddingDifference = (PaddingDifference + 1) % 4;
                                Language.ChunkSize++;
                            }

                            // Fix chunk size
                            NewLangFileWriter.BaseStream.Position = 4;
                            NewLangFileWriter.Write((int)NewLangFileWriter.BaseStream.Length - 8);

                            // Close streams
                            LangFileReader.Close();
                            LangFileMem.Close();
                            NewLangFileWriter.Close();
                            NewLangFile.Close();

                            // Copy the files in
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if (!File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup")))
                                    File.Copy(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), true);
                            Log("Successfully rebuilt" + LangFile.Name + "file.");
                        }

                        // -----------------------------------------------------

                        // Rebuild TPK file with new textures

                        RebuildTPK:
                        
                        DirectoryInfo FrontEnd = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd"));
                        FileInfo[] FrontEndBINFiles = FrontEnd.GetFiles();
                        if (FrontEndBINFiles.Length == 0)
                        {
                            goto DoneMessage;
                        }
                        
                        DirectoryInfo FrontEndTexturesTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D"));
                        FileInfo[] FrontEndTexturesDDSFiles = FrontEndTexturesTPK.GetFiles("*.dds", SearchOption.AllDirectories);

                        // Return to original file before any modifications. (To prevent overlapping and stuff)
                        File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D_orig.ini"), Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D.ini"), true);

                        // Check if required info exists in the config file. If not, add it.
                        var TPKInfoSection = new XNFSTPKToolWrapper.TPKSection();
                        var AnimationSections = new List<XNFSTPKToolWrapper.AnimationSection>();
                        var TextureSections = new List<XNFSTPKToolWrapper.TextureSectionTPKv3>();

                        using (var reader = new StreamReader(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D.ini"))) // sorry, quick and dirty
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                if (line.StartsWith("[TPK]")) // TPK Info Section
                                {
                                    line = reader.ReadLine();
                                    TPKInfoSection.TypeName = line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '=');
                                    /*
                                    line = reader.ReadLine();
                                    TPKInfoSection.TypeVal = Int32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='));
                                    */
                                    line = reader.ReadLine();
                                    TPKInfoSection.Path = line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '=');

                                    line = reader.ReadLine();
                                    TPKInfoSection.Hash = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    TPKInfoSection.Animations = Int32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='));
                                }
                                else if (line.StartsWith("[Anim")) // Any animation section
                                {
                                    var AnimSec = new XNFSTPKToolWrapper.AnimationSection();

                                    line = reader.ReadLine();
                                    AnimSec.Name = line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '=');

                                    line = reader.ReadLine();
                                    AnimSec.Hash = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Frames = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='));

                                    line = reader.ReadLine();
                                    AnimSec.Framerate = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='));

                                    line = reader.ReadLine();
                                    AnimSec.Unknown1 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Unknown2 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Unknown3 = UInt16.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Unknown4 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Unknown5 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    line = reader.ReadLine();
                                    AnimSec.Unknown6 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                    for (int f = 0; f < AnimSec.Frames; f++)
                                    {
                                        line = reader.ReadLine();
                                        AnimSec.FrameList.Add(UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber));
                                    }
                                    AnimationSections.Add(AnimSec);
                                }
                                else if (line.StartsWith("[")) // Any hashed (texture) section
                                {
                                    var TexSec = new XNFSTPKToolWrapper.TextureSectionTPKv3();

                                    if (UInt32.TryParse(line.Trim('[', ']'), NumberStyles.HexNumber, new CultureInfo("en-US", false), out TexSec.Hash))
                                    {
                                        line = reader.ReadLine();
                                        TexSec.File = line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '=');

                                        line = reader.ReadLine();
                                        TexSec.Name = line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '=');

                                        line = reader.ReadLine();
                                        TexSec.Hash2 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                        /*
                                        line = reader.ReadLine();
                                        TexSec.UnkByte1 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.UnkByte2 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.UnkByte3 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                        */
                                        line = reader.ReadLine();
                                        TexSec.Unknown1 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown3 = UInt16.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown4 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown5 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown6 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown7 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown8 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown9 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown10 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown11 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown12 = UInt32.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                        /*
                                        line = reader.ReadLine();
                                        TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);

                                        line = reader.ReadLine();
                                        TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                        */
                                        TextureSections.Add(TexSec);
                                    }
                                }
                            }
                            reader.Close();
                        }

                        foreach (FileInfo DDSFile in FrontEndTexturesDDSFiles) // names in folder
                        {
                            bool IsThereAConfigForThisDDS = false;

                            // Find if the texture exists in the config file (at least once)
                            foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)  // names in ini
                            {
                                if (TexSec.File == (@"FrontEndTextures\8D08770D\" + DDSFile.Name))
                                {
                                    IsThereAConfigForThisDDS = true;
                                    break;
                                }
                            }
                            
                            // If it doesn't, add a new entry into the ini file
                            if (!IsThereAConfigForThisDDS)
                            {
                                var ANewTexSec = new XNFSTPKToolWrapper.TextureSectionTPKv3();

                                ANewTexSec.Hash = (uint)BinHash.Hash(Path.GetFileNameWithoutExtension(DDSFile.Name));
                                ANewTexSec.File = @"FrontEndTextures\8D08770D\" + DDSFile.Name;
                                ANewTexSec.Name = Path.GetFileNameWithoutExtension(DDSFile.Name);
                                ANewTexSec.Hash2 = 0x1A93CF;

                                // Get texture dimensions for some quick maths
                                var DDSFileToGetDimensions = File.Open(DDSFile.FullName, FileMode.Open);
                                var DDSFileReader = new BinaryReader(DDSFileToGetDimensions);

                                DDSFileReader.BaseStream.Position = 0x0C;
                                int VerRes = DDSFileReader.ReadInt32();
                                int HorRes = DDSFileReader.ReadInt32();

                                DDSFileReader.Close();
                                DDSFileToGetDimensions.Close();

                                ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                ANewTexSec.UnkByte3 = 0x00;
                                ANewTexSec.Unknown1 = 0x00;
                                ANewTexSec.Unknown3 = 0x20;
                                ANewTexSec.Unknown4 = 0x500;
                                ANewTexSec.Unknown5 = 0x10200;
                                ANewTexSec.Unknown6 = 0x0;
                                ANewTexSec.Unknown7 = 0x0;
                                ANewTexSec.Unknown8 = 0x1000000;
                                ANewTexSec.Unknown9 = 0x100;
                                ANewTexSec.Unknown10 = 0x00;
                                ANewTexSec.Unknown11 = 0x00;
                                ANewTexSec.Unknown12 = 0x00;
                                ANewTexSec.Unknown17 = 0x00;
                                ANewTexSec.Unknown18 = 0x00;

                                TextureSections.Add(ANewTexSec);
                            }
                                
                        }

                        // Sort Texture Sections by hash
                        TextureSections.Sort((x, y) => x.Hash.CompareTo(y.Hash));

                        // Rebuild the ini file
                        using (var IniWriter = new StreamWriter(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D.ini"), false))
                        {
                            // Write TPK Section
                            IniWriter.WriteLine("[TPK]");
                            IniWriter.WriteLine("TypeName" + " = " + TPKInfoSection.TypeName);
                            //IniWriter.WriteLine("TypeVal" + " = " + TPKInfoSection.TypeVal);
                            IniWriter.WriteLine("Path" + " = " + TPKInfoSection.Path);
                            IniWriter.WriteLine("Hash" + " = " + TPKInfoSection.Hash.ToString("X"));
                            IniWriter.WriteLine("Animations" + " = " + TPKInfoSection.Animations);

                            IniWriter.WriteLine(); // Seperate sections with empty lines

                            // Write Animation Sections
                            foreach (XNFSTPKToolWrapper.AnimationSection AnimSec in AnimationSections)
                            {
                                IniWriter.WriteLine("[Anim" + AnimationSections.IndexOf(AnimSec) + "]");
                                IniWriter.WriteLine("Name" + " = " + AnimSec.Name);
                                IniWriter.WriteLine("Hash" + " = " + AnimSec.Hash.ToString("X"));
                                IniWriter.WriteLine("Frames" + " = " + AnimSec.Frames);
                                IniWriter.WriteLine("Framerate" + " = " + AnimSec.Framerate);
                                IniWriter.WriteLine("Unknown1" + " = " + AnimSec.Unknown1.ToString("X"));
                                IniWriter.WriteLine("Unknown2" + " = " + AnimSec.Unknown2.ToString("X"));
                                IniWriter.WriteLine("Unknown3" + " = " + AnimSec.Unknown3.ToString("X"));
                                IniWriter.WriteLine("Unknown4" + " = " + AnimSec.Unknown4.ToString("X"));
                                IniWriter.WriteLine("Unknown5" + " = " + AnimSec.Unknown5.ToString("X"));
                                IniWriter.WriteLine("Unknown6" + " = " + AnimSec.Unknown6.ToString("X"));

                                foreach (uint FrameHash in AnimSec.FrameList) IniWriter.WriteLine("Frame" + AnimSec.FrameList.IndexOf(FrameHash) + " = " + FrameHash.ToString("X"));

                                IniWriter.WriteLine(); // Seperate sections with empty lines
                            }

                            // Write Texture Sections
                            foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)
                            {
                                IniWriter.WriteLine("[" + TexSec.Hash.ToString("X") + "]");

                                IniWriter.WriteLine("File" + " = " + TexSec.File);
                                IniWriter.WriteLine("Name" + " = " + TexSec.Name);
                                IniWriter.WriteLine("Hash2" + " = " + TexSec.Hash2.ToString("X"));
                                /*
                                IniWriter.WriteLine("UnkByte1" + " = " + TexSec.UnkByte1.ToString("X"));
                                IniWriter.WriteLine("UnkByte2" + " = " + TexSec.UnkByte2.ToString("X"));
                                IniWriter.WriteLine("UnkByte3" + " = " + TexSec.UnkByte3.ToString("X"));
                                IniWriter.WriteLine("Unknown1" + " = " + TexSec.Unknown1.ToString("X"));
                                IniWriter.WriteLine("Unknown3" + " = " + TexSec.Unknown3.ToString("X"));
                                IniWriter.WriteLine("Unknown4" + " = " + TexSec.Unknown4.ToString("X"));
                                IniWriter.WriteLine("Unknown5" + " = " + TexSec.Unknown5.ToString("X"));
                                IniWriter.WriteLine("Unknown6" + " = " + TexSec.Unknown6.ToString("X"));
                                IniWriter.WriteLine("Unknown7" + " = " + TexSec.Unknown7.ToString("X"));
                                IniWriter.WriteLine("Unknown8" + " = " + TexSec.Unknown8.ToString("X"));
                                IniWriter.WriteLine("Unknown9" + " = " + TexSec.Unknown9.ToString("X"));
                                IniWriter.WriteLine("Unknown10" + " = " + TexSec.Unknown10.ToString("X"));
                                IniWriter.WriteLine("Unknown11" + " = " + TexSec.Unknown11.ToString("X"));
                                IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown12.ToString("X"));
                                IniWriter.WriteLine("Unknown17" + " = " + TexSec.Unknown17.ToString("X"));
                                IniWriter.WriteLine("Unknown18" + " = " + TexSec.Unknown18.ToString("X"));
                                */
                                // Temp solution until Xan fixes XNFSTPKTool
                                IniWriter.WriteLine("TextureFlags" + " = " + TexSec.Unknown1.ToString("X"));
                                IniWriter.WriteLine("Unknown1" + " = " + TexSec.Unknown3.ToString("X"));
                                IniWriter.WriteLine("Unknown3" + " = " + TexSec.Unknown4.ToString("X"));
                                IniWriter.WriteLine("Unknown4" + " = " + TexSec.Unknown5.ToString("X"));
                                IniWriter.WriteLine("Unknown5" + " = " + TexSec.Unknown6.ToString("X"));
                                IniWriter.WriteLine("Unknown6" + " = " + TexSec.Unknown7.ToString("X"));
                                IniWriter.WriteLine("Unknown7" + " = " + TexSec.Unknown8.ToString("X"));
                                IniWriter.WriteLine("Unknown8" + " = " + TexSec.Unknown9.ToString("X"));
                                IniWriter.WriteLine("Unknown9" + " = " + TexSec.Unknown10.ToString("X"));
                                IniWriter.WriteLine("Unknown10" + " = " + TexSec.Unknown11.ToString("X"));
                                IniWriter.WriteLine("Unknown11" + " = " + TexSec.Unknown12.ToString("X"));
                                IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown12.ToString("X"));
                                IniWriter.WriteLine(); // Seperate sections with empty lines
                            }

                            IniWriter.Close();
                        }
                        

                        // Finally rebuild the file
                        Process process = new Process();
                        process.StartInfo.FileName = "XNFSTPKTool.exe";
                        process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                        process.StartInfo.Arguments = @"-w FrontEndTextures\8D08770D.ini FrontEndTextures.tpk";
                        process.Start();
                        process.WaitForExit();

                        if (File.Exists(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")))
                        {
                            // merge other chunks in
                            var FrontB1File = File.Create(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontB1.lzc"));
                            var FrontB1FileWriter = new BinaryWriter(FrontB1File);
                            
                            // Write our new TPK
                            FrontB1FileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")));
                            
                            /*// Fix padding
                            PaddingDifference = ((int)FrontB1FileWriter.BaseStream.Position % 128);

                            while (PaddingDifference != 0)
                            {
                                FrontB1FileWriter.Write((byte)0);
                                PaddingDifference = (PaddingDifference + 1) % 128;
                            }

                            // Fix chunk size
                            FrontB1FileWriter.BaseStream.Position = 4;
                            FrontB1FileWriter.Write((int)FrontB1FileWriter.BaseStream.Length - 8);

                            // Go back to original position
                            FrontB1FileWriter.BaseStream.Position = FrontB1FileWriter.BaseStream.Length;
                            */
                            // Write the rest
                            FrontB1FileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTexturesPC.tpk")));
                            FrontB1FileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\BCHUNK_QUICKSPLINE.bin")));

                            // close stream
                            FrontB1FileWriter.Close();
                            FrontB1File.Close();

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if (!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc" + ".edbackup")))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\" + "FrontB1.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc"), true);

                            Log("Successfully rebuilt FrontB1.lzc file.");
                        }
                        else
                        {
                            Log("Cannot rebuild FrontB1.lzc file.");
                        }


                    }

                    else //if (CurrentGame == (int)EdTypes.Game.MostWanted)
                    {
                        MessageBox.Show("Adding cars for this game is not yet supported.", "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log("Adding cars for this game is not yet supported.", true);
                        return;
                    }
                }

                DoneMessage:

                MessageBox.Show("New cars added successfully!.", "Ed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log("New cars added successfully.", true);
                DeleteTempPath();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Make sure you have enough permissions (Try running Ed as administrator.)." + Environment.NewLine + Environment.NewLine + "Exception code:" + Environment.NewLine + ex.ToString(), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("ERROR! Ed was unable to add cars into the game.");
                Log("Exception: " + ex.ToString());
            }
        }

        private void keepTemporaryFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuItemTempFiles.Checked = !MenuItemTempFiles.Checked;
            KeepTempFiles = MenuItemTempFiles.Checked;
        }

        private void aboutEdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var AboutWindow = new AboutBoxEd();
            AboutWindow.Show();
        }

        private void menuUnlockFiles_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Global"));
            FileInfo[] MemoryFiles = di.GetFiles("*MemoryFile.bin", SearchOption.TopDirectoryOnly);

            foreach (FileInfo MemFile in MemoryFiles)
            {
                if (File.Exists(Path.Combine(WorkingFolder, @"Global", MemFile.Name))) File.Copy(Path.Combine(WorkingFolder, @"Global", MemFile.Name), Path.Combine(WorkingFolder, @"Global", MemFile.Name + ".edbackup"), true);
                File.Copy(MemFile.FullName, Path.Combine(WorkingFolder, @"Global", MemFile.Name), true);
            }

            MessageBox.Show("Successfully unlocked game files for modding.");
            Log("Successfully unlocked game files for modding.", true);
        }
    }
}
