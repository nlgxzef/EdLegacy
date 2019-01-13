using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ed
{
    partial class AboutBoxEd : Form
    {
        public AboutBoxEd()
        {
            InitializeComponent();
            Text = String.Format("About {0}", AssemblyTitle);
            labelProductName.Text = AssemblyProduct + " - " + AssemblyDescription;
            labelVersion.Text = "";
            labelCopyright.Text = AssemblyCopyright;
        }

        #region Derleme Öznitelik Erişimcileri

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void AboutBoxEd_Load(object sender, EventArgs e)
        {
            String[] VersionInfo = ProductVersion.Split('.');
            String VersionText = "v" + ProductVersion;
            int VersionDelta = Int32.Parse(VersionInfo[3]);

            VersionText += " " + "(" + "Build" + " " + VersionInfo[0] + (Int32.Parse(VersionInfo[1]) != 0 ? VersionInfo[1] + "." : "") + ";" + " " + "Rev." + (Int32.Parse(VersionInfo[2]) < 10 ? "0" : "") + VersionInfo[2];

            if (VersionDelta >= 100 && VersionDelta < 200) VersionText += " " + "Milestone";
            else if (VersionDelta >= 200 && VersionDelta < 400) VersionText += " " + "ALPHA";
            else if (VersionDelta >= 400 && VersionDelta < 800) VersionText += " " + "BETA";
            else if (VersionDelta >= 800 && VersionDelta < 1337) VersionText += " " + "Release Candidate";
            else if (VersionDelta >= 1338 && VersionDelta < 2000) VersionText += " " + "Hotfix";
            else if (VersionDelta >= 2000 && VersionDelta < 9999) VersionText += " " + "Post-Release Test";

            VersionText += ")";

            labelVersion.Text = VersionText;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
