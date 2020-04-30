using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

        bool DisableLogs;
        bool DisableBackups;
        bool KeepTempFiles;
        bool AutoRestoreGlobalB;
        int CurrentGame;
        string WorkingFolder = "";

        public void GetWhichGame(string _GamePath)
        {
            if (File.Exists(Path.Combine(_GamePath, "Tracks", "StreamL2RA.bun"))) CurrentGame = (int)EdTypes.Game.MostWanted;
            else if (File.Exists(Path.Combine(_GamePath, "Tracks", "StreamL5RA.bun"))) CurrentGame = (int)EdTypes.Game.Carbon;
            else if (File.Exists(Path.Combine(_GamePath, "Tracks", "STREAML6R_FE.bun"))) CurrentGame = (int)EdTypes.Game.ProStreet;
            else if (File.Exists(Path.Combine(_GamePath, "Tracks", "STREAML8R_MW2.bun"))) CurrentGame = (int)EdTypes.Game.Undercover;
            //else if (File.Exists(Path.Combine(_GamePath, "Tracks", "STREAML1RA.bun"))) CurrentGame = (int)EdTypes.Game.Underground;
            else if (File.Exists(Path.Combine(_GamePath, "Tracks", "STREAML4RA.bun"))) CurrentGame = (int)EdTypes.Game.Underground2;
            
            if (CurrentGame == 0)
            {
                MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "The directory doesn't contain any game executables.");
                Log("ERROR! Ed was unable to detect any game installations in this directory.");
                Log("The directory doesn't contain any game executables.", true);
                ToggleButtons(false);
            }
            else
            {
                RefreshConfigView();
                ToggleButtons(true);

                switch (CurrentGame)
                {
                    /*case (int)EdTypes.Game.Underground:
                        Log("NFS Underground detected.", true);
                        break;*/
                    case (int)EdTypes.Game.Underground2:
                        Log("NFS Underground 2 detected.", true);
                        break;
                    case (int)EdTypes.Game.MostWanted:
                        Log("NFS Most Wanted (2005) detected.", true);
                        break;
                    case (int)EdTypes.Game.Carbon:
                        Log("NFS Carbon detected.", true);
                        break;
                    case (int)EdTypes.Game.ProStreet:
                        Log("NFS ProStreet detected.", true);
                        break;
                    case (int)EdTypes.Game.Undercover:
                        Log("NFS Undercover detected.", true);
                        break;
                }
            }
        }

        private void SetTitleText()
        {
            string[] VersionInfo = ProductVersion.Split('.');
            string VersionText = ProductName + " - " + "The Car Dealer!" + " | " + "v" + ProductVersion;
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

                LogFile.WriteLine("# Ed " + "v" + ProductVersion + " Log File");
                LogFile.WriteLine("# File created on: " + DateTime.Now.ToString());
                LogFile.WriteLine("# ------------------------------------------------------------------------------");
                LogFile.Close();
                LogFile.Dispose();
                LogStream.Dispose();
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
                LogFile.Dispose();
                LogStream.Dispose();
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
                case (int)EdTypes.Game.Undercover:
                    ConfigPath = @"Config\NFSUC";
                    break;
                case (int)EdTypes.Game.ProStreet:
                    ConfigPath = @"Config\NFSPS";
                    break;
                case (int)EdTypes.Game.Carbon:
                    ConfigPath = @"Config\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ConfigPath = @"Config\NFSMW";
                    break;
                case (int)EdTypes.Game.Underground2:
                    ConfigPath = @"Config\NFSU2";
                    break;
                case (int)EdTypes.Game.Underground:
                    ConfigPath = @"Config\NFSU";
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
                case (int)EdTypes.Game.Undercover:
                    ResourcesPath = @"Resources\NFSUC";
                    break;
                case (int)EdTypes.Game.ProStreet:
                    ResourcesPath = @"Resources\NFSPS";
                    break;
                case (int)EdTypes.Game.Carbon:
                    ResourcesPath = @"Resources\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ResourcesPath = @"Resources\NFSMW";
                    break;
                case (int)EdTypes.Game.Underground2:
                    ResourcesPath = @"Resources\NFSU2";
                    break;
                case (int)EdTypes.Game.Underground:
                    ResourcesPath = @"Resources\NFSU";
                    break;
            }

            bool exists = Directory.Exists(ResourcesPath);

            if (!exists)
                Directory.CreateDirectory(ResourcesPath);
            
            return ResourcesPath;
        }

        public string GetBoundsPath()
        {
            string ResourcesPath = "";

            switch (CurrentGame)
            {
                case (int)EdTypes.Game.Undercover:
                    ResourcesPath = @"Resources\NFSUC\Global\BCHUNK_BOUNDS";
                    break;
                case (int)EdTypes.Game.ProStreet:
                    ResourcesPath = @"Resources\NFSPS\Global\BCHUNK_BOUNDS";
                    break;
                case (int)EdTypes.Game.Carbon:
                    ResourcesPath = @"Resources\NFSC\Global\BCHUNK_BOUNDS";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ResourcesPath = @"Resources\NFSMW\Global\BCHUNK_BOUNDS";
                    break;
            }

            bool exists = Directory.Exists(ResourcesPath);

            if (!exists)
                Directory.CreateDirectory(ResourcesPath);

            return ResourcesPath;
        }

        public string GetFrontEndTexturesPath()
        {
            string ResourcesPath = "";

            switch (CurrentGame)
            {
                case (int)EdTypes.Game.Undercover:
                    ResourcesPath = @"Resources\NFSUC\FrontEnd\LogoTextures\D78D2BB1";
                    break;
                case (int)EdTypes.Game.ProStreet:
                    ResourcesPath = @"Resources\NFSPS\FrontEnd\FrontEndTextures\B66CD660";
                    break;
                case (int)EdTypes.Game.Carbon:
                    ResourcesPath = @"Resources\NFSC\FrontEnd\FrontEndTextures\8D08770D";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    ResourcesPath = @"Resources\NFSMW\FrontEnd\FrontEndTextures\729181AD";
                    break;
                case (int)EdTypes.Game.Underground2:
                    ResourcesPath = @"Resources\NFSU2\FrontEnd\FrontEndTextures\729181AD";
                    break;
                case (int)EdTypes.Game.Underground:
                    ResourcesPath = @"Resources\NFSU\FrontEnd\FrontEndTextures\729181AD";
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
                case (int)EdTypes.Game.Undercover:
                    TempPath = @"Temp\NFSUC";
                    break;
                case (int)EdTypes.Game.ProStreet:
                    TempPath = @"Temp\NFSPS";
                    break;
                case (int)EdTypes.Game.Carbon:
                    TempPath = @"Temp\NFSC";
                    break;
                case (int)EdTypes.Game.MostWanted:
                    TempPath = @"Temp\NFSMW";
                    break;
                case (int)EdTypes.Game.Underground2:
                    TempPath = @"Temp\NFSU2";
                    break;
                case (int)EdTypes.Game.Underground:
                    TempPath = @"Temp\NFSU";
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
                            ConfigCar.Checked = true;

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
            ButtonAddCars.Enabled = _IsEnabled;
        }

        public static float ToSingle(double value)
        {
            return (float)value;
        }
        
        public int GetCarTypeIDFromResources(string _XName)
        {
            int CarTypeID = -1;
            int i;
            int CarBlockSize = 0xD0;

            if (CurrentGame == (int)EdTypes.Game.Undercover) CarBlockSize = 0x130;
            if (CurrentGame == (int)EdTypes.Game.Underground) CarBlockSize = 0xC90;
            if (CurrentGame == (int)EdTypes.Game.Underground2) CarBlockSize = 0x890;

            if (_XName.IndexOf('\0') != -1) _XName = _XName.Substring(0, _XName.IndexOf('\0')); // Fix nulls at the end

            try
            {
                var CarInfoArrayFile = new FileStream(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), FileMode.Open, FileAccess.Read);
                var CarInfoArrayFileReader = new BinaryReader(CarInfoArrayFile);


                if (CarInfoArrayFileReader.ReadUInt32() == 0x00034600)
                {
                    int CarInfoCount = (CarInfoArrayFileReader.ReadInt32() - 8) / CarBlockSize;
                    CarTypeID = CarInfoCount;

                    for (i = 0; i< CarInfoCount; i++)
                    {
                        CarInfoArrayFileReader.BaseStream.Position = 16 + (i * CarBlockSize);

                        string XName = Encoding.ASCII.GetString(CarInfoArrayFileReader.ReadBytes(16));
                        if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end
                        
                        if (XName == _XName)
                        {
                            CarTypeID = i;
                            break;
                        }
                    }
                }

                CarInfoArrayFileReader.Close();
                CarInfoArrayFileReader.Dispose();
                CarInfoArrayFile.Close();
                CarInfoArrayFile.Dispose();
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

            if (_XName.IndexOf('\0') != -1) _XName = _XName.Substring(0, _XName.IndexOf('\0')); // Fix nulls at the end

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
                CarInfoCarPart5FileReader.Dispose();
                CarInfoCarPart5File.Close();
                CarInfoCarPart5File.Dispose();
            }
            catch (Exception ex)
            {
                Log("ERROR! Ed was unable to read a resource file.");
                Log("Exception: " + ex.ToString());
            }

            return CarPartID;

        }

        public static string ReadStringZ(BinaryReader reader)
        {
            StringBuilder result = new StringBuilder();
            char c;
            for (int i = 0; i < reader.BaseStream.Length; i++)
            {
                if ((c = (char)reader.ReadByte()) == 0)
                {
                    break;
                }
                result.Append(c.ToString());
            }
            return result.ToString();
        }

        private void ButtonBrowseConfigFolder_Click(object sender, EventArgs e)
        {
            string ConfigPath = GetConfigPath();
            Process.Start("explorer", ConfigPath);
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
            CreateLogFile();

            // Load settings
            DisableLogs = !Ed.Default.DoLogs;
            MenuItemLog.Checked = Ed.Default.DoLogs;

            DisableBackups = !Ed.Default.DoBackups;
            MenuItemBackup.Checked = Ed.Default.DoBackups;

            KeepTempFiles = Ed.Default.KeepTemporaryFiles;
            MenuItemTempFiles.Checked = KeepTempFiles;

            AutoRestoreGlobalB = Ed.Default.AutoRestoreGlobalB;
            MenuItemRestoreGlobalB.Checked = AutoRestoreGlobalB;

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
            if (DialogWorkingFolder.ShowDialog() == DialogResult.OK)
            {
                LabelWorkingDir.Text = DialogWorkingFolder.SelectedPath;
                WorkingFolder = DialogWorkingFolder.SelectedPath;

                try
                {
                    GetWhichGame(WorkingFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ed was unable to detect any game installations in this directory." + Environment.NewLine + "Make sure you have enough permissions (Try running Ed as administrator.)." + Environment.NewLine + Environment.NewLine + "Exception code:" + Environment.NewLine + ex.ToString(), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log("ERROR! Ed was unable to detect any game installations in this directory.");
                    ToggleButtons(false);
                    Log("Exception: " + ex.ToString());
                }
            }
        }

        private void MenuItemLog_Click(object sender, EventArgs e)
        {
            MenuItemLog.Checked = !MenuItemLog.Checked;
            DisableLogs = !MenuItemLog.Checked;

            Ed.Default.DoLogs = MenuItemLog.Checked;
            Ed.Default.Save();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            MenuItemBackup.Checked = !MenuItemBackup.Checked;
            DisableBackups = !MenuItemBackup.Checked;

            Ed.Default.DoBackups = MenuItemBackup.Checked;
            Ed.Default.Save();
        }

        private void MenuItemBrowseResources_Click(object sender, EventArgs e)
        {
            string Resources = GetResourcesPath();
            Process.Start("explorer", Resources);
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
                Process.Start(Path.Combine(Config, item.Text + ".ini"));
            }
        }

        private void addCarsFromConfigFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ConfigPath = GetConfigPath();

            if (CurrentGame != 0) try
            {
                List<FileInfo> INIFiles = new List<FileInfo>();

                foreach (ListViewItem lwi in ListCarsToAdd.Items)
                {
                    if (lwi.Checked) INIFiles.Add(new FileInfo(Path.Combine(GetConfigPath(), lwi.Text + ".ini")));
                }

                int AddedCarCount = 0;

                if (INIFiles.Count == 0)
                {
                    MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "The directory doesn't contain any config files.");
                    Log("ERROR! Ed was unable to add cars into the game.");
                    Log("The directory doesn't contain any config files.", true);
                    return;
                }
                else
                {
                    var NewCarTypeInfoArray = new List<EdTypes.CarTypeInfo>();
                    var NewSlotTypeOverrides = new List<EdTypes.SlotType>();
                    var NewCollisionList = new List<EdTypes.CarCollision>();
                    var NewResourcesList = new List<EdTypes.CarResource>();

                    foreach (FileInfo i in INIFiles)
                    {
                        string INIFilePath = i.FullName;

                        if (i.Name == "config.ini")
                        {
                            MessageBox.Show("Config files from old versions of Texture Compiler aren't supported.", "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("WARNING! Config files from old versions of Texture Compiler aren't supported.");
                        }
                        else
                        {
                            var Car = new EdTypes.CarTypeInfo();
                            var SpoilerType = new EdTypes.SlotType();
                            var RoofScoopType = new EdTypes.SlotType();
                            var MirrorType = new EdTypes.SlotType();
                            var Collision = new EdTypes.CarCollision();
                            

                            var IniReader = new IniFile(INIFilePath);

                            string XName = Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false));

                            if (CurrentGame == (int)EdTypes.Game.Undercover)
                            {
                                if (XName.Length > 14)
                                {
                                    MessageBox.Show("Car names cannot be longer than 14 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Log("Car names cannot be longer than 14 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)));
                                    continue;
                                }
                            }
                            else
                            {
                                if (XName.Length > 13)
                                {
                                    MessageBox.Show("Car names cannot be longer than 13 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Log("Car names cannot be longer than 13 characters. Skipping " + Path.GetFileNameWithoutExtension(INIFilePath).ToUpper(new CultureInfo("en-US", false)));
                                    continue;
                                }
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
                            Car.GeometryLZCFilename = Encoding.ASCII.GetBytes("CARS\\" + XName + "\\GEOMETRY.LZC");
                            Car.ManufacturerName = Encoding.ASCII.GetBytes(IniReader.GetValue("INFO", "Manufacturer"));

                            // Do some byte array magic to avoid squishing
                            Array.Resize(ref Car.CarTypeName, 16);
                            Array.Resize(ref Car.BaseModelName, 16);
                            Array.Resize(ref Car.GeometryFilename, 32);
                            Array.Resize(ref Car.GeometryLZCFilename, 32);
                            Array.Resize(ref Car.ManufacturerName, 16);

                            // Continue reading
                            
                            if (CurrentGame == (int)EdTypes.Game.ProStreet || CurrentGame == (int)EdTypes.Game.Undercover)
                            {
                                Car.CarTypeNameHash = (uint)BinHash.Hash(XName);
                                Car.HeadlightFOV = ToSingle(IniReader.GetDouble("INFO", "HeadlightFOV", 0.0f));
                                Car.padHighPerformance = (byte)IniReader.GetInteger("INFO", "padHighPerformance", 0, byte.MinValue, byte.MaxValue);
                                Car.NumAvailableSkinNumbers = (byte)IniReader.GetInteger("INFO", "NumAvailableSkinNumbers", 0, byte.MinValue, byte.MaxValue);
                                Car.WhatGame = (byte)IniReader.GetInteger("INFO", "WhatGame", 0xFF, byte.MinValue, byte.MaxValue);
                                Car.ConvertableFlag = (byte)IniReader.GetInteger("INFO", "ConvertableFlag", 0, byte.MinValue, byte.MaxValue);
                                Car.WheelOuterRadius = (byte)IniReader.GetInteger("INFO", "WheelOuterRadius", 0, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMin = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMin", 0, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMax = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMax", 0, byte.MinValue, byte.MaxValue);
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
                            }

                            if (CurrentGame == (int)EdTypes.Game.Carbon)
                        {
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
                        }

                            if (CurrentGame == (int)EdTypes.Game.MostWanted)
                            {
                                Car.CarTypeNameHash = (uint)BinHash.Hash(XName);
                                Car.HeadlightFOV = ToSingle(IniReader.GetDouble("INFO", "HeadlightFOV", 1.92f));
                                Car.padHighPerformance = (byte)IniReader.GetInteger("INFO", "padHighPerformance", 0, byte.MinValue, byte.MaxValue);
                                Car.NumAvailableSkinNumbers = (byte)IniReader.GetInteger("INFO", "NumAvailableSkinNumbers", 0, byte.MinValue, byte.MaxValue);
                                Car.WhatGame = (byte)IniReader.GetInteger("INFO", "WhatGame", 1, byte.MinValue, byte.MaxValue);
                                Car.ConvertableFlag = (byte)IniReader.GetInteger("INFO", "ConvertableFlag", 0, byte.MinValue, byte.MaxValue);
                                Car.WheelOuterRadius = (byte)IniReader.GetInteger("INFO", "WheelOuterRadius", 26, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMin = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMin", 17, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMax = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMax", 20, byte.MinValue, byte.MaxValue);
                                Car.pad0 = (byte)IniReader.GetInteger("INFO", "pad0", 0, byte.MinValue, byte.MaxValue);
                                Car.HeadlightPosition.x = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionX", 1.28f));
                                Car.HeadlightPosition.y = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionY", 0.0f));
                                Car.HeadlightPosition.z = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionZ", 0.32f));
                                Car.HeadlightPosition.pad = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionPad", 0.0f));
                                Car.DriverRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetX", -0.242f));
                                Car.DriverRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetY", 0.405f));
                                Car.DriverRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetZ", -0.04f));
                                Car.DriverRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetPad", 0.0f));
                                Car.InCarSteeringWheelRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetX", 0.503f));
                                Car.InCarSteeringWheelRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetY", 0.0f));
                                Car.InCarSteeringWheelRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetZ", 0.605f));
                                Car.InCarSteeringWheelRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetPad", 0.0f));
                            }

                            if (CurrentGame == (int)EdTypes.Game.Underground2)
                            {
                                Car.CarTypeNameHash = (uint)BinHash.Hash(XName);
                                Car.HeadlightFOV = ToSingle(IniReader.GetDouble("INFO", "HeadlightFOV", 1.92f));
                                Car.padHighPerformance = (byte)IniReader.GetInteger("INFO", "padHighPerformance", 0, byte.MinValue, byte.MaxValue);
                                Car.NumAvailableSkinNumbers = (byte)IniReader.GetInteger("INFO", "NumAvailableSkinNumbers", 1, byte.MinValue, byte.MaxValue);
                                Car.WhatGame = (byte)IniReader.GetInteger("INFO", "WhatGame", 2, byte.MinValue, byte.MaxValue);
                                Car.ConvertableFlag = (byte)IniReader.GetInteger("INFO", "ConvertableFlag", 0, byte.MinValue, byte.MaxValue);
                                Car.WheelOuterRadius = (byte)IniReader.GetInteger("INFO", "WheelOuterRadius", 24, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMin = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMin", 16, byte.MinValue, byte.MaxValue);
                                Car.WheelInnerRadiusMax = (byte)IniReader.GetInteger("INFO", "WheelInnerRadiusMax", 18, byte.MinValue, byte.MaxValue);
                                Car.pad0 = (byte)IniReader.GetInteger("INFO", "pad0", 0, byte.MinValue, byte.MaxValue);
                                Car.HeadlightPosition.x = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionX", 1.28f));
                                Car.HeadlightPosition.y = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionY", 0.0f));
                                Car.HeadlightPosition.z = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionZ", 0.32f));
                                Car.HeadlightPosition.pad = ToSingle(IniReader.GetDouble("INFO", "HeadlightPositionPad", 0.0f));
                                Car.DriverRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetX", -0.242f));
                                Car.DriverRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetY", 0.405f));
                                Car.DriverRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetZ", -0.04f));
                                Car.DriverRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "DriverRenderingOffsetPad", 0.0f));
                                Car.InCarSteeringWheelRenderingOffset.x = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetX", 0.503f));
                                Car.InCarSteeringWheelRenderingOffset.y = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetY", 0.0f));
                                Car.InCarSteeringWheelRenderingOffset.z = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetZ", 0.605f));
                                Car.InCarSteeringWheelRenderingOffset.pad = ToSingle(IniReader.GetDouble("INFO", "InCarSteeringWheelRenderingOffsetPad", 0.0f));
                                Car.Unknown1.x = ToSingle(IniReader.GetDouble("INFO", "Unknown1X", 0.19f));
                                Car.Unknown1.y = ToSingle(IniReader.GetDouble("INFO", "Unknown1Y", 0.0f));
                                Car.Unknown1.z = ToSingle(IniReader.GetDouble("INFO", "Unknown1Z", 0.27f));
                                Car.Unknown1.pad = ToSingle(IniReader.GetDouble("INFO", "Unknown1Pad", 0.0f));

                                    Car.CopyFrom = IniReader.GetValue("INFO", "CopyFrom", "FOCUS");

                                    Car.WheelFrontLeftX = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftX", 1.18f));
                                    Car.WheelFrontLeftSprings = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftSprings", 0.747f));
                                    Car.WheelFrontLeftRideHeight = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftRideHeight", 0.09754f));
                                    Car.WheelFrontLeftUnk3 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftUnk3", 0.0f));
                                    Car.WheelFrontLeftDiameter = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftDiameter", 0.3075f));
                                    Car.WheelFrontLeftTireSkidWidth = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftTireSkidWidth", 0.195f));
                                    Car.WheelFrontLeftID = IniReader.GetInteger("INFO", "WheelFrontLeftID", 0);
                                    Car.WheelFrontLeftY = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftY", 0.83f));
                                    Car.WheelFrontLeftWideBodyY = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftWideBodyY", 0.93f));
                                    Car.WheelFrontLeftUnk5 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftUnk5", 0.0f));
                                    Car.WheelFrontLeftUnk6 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftUnk6", 0.0f));
                                    Car.WheelFrontLeftUnk7 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontLeftUnk7", 0.0f));
                                    Car.WheelFrontRightX = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightX", 1.18f));
                                    Car.WheelFrontRightSprings = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightSprings", -0.747f));
                                    Car.WheelFrontRightRideHeight = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightRideHeight", 0.09754f));
                                    Car.WheelFrontRightUnk3 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightUnk3", 0.0f));
                                    Car.WheelFrontRightDiameter = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightDiameter", 0.3075f));
                                    Car.WheelFrontRightTireSkidWidth = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightTireSkidWidth", 0.195f));
                                    Car.WheelFrontRightID = IniReader.GetInteger("INFO", "WheelFrontRightID", 1);
                                    Car.WheelFrontRightY = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightY", -0.83f));
                                    Car.WheelFrontRightWideBodyY = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightWideBodyY", -0.93f));
                                    Car.WheelFrontRightUnk5 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightUnk5", 0.0f));
                                    Car.WheelFrontRightUnk6 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightUnk6", 0.0f));
                                    Car.WheelFrontRightUnk7 = ToSingle(IniReader.GetDouble("INFO", "WheelFrontRightUnk7", 0.0f));
                                    Car.WheelRearRightX = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightX", -1.44f));
                                    Car.WheelRearRightSprings = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightSprings", -0.743f));
                                    Car.WheelRearRightRideHeight = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightRideHeight", 0.09754f));
                                    Car.WheelRearRightUnk3 = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightUnk3", 0.0f));
                                    Car.WheelRearRightDiameter = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightDiameter", 0.3075f));
                                    Car.WheelRearRightTireSkidWidth = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightTireSkidWidth", 0.195f));
                                    Car.WheelRearRightID = IniReader.GetInteger("INFO", "WheelRearRightID", 2);
                                    Car.WheelRearRightY = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightY", -0.83f));
                                    Car.WheelRearRightWideBodyY = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightWideBodyY", -0.93f));
                                    Car.WheelRearRightUnk5 = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightUnk5", 0.0f));
                                    Car.WheelRearRightUnk6 = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightUnk6", 0.0f));
                                    Car.WheelRearRightUnk7 = ToSingle(IniReader.GetDouble("INFO", "WheelRearRightUnk7", 0.0f));
                                    Car.WheelRearLeftX = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftX", -1.44f));
                                    Car.WheelRearLeftSprings = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftSprings", 0.743f));
                                    Car.WheelRearLeftRideHeight = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftRideHeight", 0.09754f));
                                    Car.WheelRearLeftUnk3 = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftUnk3", 0.0f));
                                    Car.WheelRearLeftDiameter = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftDiameter", 0.3075f));
                                    Car.WheelRearLeftTireSkidWidth = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftTireSkidWidth", 0.195f));
                                    Car.WheelRearLeftID = IniReader.GetInteger("INFO", "WheelRearLeftID", 3);
                                    Car.WheelRearLeftY = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftY", 0.83f));
                                    Car.WheelRearLeftWideBodyY = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftWideBodyY", 0.93f));
                                    Car.WheelRearLeftUnk5 = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftUnk5", 0.0f));
                                    Car.WheelRearLeftUnk6 = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftUnk6", 0.0f));
                                    Car.WheelRearLeftUnk7 = ToSingle(IniReader.GetDouble("INFO", "WheelRearLeftUnk7", 0.0f));
                                    Car.SuspensionUnk1F = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk1F", 5.0f));
                                    Car.FrontSprings = ToSingle(IniReader.GetDouble("INFO", "FrontSprings", 37.0f));
                                    Car.FrontShocks1 = ToSingle(IniReader.GetDouble("INFO", "FrontShocks1", 5.0f));
                                    Car.FrontShocks2 = ToSingle(IniReader.GetDouble("INFO", "FrontShocks2", 5.9f));
                                    Car.FrontSwayBar = ToSingle(IniReader.GetDouble("INFO", "FrontSwayBar", 37.0f));
                                    Car.SuspensionUnk2F = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk2F", 0.15f));
                                    Car.SuspensionUnk3F = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk3F", -0.15f));
                                    Car.SuspensionUnk4F = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk4F", 0.15f));
                                    Car.SuspensionUnk1R = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk1R", 5.0f));
                                    Car.RearSprings = ToSingle(IniReader.GetDouble("INFO", "RearSprings", 38.5f));
                                    Car.RearShocks1 = ToSingle(IniReader.GetDouble("INFO", "RearShocks1", 5.65f));
                                    Car.RearShocks2 = ToSingle(IniReader.GetDouble("INFO", "RearShocks2", 5.1f));
                                    Car.RearSwayBar = ToSingle(IniReader.GetDouble("INFO", "RearSwayBar", 37.0f));
                                    Car.SuspensionUnk2R = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk2R", 0.15f));
                                    Car.SuspensionUnk3R = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk3R", -0.15f));
                                    Car.SuspensionUnk4R = ToSingle(IniReader.GetDouble("INFO", "SuspensionUnk4R", 0.1f));
                                    Car.Unknown0pt8 = ToSingle(IniReader.GetDouble("INFO", "Unknown0pt8", 0.8f));
                                    Car.Unknown500 = ToSingle(IniReader.GetDouble("INFO", "Unknown500", 500.0f));
                                    Car.FinalDriveRatio = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio", 3.82f));
                                    Car.FinalDriveRatio2 = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio2", Car.FinalDriveRatio));
                                    Car.NumberOfGears = IniReader.GetInteger("INFO", "NumberOfGears", 5);
                                    Car.GearRatioR = ToSingle(IniReader.GetDouble("INFO", "GearRatioR", -3.72f));
                                    Car.GearRatioN = ToSingle(IniReader.GetDouble("INFO", "GearRatioN", 0.0f));
                                    Car.GearRatio1 = ToSingle(IniReader.GetDouble("INFO", "GearRatio1", 3.67f));
                                    Car.GearRatio2 = ToSingle(IniReader.GetDouble("INFO", "GearRatio2", 2.13f));
                                    Car.GearRatio3 = ToSingle(IniReader.GetDouble("INFO", "GearRatio3", 1.45f));
                                    Car.GearRatio4 = ToSingle(IniReader.GetDouble("INFO", "GearRatio4", 1.02f));
                                    Car.GearRatio5 = ToSingle(IniReader.GetDouble("INFO", "GearRatio5", 0.85f));
                                    Car.GearRatio6 = ToSingle(IniReader.GetDouble("INFO", "GearRatio6", 0.0f));
                                    Car.FinalDriveRatio_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio_StreetPackage", 3.745f));
                                    Car.FinalDriveRatio2_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio2_StreetPackage", Car.FinalDriveRatio_StreetPackage));
                                    Car.NumberOfGears_StreetPackage = IniReader.GetInteger("INFO", "NumberOfGears_StreetPackage", 5);
                                    Car.GearRatioR_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioR_StreetPackage", -3.72f));
                                    Car.GearRatioN_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioN_StreetPackage", 0.0f));
                                    Car.GearRatio1_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio1_StreetPackage", 3.67f));
                                    Car.GearRatio2_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio2_StreetPackage", 2.13f));
                                    Car.GearRatio3_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio3_StreetPackage", 1.45f));
                                    Car.GearRatio4_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio4_StreetPackage", 1.03f));
                                    Car.GearRatio5_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio5_StreetPackage", 0.82f));
                                    Car.GearRatio6_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio6_StreetPackage", 0.0f));
                                    Car.FinalDriveRatio_ProPackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio_ProPackage", 3.6726f));
                                    Car.FinalDriveRatio2_ProPackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio2_ProPackage", Car.FinalDriveRatio_ProPackage));
                                    Car.NumberOfGears_ProPackage = IniReader.GetInteger("INFO", "NumberOfGears_ProPackage", 5);
                                    Car.GearRatioR_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioR_ProPackage", -3.72f));
                                    Car.GearRatioN_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioN_ProPackage", 0.0f));
                                    Car.GearRatio1_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio1_ProPackage", 3.67f));
                                    Car.GearRatio2_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio2_ProPackage", 2.13f));
                                    Car.GearRatio3_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio3_ProPackage", 1.45f));
                                    Car.GearRatio4_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio4_ProPackage", 1.03f));
                                    Car.GearRatio5_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio5_ProPackage", 0.80f));
                                    Car.GearRatio6_ProPackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio6_ProPackage", 0.0f));
                                    Car.FinalDriveRatio_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio_ExtremePackage", 3.6f));
                                    Car.FinalDriveRatio2_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "FinalDriveRatio2_ExtremePackage", Car.FinalDriveRatio_ExtremePackage));
                                    Car.NumberOfGears_ExtremePackage = IniReader.GetInteger("INFO", "NumberOfGears_ExtremePackage", 6);
                                    Car.GearRatioR_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioR_ExtremePackage", -3.72f));
                                    Car.GearRatioN_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatioN_ExtremePackage", 0.0f));
                                    Car.GearRatio1_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio1_ExtremePackage", 3.67f));
                                    Car.GearRatio2_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio2_ExtremePackage", 2.13f));
                                    Car.GearRatio3_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio3_ExtremePackage", 1.45f));
                                    Car.GearRatio4_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio4_ExtremePackage", 1.10f));
                                    Car.GearRatio5_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio5_ExtremePackage", 0.94f));
                                    Car.GearRatio6_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "GearRatio6_ExtremePackage", 0.8f));
                                    Car.IdleRPM = ToSingle(IniReader.GetDouble("INFO", "IdleRPM", 850.0f));
                                    Car.RedLineRPM = ToSingle(IniReader.GetDouble("INFO", "RedLineRPM", 6500.0f));
                                    Car.MAXRPM = ToSingle(IniReader.GetDouble("INFO", "MaxRPM", 7000.0f));
                                    Car.ECUx1000 = ToSingle(IniReader.GetDouble("INFO", "ECUx1000", 0.12f));
                                    Car.ECUx2000 = ToSingle(IniReader.GetDouble("INFO", "ECUx2000", 0.13f));
                                    Car.ECUx3000 = ToSingle(IniReader.GetDouble("INFO", "ECUx3000", 0.14f));
                                    Car.ECUx4000 = ToSingle(IniReader.GetDouble("INFO", "ECUx4000", 0.155f));
                                    Car.ECUx5000 = ToSingle(IniReader.GetDouble("INFO", "ECUx5000", 0.17f));
                                    Car.ECUx6000 = ToSingle(IniReader.GetDouble("INFO", "ECUx6000", 0.182f));
                                    Car.ECUx7000 = ToSingle(IniReader.GetDouble("INFO", "ECUx7000", 0.17f));
                                    Car.ECUx8000 = ToSingle(IniReader.GetDouble("INFO", "ECUx8000", 0.145f));
                                    Car.ECUx9000 = ToSingle(IniReader.GetDouble("INFO", "ECUx9000", 0.125f));
                                    Car.ECUx10000 = ToSingle(IniReader.GetDouble("INFO", "ECUx10000", 0.0f));
                                    Car.ECUx11000 = ToSingle(IniReader.GetDouble("INFO", "ECUx11000", 0.0f));
                                    Car.ECUx12000 = ToSingle(IniReader.GetDouble("INFO", "ECUx12000", 0.0f));
                                    Car.IdleRPMAdd_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "IdleRPMAdd_StreetPackage", 0.0f));
                                    Car.RedLineRPMAdd_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "RedLineRPMAdd_StreetPackage", 500.0f));
                                    Car.MAXRPMAdd_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "MaxRPMAdd_StreetPackage", 500.0f));
                                    Car.ECUx1000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx1000Add_StreetPackage", 0.01224f));
                                    Car.ECUx2000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx2000Add_StreetPackage", 0.01326f));
                                    Car.ECUx3000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx3000Add_StreetPackage", 0.01428f));
                                    Car.ECUx4000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx4000Add_StreetPackage", 0.01581f));
                                    Car.ECUx5000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx5000Add_StreetPackage", 0.01734f));
                                    Car.ECUx6000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx6000Add_StreetPackage", 0.018564f));
                                    Car.ECUx7000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx7000Add_StreetPackage", 0.01734f));
                                    Car.ECUx8000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx8000Add_StreetPackage", 0.01479f));
                                    Car.ECUx9000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx9000Add_StreetPackage", 0.01275f));
                                    Car.ECUx10000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx10000Add_StreetPackage", 0.01326f));
                                    Car.ECUx11000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx11000Add_StreetPackage", 0.0119f));
                                    Car.ECUx12000Add_StreetPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx12000Add_StreetPackage", 0.01275f));
                                    Car.IdleRPMAdd_ProPackage = ToSingle(IniReader.GetDouble("INFO", "IdleRPMAdd_ProPackage", 0.0f));
                                    Car.RedLineRPMAdd_ProPackage = ToSingle(IniReader.GetDouble("INFO", "RedLineRPMAdd_ProPackage", 1000.0f));
                                    Car.MAXRPMAdd_ProPackage = ToSingle(IniReader.GetDouble("INFO", "MaxRPMAdd_ProPackage", 1000.0f));
                                    Car.ECUx1000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx1000Add_ProPackage", 0.02448f));
                                    Car.ECUx2000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx2000Add_ProPackage", 0.02652f));
                                    Car.ECUx3000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx3000Add_ProPackage", 0.02856f));
                                    Car.ECUx4000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx4000Add_ProPackage", 0.03162f));
                                    Car.ECUx5000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx5000Add_ProPackage", 0.03468f));
                                    Car.ECUx6000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx6000Add_ProPackage", 0.037128f));
                                    Car.ECUx7000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx7000Add_ProPackage", 0.03468f));
                                    Car.ECUx8000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx8000Add_ProPackage", 0.02958f));
                                    Car.ECUx9000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx9000Add_ProPackage", 0.0255f));
                                    Car.ECUx10000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx10000Add_ProPackage", 0.02652f));
                                    Car.ECUx11000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx11000Add_ProPackage", 0.0238f));
                                    Car.ECUx12000Add_ProPackage = ToSingle(IniReader.GetDouble("INFO", "ECUx12000Add_ProPackage", 0.0255f));
                                    Car.IdleRPMAdd_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "IdleRPMAdd_ExtremePackage", 0.0f));
                                    Car.RedLineRPMAdd_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "RedLineRPMAdd_ExtremePackage", 1500.0f));
                                    Car.MAXRPMAdd_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "MaxRPMAdd_ExtremePackage", 1500.0f));
                                    Car.ECUx1000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx1000Add_ExtremePackage", 0.036f));
                                    Car.ECUx2000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx2000Add_ExtremePackage", 0.039f));
                                    Car.ECUx3000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx3000Add_ExtremePackage", 0.042f));
                                    Car.ECUx4000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx4000Add_ExtremePackage", 0.046f));
                                    Car.ECUx5000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx5000Add_ExtremePackage", 0.051f));
                                    Car.ECUx6000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx6000Add_ExtremePackage", 0.0456f));
                                    Car.ECUx7000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx7000Add_ExtremePackage", 0.051f));
                                    Car.ECUx8000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx8000Add_ExtremePackage", 0.043f));
                                    Car.ECUx9000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx9000Add_ExtremePackage", 0.0375f));
                                    Car.ECUx10000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx10000Add_ExtremePackage", 0.039f));
                                    Car.ECUx11000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx11000Add_ExtremePackage", 0.035f));
                                    Car.ECUx12000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "ECUx12000Add_ExtremePackage", 0.0375f));
                                    Car.Turbox1000 = ToSingle(IniReader.GetDouble("INFO", "Turbox1000", 0.0f));
                                    Car.Turbox2000 = ToSingle(IniReader.GetDouble("INFO", "Turbox2000", 0.0f));
                                    Car.Turbox3000 = ToSingle(IniReader.GetDouble("INFO", "Turbox3000", 0.0f));
                                    Car.Turbox4000 = ToSingle(IniReader.GetDouble("INFO", "Turbox4000", 0.0f));
                                    Car.Turbox5000 = ToSingle(IniReader.GetDouble("INFO", "Turbox5000", 0.0f));
                                    Car.Turbox6000 = ToSingle(IniReader.GetDouble("INFO", "Turbox6000", 0.0f));
                                    Car.Turbox7000 = ToSingle(IniReader.GetDouble("INFO", "Turbox7000", 0.082f));
                                    Car.Turbox8000 = ToSingle(IniReader.GetDouble("INFO", "Turbox8000", 0.14f));
                                    Car.Turbox9000 = ToSingle(IniReader.GetDouble("INFO", "Turbox9000", 0.085f));
                                    Car.Turbox10000 = ToSingle(IniReader.GetDouble("INFO", "Turbox10000", 0.0f));
                                    Car.Turbox11000 = ToSingle(IniReader.GetDouble("INFO", "Turbox11000", 0.0f));
                                    Car.Turbox12000 = ToSingle(IniReader.GetDouble("INFO", "Turbox12000", 0.0f));
                                    Car.Turbox1000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox1000Add_ExtremePackage", 0.0f));
                                    Car.Turbox2000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox2000Add_ExtremePackage", 0.0f));
                                    Car.Turbox3000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox3000Add_ExtremePackage", 0.06f));
                                    Car.Turbox4000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox4000Add_ExtremePackage", 0.09f));
                                    Car.Turbox5000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox5000Add_ExtremePackage", 0.12f));
                                    Car.Turbox6000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox6000Add_ExtremePackage", 0.16f));
                                    Car.Turbox7000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox7000Add_ExtremePackage", 0.17f));
                                    Car.Turbox8000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox8000Add_ExtremePackage", 0.17f));
                                    Car.Turbox9000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox9000Add_ExtremePackage", 0.155f));
                                    Car.Turbox10000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox10000Add_ExtremePackage", 0.0f));
                                    Car.Turbox11000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox11000Add_ExtremePackage", 0.0f));
                                    Car.Turbox12000Add_ExtremePackage = ToSingle(IniReader.GetDouble("INFO", "Turbox12000Add_ExtremePackage", 0.0f));
                                    Car.FrontDownForce = ToSingle(IniReader.GetDouble("INFO", "FrontDownForce", 0.13f));
                                    Car.RearDownForce = ToSingle(IniReader.GetDouble("INFO", "RearDownForce", 0.145f));
                                    Car.unk0x37c = ToSingle(IniReader.GetDouble("INFO", "unk0x37c", 0.0f));
                                    Car.SteeringRatio = ToSingle(IniReader.GetDouble("INFO", "SteeringRatio", 1.0f));
                                    Car.FrontBrakeStrength = ToSingle(IniReader.GetDouble("INFO", "FrontBrakeStrength", 0.425f));
                                    Car.RearBrakeStrength = ToSingle(IniReader.GetDouble("INFO", "RearBrakeStrength", 0.52f));
                                    Car.BrakeBias = ToSingle(IniReader.GetDouble("INFO", "BrakeBias", 0.57f));
                                    Car.DefaultBasePaint2 = (uint)BinHash.Hash(IniReader.GetValue("INFO", "DefaultBasePaint2", "GLOSS_L2_COLOR03"));
                                    Car.IsSUV = (short)IniReader.GetInteger("INFO", "IsSUV", 0, 0, 1);
                                    Car.Cost = IniReader.GetInteger("INFO", "Cost", 35000);

                                }

                                // Will be set while adding to GlobalB
                                Car.Type = GetCarTypeIDFromResources(XName);
                            if (Car.Type >= GetCarTypeIDFromResources("")) // Increase the ID if it's a new car.
                            {
                                Car.Type += AddedCarCount; 
                                AddedCarCount++;
                            }

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
                            }

                            if (CurrentGame == (int)EdTypes.Game.Carbon || CurrentGame == (int)EdTypes.Game.ProStreet || CurrentGame == (int)EdTypes.Game.Undercover)
                            {
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

                            }

                            if (CurrentGame == (int)EdTypes.Game.MostWanted)
                            {
                                Car.MaxInstances[0] = (byte)IniReader.GetInteger("INFO", "MaxInstances1", 5, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[1] = (byte)IniReader.GetInteger("INFO", "MaxInstances2", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[2] = (byte)IniReader.GetInteger("INFO", "MaxInstances3", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[3] = (byte)IniReader.GetInteger("INFO", "MaxInstances4", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[4] = (byte)IniReader.GetInteger("INFO", "MaxInstances5", 2, byte.MinValue, byte.MaxValue);

                                Car.WantToKeepLoaded[0] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded1", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[1] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded2", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[2] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded3", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[3] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded4", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[4] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded5", 0, byte.MinValue, byte.MaxValue);

                                Car.pad4[0] = (byte)IniReader.GetInteger("INFO", "pad41", 0, byte.MinValue, byte.MaxValue);
                                Car.pad4[1] = (byte)IniReader.GetInteger("INFO", "pad42", 0, byte.MinValue, byte.MaxValue);

                                Car.MinTimeBetweenUses[0] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses1", 5.0f));
                                Car.MinTimeBetweenUses[1] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses2", 5.0f));
                                Car.MinTimeBetweenUses[2] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses3", 5.0f));
                                Car.MinTimeBetweenUses[3] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses4", 5.0f));
                                Car.MinTimeBetweenUses[4] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses5", 5.0f));

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

                            }

                            if (CurrentGame == (int)EdTypes.Game.Underground2)
                            {
                                Car.MaxInstances[0] = (byte)IniReader.GetInteger("INFO", "MaxInstances1", 5, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[1] = (byte)IniReader.GetInteger("INFO", "MaxInstances2", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[2] = (byte)IniReader.GetInteger("INFO", "MaxInstances3", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[3] = (byte)IniReader.GetInteger("INFO", "MaxInstances4", 2, byte.MinValue, byte.MaxValue);
                                Car.MaxInstances[4] = (byte)IniReader.GetInteger("INFO", "MaxInstances5", 2, byte.MinValue, byte.MaxValue);

                                Car.WantToKeepLoaded[0] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded1", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[1] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded2", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[2] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded3", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[3] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded4", 0, byte.MinValue, byte.MaxValue);
                                Car.WantToKeepLoaded[4] = (byte)IniReader.GetInteger("INFO", "WantToKeepLoaded5", 0, byte.MinValue, byte.MaxValue);

                                Car.pad4[0] = (byte)IniReader.GetInteger("INFO", "pad41", 0, byte.MinValue, byte.MaxValue);
                                Car.pad4[1] = (byte)IniReader.GetInteger("INFO", "pad42", 0, byte.MinValue, byte.MaxValue);

                                Car.MinTimeBetweenUses[0] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses1", 5.0f));
                                Car.MinTimeBetweenUses[1] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses2", 5.0f));
                                Car.MinTimeBetweenUses[2] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses3", 5.0f));
                                Car.MinTimeBetweenUses[3] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses4", 5.0f));
                                Car.MinTimeBetweenUses[4] = ToSingle(IniReader.GetDouble("INFO", "MinTimeBetweenUses5", 5.0f));

                                Car.AvailableSkinNumbers[0] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers1", 1, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[1] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers2", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[2] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers3", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[3] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers4", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[4] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers5", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[5] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers6", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[6] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers7", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[7] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers8", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[8] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers9", 0, byte.MinValue, byte.MaxValue);
                                Car.AvailableSkinNumbers[9] = (byte)IniReader.GetInteger("INFO", "AvailableSkinNumbers10", 0, byte.MinValue, byte.MaxValue);

                            }

                            Car.DefaultSkinNumber = (byte)IniReader.GetInteger("INFO", "DefaultSkinNumber", 1, byte.MinValue, byte.MaxValue);
                            Car.Skinnable = (byte)IniReader.GetInteger("INFO", "Skinnable", Car.UsageType == (int)EdTypes.CarUsageType.Racer ? 1 : 0, byte.MinValue, byte.MaxValue); // Checks usage type if not set in ini
                            Car.Padding = IniReader.GetInteger("INFO", "Padding");
                            Car.DefaultBasePaint = (uint)BinHash.Hash(IniReader.GetValue("INFO", "DefaultBasePaint", (CurrentGame == (int)EdTypes.Game.MostWanted || CurrentGame == (int)EdTypes.Game.Underground || CurrentGame == (int)EdTypes.Game.Underground2) ? "GLOSS_L1_COLOR01" : "GLOSS_L1_COLOR17"));

                            NewCarTypeInfoArray.Add(Car);

                            // ----------------------------------------------------------------------
                            // SlotTypes 

                            // Spoilers
                            SpoilerType.CarTypeNameHash = (uint)BinHash.Hash(XName);
                            SpoilerType.SlotTypeHash = (uint)BinHash.Hash(IniReader.GetValue("SPOILER", "SpoilerSet", "SPOILER"));

                            if (CurrentGame == (int)EdTypes.Game.Carbon)
                            {
                                SpoilerType.CarSlotID = 0x30;
                                SpoilerType.SlotType2Hash = (uint)BinHash.Hash(IniReader.GetValue("SPOILER", "AutosculptSpoilerSet", "SPOILER_AS2"));
                                SpoilerType.SlotType3Hash = 0xC2F6EBB0; // SPOILER_AS
                                SpoilerType.SlotType4Hash = 0;
                                SpoilerType.SlotType5Hash = 0;
                                SpoilerType.SlotType6Hash = 0;
                            }

                            if (CurrentGame == (int)EdTypes.Game.MostWanted)
                            {
                                SpoilerType.CarSlotID = 0x2C;
                            }

                            if (CurrentGame == (int)EdTypes.Game.Underground2)
                            {
                                SpoilerType.CarSlotID = 0x0C;
                            }

                            NewSlotTypeOverrides.Add(SpoilerType);

                            // Roof Scoops
                            RoofScoopType.CarTypeNameHash = (uint)BinHash.Hash(XName);

                            if (CurrentGame == (int)EdTypes.Game.Carbon)
                            {
                                RoofScoopType.CarSlotID = 0x4D;
                                RoofScoopType.SlotTypeHash = (uint)BinHash.Hash(IniReader.GetValue("ROOFSCOOP", "RoofScoopSet", "ROOF_SCOOP"));
                                RoofScoopType.SlotType2Hash = RoofScoopType.SlotTypeHash;
                                RoofScoopType.SlotType3Hash = RoofScoopType.SlotTypeHash;
                                RoofScoopType.SlotType4Hash = 0;
                                RoofScoopType.SlotType5Hash = 0;
                                RoofScoopType.SlotType6Hash = 0;
                            }

                            if (CurrentGame == (int)EdTypes.Game.MostWanted)
                            {
                                RoofScoopType.CarSlotID = 0x3E;
                                RoofScoopType.SlotTypeHash = (uint)BinHash.Hash(IniReader.GetValue("ROOFSCOOP", "RoofScoopSet", "ROOF"));
                            }

                            if (CurrentGame == (int)EdTypes.Game.Underground2)
                            {
                                RoofScoopType.CarSlotID = 0x07;
                                RoofScoopType.SlotTypeHash = (uint)BinHash.Hash(IniReader.GetValue("ROOFSCOOP", "RoofScoopSet", "ROOF"));
                            }

                            NewSlotTypeOverrides.Add(RoofScoopType);

                            // Mirrors
                            MirrorType.CarTypeNameHash = (uint)BinHash.Hash(XName);

                            if (CurrentGame == (int)EdTypes.Game.Underground2)
                            {
                                MirrorType.CarSlotID = 0x20;
                                MirrorType.SlotTypeHash = (uint)BinHash.Hash(IniReader.GetValue("MIRROR", "MirrorSet", "MIRRORS"));
                            }

                            NewSlotTypeOverrides.Add(MirrorType);

                            // ----------------------------------------------------------------------
                            // Collision

                            Collision.CopyFrom = IniReader.GetValue("COLLISION", "CopyFrom", "");
                            Collision.CopyTo = XName;

                            NewCollisionList.Add(Collision);

                            // ----------------------------------------------------------------------
                            // Resources

                            int NumberOfResources = IniReader.GetInteger("RESOURCES", "NumberOfResources", 0, 0);
                            
                            if (NumberOfResources == 0) // Old ini format
                            {
                                var Resource = new EdTypes.CarResource();

                                Resource.Label = IniReader.GetValue("RESOURCES", "Label", "");
                                Resource.Name = IniReader.GetValue("RESOURCES", "Name", "");

                                if (!(String.IsNullOrEmpty(Resource.Label) || String.IsNullOrEmpty(Resource.Name))) NewResourcesList.Add(Resource);
                            }
                            else // New ini format with multiple resoruces support
                            {
                                for (int r=1; r<=NumberOfResources; r++)
                                {
                                    var Resource = new EdTypes.CarResource(); 

                                    Resource.Label = IniReader.GetValue(String.Format("RESOURCE{0}", r), "Label", "");
                                    Resource.Name = IniReader.GetValue(String.Format("RESOURCE{0}", r), "Name", "");

                                    if (!(String.IsNullOrEmpty(Resource.Label) || String.IsNullOrEmpty(Resource.Name))) NewResourcesList.Add(Resource);
                                }
                            }
                            
                        }
                    }

                    // ----------------------------------------------------------------------

                    // Start Working On GlobalB

                    uint ChunkID, ChunkSize, NewChunkSize = 0;
                    int PaddingDifference = 0;
                    byte CarID = 0;

                    if (CurrentGame == (int)EdTypes.Game.MostWanted 
                        || CurrentGame == (int)EdTypes.Game.Carbon)
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
                        ChunkID = CarInfoArrayReader.ReadUInt32();
                        ChunkSize = CarInfoArrayReader.ReadUInt32();

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

                            if (Car.Type < NumberOfCars) continue;

                            // Increase car ID for the next car
                            NumberOfCars++;

                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)NumberOfCars * 0xD0 + 8;

                        CarInfoArrayWriter.BaseStream.Position = 4; // Go to size
                        CarInfoArrayWriter.Write(NewChunkSize); // Write new size

                        CarInfoArrayWriter.Close();
                        CarInfoArrayWriter.Dispose();
                        CarInfoArrayReader.Close();
                        CarInfoArrayReader.Dispose();
                        CarInfoArray.Dispose();

                    }

                        if (CurrentGame == (int)EdTypes.Game.ProStreet)
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
                        ChunkID = CarInfoArrayReader.ReadUInt32();
                        ChunkSize = CarInfoArrayReader.ReadUInt32();

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
                            CarInfoArrayWriter.Write(Car.CarTypeName);
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
                            CarInfoArrayWriter.Write(Car.CarTypeNameHash);
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

                            if (Car.Type < NumberOfCars) continue;

                            // Increase car ID for the next car
                            NumberOfCars++;

                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)NumberOfCars * 0xD0 + 8;

                        CarInfoArrayWriter.BaseStream.Position = 4; // Go to size
                        CarInfoArrayWriter.Write(NewChunkSize); // Write new size

                        CarInfoArrayWriter.Close();
                        CarInfoArrayWriter.Dispose();
                        CarInfoArrayReader.Close();
                        CarInfoArrayReader.Dispose();
                        CarInfoArray.Dispose();

                    }

                    if (CurrentGame == (int)EdTypes.Game.Undercover)
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
                        ChunkID = CarInfoArrayReader.ReadUInt32();
                        ChunkSize = CarInfoArrayReader.ReadUInt32();

                        if (ChunkID != 0x00034600)
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                            return;
                        }

                        // Add new data
                        int NumberOfCars = (int)(ChunkSize - 8) / 0x130; // CarTypeID for the next car
                        CarInfoArrayWriter.BaseStream.Position = CarInfoArrayWriter.BaseStream.Length;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            if (Car.Type == -1) continue;

                            CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x130);

                            CarInfoArrayWriter.Write(Car.CarTypeName);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                            CarInfoArrayWriter.Write(Car.BaseModelName);
                            CarInfoArrayWriter.Write(Car.GeometryFilename);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                            CarInfoArrayWriter.Write(Car.CarTypeName);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
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
                            CarInfoArrayWriter.Write(Car.CarTypeNameHash);
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

                            if (Car.Type < NumberOfCars) continue;

                            // Increase car ID for the next car
                            NumberOfCars++;

                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)NumberOfCars * 0x130 + 8;

                        CarInfoArrayWriter.BaseStream.Position = 4; // Go to size
                        CarInfoArrayWriter.Write(NewChunkSize); // Write new size

                        CarInfoArrayWriter.Close();
                        CarInfoArrayWriter.Dispose();
                        CarInfoArrayReader.Close();
                        CarInfoArrayReader.Dispose();
                        CarInfoArray.Dispose();

                    }

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
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
                            ChunkID = CarInfoArrayReader.ReadUInt32();
                            ChunkSize = CarInfoArrayReader.ReadUInt32();

                            if (ChunkID != 0x00034600)
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                                return;
                            }

                            // Add new data
                            int NumberOfCars = (int)(ChunkSize - 8) / 0x890; // CarTypeID for the next car
                            CarInfoArrayWriter.BaseStream.Position = CarInfoArrayWriter.BaseStream.Length;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                if (Car.Type == -1) continue;

                                // Copy unknown data from another car
                                int CarToCopyUnkDataFrom = GetCarTypeIDFromResources(Car.CopyFrom);
                                if (CarToCopyUnkDataFrom != -1)
                                {
                                    CarInfoArrayReader.BaseStream.Position = 16 + (CarToCopyUnkDataFrom * 0x890);
                                    byte[] DataToCopy = CarInfoArrayReader.ReadBytes(0x890);

                                    CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890);
                                    CarInfoArrayWriter.Write(DataToCopy);
                                }

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890);

                                // Overwrite known data
                                CarInfoArrayWriter.Write(Car.CarTypeName); //0x00
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(Car.BaseModelName); //0x20
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(Car.GeometryFilename); //0x40
                                CarInfoArrayWriter.Write(Car.GeometryLZCFilename); //0x60
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(0);
                                CarInfoArrayWriter.Write(Car.ManufacturerName); // 0xC0
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
                                CarInfoArrayWriter.Write(Car.HeadlightPosition.x); // 0xE0
                                CarInfoArrayWriter.Write(Car.HeadlightPosition.y);
                                CarInfoArrayWriter.Write(Car.HeadlightPosition.z);
                                CarInfoArrayWriter.Write(Car.HeadlightPosition.pad);
                                CarInfoArrayWriter.Write(Car.DriverRenderingOffset.x); // 0xF0
                                CarInfoArrayWriter.Write(Car.DriverRenderingOffset.y);
                                CarInfoArrayWriter.Write(Car.DriverRenderingOffset.z);
                                CarInfoArrayWriter.Write(Car.DriverRenderingOffset.pad);
                                CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.x); // 0x100
                                CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.y);
                                CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.z);
                                CarInfoArrayWriter.Write(Car.InCarSteeringWheelRenderingOffset.pad);
                                CarInfoArrayWriter.Write(Car.Unknown1.x); // 0x110
                                CarInfoArrayWriter.Write(Car.Unknown1.y);
                                CarInfoArrayWriter.Write(Car.Unknown1.z);
                                CarInfoArrayWriter.Write(Car.Unknown1.pad);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftX);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftSprings);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftRideHeight);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftUnk3);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftDiameter);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftTireSkidWidth);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftID);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftY);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftWideBodyY);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftUnk5);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftUnk6);
                                CarInfoArrayWriter.Write(Car.WheelFrontLeftUnk7);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightX);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightSprings);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightRideHeight);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightUnk3);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightDiameter);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightTireSkidWidth);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightID);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightY);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightWideBodyY);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightUnk5);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightUnk6);
                                CarInfoArrayWriter.Write(Car.WheelFrontRightUnk7);
                                CarInfoArrayWriter.Write(Car.WheelRearRightX);
                                CarInfoArrayWriter.Write(Car.WheelRearRightSprings);
                                CarInfoArrayWriter.Write(Car.WheelRearRightRideHeight);
                                CarInfoArrayWriter.Write(Car.WheelRearRightUnk3);
                                CarInfoArrayWriter.Write(Car.WheelRearRightDiameter);
                                CarInfoArrayWriter.Write(Car.WheelRearRightTireSkidWidth);
                                CarInfoArrayWriter.Write(Car.WheelRearRightID);
                                CarInfoArrayWriter.Write(Car.WheelRearRightY);
                                CarInfoArrayWriter.Write(Car.WheelRearRightWideBodyY);
                                CarInfoArrayWriter.Write(Car.WheelRearRightUnk5);
                                CarInfoArrayWriter.Write(Car.WheelRearRightUnk6);
                                CarInfoArrayWriter.Write(Car.WheelRearRightUnk7);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftX);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftSprings);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftRideHeight);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftUnk3);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftDiameter);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftTireSkidWidth);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftID);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftY);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftWideBodyY);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftUnk5);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftUnk6);
                                CarInfoArrayWriter.Write(Car.WheelRearLeftUnk7);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x280;
                                CarInfoArrayWriter.Write(Car.SuspensionUnk1F); // 0x280
                                CarInfoArrayWriter.Write(Car.FrontSprings);   
                                CarInfoArrayWriter.Write(Car.FrontShocks1);   
                                CarInfoArrayWriter.Write(Car.FrontShocks2);   
                                CarInfoArrayWriter.Write(Car.FrontSwayBar);   
                                CarInfoArrayWriter.Write(Car.SuspensionUnk2F);
                                CarInfoArrayWriter.Write(Car.SuspensionUnk3F);
                                CarInfoArrayWriter.Write(Car.SuspensionUnk4F);
                                CarInfoArrayWriter.Write(Car.SuspensionUnk1R);
                                CarInfoArrayWriter.Write(Car.RearSprings);    
                                CarInfoArrayWriter.Write(Car.RearShocks1);    
                                CarInfoArrayWriter.Write(Car.RearShocks2);    
                                CarInfoArrayWriter.Write(Car.RearSwayBar);    
                                CarInfoArrayWriter.Write(Car.SuspensionUnk2R);
                                CarInfoArrayWriter.Write(Car.SuspensionUnk3R);
                                CarInfoArrayWriter.Write(Car.SuspensionUnk4R);
                                CarInfoArrayWriter.Write(Car.Unknown0pt8);    
                                CarInfoArrayWriter.Write(Car.Unknown500);     
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio2);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x2D8;
                                CarInfoArrayWriter.Write(Car.NumberOfGears);

                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.GearRatioR); // 0x2E0
                                CarInfoArrayWriter.Write(Car.GearRatioN);
                                CarInfoArrayWriter.Write(Car.GearRatio1);
                                CarInfoArrayWriter.Write(Car.GearRatio2);
                                CarInfoArrayWriter.Write(Car.GearRatio3);
                                CarInfoArrayWriter.Write(Car.GearRatio4);
                                CarInfoArrayWriter.Write(Car.GearRatio5);
                                CarInfoArrayWriter.Write(Car.GearRatio6);
                                CarInfoArrayWriter.Write(Car.IdleRPM);
                                CarInfoArrayWriter.Write(Car.RedLineRPM);
                                CarInfoArrayWriter.Write(Car.MAXRPM);
                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.ECUx1000); // 0x310
                                CarInfoArrayWriter.Write(Car.ECUx2000); 
                                CarInfoArrayWriter.Write(Car.ECUx3000); 
                                CarInfoArrayWriter.Write(Car.ECUx4000); 
                                CarInfoArrayWriter.Write(Car.ECUx5000); 
                                CarInfoArrayWriter.Write(Car.ECUx6000); 
                                CarInfoArrayWriter.Write(Car.ECUx7000); 
                                CarInfoArrayWriter.Write(Car.ECUx8000); 
                                CarInfoArrayWriter.Write(Car.ECUx9000); 
                                CarInfoArrayWriter.Write(Car.ECUx10000);
                                CarInfoArrayWriter.Write(Car.ECUx11000);
                                CarInfoArrayWriter.Write(Car.ECUx12000);
                                CarInfoArrayWriter.Write(Car.Turbox1000); // 0x340
                                CarInfoArrayWriter.Write(Car.Turbox2000);
                                CarInfoArrayWriter.Write(Car.Turbox3000);
                                CarInfoArrayWriter.Write(Car.Turbox4000);
                                CarInfoArrayWriter.Write(Car.Turbox5000);
                                CarInfoArrayWriter.Write(Car.Turbox6000);
                                CarInfoArrayWriter.Write(Car.Turbox7000);
                                CarInfoArrayWriter.Write(Car.Turbox8000);
                                CarInfoArrayWriter.Write(Car.Turbox9000);
                                CarInfoArrayWriter.Write(Car.Turbox10000);
                                CarInfoArrayWriter.Write(Car.Turbox11000);
                                CarInfoArrayWriter.Write(Car.Turbox12000);
                                CarInfoArrayWriter.BaseStream.Position += 4; // 0x370
                                CarInfoArrayWriter.Write(Car.FrontDownForce);
                                CarInfoArrayWriter.Write(Car.RearDownForce);
                                CarInfoArrayWriter.Write(Car.unk0x37c);
                                CarInfoArrayWriter.Write(Car.SteeringRatio);
                                CarInfoArrayWriter.Write(Car.FrontBrakeStrength);
                                CarInfoArrayWriter.Write(Car.RearBrakeStrength);
                                CarInfoArrayWriter.Write(Car.BrakeBias);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x460;
                                CarInfoArrayWriter.Write(Car.Unknown0pt8);
                                CarInfoArrayWriter.Write(Car.Unknown500);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio_StreetPackage);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio2_StreetPackage);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x478;
                                CarInfoArrayWriter.Write(Car.NumberOfGears_StreetPackage);

                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.GearRatioR_StreetPackage); // 0x480
                                CarInfoArrayWriter.Write(Car.GearRatioN_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio1_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio2_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio3_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio4_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio5_StreetPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio6_StreetPackage);

                                CarInfoArrayWriter.Write(Car.Unknown0pt8);
                                CarInfoArrayWriter.Write(Car.Unknown500);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio_ProPackage);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio2_ProPackage);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x4B8;
                                CarInfoArrayWriter.Write(Car.NumberOfGears_ProPackage);

                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.GearRatioR_ProPackage); // 0x4C0
                                CarInfoArrayWriter.Write(Car.GearRatioN_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio1_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio2_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio3_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio4_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio5_ProPackage);
                                CarInfoArrayWriter.Write(Car.GearRatio6_ProPackage);

                                CarInfoArrayWriter.Write(Car.Unknown0pt8);
                                CarInfoArrayWriter.Write(Car.Unknown500);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.FinalDriveRatio2_ExtremePackage);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x4F8;
                                CarInfoArrayWriter.Write(Car.NumberOfGears_ExtremePackage);

                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.GearRatioR_ExtremePackage); // 0x4E0
                                CarInfoArrayWriter.Write(Car.GearRatioN_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio1_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio2_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio3_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio4_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio5_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.GearRatio6_ExtremePackage);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x560;
                                CarInfoArrayWriter.Write(Car.IdleRPMAdd_StreetPackage);
                                CarInfoArrayWriter.Write(Car.RedLineRPMAdd_StreetPackage);
                                CarInfoArrayWriter.Write(Car.MAXRPMAdd_StreetPackage);
                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.ECUx1000Add_StreetPackage); // 0x570
                                CarInfoArrayWriter.Write(Car.ECUx2000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx3000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx4000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx5000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx6000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx7000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx8000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx9000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx10000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx11000Add_StreetPackage);
                                CarInfoArrayWriter.Write(Car.ECUx12000Add_StreetPackage);

                                CarInfoArrayWriter.Write(Car.IdleRPMAdd_ProPackage);
                                CarInfoArrayWriter.Write(Car.RedLineRPMAdd_ProPackage);
                                CarInfoArrayWriter.Write(Car.MAXRPMAdd_ProPackage);
                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.ECUx1000Add_ProPackage); // 0x5B0
                                CarInfoArrayWriter.Write(Car.ECUx2000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx3000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx4000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx5000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx6000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx7000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx8000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx9000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx10000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx11000Add_ProPackage);
                                CarInfoArrayWriter.Write(Car.ECUx12000Add_ProPackage);

                                CarInfoArrayWriter.Write(Car.IdleRPMAdd_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.RedLineRPMAdd_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.MAXRPMAdd_ExtremePackage);
                                CarInfoArrayWriter.BaseStream.Position += 4;
                                CarInfoArrayWriter.Write(Car.ECUx1000Add_ExtremePackage); // 0x5F0
                                CarInfoArrayWriter.Write(Car.ECUx2000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx3000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx4000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx5000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx6000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx7000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx8000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx9000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx10000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx11000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.ECUx12000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox1000Add_ExtremePackage); // 0x620
                                CarInfoArrayWriter.Write(Car.Turbox2000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox3000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox4000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox5000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox6000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox7000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox8000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox9000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox10000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox11000Add_ExtremePackage);
                                CarInfoArrayWriter.Write(Car.Turbox12000Add_ExtremePackage);

                                CarInfoArrayWriter.BaseStream.Position = 16 + (Car.Type * 0x890) + 0x840;
                                CarInfoArrayWriter.Write(Car.Type);
                                CarInfoArrayWriter.Write(Car.UsageType);
                                CarInfoArrayWriter.BaseStream.Position += 4; // 0x848
                                CarInfoArrayWriter.Write(Car.DefaultBasePaint);
                                CarInfoArrayWriter.Write(Car.DefaultBasePaint2);
                                CarInfoArrayWriter.BaseStream.Position += 0x0C; // 0x854
                                CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[0]); // 0x860
                                CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[1]);
                                CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[2]);
                                CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[3]);
                                CarInfoArrayWriter.Write(Car.MinTimeBetweenUses[4]);
                                CarInfoArrayWriter.Write((int)Car.DefaultSkinNumber);
                                CarInfoArrayWriter.Write(Car.Padding);
                                CarInfoArrayWriter.Write(Car.Cost); // 0x87C
                                for (byte SkinNr = 1; SkinNr <= 10; SkinNr++) CarInfoArrayWriter.Write((SkinNr <= Car.NumAvailableSkinNumbers) ? (byte)SkinNr : (byte)0x00); //0x880
                                CarInfoArrayWriter.Write(Car.IsSUV);
                                CarInfoArrayWriter.Write((int)Car.Skinnable);

                                CarInfoArrayWriter.BaseStream.Position = CarInfoArrayWriter.BaseStream.Length;

                                if (Car.Type < NumberOfCars) continue;

                                // Increase car ID for the next car
                                NumberOfCars++;

                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)NumberOfCars * 0x890 + 8;

                            CarInfoArrayWriter.BaseStream.Position = 4; // Go to size
                            CarInfoArrayWriter.Write(NewChunkSize); // Write new size

                            CarInfoArrayWriter.Close();
                            CarInfoArrayWriter.Dispose();
                            CarInfoArrayReader.Close();
                            CarInfoArrayReader.Dispose();
                            CarInfoArray.Dispose();

                        }

                        // -----------------------------------------------------

                        // SlotTypes

                    if ((CurrentGame == (int)EdTypes.Game.MostWanted)
                            || (CurrentGame == (int)EdTypes.Game.Underground2))
                    {
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
                    int NumberOfSlotTypeOverrides = 0;
                    CarInfoSlotTypesWriter.BaseStream.Position = CarInfoSlotTypesWriter.BaseStream.Length;

                    foreach (EdTypes.SlotType SlotTypeOverride in NewSlotTypeOverrides)
                    {
                        // Skip if types are default
                        if ((SlotTypeOverride.SlotTypeHash == (uint)BinHash.Hash("SPOILER"))
                            || (SlotTypeOverride.SlotTypeHash == (uint)BinHash.Hash("ROOF"))) continue;

                        CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarTypeNameHash);
                        CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarSlotID);
                        CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarTypeNameHash);
                        CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotTypeHash);

                        // Increase count for chunk size calculation
                        NumberOfSlotTypeOverrides++;
                    }

                    // Fix size in Chunk Header
                    NewChunkSize = (uint)(ChunkSize + NumberOfSlotTypeOverrides * 0x10);

                    // Fix padding
                    CarInfoSlotTypesWriter.BaseStream.Position = CarInfoSlotTypesWriter.BaseStream.Length;
                    PaddingDifference = (int)(NewChunkSize % 16);

                    if (CurrentGame == (int)EdTypes.Game.MostWanted)
                    {
                        while (PaddingDifference != 8)
                        {
                            CarInfoSlotTypesWriter.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }
                    }
                    else
                    {
                        while (PaddingDifference != 0)
                        {
                            CarInfoSlotTypesWriter.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }
                    }
                    

                    CarInfoSlotTypesWriter.BaseStream.Position = 4; // Go to size
                    CarInfoSlotTypesWriter.Write(NewChunkSize); // Write new size

                    CarInfoSlotTypesWriter.Close();
                    CarInfoSlotTypesWriter.Dispose();
                    CarInfoSlotTypesReader.Close();
                    CarInfoSlotTypesReader.Dispose();
                    CarInfoSlotTypes.Dispose();
                }

                    if (CurrentGame == (int)EdTypes.Game.Carbon)
                    {
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
                        int NumberOfSlotTypeOverrides = 0;
                        CarInfoSlotTypesWriter.BaseStream.Position = CarInfoSlotTypesWriter.BaseStream.Length;

                        foreach (EdTypes.SlotType SlotTypeOverride in NewSlotTypeOverrides)
                        {
                            // Skip if types are default
                            if ((SlotTypeOverride.SlotTypeHash == (uint)BinHash.Hash("SPOILER")) && (SlotTypeOverride.SlotType2Hash == (uint)BinHash.Hash("SPOILER_AS2"))
                                || (SlotTypeOverride.SlotTypeHash == (uint)BinHash.Hash("ROOF_SCOOP"))) continue;

                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarTypeNameHash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarSlotID);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.CarTypeNameHash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotTypeHash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotType2Hash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotType3Hash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotType4Hash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotType5Hash);
                            CarInfoSlotTypesWriter.Write(SlotTypeOverride.SlotType6Hash);

                            // Increase spoiler count for chunk size calculation
                            NumberOfSlotTypeOverrides++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfSlotTypeOverrides * 0x24);

                        // Fix padding
                        CarInfoSlotTypesWriter.BaseStream.Position = CarInfoSlotTypesWriter.BaseStream.Length;
                        PaddingDifference = (int)(NewChunkSize % 16);

                        while (PaddingDifference != 8)
                        {
                            CarInfoSlotTypesWriter.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }

                        CarInfoSlotTypesWriter.BaseStream.Position = 4; // Go to size
                        CarInfoSlotTypesWriter.Write(NewChunkSize); // Write new size

                        CarInfoSlotTypesWriter.Close();
                        CarInfoSlotTypesWriter.Dispose();
                        CarInfoSlotTypesReader.Close();
                        CarInfoSlotTypesReader.Dispose();
                        CarInfoSlotTypes.Dispose();

                    }

                    if ((CurrentGame == (int)EdTypes.Game.ProStreet) ||
                            (CurrentGame == (int)EdTypes.Game.Undercover))
                    {
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"), true);

                    }

                        // -----------------------------------------------------

                        // CarPart Chunk 2

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {
                            if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin")))
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"));
                                return;
                            }
                            File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), true);

                            var CarInfoCarParts2 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), FileMode.Open, FileAccess.ReadWrite);
                            var CarInfoCarParts2Reader = new BinaryReader(CarInfoCarParts2);
                            var CarInfoCarParts2Writer = new BinaryWriter(CarInfoCarParts2);

                            // Get ID and Size to verify chunk
                            CarInfoCarParts2Reader.BaseStream.Position = 0;
                            ChunkID = CarInfoCarParts2Reader.ReadUInt32();
                            ChunkSize = CarInfoCarParts2Reader.ReadUInt32();

                            if (ChunkID != 0x0003460C) // Car Parts List
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"));
                                return;
                            }

                            // Add new data
                            int NumberOfNewEntries = 0;

                            // Get last used id
                            ChunkSize = (ChunkSize / 4) * 4; // Fix padding

                            var CarPart3Arr = new FileInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"));
                            short CarPart3ResourceCount = (short)((CarPart3Arr.Length) / 8 - 1);
                            
                            CarInfoCarParts2Writer.BaseStream.Position = ChunkSize + 8;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                string XName = Encoding.ASCII.GetString(Car.CarTypeName);
                                if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end

                                if (Car.UsageType != 2) // not Traffic
                                {
                                    // CARNAME_CV - KIT00_BODY
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCB);
                                    // CARNAME_W01_CV
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    // CARNAME_W02_CV
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    // CARNAME_W03_CV
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    // CARNAME_W04_CV
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    // CARNAME_KIT00_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CDC);
                                    // CARNAME_STYLE01_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE04_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE10_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE11_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE02_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE03_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE05_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE08_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE13_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE06_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE07_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE09_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE12_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE14_HEADLIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_KIT00_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CED);
                                    // CARNAME_STYLE01_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE04_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE10_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE11_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE02_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE03_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0CCE);
                                    CarInfoCarParts2Writer.Write((short)0x0CDE);
                                    // CARNAME_STYLE05_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE08_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE13_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE06_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE07_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE09_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE12_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE14_BRAKELIGHT
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_KIT22_HOOD_UNDER
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT24_HOOD_UNDER
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)(CarPart3ResourceCount + 1));
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT23_HOOD_UNDER
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)(CarPart3ResourceCount + 2));
                                    CarInfoCarParts2Writer.Write((short)0x0D07);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT25_HOOD_UNDER
                                    CarInfoCarParts2Writer.Write((short)3);
                                    CarInfoCarParts2Writer.Write((short)(CarPart3ResourceCount + 3));
                                    CarInfoCarParts2Writer.Write((short)0x0D07);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT21_HOOD_UNDER
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)(CarPart3ResourceCount + 4));
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT22_HOOD_UNDER CF
                                    CarInfoCarParts2Writer.Write((short)4);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0D0A);
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT24_HOOD_UNDER CF
                                    CarInfoCarParts2Writer.Write((short)4);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0D0B);
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT23_HOOD_UNDER CF
                                    CarInfoCarParts2Writer.Write((short)5);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0D07);
                                    CarInfoCarParts2Writer.Write((short)0x0D0C);
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT25_HOOD_UNDER CF
                                    CarInfoCarParts2Writer.Write((short)5);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0D07);
                                    CarInfoCarParts2Writer.Write((short)0x0D0D);
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_KIT21_HOOD_UNDER CF
                                    CarInfoCarParts2Writer.Write((short)4);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    CarInfoCarParts2Writer.Write((short)0x0D0E);
                                    CarInfoCarParts2Writer.Write((short)2);
                                    CarInfoCarParts2Writer.Write((short)0x0D03);
                                    // CARNAME_ENGINE
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE01_ENGINE
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);
                                    // CARNAME_STYLE02_ENGINE
                                    CarInfoCarParts2Writer.Write((short)1);
                                    CarInfoCarParts2Writer.Write((short)CarPart3ResourceCount++);


                                    NumberOfNewEntries += 143;
                                }
                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)((ChunkSize) + (NumberOfNewEntries * 2));

                            // Fix padding
                            CarInfoCarParts2Writer.BaseStream.Position = CarInfoCarParts2Writer.BaseStream.Length;
                            PaddingDifference = (int)(NewChunkSize % 4);

                            while (PaddingDifference != 0)
                            {
                                CarInfoCarParts2Writer.Write((byte)0);
                                PaddingDifference = (PaddingDifference + 1) % 4;
                                NewChunkSize++;
                            }

                            CarInfoCarParts2Writer.BaseStream.Position = 4; // Go to size
                            CarInfoCarParts2Writer.Write(NewChunkSize); // Write new size

                            CarInfoCarParts2Writer.Close();
                            CarInfoCarParts2Writer.Dispose();
                            CarInfoCarParts2Reader.Close();
                            CarInfoCarParts2Reader.Dispose();
                            CarInfoCarParts2.Dispose();

                        }

                        uint CarPart2NewChunkSize = NewChunkSize;


                        // -----------------------------------------------------

                        // CarPart Chunk 3

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {
                            if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin")))
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"));
                                return;
                            }
                            File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), true);

                            var CarInfoCarParts3 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), FileMode.Open, FileAccess.ReadWrite);
                            var CarInfoCarParts3Reader = new BinaryReader(CarInfoCarParts3);
                            var CarInfoCarParts3Writer = new BinaryWriter(CarInfoCarParts3);

                            // Get ID and Size to verify chunk
                            CarInfoCarParts3Reader.BaseStream.Position = 0;
                            ChunkID = CarInfoCarParts3Reader.ReadUInt32();
                            ChunkSize = CarInfoCarParts3Reader.ReadUInt32();

                            if (ChunkID != 0x00034605) // Car Parts List
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"));
                                return;
                            }

                            // Add new data
                            int NumberOfRacerPartLists = 0;

                            // Get last used id
                            ChunkSize = (ChunkSize / 8) * 8; // Fix padding

                            CarInfoCarParts3Writer.BaseStream.Position = ChunkSize + 8;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                string XName = Encoding.ASCII.GetString(Car.CarTypeName);
                                if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end

                                if (Car.UsageType != 2) // not Traffic
                                {
                                    for (int q = 0; q < 43; q++)
                                    {
                                        CarInfoCarParts3Writer.Write((q >= 35 && q <= 39) ? BinHash.Hash("HOODUNDER") : 0x10C98090);
                                        CarInfoCarParts3Writer.Write(BinHash.Hash(XName + CarPartIDsUnderground2.CarPart3PartNames[q]));
                                    }
                                    NumberOfRacerPartLists++;
                                }
                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)((ChunkSize) + (NumberOfRacerPartLists * 8 * 43));

                            CarInfoCarParts3Writer.BaseStream.Position = 4; // Go to size
                            CarInfoCarParts3Writer.Write(NewChunkSize); // Write new size

                            CarInfoCarParts3Writer.Close();
                            CarInfoCarParts3Writer.Dispose();
                            CarInfoCarParts3Reader.Close();
                            CarInfoCarParts3Reader.Dispose();
                            CarInfoCarParts3.Dispose();

                        }

                        uint CarPart3NewChunkSize = NewChunkSize;


                        // -----------------------------------------------------

                        // CarPart Chunk 4: CarPartModelsTable

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {
                            if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin")))
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"));
                                return;
                            }
                            File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), true);

                            var CarInfoCarParts4 = new FileStream(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), FileMode.Open, FileAccess.ReadWrite);
                            var CarInfoCarParts4Reader = new BinaryReader(CarInfoCarParts4);
                            var CarInfoCarParts4Writer = new BinaryWriter(CarInfoCarParts4);

                            // Get ID and Size to verify chunk
                            CarInfoCarParts4Reader.BaseStream.Position = 0;
                            ChunkID = CarInfoCarParts4Reader.ReadUInt32();
                            ChunkSize = CarInfoCarParts4Reader.ReadUInt32();

                            if (ChunkID != 0x0003460A) // Car Parts List
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"));
                                return;
                            }

                            // Add new data
                            int NumberOfRacerPartLists = 0;
                            int NumberOfTrafficPartLists = 0;

                            // Get last used id
                            ChunkSize = (ChunkSize / 0x24) * 0x24; // Fix padding

                            CarInfoCarParts4Writer.BaseStream.Position = ChunkSize + 8;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                string XName = Encoding.ASCII.GetString(Car.CarTypeName);
                                if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end

                                if (Car.UsageType == 2) // Traffic
                                {
                                    // CARNAME_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // Tire
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_TIRE_FRONT_A"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_TIRE_FRONT_B"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_TIRE_FRONT_B"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_TIRE_FRONT_B"));
                                    for (int q = 0; q < 4; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    NumberOfTrafficPartLists++;
                                }
                                else
                                {
                                    // CARNAME_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // CARNAME_W01_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_W01_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // CARNAME_W02_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_W02_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // CARNAME_W03_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_W03_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // CARNAME_W04_CV
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_W04_CV"));
                                    for (int q = 0; q < 7; q++) CarInfoCarParts4Writer.Write((int)-1);
                                    // brakes
                                    CarInfoCarParts4Writer.Write(0xFFFF0000);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_FRONT_BRAKE_A"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_FRONT_BRAKE_B"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_FRONT_BRAKE_B"));
                                    CarInfoCarParts4Writer.Write((int)-1);
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_REAR_BRAKE_A"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_REAR_BRAKE_B"));
                                    CarInfoCarParts4Writer.Write(BinHash.Hash(XName + "_KIT00_REAR_BRAKE_C"));
                                    CarInfoCarParts4Writer.Write((int)-1);
                                    NumberOfRacerPartLists++;
                                }
                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)((ChunkSize) + (NumberOfRacerPartLists * 0x24 * 6) + (NumberOfTrafficPartLists * 0x24 * 2));

                            CarInfoCarParts4Writer.BaseStream.Position = 4; // Go to size
                            CarInfoCarParts4Writer.Write(NewChunkSize); // Write new size

                            CarInfoCarParts4Writer.Close();
                            CarInfoCarParts4Writer.Dispose();
                            CarInfoCarParts4Reader.Close();
                            CarInfoCarParts4Reader.Dispose();
                            CarInfoCarParts4.Dispose();

                        }

                        uint CarPart4NewChunkSize = NewChunkSize;

                        // -----------------------------------------------------

                        // CarPart Chunk 5: Car XName Hash List                    
                        string CarPartDirectory = @"Global\BCHUNK_CARINFO_CARPART";

                        if (CurrentGame == (int)EdTypes.Game.Underground)
                        {
                            Log("Skipping BCHUNK_CARINFO_CARPART\\5_0003460B.bin. This game doesn't require any editing on this chunk.");
                        }

                        else if (Directory.Exists(Path.Combine(GetResourcesPath(), CarPartDirectory)))
                        {

                            Directory.CreateDirectory(Path.Combine(GetTempPath(), CarPartDirectory));

                            if (!File.Exists(Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin")))
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin"));
                                return;
                            }
                            File.Copy(Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin"), Path.Combine(GetTempPath(), CarPartDirectory, "5_0003460B.bin"), true);

                            var CarInfoCarParts5 = new FileStream(Path.Combine(GetTempPath(), CarPartDirectory, "5_0003460B.bin"), FileMode.Open, FileAccess.ReadWrite);
                            var CarInfoCarParts5Reader = new BinaryReader(CarInfoCarParts5);
                            var CarInfoCarParts5Writer = new BinaryWriter(CarInfoCarParts5);

                            // Get ID and Size to verify chunk
                            CarInfoCarParts5Reader.BaseStream.Position = 0;
                            ChunkID = CarInfoCarParts5Reader.ReadUInt32();
                            ChunkSize = CarInfoCarParts5Reader.ReadUInt32();

                            if (ChunkID != 0x0003460B)
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file has an incorrect header: " + Environment.NewLine + Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource file has an incorrect header: " + Path.Combine(GetResourcesPath(), CarPartDirectory, "5_0003460B.bin"));
                                return;
                            }

                            // Add new data
                            int NumberOfXNames = (int)ChunkSize / 4;
                            int NumberOfAddedXNames = 0;
                            CarInfoCarParts5Writer.BaseStream.Position = CarInfoCarParts5Writer.BaseStream.Length;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                // Skip if data already exists
                                bool DoesXNameExist = false;

                                CarInfoCarParts5Reader.BaseStream.Position = 8; // skip ID and size

                                while (CarInfoCarParts5Reader.BaseStream.Position < CarInfoCarParts5Reader.BaseStream.Length)
                                {
                                    if (Car.CarTypeNameHash == CarInfoCarParts5Reader.ReadUInt32() && (Car.CarTypeNameHash != BinHash.Hash("NEON"))) // Add exception for Dodge Neon
                                    {
                                        DoesXNameExist = true;
                                        break;
                                    }
                                }
                                if (DoesXNameExist) continue;

                                // Move to the end and write data
                                CarInfoCarParts5Writer.BaseStream.Position = CarInfoCarParts5Writer.BaseStream.Length;
                                CarInfoCarParts5Writer.Write(Car.CarTypeNameHash);

                                // Increase count for chunk size calculation
                                NumberOfXNames++;
                                NumberOfAddedXNames++;
                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)(NumberOfXNames * 4);

                            CarInfoCarParts5Writer.BaseStream.Position = 4; // Go to size
                            CarInfoCarParts5Writer.Write(NewChunkSize); // Write new size

                            CarInfoCarParts5Writer.Close();
                            CarInfoCarParts5Writer.Dispose();
                            CarInfoCarParts5Reader.Close();
                            CarInfoCarParts5Reader.Dispose();
                            CarInfoCarParts5.Dispose();

                        }

                        // -----------------------------------------------------

                        // CarPart Chunk 6: Car XName Hash List

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {
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
                            int NumberOfTrafficPartLists = 0;

                            // Get last used id
                            ChunkSize = (ChunkSize / 0xE) * 0xE; // Fix padding
                            CarInfoCarParts6Reader.BaseStream.Position = ChunkSize - 7 + 8;
                            CarID = CarInfoCarParts6Reader.ReadByte();
                            /*
                            // Get CarPart 2 Count
                            var CarPart2Arr = new FileInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"));
                            short CarPart2ResourceCount = (short)((CarPart2Arr.Length - 8) / 2);
                            */
                            // Get CarPart 4 Count
                            var CarPart4Arr = new FileInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"));
                            short CarPart4ResourceCount = (short)((CarPart4Arr.Length - 8) / 0x24);

                            CarInfoCarParts6Writer.BaseStream.Position = ChunkSize + 8;
                            CarID++;

                            foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                            {
                                // Skip if data already exists
                                //if (((GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) + NumberOfRacerPartLists + NumberOfTrafficPartLists) < CarID) && (Car.CarTypeNameHash != BinHash.Hash("NEON"))) continue; // Add exception for Dodge Neon

                                string XName = Encoding.ASCII.GetString(Car.CarTypeName);
                                if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end

                                if (Car.UsageType == 2) // Traffic
                                {
                                    for (int p = 0; p < 3; p++)
                                    {
                                        // Write data for each part.
                                        CarInfoCarParts6Writer.Write(BinHash.Hash(XName + CarPartIDsUnderground2.TrafficPartNames[p]));
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.TrafficCarSlotIDs[p]);
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.TrafficUnk1s[p]);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.TrafficCarPart1Offsets[p]);
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.TrafficUnk2s[p]);
                                        CarInfoCarParts6Writer.Write(p != 0 ? (ushort)(CarPart4ResourceCount + CarPartIDsUnderground2.TrafficFeCustRecIDs[p] + (NumberOfTrafficPartLists * 2) + (NumberOfRacerPartLists * 6)) : CarPartIDsUnderground2.TrafficFeCustRecIDs[p]);
                                    }

                                    NumberOfTrafficPartLists++;
                                    CarID++;
                                }
                                else
                                {
                                    for (int p = 0; p < 270; p++)
                                    {
                                        // Write data for each part.
                                        CarInfoCarParts6Writer.Write(BinHash.Hash(XName + CarPartIDsUnderground2.PartNames[p]));
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.CarSlotIDs[p]);
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.Unk1s[p]);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(CarPartIDsUnderground2.CarPart1Offsets[p]);
                                        CarInfoCarParts6Writer.Write((p == 63 || (p >= 65 && p <= 68) || (p >= 94 && p <= 98) || (p >= 122 && p <= 126) || (p >= 160 && p <= 193)) ? (ushort)(CarPartIDsUnderground2.Unk2s[p] + 0x8F * (NumberOfRacerPartLists + 1) + 1 ) : CarPartIDsUnderground2.Unk2s[p]);
                                        CarInfoCarParts6Writer.Write((p == 232 || p > 264) ? (ushort)(CarPart4ResourceCount + CarPartIDsUnderground2.FeCustRecIDs[p] + (NumberOfTrafficPartLists * 2) + (NumberOfRacerPartLists * 6)) : CarPartIDsUnderground2.FeCustRecIDs[p]);
                                    }

                                    NumberOfRacerPartLists++;
                                    CarID++;
                                }
                            }

                            // Fix size in Chunk Header
                            NewChunkSize = (uint)((ChunkSize) + (NumberOfRacerPartLists * 0xE * 270) + (NumberOfTrafficPartLists * 0xE * 3));

                            // Fix padding
                            //CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                            PaddingDifference = (int)(NewChunkSize % 4);

                            while (PaddingDifference != 0)
                            {
                                CarInfoCarParts6Writer.Write((byte)0);
                                PaddingDifference = (PaddingDifference + 1) % 4;
                                NewChunkSize++;
                            }

                            CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                            CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                            CarInfoCarParts6Writer.Close();
                            CarInfoCarParts6Writer.Dispose();
                            CarInfoCarParts6Reader.Close();
                            CarInfoCarParts6Reader.Dispose();
                            CarInfoCarParts6.Dispose();

                        }

                        if (CurrentGame == (int)EdTypes.Game.MostWanted)
                    {
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

                        // Get last used id
                        ChunkSize = (ChunkSize / 0xE) * 0xE; // Fix padding
                        CarInfoCarParts6Reader.BaseStream.Position = ChunkSize - 7 + 8;
                        CarID = CarInfoCarParts6Reader.ReadByte();


                        CarInfoCarParts6Writer.BaseStream.Position = ChunkSize + 8;
                        CarID++;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if ((GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) + NumberOfRacerPartLists) < CarID) continue;

                            string XName = Encoding.ASCII.GetString(Car.CarTypeName);
                            if (XName.IndexOf('\0') != -1) XName = XName.Substring(0, XName.IndexOf('\0')); // Fix nulls at the end

                            for (int p = 0; p < 0xBF; p++)
                            {
                                // Write data for each part.
                                CarInfoCarParts6Writer.Write(BinHash.Hash(XName + CarPartIDsMostWanted.PartNames[p]));
                                CarInfoCarParts6Writer.Write(CarPartIDsMostWanted.CarSlotIDs[p]);
                                CarInfoCarParts6Writer.Write(CarPartIDsMostWanted.Unk1s[p]);
                                CarInfoCarParts6Writer.Write(CarID);
                                CarInfoCarParts6Writer.Write(CarPartIDsMostWanted.CarPart1Offsets[p]);
                                CarInfoCarParts6Writer.Write(CarPartIDsMostWanted.Unk2s[p]);
                                CarInfoCarParts6Writer.Write(CarPartIDsMostWanted.FeCustRecIDs[p]);
                            }

                            NumberOfRacerPartLists++;
                            CarID++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfRacerPartLists * 0xA72);

                        // Fix padding
                        //CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        PaddingDifference = (int)(NewChunkSize % 4);

                        while (PaddingDifference != 0)
                        {
                            CarInfoCarParts6Writer.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 4;
                            NewChunkSize++;
                        }

                        CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts6Writer.Close();
                        CarInfoCarParts6Writer.Dispose();
                        CarInfoCarParts6Reader.Close();
                        CarInfoCarParts6Reader.Dispose();
                        CarInfoCarParts6.Dispose();

                    }

                        if (CurrentGame == (int)EdTypes.Game.Carbon)
                    {

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
                        CarID = CarInfoCarParts6Reader.ReadByte();
                        CarInfoCarParts6Reader.BaseStream.Position += 2;


                        CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        CarID++;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if ((GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) + NumberOfRacerPartLists + NumberOfCopPartLists + NumberOfTrafficPartLists) < CarID) continue;

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
                                    CarID++;
                                    break;

                                case (int)EdTypes.CarUsageType.Cop:
                                    foreach (ushort u in CarPartIDsCarbon.Cop)
                                    {
                                        CarInfoCarParts6Writer.Write((byte)0);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(u);
                                    }
                                    NumberOfCopPartLists++;
                                    CarID++;
                                    break;

                                case (int)EdTypes.CarUsageType.Traffic:
                                    foreach (ushort u in CarPartIDsCarbon.Traffic)
                                    {
                                        CarInfoCarParts6Writer.Write((byte)0);
                                        CarInfoCarParts6Writer.Write(CarID);
                                        CarInfoCarParts6Writer.Write(u);
                                    }
                                    NumberOfTrafficPartLists++;
                                    CarID++;
                                    break;
                            }
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfRacerPartLists * 0x454 + NumberOfCopPartLists * 0xCC + NumberOfTrafficPartLists * 0x80);
                        /*
                        // Fix padding
                        //CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        PaddingDifference = (int)(NewChunkSize % 16);

                        while (PaddingDifference != 8)
                        {
                            CarInfoCarParts6Writer.Write((byte)0);
                            PaddingDifference = (PaddingDifference + 1) % 16;
                            NewChunkSize++;
                        }
                        */
                        CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts6Writer.Close();
                        CarInfoCarParts6Writer.Dispose();
                        CarInfoCarParts6Reader.Close();
                        CarInfoCarParts6Reader.Dispose();
                        CarInfoCarParts6.Dispose();

                    }

                        if (CurrentGame == (int)EdTypes.Game.ProStreet)
                    {

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

                        // Get last used id
                        CarInfoCarParts6Reader.BaseStream.Position = CarInfoCarParts6Reader.BaseStream.Length - 0xF;
                        CarID = CarInfoCarParts6Reader.ReadByte();
                        CarInfoCarParts6Reader.BaseStream.Position += 0xE;


                        CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        CarID++;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if ((GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) + NumberOfRacerPartLists) < CarID) continue;

                            
                            foreach (ushort u in CarPartIDsProStreet.Racer)
                            {
                                CarInfoCarParts6Writer.Write((byte)0);
                                CarInfoCarParts6Writer.Write(CarID);
                                CarInfoCarParts6Writer.Write(u);
                                CarInfoCarParts6Writer.Write(0);
                                CarInfoCarParts6Writer.Write(0);
                                CarInfoCarParts6Writer.Write(0);
                            }

                            NumberOfRacerPartLists++;
                            CarID++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfRacerPartLists * 0x12D0);

                        CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts6Writer.Close();
                        CarInfoCarParts6Writer.Dispose();
                        CarInfoCarParts6Reader.Close();
                        CarInfoCarParts6Reader.Dispose();
                        CarInfoCarParts6.Dispose();

                    }

                        if (CurrentGame == (int)EdTypes.Game.Undercover)
                    {

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

                        // Get last used id
                        CarInfoCarParts6Reader.BaseStream.Position = CarInfoCarParts6Reader.BaseStream.Length - 0xF;
                        CarID = CarInfoCarParts6Reader.ReadByte();
                        CarInfoCarParts6Reader.BaseStream.Position += 0xE;


                        CarInfoCarParts6Writer.BaseStream.Position = CarInfoCarParts6Writer.BaseStream.Length;
                        CarID++;

                        foreach (EdTypes.CarTypeInfo Car in NewCarTypeInfoArray)
                        {
                            // Skip if data already exists
                            if ((GetCarPartIDFromResources(Encoding.ASCII.GetString(Car.CarTypeName)) + NumberOfRacerPartLists) < CarID) continue;


                            foreach (ushort u in CarPartIDsUndercover.Racer)
                            {
                                CarInfoCarParts6Writer.Write((byte)0);
                                CarInfoCarParts6Writer.Write(CarID);
                                CarInfoCarParts6Writer.Write(u);
                                CarInfoCarParts6Writer.Write(0);
                                CarInfoCarParts6Writer.Write(0);
                                CarInfoCarParts6Writer.Write(0);
                            }

                            NumberOfRacerPartLists++;
                            CarID++;
                        }

                        // Fix size in Chunk Header
                        NewChunkSize = (uint)(ChunkSize + NumberOfRacerPartLists * 0xAF0);

                        CarInfoCarParts6Writer.BaseStream.Position = 4; // Go to size
                        CarInfoCarParts6Writer.Write(NewChunkSize); // Write new size

                        CarInfoCarParts6Writer.Close();
                        CarInfoCarParts6Writer.Dispose();
                        CarInfoCarParts6Reader.Close();
                        CarInfoCarParts6Reader.Dispose();
                        CarInfoCarParts6.Dispose();

                    }

                        uint CarPart6NewChunkSize = NewChunkSize;

                        // -----------------------------------------------------


                        // CarPart Chunk 0: Zero (Info) Chunk

                        if (CurrentGame == (int)EdTypes.Game.Underground)
                        {

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

                            // Write new Parts Count here
                            CarInfoCarParts0Writer.BaseStream.Position = 0x18;
                            CarInfoCarParts0Writer.Write(CarPart6NewChunkSize / 0x30);

                            CarInfoCarParts0Writer.Close();
                            CarInfoCarParts0Writer.Dispose();
                            CarInfoCarParts0Reader.Close();
                            CarInfoCarParts0Reader.Dispose();
                            CarInfoCarParts0.Dispose();

                        }

                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {

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

                            // CarPart 3 size
                            CarInfoCarParts0Writer.BaseStream.Position = 0x28;
                            CarInfoCarParts0Writer.Write(CarPart3NewChunkSize / 0x8);

                            // Write new CarID here
                            CarInfoCarParts0Writer.BaseStream.Position = 0x30;
                            CarInfoCarParts0Writer.Write(CarID);

                            // Write new Model List count here
                            CarInfoCarParts0Writer.BaseStream.Position = 0x38;
                            CarInfoCarParts0Writer.Write(CarPart4NewChunkSize / 0x24);

                            // Write new Parts Count here
                            CarInfoCarParts0Writer.BaseStream.Position = 0x40;
                            CarInfoCarParts0Writer.Write(CarPart6NewChunkSize / 0xE);


                            CarInfoCarParts0Writer.Close();
                            CarInfoCarParts0Writer.Dispose();
                            CarInfoCarParts0Reader.Close();
                            CarInfoCarParts0Reader.Dispose();
                            CarInfoCarParts0.Dispose();

                        }

                        if (CurrentGame == (int)EdTypes.Game.MostWanted)
                    {

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
                    CarInfoCarParts0Writer.Write(CarPart6NewChunkSize / 0xE);


                    CarInfoCarParts0Writer.Close();
                    CarInfoCarParts0Writer.Dispose();
                    CarInfoCarParts0Reader.Close();
                    CarInfoCarParts0Reader.Dispose();
                    CarInfoCarParts0.Dispose();

                }
                    
                    if (CurrentGame == (int)EdTypes.Game.Carbon)
                    {

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
                        CarInfoCarParts0Writer.Write(CarPart6NewChunkSize / 4);


                        CarInfoCarParts0Writer.Close();
                        CarInfoCarParts0Writer.Dispose();
                        CarInfoCarParts0Reader.Close();
                        CarInfoCarParts0Reader.Dispose();
                        CarInfoCarParts0.Dispose();

                    }

                    if (CurrentGame == (int)EdTypes.Game.ProStreet || CurrentGame == (int)EdTypes.Game.Undercover)
                    {

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
                        CarInfoCarParts0Writer.Write(CarPart6NewChunkSize / 16);


                        CarInfoCarParts0Writer.Close();
                        CarInfoCarParts0Writer.Dispose();
                        CarInfoCarParts0Reader.Close();
                        CarInfoCarParts0Reader.Dispose();
                        CarInfoCarParts0.Dispose();

                    }

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
                    if (CurrentGame != (int)EdTypes.Game.Underground2)
                    {
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\2_0003460C.bin"), true);
                    }

                    // 3
                    if (CurrentGame != (int)EdTypes.Game.Underground2)
                    {
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\3_00034605.bin"), true);
                    }

                    // 4
                    if (CurrentGame != (int)EdTypes.Game.Underground2)
                    {
                        if (!File.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin")))
                        {
                            MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource file cannot be found: " + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), "Ed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("ERROR! Ed was unable to add cars into the game.");
                            Log("Resource file cannot be found: " + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"));
                            return;
                        }
                        File.Copy(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART\4_0003460A.bin"), true);

                    }

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

                    if (CurrentGame == (int)EdTypes.Game.Underground2) // Ends at 4
                    {
                        // Fix padding
                        CarPartFileMergedWriter.BaseStream.Position = CarPartFileMergedWriter.BaseStream.Length;
                        int EndOfCarPart = (int)(CarPartFileMergedWriter.BaseStream.Position % 16);
                        PaddingDifference = (int)(CarPartMergedFileSize % 16);

                        if (PaddingDifference != 4)
                        {
                            int NumberOfBytesNeeded = EndOfCarPart - PaddingDifference;
                            if (NumberOfBytesNeeded < 0) NumberOfBytesNeeded += 16;

                            CarPartFileMergedWriter.Write((int)0);
                            CarPartFileMergedWriter.Write(NumberOfBytesNeeded - 8);
                            for (int pd = 0; pd < (NumberOfBytesNeeded - 8); pd += 4)
                            {
                                CarPartFileMergedWriter.Write((int)0);
                            }

                            CarPartMergedFileSize += NumberOfBytesNeeded;
                        }
                    }

                    if (CurrentGame == (int)EdTypes.Game.MostWanted || CurrentGame == (int)EdTypes.Game.Carbon) // Ends at C
                    {
                        // Fix padding
                        CarPartFileMergedWriter.BaseStream.Position = CarPartFileMergedWriter.BaseStream.Length;
                        int EndOfCarPart = (int)(CarPartFileMergedWriter.BaseStream.Position % 16);
                        PaddingDifference = (int)(CarPartMergedFileSize % 16);

                        if (PaddingDifference != 0xC)
                        {
                            int NumberOfBytesNeeded = EndOfCarPart - PaddingDifference;
                            if (NumberOfBytesNeeded < 0) NumberOfBytesNeeded += 16;

                            CarPartFileMergedWriter.Write((int)0);
                            CarPartFileMergedWriter.Write(NumberOfBytesNeeded - 8);
                            for (int pd = 0; pd < (NumberOfBytesNeeded - 8); pd += 4)
                            {
                                CarPartFileMergedWriter.Write((int)0);
                            }

                            CarPartMergedFileSize += NumberOfBytesNeeded;
                        }
                    }

                    if (CurrentGame == (int)EdTypes.Game.ProStreet) // Ends at 0
                    {
                        // Fix padding
                        CarPartFileMergedWriter.BaseStream.Position = CarPartFileMergedWriter.BaseStream.Length;
                        int EndOfCarPart = (int)(CarPartFileMergedWriter.BaseStream.Position % 16);
                        PaddingDifference = (int)(CarPartMergedFileSize % 16);

                        if (PaddingDifference != 0)
                        {
                            int NumberOfBytesNeeded = EndOfCarPart - PaddingDifference;
                            if (NumberOfBytesNeeded < 0) NumberOfBytesNeeded += 16;

                            CarPartFileMergedWriter.Write((int)0);
                            CarPartFileMergedWriter.Write(NumberOfBytesNeeded - 8);
                            for (int pd = 0; pd < (NumberOfBytesNeeded - 8); pd += 4)
                            {
                                CarPartFileMergedWriter.Write((int)0);
                            }

                            CarPartMergedFileSize += NumberOfBytesNeeded;
                        }
                    }

                    if (CurrentGame == (int)EdTypes.Game.Undercover
                            || CurrentGame == (int)EdTypes.Game.Underground) // Ends at 8
                    {
                        // Fix padding
                        CarPartFileMergedWriter.BaseStream.Position = CarPartFileMergedWriter.BaseStream.Length;
                        int EndOfCarPart = (int)(CarPartFileMergedWriter.BaseStream.Position % 16);
                        PaddingDifference = (int)(CarPartMergedFileSize % 16);

                        if (PaddingDifference != 8)
                        {
                            int NumberOfBytesNeeded = EndOfCarPart - PaddingDifference;
                            if (NumberOfBytesNeeded < 0) NumberOfBytesNeeded += 16;

                            CarPartFileMergedWriter.Write((int)0);
                            CarPartFileMergedWriter.Write(NumberOfBytesNeeded - 8);
                            for (int pd = 0; pd < (NumberOfBytesNeeded - 8); pd += 4)
                            {
                                CarPartFileMergedWriter.Write((int)0);
                            }

                            CarPartMergedFileSize += NumberOfBytesNeeded;
                        }
                    }

                    CarPartFileMergedWriter.BaseStream.Position = 4;
                    CarPartFileMergedWriter.Write(CarPartMergedFileSize);

                    CarPartFileMergedWriter.Close();
                    CarPartFileMergedWriter.Dispose();
                    CarPartFileMerged.Close();
                    CarPartFileMerged.Dispose();


                        // -----------------------------------------------------

                        // Collision : Copy and merge

                        string BoundsDirectory = @"Global\BCHUNK_BOUNDS";

                        if (!Directory.Exists(Path.Combine(GetResourcesPath(), BoundsDirectory)))
                        {
                            Log("Skipping BCHUNK_BOUNDS. Temporary resource folder doesn't exist:" + Environment.NewLine + Path.Combine(GetResourcesPath(), BoundsDirectory));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.Combine(GetTempPath(), BoundsDirectory));

                            DirectoryInfo TempCollisions = new DirectoryInfo(Path.Combine(GetResourcesPath(), BoundsDirectory));
                            FileInfo[] TempCollisionBINFiles = TempCollisions.GetFiles("*.bin");
                            if (TempCollisionBINFiles.Length == 0)
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), BoundsDirectory));
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), BoundsDirectory));
                                return;
                            }

                            // Copy to temp
                            foreach (FileInfo CollisionFile in TempCollisionBINFiles)
                            {
                                File.Copy(Path.Combine(GetResourcesPath(), BoundsDirectory, CollisionFile.Name), Path.Combine(GetTempPath(), BoundsDirectory, CollisionFile.Name), true);
                            }


                            // Copy the new ones with new names and fix their hashes
                            foreach (EdTypes.CarCollision Coll in NewCollisionList)
                            {
                                if (string.IsNullOrEmpty(Coll.CopyFrom)) continue; // ignore if it's not found in config
                                if (Coll.CopyFrom == Coll.CopyTo) continue; // ignore if user tries to copy an existing collision with the same name
                                if (!File.Exists(Path.Combine(GetTempPath(), BoundsDirectory, Coll.CopyFrom + ".bin"))) // Skip adding collision if data cannot be found
                                {
                                    continue;
                                }
                                File.Copy(Path.Combine(GetTempPath(), BoundsDirectory, Coll.CopyFrom + ".bin"), Path.Combine(GetTempPath(), BoundsDirectory, Coll.CopyTo + ".bin"), true);

                                var NewCollisionFile = File.Open(Path.Combine(GetTempPath(), BoundsDirectory, Coll.CopyTo + ".bin"), FileMode.Open, FileAccess.Write);
                                var NewCollisionFileWriter = new BinaryWriter(NewCollisionFile);

                                if (CurrentGame == (int)EdTypes.Game.Undercover)
                                {
                                    NewCollisionFileWriter.BaseStream.Position = 0x40;
                                    NewCollisionFileWriter.Write(JenkinsHash.getHash32(Coll.CopyTo)); // Write Jenkins hash of the car folder.

                                    NewCollisionFileWriter.BaseStream.Position = 0x70;
                                    byte[] CollCopyToGetXName = Encoding.ASCII.GetBytes(Coll.CopyTo); // Write XNAME
                                    Array.Resize(ref CollCopyToGetXName, 0x10);
                                    NewCollisionFileWriter.Write(CollCopyToGetXName);
                                }
                                else
                                {
                                    NewCollisionFileWriter.BaseStream.Position = 16;
                                    NewCollisionFileWriter.Write(JenkinsHash.getHash32(Coll.CopyTo)); // Write Jenkins hash of the car folder.
                                }


                                NewCollisionFileWriter.Close();
                                NewCollisionFileWriter.Dispose();
                                NewCollisionFile.Close();
                                NewCollisionFile.Dispose();
                            }


                            // Merge

                            DirectoryInfo TempCarColl = new DirectoryInfo(Path.Combine(GetTempPath(), BoundsDirectory));
                            FileInfo[] TempCarCollBINFiles = TempCarColl.GetFiles("*.bin");
                            if (TempCarCollBINFiles.Length == 0)
                            {
                                MessageBox.Show("Ed was unable to add cars into the game." + Environment.NewLine + "Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), BoundsDirectory));
                                Log("ERROR! Ed was unable to add cars into the game.");
                                Log("Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetTempPath(), BoundsDirectory));
                                return;
                            }

                            var CarCollFileMerged = File.Create(Path.Combine(GetTempPath(), BoundsDirectory + ".bin"));
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
                            /*
                            PaddingDifference = (int)(CarCollMergedFileSize % 16);

                            while (PaddingDifference != 0)
                            {
                                CarCollFileMergedWriter.Write((byte)0);
                                PaddingDifference = (PaddingDifference + 1) % 16;
                                CarCollMergedFileSize++;
                            }
                            */
                            CarCollFileMergedWriter.BaseStream.Position = 4;
                            CarCollFileMergedWriter.Write(CarCollMergedFileSize);

                            CarCollFileMergedWriter.Close();
                            CarCollFileMergedWriter.Dispose();
                            CarCollFileMerged.Close();
                            CarCollFileMerged.Dispose();
                        }


                        // -----------------------------------------------------

                        // Preset Skins

                        string PresetSkinDirectory = Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETSKINS");

                        if (Directory.Exists(PresetSkinDirectory))
                        {
                            if (CurrentGame == (int)EdTypes.Game.Carbon)
                            {
                                DirectoryInfo ResCarPresetSkin = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETSKINS"));
                                FileInfo[] CarPresetSkinFiles = ResCarPresetSkin.GetFiles("*.bin");
                                if (CarPresetSkinFiles.Length == 0)
                                {
                                    Log("Skipping BCHUNK_FEPRESETSKINS. Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETSKINS"));
                                }
                                else
                                {
                                    var CarPresetSkinFileMerged = File.Create(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETSKINS.bin"));
                                    var CarPresetSkinFileMergedWriter = new BinaryWriter(CarPresetSkinFileMerged);

                                    // Reserve first 8 bytes for chunk header and size
                                    CarPresetSkinFileMergedWriter.BaseStream.Position = 0;
                                    CarPresetSkinFileMergedWriter.Write(0x00030250);
                                    CarPresetSkinFileMergedWriter.Write(-1);

                                    int CarPresetSkinFileSize = 0;
                                    int SinglePresetSkinSize = 104;

                                    foreach (FileInfo BINFile in CarPresetSkinFiles)
                                    {
                                        byte[] buf = File.ReadAllBytes(BINFile.FullName);

                                        if (buf.Length == SinglePresetSkinSize)
                                        {
                                            CarPresetSkinFileMerged.Write(buf, 0, buf.Length);
                                            CarPresetSkinFileSize += buf.Length;
                                        }
                                        else Log("Skipping " + BINFile.FullName + ". " + "The file isn't compatible with current game (size mismatch).");
                                    }
                                    
                                    // Fix padding
                                    PaddingDifference = (int)(CarPresetSkinFileSize % 16);

                                    while (PaddingDifference != 0)
                                    {
                                        CarPresetSkinFileMergedWriter.Write((byte)0);
                                        PaddingDifference = (PaddingDifference + 1) % 16;
                                        CarPresetSkinFileSize++;
                                    }
                                    
                                    CarPresetSkinFileMergedWriter.BaseStream.Position = 4;
                                    CarPresetSkinFileMergedWriter.Write(CarPresetSkinFileSize);

                                    CarPresetSkinFileMergedWriter.Close();
                                    CarPresetSkinFileMergedWriter.Dispose();
                                    CarPresetSkinFileMerged.Close();
                                    CarPresetSkinFileMerged.Dispose();
                                }
                            }

                        }

                        else
                        {
                            Log("Skipping BCHUNK_FEPRESETSKINS. Temporary resource folder doesn't exist:" + Environment.NewLine + PresetSkinDirectory);
                        }

                        // -----------------------------------------------------

                        // Preset Cars

                        if (Directory.Exists(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETCARS")))
                        {
                            if (CurrentGame == (int)EdTypes.Game.Underground2
                            || CurrentGame == (int)EdTypes.Game.MostWanted
                            || CurrentGame == (int)EdTypes.Game.Carbon)
                            {
                                DirectoryInfo ResPresetCar = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETCARS"));
                                FileInfo[] CarPresetFiles = ResPresetCar.GetFiles("*.bin");
                                if (CarPresetFiles.Length == 0)
                                {
                                    Log("Skipping BCHUNK_FEPRESETCARS. Temporary resource folder is empty:" + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETCARS"));
                                }
                                else
                                {
                                    var CarPresetFileMerged = File.Create(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETCARS.bin"));
                                    var CarPresetFileMergedWriter = new BinaryWriter(CarPresetFileMerged);

                                    // Reserve first 8 bytes for chunk header and size
                                    CarPresetFileMergedWriter.BaseStream.Position = 0;
                                    CarPresetFileMergedWriter.Write(0x00030220);
                                    CarPresetFileMergedWriter.Write(-1);

                                    int CarPresetFilesize = 0;
                                    int SinglePresetCarSize = 0;

                                    foreach (FileInfo BINFile in CarPresetFiles)
                                    {
                                        byte[] buf = File.ReadAllBytes(BINFile.FullName);

                                        switch (CurrentGame)
                                        {
                                            case (int)EdTypes.Game.Underground2: SinglePresetCarSize = 824; break;
                                            case (int)EdTypes.Game.MostWanted: SinglePresetCarSize = 656; break;
                                            case (int)EdTypes.Game.Carbon: SinglePresetCarSize = 1536; break;
                                            default: SinglePresetCarSize = 0; break;
                                        }

                                        if (SinglePresetCarSize != 0)
                                        {
                                            if (buf.Length == SinglePresetCarSize)
                                            {
                                                CarPresetFileMerged.Write(buf, 0, buf.Length);
                                                CarPresetFilesize += buf.Length;
                                            }
                                            else Log("Skipping " + BINFile.FullName + ". " + "The file isn't compatible with current game (size mismatch).");
                                        }
                                    }

                                    if (CurrentGame == (int)EdTypes.Game.Underground2)
                                    {
                                        PaddingDifference = (int)(CarPresetFilesize % 16);

                                        while (PaddingDifference != 0)
                                        {
                                            CarPresetFileMergedWriter.Write((byte)0);
                                            PaddingDifference = (PaddingDifference + 1) % 16;
                                            CarPresetFilesize++;
                                        }
                                    }

                                    CarPresetFileMergedWriter.BaseStream.Position = 4;
                                    CarPresetFileMergedWriter.Write(CarPresetFilesize);

                                    CarPresetFileMergedWriter.Close();
                                    CarPresetFileMergedWriter.Dispose();
                                    CarPresetFileMerged.Close();
                                    CarPresetFileMerged.Dispose();
                                }
                            }
                        }

                        else
                        {
                            Log("Skipping BCHUNK_FEPRESETCARS. Temporary resource folder doesn't exist:" + Environment.NewLine + Path.Combine(GetResourcesPath(), @"Global\BCHUNK_FEPRESETCARS"));
                        }

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
                    string GlobalBBUNPath = Path.Combine(WorkingFolder, @"Global\GlobalB.bun");

                    // Make a backup (or restore if it exists)
                    if (!DisableBackups)
                    {
                        if (!File.Exists(GlobalBPath + ".edbackup")) File.Copy(GlobalBPath, GlobalBPath + ".edbackup");
                        else if (AutoRestoreGlobalB) File.Copy(GlobalBPath + ".edbackup", GlobalBPath, true);
                    }

                    if (CurrentGame == (int)EdTypes.Game.ProStreet || CurrentGame == (int)EdTypes.Game.Undercover)
                    {
                        if (File.Exists(GlobalBBUNPath)) File.Copy(GlobalBBUNPath, GlobalBPath, true);
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
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin")))
                                {
                                    byte[] bufCarInfoArray = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_ARRAY.bin"));
                                    GlobalBWriter.Write(bufCarInfoArray, 0, bufCarInfoArray.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_CARINFO_ARRAY into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x00034607:  // Write SlotTypes (177)
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin")))
                                {
                                    byte[] bufSlotTypes = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_SLOTTYPES.bin"));
                                    GlobalBWriter.Write(bufSlotTypes, 0, bufSlotTypes.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_CARINFO_SLOTTYPES into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x80034602: // Write CarPart (178)
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART.bin")))
                                {
                                    byte[] bufCarPart = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_CARINFO_CARPART.bin"));
                                    GlobalBWriter.Write(bufCarPart, 0, bufCarPart.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_CARINFO_CARPART into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x8003b900: // Write Bounds (181)
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS.bin")))
                                {
                                    byte[] bufBounds = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_BOUNDS.bin"));
                                    GlobalBWriter.Write(bufBounds, 0, bufBounds.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_BOUNDS into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x00030250: // Write Preset Skins
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETSKINS.bin")))
                                {
                                    byte[] bufBounds = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETSKINS.bin"));
                                    GlobalBWriter.Write(bufBounds, 0, bufBounds.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_FEPRESETSKINS into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x00030220: // Write Preset Cars
                                if (File.Exists(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETCARS.bin")))
                                {
                                    byte[] bufBounds = File.ReadAllBytes(Path.Combine(GetTempPath(), @"Global\BCHUNK_FEPRESETCARS.bin"));
                                    GlobalBWriter.Write(bufBounds, 0, bufBounds.Length);
                                    GlobalBReader.BaseStream.Position += WriterChunkSize; // for other chunks
                                    Log("Wrote BCHUNK_FEPRESETCARS into GlobalB.lzc.");
                                    break;
                                }
                                else goto default;

                            case 0x0003bd00: // BCHUNK_EMITTERSYSTEM_TEXTUREPAGE

                            case 0x80134000: // BCHUNK_SPEED_ESOLID_LIST_CHUNKS

                            case 0xb3300000: // BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS
                                //goto case 0xb0300100; // pseudo fall-through :(

                            case 0xb0300100: // BCHUNK_SPEED_TEXTURE_PACK_LIST_CHUNKS_ANIM
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
                    GlobalBWriter.Dispose();

                    GlobalBReader.Close();
                    GlobalBReader.Dispose();

                    GlobalB.Close();
                    GlobalB.Dispose();


                    // -----------------------------------------------------

                    // Language : Copy and merge

                    if (Ed.Default.DoStrings == true)
                    {
                        if (CurrentGame == (int)EdTypes.Game.MostWanted || CurrentGame == (int)EdTypes.Game.Underground || CurrentGame == (int)EdTypes.Game.Underground2)
                        {
                            Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Languages"));

                            DirectoryInfo TempLanguages = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Languages"));
                            FileInfo[] TempLanguageBINFiles = TempLanguages.GetFiles("*.bin");

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
                                Language.UnknownData = LangFileReader.ReadInt32();
                                Language.NumberOfEntries = LangFileReader.ReadInt32();
                                Language.HashBlockPosition = LangFileReader.ReadInt32();
                                Language.TextBlockPosition = LangFileReader.ReadInt32();

                                var UnkData1 = LangFileReader.ReadBytes(Language.HashBlockPosition - 16); // Unknown Data before Hash List

                                var HashList = new List<LanguageFile.HashInfo>();

                                LangFileReader.BaseStream.Position = 8 + Language.HashBlockPosition;

                                for (int l = 0; l < Language.NumberOfEntries; l++)
                                {
                                    var AHashInfo = new LanguageFile.HashInfo();

                                    AHashInfo.StringHash = LangFileReader.ReadUInt32();
                                    AHashInfo.OffsetInTextBlock = LangFileReader.ReadUInt32();

                                    HashList.Add(AHashInfo);
                                }

                                var CurrentTextBlock = LangFileReader.ReadBytes((Language.ChunkSize - Language.TextBlockPosition));
                                LangFileReader.BaseStream.Position = Language.ChunkSize + 8;
                                var UnkData2 = LangFileReader.ReadBytes((int)(LangFileReader.BaseStream.Length - LangFileReader.BaseStream.Position)); // Unknown Data after strings

                                var ByteArrayForNewStrings = new List<byte>();

                                foreach (EdTypes.CarResource CarRes in NewResourcesList)
                                {
                                    if (string.IsNullOrEmpty(CarRes.Label) || string.IsNullOrEmpty(CarRes.Name)) continue;

                                    var AHashInfo = new LanguageFile.HashInfo();
                                    AHashInfo.StringHash = (uint)BinHash.Hash(CarRes.Label);
                                    AHashInfo.OffsetInTextBlock = (uint)(Language.ChunkSize - Language.TextBlockPosition); // The address string will start from

                                    if (!HashList.Exists(x => x.StringHash == AHashInfo.StringHash))
                                    {
                                        HashList.Add(AHashInfo);
                                        Language.NumberOfEntries++;
                                        Language.TextBlockPosition += 8;
                                        Language.ChunkSize += 8;

                                        if (LangFile.Name.ToUpper(new CultureInfo("en-US", false)) == "LABELS.BIN")
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Label)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Label).Count() + 1;
                                        }
                                        else
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Name)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Name).Count() + 1;
                                        }
                                    }

                                }

                                // Sort by hash
                                HashList.Sort((x, y) => x.StringHash.CompareTo(y.StringHash));

                                // Start Writing New File
                                NewLangFileWriter.BaseStream.Position = 0;

                                NewLangFileWriter.Write(Language.ChunkID);
                                NewLangFileWriter.Write(Language.ChunkSize);
                                NewLangFileWriter.Write(Language.UnknownData);
                                NewLangFileWriter.Write(Language.NumberOfEntries);
                                NewLangFileWriter.Write(Language.HashBlockPosition);
                                NewLangFileWriter.Write(Language.TextBlockPosition);
                                NewLangFileWriter.Write(UnkData1);

                                // Write hash block
                                foreach (LanguageFile.HashInfo i in HashList)
                                {
                                    NewLangFileWriter.Write(i.StringHash);
                                    NewLangFileWriter.Write(i.OffsetInTextBlock);
                                }

                                // Write original string info
                                NewLangFileWriter.Write(CurrentTextBlock);

                                foreach (byte i in ByteArrayForNewStrings)
                                {
                                    NewLangFileWriter.Write(i);
                                }

                                // Fix padding
                                NewLangFileWriter.BaseStream.Position = NewLangFileWriter.BaseStream.Length;
                                PaddingDifference = (Language.ChunkSize % 4);

                                while (PaddingDifference != 0)
                                {
                                    NewLangFileWriter.Write((byte)0);
                                    PaddingDifference = (PaddingDifference + 1) % 4;
                                    Language.ChunkSize++;
                                }

                                // Fix chunk size
                                NewLangFileWriter.BaseStream.Position = 4;
                                NewLangFileWriter.Write(Language.ChunkSize);

                                // Write the rest of the data
                                NewLangFileWriter.BaseStream.Position = Language.ChunkSize + 8;
                                NewLangFileWriter.Write(UnkData2);

                                // Close streams
                                LangFileReader.Close();
                                LangFileReader.Dispose();

                                LangFileMem.Close();
                                LangFileMem.Dispose();

                                NewLangFileWriter.Close();
                                NewLangFileWriter.Dispose();

                                NewLangFile.Close();
                                NewLangFile.Dispose();

                                // Copy the files in
                                // Make a backup
                                if (!DisableBackups)
                                {
                                    if ((!File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"))) && File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name)))
                                        File.Copy(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"), true);
                                }
                                // Copy
                                File.Copy(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), true);
                                Log("Successfully rebuilt " + LangFile.Name + " file.");
                            }
                        }

                        if (CurrentGame == (int)EdTypes.Game.Carbon)
                        {
                            Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Languages"));

                            DirectoryInfo TempLanguages = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Languages"));
                            FileInfo[] TempLanguageBINFiles = TempLanguages.GetFiles("*_Frontend.bin");

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

                                for (int l = 0; l < Language.NumberOfEntries; l++)
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

                                    if (!HashList.Exists(x => x.StringHash == AHashInfo.StringHash))
                                    {
                                        HashList.Add(AHashInfo);
                                        Language.NumberOfEntries++;
                                        Language.TextBlockPosition += 8;
                                        Language.ChunkSize += 8;

                                        if (LangFile.Name.ToUpper(new CultureInfo("en-US", false)) == "LABELS_FRONTEND.BIN")
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Label)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Label).Count() + 1;
                                        }
                                        else
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Name)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Name).Count() + 1;
                                        }
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
                                NewLangFileWriter.BaseStream.Position = NewLangFileWriter.BaseStream.Length;
                                PaddingDifference = (Language.ChunkSize % 4);

                                while (PaddingDifference != 0)
                                {
                                    NewLangFileWriter.Write((byte)0);
                                    PaddingDifference = (PaddingDifference + 1) % 4;
                                    Language.ChunkSize++;
                                }

                                // Fix chunk size
                                NewLangFileWriter.BaseStream.Position = 4;
                                NewLangFileWriter.Write(Language.ChunkSize);

                                // Close streams
                                LangFileReader.Close();
                                LangFileReader.Dispose();

                                LangFileMem.Close();
                                LangFileMem.Dispose();

                                NewLangFileWriter.Close();
                                NewLangFileWriter.Dispose();

                                NewLangFile.Close();
                                NewLangFile.Dispose();

                                // Copy the files in
                                // Make a backup
                                if (!DisableBackups)
                                {
                                    if ((!File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"))) && File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name)))
                                        File.Copy(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"), true);
                                }
                                // Copy
                                File.Copy(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), true);
                                Log("Successfully rebuilt " + LangFile.Name + " file.");
                            }

                        }

                        if (CurrentGame == (int)EdTypes.Game.ProStreet
                            || CurrentGame == (int)EdTypes.Game.Undercover)
                        {
                            Directory.CreateDirectory(Path.Combine(GetTempPath(), @"Languages"));

                            DirectoryInfo TempLanguages = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"Languages"));
                            FileInfo[] TempLanguageBINFiles = TempLanguages.GetFiles("*_Global.bin");

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

                                for (int l = 0; l < Language.NumberOfEntries; l++)
                                {
                                    var AHashInfo = new LanguageFile.HashInfo();

                                    AHashInfo.StringHash = LangFileReader.ReadUInt32();
                                    AHashInfo.OffsetInTextBlock = LangFileReader.ReadUInt32();

                                    HashList.Add(AHashInfo);
                                }

                                var CurrentTextBlock = LangFileReader.ReadBytes((Language.ChunkSize - Language.TextBlockPosition));
                                LangFileReader.BaseStream.Position = Language.ChunkSize + 8;
                                var UnkData2 = LangFileReader.ReadBytes((int)(LangFileReader.BaseStream.Length - LangFileReader.BaseStream.Position)); // Unknown Data after strings

                                var ByteArrayForNewStrings = new List<byte>();

                                foreach (EdTypes.CarResource CarRes in NewResourcesList)
                                {
                                    if (string.IsNullOrEmpty(CarRes.Label) || string.IsNullOrEmpty(CarRes.Name)) continue;

                                    var AHashInfo = new LanguageFile.HashInfo();
                                    AHashInfo.StringHash = (uint)BinHash.Hash(CarRes.Label);
                                    AHashInfo.OffsetInTextBlock = (uint)(Language.ChunkSize - Language.TextBlockPosition); // The address string will start from

                                    if (!HashList.Exists(x => x.StringHash == AHashInfo.StringHash))
                                    {
                                        HashList.Add(AHashInfo);
                                        Language.NumberOfEntries++;
                                        Language.TextBlockPosition += 8;
                                        Language.ChunkSize += 8;

                                        if (LangFile.Name.ToUpper(new CultureInfo("en-US", false)) == "LABELS_GLOBAL.BIN")
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Label)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Label).Count() + 1;
                                        }
                                        else
                                        {
                                            ByteArrayForNewStrings.AddRange(Encoding.UTF8.GetBytes(CarRes.Name)); // String files are coded as UTF-8
                                            ByteArrayForNewStrings.Add(0);
                                            Language.ChunkSize += Encoding.UTF8.GetBytes(CarRes.Name).Count() + 1;
                                        }
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

                                // Read original string info
                                NewLangFileWriter.Write(CurrentTextBlock);

                                foreach (byte i in ByteArrayForNewStrings)
                                {
                                    NewLangFileWriter.Write(i);
                                }

                                // Fix padding
                                NewLangFileWriter.BaseStream.Position = NewLangFileWriter.BaseStream.Length;
                                PaddingDifference = (Language.ChunkSize % 4);

                                while (PaddingDifference != 0)
                                {
                                    NewLangFileWriter.Write((byte)0);
                                    PaddingDifference = (PaddingDifference + 1) % 4;
                                    Language.ChunkSize++;
                                }

                                // Fix chunk size
                                NewLangFileWriter.BaseStream.Position = 4;
                                NewLangFileWriter.Write(Language.ChunkSize);

                                // Write the rest of the data
                                NewLangFileWriter.BaseStream.Position = Language.ChunkSize + 8;
                                NewLangFileWriter.Write(UnkData2);

                                // Close streams
                                LangFileReader.Close();
                                LangFileReader.Dispose();

                                LangFileMem.Close();
                                LangFileMem.Dispose();

                                NewLangFileWriter.Close();
                                NewLangFileWriter.Dispose();

                                NewLangFile.Close();
                                NewLangFile.Dispose();

                                // Copy the files in
                                // Make a backup
                                if (!DisableBackups)
                                {
                                    if ((!File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"))) && File.Exists(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name)))
                                        File.Copy(Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name + ".edbackup"), true);
                                }
                                // Copy
                                File.Copy(Path.Combine(GetTempPath(), @"Languages\" + LangFile.Name), Path.Combine(WorkingFolder, @"Languages\" + LangFile.Name), true);
                                Log("Successfully rebuilt " + LangFile.Name + " file.");
                            }

                        }

                    }
                    else Log("Skipped string adding, it's disabled from options.");

                    // -----------------------------------------------------

                    // Rebuild TPK file with new textures

                    if (Ed.Default.DoTextures == true)
                    {
                        if (CurrentGame == (int)EdTypes.Game.Underground2)
                        {
                            DirectoryInfo FrontEndTexturesTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD"));
                            FileInfo[] FrontEndTexturesDDSFiles = FrontEndTexturesTPK.GetFiles("*.dds", SearchOption.AllDirectories);
                            if (FrontEndTexturesDDSFiles.Length == 0)
                            {
                                goto DoneMessage;
                            }

                            // Return to original file before any modifications. (To prevent overlapping and stuff)
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD_orig.ini"), Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"), true);

                            // Check if required info exists in the config file. If not, add it.
                            var TPKInfoSection = new XNFSTPKToolWrapper.TPKSection();
                            var AnimationSections = new List<XNFSTPKToolWrapper.AnimationSection>();
                            var TextureSections = new List<XNFSTPKToolWrapper.TextureSectionTPKv3>();

                            using (var reader = new StreamReader(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"))) // sorry, quick and dirty
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

                                            line = reader.ReadLine();
                                            TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            /*
                                            line = reader.ReadLine();
                                            TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            */
                                            TextureSections.Add(TexSec);
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                            }

                            foreach (FileInfo DDSFile in FrontEndTexturesDDSFiles) // names in folder
                            {
                                bool IsThereAConfigForThisDDS = false;

                                // Find if the texture exists in the config file (at least once)
                                foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)  // names in ini
                                {
                                    if (TexSec.File == (@"FrontEndTextures\729181AD\" + DDSFile.Name))
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
                                    ANewTexSec.File = @"FrontEndTextures\729181AD\" + DDSFile.Name;
                                    ANewTexSec.Name = Path.GetFileNameWithoutExtension(DDSFile.Name);
                                    ANewTexSec.Hash2 = 0x1A93CF;

                                    // Get texture dimensions for some quick maths
                                    var DDSFileToGetDimensions = File.Open(DDSFile.FullName, FileMode.Open);
                                    var DDSFileReader = new BinaryReader(DDSFileToGetDimensions);

                                    DDSFileReader.BaseStream.Position = 0x0C;
                                    int VerRes = DDSFileReader.ReadInt32();
                                    int HorRes = DDSFileReader.ReadInt32();

                                    DDSFileReader.Close();
                                    DDSFileReader.Dispose();
                                    DDSFileToGetDimensions.Close();
                                    DDSFileToGetDimensions.Dispose();

                                    ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                    ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                    ANewTexSec.UnkByte3 = 0x00;

                                    ANewTexSec.Unknown1 = 0x10000; // temp
                                    ANewTexSec.Unknown3 = 0;
                                    ANewTexSec.Unknown4 = 0x24;
                                    ANewTexSec.Unknown5 = 0x700;
                                    ANewTexSec.Unknown6 = 0x10200;
                                    ANewTexSec.Unknown7 = 0x100;
                                    ANewTexSec.Unknown8 = 0;
                                    ANewTexSec.Unknown9 = 0x1000000;
                                    ANewTexSec.Unknown10 = 0x100;
                                    ANewTexSec.Unknown11 = 0x01;
                                    ANewTexSec.Unknown12 = 0x05;
                                    ANewTexSec.Unknown17 = 0x06;
                                    ANewTexSec.Unknown18 = 0x00;

                                    TextureSections.Add(ANewTexSec);
                                }

                            }

                            // Sort Texture Sections by hash
                            TextureSections.Sort((x, y) => x.Hash.CompareTo(y.Hash));

                            // Rebuild the ini file
                            using (var IniWriter = new StreamWriter(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"), false))
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
                                    IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown17.ToString("X"));
                                    IniWriter.WriteLine(); // Seperate sections with empty lines
                                }

                                IniWriter.Close();
                                IniWriter.Dispose();
                            }

                            Log("Rebuilding FrontEndTextures.tpk for FrontB.lzc using XNFSTpkTool...");

                            using (Process process = new Process())
                            {
                                process.StartInfo.FileName = Path.Combine(GetResourcesPath(), @"FrontEnd", @"XNFSTPKTool.exe");
                                process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                                process.StartInfo.Arguments = @"-w2 FrontEndTextures\729181AD.ini FrontEndTextures.tpk";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                Log(process.StandardOutput.ReadToEnd());

                                process.WaitForExit();
                            }

                            if (File.Exists(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")))
                            {
                                // merge other chunks in
                                var FrontBFile = File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontB.lzc"));
                                var FrontAFile = File.Create(Path.Combine(GetTempPath(), @"FrontEnd\FrontB.lzc"));
                                var FrontAFileWriter = new BinaryWriter(FrontAFile);
                                var FrontBFileReader = new BinaryReader(new MemoryStream(FrontBFile));

                                bool DidWeWriteTheTextureChunk = false;

                                // Write our new TPK
                                while (FrontBFileReader.BaseStream.Position < FrontBFileReader.BaseStream.Length)
                                {
                                    uint WriterChunkOffset = (uint)FrontBFileReader.BaseStream.Position;
                                    uint WriterChunkID = FrontBFileReader.ReadUInt32();
                                    int WriterChunkSize = FrontBFileReader.ReadInt32();

                                    switch (WriterChunkID)
                                    {
                                        case 0xb3300000:
                                            goto case 0xb0300100; // pseudo fall-through :(

                                        case 0xb0300100: // Texture chunks
                                                            // Fix starting position (They should start at an offset % 0x80 = 0)
                                            PaddingDifference = (int)(FrontAFileWriter.BaseStream.Position % 128);

                                            while (PaddingDifference != 0)
                                            {
                                                FrontAFileWriter.Write((byte)0);
                                                PaddingDifference = (PaddingDifference + 1) % 128;
                                            }

                                            if (DidWeWriteTheTextureChunk == false)
                                            {
                                                //write
                                                FrontAFileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk"))); // Write new textures
                                                FrontBFileReader.BaseStream.Position += WriterChunkSize; // Advance the reader
                                                DidWeWriteTheTextureChunk = true;
                                                break;
                                            }
                                            else goto default; // pseudo fall-through :(

                                        default: // copy data
                                            FrontAFileWriter.Write(WriterChunkID);
                                            FrontAFileWriter.Write(WriterChunkSize);
                                            FrontAFileWriter.Write(FrontBFileReader.ReadBytes((WriterChunkSize)));
                                            break;
                                    }
                                }

                                // close stream
                                FrontAFileWriter.Close();
                                FrontAFileWriter.Dispose();
                                FrontBFileReader.Close();
                                FrontBFileReader.Dispose();
                                FrontAFile.Close();
                                FrontAFile.Dispose();

                                Log("Successfully rebuilt FrontB.lzc file.");
                            }

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if ((!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB.lzc" + ".edbackup"))) && File.Exists((Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB.lzc"))))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB.lzc" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetTempPath(), @"FrontEnd\" + "FrontB.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB.lzc"), true);


                        }


                        if (CurrentGame == (int)EdTypes.Game.MostWanted)
                        {
                            DirectoryInfo FrontEndTexturesTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD"));
                            FileInfo[] FrontEndTexturesDDSFiles = FrontEndTexturesTPK.GetFiles("*.dds", SearchOption.AllDirectories);
                            if (FrontEndTexturesDDSFiles.Length == 0)
                            {
                                goto DoneMessage;
                            }

                            // Return to original file before any modifications. (To prevent overlapping and stuff)
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD_orig.ini"), Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"), true);

                            // Check if required info exists in the config file. If not, add it.
                            var TPKInfoSection = new XNFSTPKToolWrapper.TPKSection();
                            var AnimationSections = new List<XNFSTPKToolWrapper.AnimationSection>();
                            var TextureSections = new List<XNFSTPKToolWrapper.TextureSectionTPKv3>();

                            using (var reader = new StreamReader(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"))) // sorry, quick and dirty
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

                                            line = reader.ReadLine();
                                            TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            /*
                                            line = reader.ReadLine();
                                            TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            */
                                            TextureSections.Add(TexSec);
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                            }

                            foreach (FileInfo DDSFile in FrontEndTexturesDDSFiles) // names in folder
                            {
                                bool IsThereAConfigForThisDDS = false;

                                // Find if the texture exists in the config file (at least once)
                                foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)  // names in ini
                                {
                                    if (TexSec.File == (@"FrontEndTextures\729181AD\" + DDSFile.Name))
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
                                    ANewTexSec.File = @"FrontEndTextures\729181AD\" + DDSFile.Name;
                                    ANewTexSec.Name = Path.GetFileNameWithoutExtension(DDSFile.Name);
                                    ANewTexSec.Hash2 = 0x1A93CF;

                                    // Get texture dimensions for some quick maths
                                    var DDSFileToGetDimensions = File.Open(DDSFile.FullName, FileMode.Open);
                                    var DDSFileReader = new BinaryReader(DDSFileToGetDimensions);

                                    DDSFileReader.BaseStream.Position = 0x0C;
                                    int VerRes = DDSFileReader.ReadInt32();
                                    int HorRes = DDSFileReader.ReadInt32();

                                    DDSFileReader.Close();
                                    DDSFileReader.Dispose();
                                    DDSFileToGetDimensions.Close();
                                    DDSFileToGetDimensions.Dispose();

                                    ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                    ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                    ANewTexSec.UnkByte3 = 0x00;
                                    /*
                                    ANewTexSec.Unknown1 = 0x00; // temp
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
                                    */

                                    ANewTexSec.Unknown1 = 0x10000; // temp
                                    ANewTexSec.Unknown3 = 0;
                                    ANewTexSec.Unknown4 = 0x24;
                                    ANewTexSec.Unknown5 = 0x500;
                                    ANewTexSec.Unknown6 = 0x10200;
                                    ANewTexSec.Unknown7 = 0x100;
                                    ANewTexSec.Unknown8 = 0;
                                    ANewTexSec.Unknown9 = 0x1000000;
                                    ANewTexSec.Unknown10 = 0x100;
                                    ANewTexSec.Unknown11 = 0x01;
                                    ANewTexSec.Unknown12 = 0x05;
                                    ANewTexSec.Unknown17 = 0x06;
                                    ANewTexSec.Unknown18 = 0x00;

                                    TextureSections.Add(ANewTexSec);
                                }

                            }

                            // Sort Texture Sections by hash
                            TextureSections.Sort((x, y) => x.Hash.CompareTo(y.Hash));

                            // Rebuild the ini file
                            using (var IniWriter = new StreamWriter(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\729181AD.ini"), false))
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
                                    IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown17.ToString("X"));
                                    IniWriter.WriteLine(); // Seperate sections with empty lines
                                }

                                IniWriter.Close();
                                IniWriter.Dispose();
                            }

                            Log("Rebuilding FrontEndTextures.tpk for FrontA.bun using XNFSTpkTool...");

                            using (Process process = new Process())
                            {
                                process.StartInfo.FileName = Path.Combine(GetResourcesPath(), @"FrontEnd", @"XNFSTPKTool.exe");
                                process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                                process.StartInfo.Arguments = @"-w2 FrontEndTextures\729181AD.ini FrontEndTextures.tpk";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                Log(process.StandardOutput.ReadToEnd());

                                process.WaitForExit();
                            }

                            if (File.Exists(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")))
                            {
                                // merge other chunks in
                                var FrontAFile = File.Create(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontA.bun"));
                                var FrontAFileWriter = new BinaryWriter(FrontAFile);

                                // Write our new TPK
                                FrontAFileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")));

                                // Write the rest
                                FrontAFileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FONT_MW_TITLE.bin")));
                                FrontAFileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FONT_MW_CUSTOM_NUMBERS.bin")));

                                // close stream
                                FrontAFileWriter.Close();
                                FrontAFileWriter.Dispose();
                                FrontAFile.Close();
                                FrontAFile.Dispose();

                                Log("Successfully rebuilt FrontA.bun file.");
                            }

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if ((!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontA.bun" + ".edbackup"))) && File.Exists((Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontA.bun"))))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontA.bun"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontA.bun" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\" + "FrontA.bun"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontA.bun"), true);


                        }

                        if (CurrentGame == (int)EdTypes.Game.Carbon)
                        {
                            DirectoryInfo FrontEndTexturesTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\8D08770D"));
                            FileInfo[] FrontEndTexturesDDSFiles = FrontEndTexturesTPK.GetFiles("*.dds", SearchOption.AllDirectories);
                            if (FrontEndTexturesDDSFiles.Length == 0)
                            {
                                goto DoneMessage;
                            }

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

                                            line = reader.ReadLine();
                                            TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            /*
                                            line = reader.ReadLine();
                                            TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            */
                                            TextureSections.Add(TexSec);
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
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
                                    DDSFileReader.Dispose();
                                    DDSFileToGetDimensions.Close();
                                    DDSFileToGetDimensions.Dispose();

                                    ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                    ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                    ANewTexSec.UnkByte3 = 0x00;
                                    /*
                                    ANewTexSec.Unknown1 = 0x00; // temp
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
                                    */

                                    ANewTexSec.Unknown1 = 0x10000; // temp
                                    ANewTexSec.Unknown3 = 0;
                                    ANewTexSec.Unknown4 = 0x20;
                                    ANewTexSec.Unknown5 = 0x500;
                                    ANewTexSec.Unknown6 = 0x10200;
                                    ANewTexSec.Unknown7 = 0x0;
                                    ANewTexSec.Unknown8 = 0;
                                    ANewTexSec.Unknown9 = 0x1000000;
                                    ANewTexSec.Unknown10 = 0x100;
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
                                    IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown17.ToString("X"));
                                    IniWriter.WriteLine(); // Seperate sections with empty lines
                                }

                                IniWriter.Close();
                                IniWriter.Dispose();
                            }

                            // Finally rebuild the file
                            Log("Rebuilding FrontEndTextures.tpk for FrontB1.lzc using XNFSTpkTool...");

                            using (Process process = new Process())
                            {
                                process.StartInfo.FileName = Path.Combine(GetResourcesPath(), @"FrontEnd", @"XNFSTPKTool.exe");
                                process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                                process.StartInfo.Arguments = @"-w FrontEndTextures\8D08770D.ini FrontEndTextures.tpk";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                Log(process.StandardOutput.ReadToEnd());

                                process.WaitForExit();
                            }

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
                                FrontB1FileWriter.Write(File.ReadAllBytes(Path.Combine(GetResourcesPath(), @"FrontEnd\FONT_NFS_MOVIE_LARGE.bin")));

                                // close stream
                                FrontB1FileWriter.Close();
                                FrontB1FileWriter.Dispose();
                                FrontB1File.Close();
                                FrontB1File.Dispose();

                                Log("Successfully rebuilt FrontB1.lzc file.");
                            }

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if ((!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc" + ".edbackup"))) && File.Exists((Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc"))))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\" + "FrontB1.lzc"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.lzc"), true);

                        }

                        if (CurrentGame == (int)EdTypes.Game.ProStreet)
                        {
                            DirectoryInfo FrontEndTexturesTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\B66CD660"));
                            FileInfo[] FrontEndTexturesDDSFiles = FrontEndTexturesTPK.GetFiles("*.dds", SearchOption.AllDirectories);
                            if (FrontEndTexturesDDSFiles.Length == 0)
                            {
                                goto DoneMessage;
                            }

                            // Return to original file before any modifications. (To prevent overlapping and stuff)
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\B66CD660_orig.ini"), Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\B66CD660.ini"), true);

                            // Check if required info exists in the config file. If not, add it.
                            var TPKInfoSection = new XNFSTPKToolWrapper.TPKSection();
                            var AnimationSections = new List<XNFSTPKToolWrapper.AnimationSection>();
                            var TextureSections = new List<XNFSTPKToolWrapper.TextureSectionTPKv3>();

                            using (var reader = new StreamReader(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\B66CD660.ini"))) // sorry, quick and dirty
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

                                            line = reader.ReadLine();
                                            TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            /*
                                            line = reader.ReadLine();
                                            TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            */
                                            TextureSections.Add(TexSec);
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                            }

                            foreach (FileInfo DDSFile in FrontEndTexturesDDSFiles) // names in folder
                            {
                                bool IsThereAConfigForThisDDS = false;

                                // Find if the texture exists in the config file (at least once)
                                foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)  // names in ini
                                {
                                    if (TexSec.File == (@"FrontEndTextures\B66CD660\" + DDSFile.Name))
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
                                    ANewTexSec.File = @"FrontEndTextures\B66CD660\" + DDSFile.Name;
                                    ANewTexSec.Name = Path.GetFileNameWithoutExtension(DDSFile.Name);
                                    ANewTexSec.Hash2 = 0x1A93CF;

                                    // Get texture dimensions for some quick maths
                                    var DDSFileToGetDimensions = File.Open(DDSFile.FullName, FileMode.Open);
                                    var DDSFileReader = new BinaryReader(DDSFileToGetDimensions);

                                    DDSFileReader.BaseStream.Position = 0x0C;
                                    int VerRes = DDSFileReader.ReadInt32();
                                    int HorRes = DDSFileReader.ReadInt32();

                                    DDSFileReader.Close();
                                    DDSFileReader.Dispose();
                                    DDSFileToGetDimensions.Close();
                                    DDSFileToGetDimensions.Dispose();

                                    ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                    ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                    ANewTexSec.UnkByte3 = 0x00;
                                    /*
                                    ANewTexSec.Unknown1 = 0x00; // temp
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
                                    */

                                    ANewTexSec.Unknown1 = 0x10000; // temp
                                    ANewTexSec.Unknown3 = 0;
                                    ANewTexSec.Unknown4 = 0x20;
                                    ANewTexSec.Unknown5 = 0x500;
                                    ANewTexSec.Unknown6 = 0x10200;
                                    ANewTexSec.Unknown7 = 0x0;
                                    ANewTexSec.Unknown8 = 0;
                                    ANewTexSec.Unknown9 = 0;
                                    ANewTexSec.Unknown10 = 0x1000100;
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
                            using (var IniWriter = new StreamWriter(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures\B66CD660.ini"), false))
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
                                    IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown17.ToString("X"));
                                    IniWriter.WriteLine(); // Seperate sections with empty lines
                                }

                                IniWriter.Close();
                                IniWriter.Dispose();
                            }

                            // Finally rebuild the file
                            Log("Rebuilding FrontEndTextures.tpk for FrontB1.bun using XNFSTpkTool...");

                            using (Process process = new Process())
                            {
                                process.StartInfo.FileName = Path.Combine(GetResourcesPath(), @"FrontEnd", @"XNFSTPKTool.exe");
                                process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                                process.StartInfo.Arguments = @"-w FrontEndTextures\B66CD660.ini FrontEndTextures.tpk";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                Log(process.StandardOutput.ReadToEnd());

                                process.WaitForExit();
                            }

                            if (File.Exists(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontEndTextures.tpk")))
                            {
                                // merge other chunks in
                                var FrontB1File = File.Create(Path.Combine(GetResourcesPath(), @"FrontEnd\FrontB1.bun"));
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
                                FrontB1FileWriter.Dispose();
                                FrontB1File.Close();
                                FrontB1File.Dispose();

                                Log("Successfully rebuilt FrontB1.bun file.");
                            }

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if ((!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.bun" + ".edbackup"))) && File.Exists((Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.bun"))))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.bun"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.bun" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\" + "FrontB1.bun"), Path.Combine(WorkingFolder, @"FrontEnd\" + "FrontB1.bun"), true);

                        }

                        if (CurrentGame == (int)EdTypes.Game.Undercover)
                        {
                            DirectoryInfo LOGOTEXTURESTPK = new DirectoryInfo(Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES\D78D2BB1"));
                            FileInfo[] LOGOTEXTURESDDSFiles = LOGOTEXTURESTPK.GetFiles("*.dds", SearchOption.AllDirectories);
                            if (LOGOTEXTURESDDSFiles.Length == 0)
                            {
                                goto DoneMessage;
                            }

                            // Return to original file before any modifications. (To prevent overlapping and stuff)
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES\D78D2BB1_orig.ini"), Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES\D78D2BB1.ini"), true);

                            // Check if required info exists in the config file. If not, add it.
                            var TPKInfoSection = new XNFSTPKToolWrapper.TPKSection();
                            var AnimationSections = new List<XNFSTPKToolWrapper.AnimationSection>();
                            var TextureSections = new List<XNFSTPKToolWrapper.TextureSectionTPKv3>();

                            using (var reader = new StreamReader(Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES\D78D2BB1.ini"))) // sorry, quick and dirty
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

                                            line = reader.ReadLine();
                                            TexSec.Unknown17 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            /*
                                            line = reader.ReadLine();
                                            TexSec.Unknown18 = Byte.Parse(line.Substring(line.IndexOf('=') + 1).TrimStart(' ', '='), NumberStyles.HexNumber);
                                            */
                                            TextureSections.Add(TexSec);
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                            }

                            foreach (FileInfo DDSFile in LOGOTEXTURESDDSFiles) // names in folder
                            {
                                bool IsThereAConfigForThisDDS = false;

                                // Find if the texture exists in the config file (at least once)
                                foreach (XNFSTPKToolWrapper.TextureSectionTPKv3 TexSec in TextureSections)  // names in ini
                                {
                                    if (TexSec.File == (@"LOGOTEXTURES\D78D2BB1\" + DDSFile.Name))
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
                                    ANewTexSec.File = @"LOGOTEXTURES\D78D2BB1\" + DDSFile.Name;
                                    ANewTexSec.Name = Path.GetFileNameWithoutExtension(DDSFile.Name);
                                    ANewTexSec.Hash2 = 0x1A93CF;

                                    // Get texture dimensions for some quick maths
                                    var DDSFileToGetDimensions = File.Open(DDSFile.FullName, FileMode.Open);
                                    var DDSFileReader = new BinaryReader(DDSFileToGetDimensions);

                                    DDSFileReader.BaseStream.Position = 0x0C;
                                    int VerRes = DDSFileReader.ReadInt32();
                                    int HorRes = DDSFileReader.ReadInt32();

                                    DDSFileReader.Close();
                                    DDSFileReader.Dispose();
                                    DDSFileToGetDimensions.Close();
                                    DDSFileToGetDimensions.Dispose();

                                    ANewTexSec.UnkByte1 = (byte)Math.Log(HorRes, 2); // Looks like there were some quick maths
                                    ANewTexSec.UnkByte2 = (byte)Math.Log(VerRes, 2); // The values are log based 2 of res values.
                                    ANewTexSec.UnkByte3 = 0x00;
                                    /*
                                    ANewTexSec.Unknown1 = 0x00; // temp
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
                                    */

                                    ANewTexSec.Unknown1 = 0x10000; // temp
                                    ANewTexSec.Unknown3 = 0;
                                    ANewTexSec.Unknown4 = 0x26;
                                    ANewTexSec.Unknown5 = 0x500;
                                    ANewTexSec.Unknown6 = 0x20200;
                                    ANewTexSec.Unknown7 = 0x6;
                                    ANewTexSec.Unknown8 = 0;
                                    ANewTexSec.Unknown9 = 0;
                                    ANewTexSec.Unknown10 = 0x1000100;
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
                            using (var IniWriter = new StreamWriter(Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES\D78D2BB1.ini"), false))
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
                                    IniWriter.WriteLine("Unknown12" + " = " + TexSec.Unknown17.ToString("X"));
                                    IniWriter.WriteLine(); // Seperate sections with empty lines
                                }

                                IniWriter.Close();
                                IniWriter.Dispose();
                            }

                            // Finally rebuild the file
                            Log("Rebuilding LOGOTEXTURES.BUN using XNFSTpkTool...");

                            using (Process process = new Process())
                            {
                                process.StartInfo.FileName = Path.Combine(GetResourcesPath(), @"FrontEnd", @"XNFSTPKTool.exe");
                                process.StartInfo.WorkingDirectory = Path.Combine(GetResourcesPath(), @"FrontEnd");
                                process.StartInfo.Arguments = @"-w LOGOTEXTURES\D78D2BB1.ini LOGOTEXTURES.BUN";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.Start();

                                Log(process.StandardOutput.ReadToEnd());

                                process.WaitForExit();
                            }

                            if (File.Exists(Path.Combine(GetResourcesPath(), @"FrontEnd\LOGOTEXTURES.BUN")))
                            {
                                Log("Successfully rebuilt LOGOTEXTURES.BUN file.");
                            }

                            // Copy it to the game dir w/ a backup
                            // Make a backup
                            if (!DisableBackups)
                            {
                                if ((!File.Exists(Path.Combine(WorkingFolder, @"FrontEnd\" + "LOGOTEXTURES.BUN" + ".edbackup"))) && File.Exists((Path.Combine(WorkingFolder, @"FrontEnd\" + "LOGOTEXTURES.BUN"))))
                                    File.Copy(Path.Combine(WorkingFolder, @"FrontEnd\" + "LOGOTEXTURES.BUN"), Path.Combine(WorkingFolder, @"FrontEnd\" + "LOGOTEXTURES.BUN" + ".edbackup"), true);
                            }
                            // Copy
                            File.Copy(Path.Combine(GetResourcesPath(), @"FrontEnd\" + "LOGOTEXTURES.BUN"), Path.Combine(WorkingFolder, @"FrontEnd\" + "LOGOTEXTURES.BUN"), true);

                        }

                    }
                    else Log("Skipped rebuilding FrontEnd textures, it's disabled from options.");
                    }

                DoneMessage:

                MessageBox.Show("New cars added successfully!.", "Ed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log("New cars added successfully.", true);
                DeleteTempPath();

               switch (CurrentGame)
               {
                    case (int)EdTypes.Game.Underground:
                        /*if (!File.Exists(Path.Combine(WorkingFolder, "scripts", "NFSUUnlimiter.asi")))
                        {
                            var DownloadU1Unl = MessageBox.Show("Looks like you don't have NFSU Unlimiter installed." + Environment.NewLine
                                + "Do you want to download NFSU Unlimiter now?", "Ed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (DownloadU1Unl == DialogResult.Yes) Process.Start("https://github.com/nlgzrgn/NFSUUnlimiter/releases");
                        }
                        else goto default;*/
                        break;
                    case (int)EdTypes.Game.Underground2:
                        if (!File.Exists(Path.Combine(WorkingFolder, "scripts", "NFSU2Unlimiter.asi")))
                        {
                            var DownloadU2Unl = MessageBox.Show("Looks like you don't have NFSU2 Unlimiter installed." + Environment.NewLine
                                + "Do you want to download NFSU2 Unlimiter now?", "Ed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (DownloadU2Unl == DialogResult.Yes) Process.Start("https://github.com/nlgzrgn/NFSU2Unlimiter/releases");
                        }
                        else goto default;
                        break;
                    case (int)EdTypes.Game.MostWanted:
                        if (!File.Exists(Path.Combine(WorkingFolder, "scripts", "NFSMWUnlimiter.asi")))
                        {
                            var DownloadMWUnl = MessageBox.Show("Looks like you don't have NFSMW Unlimiter installed." + Environment.NewLine
                                + "Do you want to download NFSMW Unlimiter now?", "Ed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (DownloadMWUnl == DialogResult.Yes) Process.Start("https://github.com/nlgzrgn/NFSMWUnlimiter/releases");
                        }
                        else goto default;
                        break;
                    case (int)EdTypes.Game.Carbon:
                        if (!(File.Exists(Path.Combine(WorkingFolder, "scripts", "NFSCUnlimiter.asi")) || (File.Exists(Path.Combine(WorkingFolder, "scripts", "nfsc-limits.asi")))))
                        {
                            var DownloadCUnl = MessageBox.Show("Looks like you don't have NFSC Unlimiter or Car Array Patch installed." + Environment.NewLine
                                + "Do you want to download NFSC Unlimiter now?", "Ed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (DownloadCUnl == DialogResult.Yes) Process.Start("https://github.com/nlgzrgn/NFSCUnlimiter/releases");
                        }
                        else goto default;
                        break;
                     default:
                        var RunGameNow = MessageBox.Show("Do you want to run the game now?", "Ed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (RunGameNow == DialogResult.Yes) executeTheGameToolStripMenuItem_Click(sender, new EventArgs());
                        break;
               }

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

            Ed.Default.KeepTemporaryFiles = KeepTempFiles;
            Ed.Default.Save();
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

        private void automaticallyRestoreGlobalBBackupBeforeAddingCarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuItemRestoreGlobalB.Checked = !MenuItemRestoreGlobalB.Checked;
            AutoRestoreGlobalB = MenuItemRestoreGlobalB.Checked;

            Ed.Default.AutoRestoreGlobalB = AutoRestoreGlobalB;
            Ed.Default.Save();
        }

        private void boundsCollisionDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Resources = GetBoundsPath();
            Process.Start("explorer", Resources);
        }

        private void frontEndTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Resources = GetFrontEndTexturesPath();
            Process.Start("explorer", Resources);
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Ed.log");
        }

        private void executeTheGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (CurrentGame)
            {
                case (int)EdTypes.Game.Underground:
                case (int)EdTypes.Game.MostWanted:
                    ProcessStartInfo mw = new ProcessStartInfo("speed.exe");
                    mw.WorkingDirectory = WorkingFolder;
                    Process.Start(mw);
                    break;
                case (int)EdTypes.Game.Underground2:
                    ProcessStartInfo u2 = new ProcessStartInfo("speed2.exe");
                    u2.WorkingDirectory = WorkingFolder;
                    Process.Start(u2);
                    break;
                case (int)EdTypes.Game.Carbon:
                    ProcessStartInfo crbn = new ProcessStartInfo("NFSC.exe");
                    crbn.WorkingDirectory = WorkingFolder;
                    Process.Start(crbn);
                    break;
                case (int)EdTypes.Game.ProStreet:
                case (int)EdTypes.Game.Undercover:
                    ProcessStartInfo psuc = new ProcessStartInfo("NFS.exe");
                    psuc.WorkingDirectory = WorkingFolder;
                    Process.Start(psuc);
                    break;
            }
        }

        private void openGameDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", WorkingFolder);
        }

        private void MenuItemTextures_Click(object sender, EventArgs e)
        {
            MenuItemTextures.Checked = !MenuItemTextures.Checked;

            Ed.Default.DoTextures = MenuItemTextures.Checked;
            Ed.Default.Save();
        }

        private void MenuItemStrings_Click(object sender, EventArgs e)
        {
            MenuItemStrings.Checked = !MenuItemStrings.Checked;

            Ed.Default.DoStrings = MenuItemStrings.Checked;
            Ed.Default.Save();
        }
    }
}
