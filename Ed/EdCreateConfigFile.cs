using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ed
{
    public partial class EdCreateConfigFile : Form
    {
        /*
         * string key = ((KeyValuePair<string, string>)combo.SelectedItem).Key;
         * string value = ((KeyValuePair<string, string>)combo.SelectedItem).Value;
         */
        public Dictionary<string, string> UsageTypes = new Dictionary<string, string>()
        {
            {"0", "Racer" },
            {"1", "Cop" },
            {"2", "Traffic" },
            {"3", "Wheels" },
            {"4", "Universal" }
        };

        public EdCreateConfigFile()
        {
            InitializeComponent();
        }

        public void FillUsageType()
        {
            SelectUsageType.DataSource = new BindingSource(UsageTypes, null);
            SelectUsageType.DisplayMember = "Value";
            SelectUsageType.ValueMember = "Key";
        }
        
        public void FillDefaultColor()
        {


        }

        public void FillCopyFrom()
        {

        }

        public void FillSpoiler()
        {

        }

        public void FillSpoilerAS()
        {

        }

        private void EdCreateConfigFile_Load(object sender, EventArgs e)
        {
            FillUsageType();
            FillDefaultColor();
            FillCopyFrom();
            FillSpoiler();
            FillSpoilerAS();
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Label34_Click(object sender, EventArgs e)
        {

        }

        private void Label36_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void Label45_Click(object sender, EventArgs e)
        {

        }

        private void Label44_Click(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void TextBoxXName_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }
    }
}
