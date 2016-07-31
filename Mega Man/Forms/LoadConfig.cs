using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MegaMan.Engine.Forms
{
    public partial class LoadConfig : Form
    {
        public event Action Apply;
        
        private UserSettings userSettingObject;
        public Setting settingsSelected { get; private set; }

        public LoadConfig()
        {
            InitializeComponent();
        }

        public void showFormIfNeeded(string currentGameName, UserSettings currentUserSettingObject, bool useDefaultConfig)
        {
            string currentSettingName = "";
            List<string> configNames = null;

            // We receive as parameter the current game name. However it may not be the current config.
            if (!useDefaultConfig) currentSettingName = currentGameName;

            userSettingObject = currentUserSettingObject;

            configNames = userSettingObject.GetAllConfigsGameNameFromCurrentUserSettings();
            configNames.Remove(currentSettingName);


            configNames.Insert(0, Constants.settingNameForFactorySettings);

            // In the list, no game string value is nothing. Replace it with No Game so user understands it
            for (int i = 0; i < configNames.Count; i++)
                if (configNames[i] == "") configNames[i] = Constants.noGameConfigNameToDisplayToUser;

            cbxConfigToPickFrom.DataSource = configNames;

            this.Show();
        }

        /// <summary>
        /// Form isn't closed, we just hide it and show it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;    // If closing it, there will be a failure on call of show method.
            base.OnClosing(e);
            this.Hide();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            int selection = cbxConfigToPickFrom.SelectedIndex;

            // If 0, the added option Factory index is selected
            if (selection == 0) settingsSelected = UserSettings.Default;
            else settingsSelected = userSettingObject.GetSettingByIndex(selection - 1); // -1 because an option is added at position 0

            RaiseApply();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RaiseApply()
        {
            var apply = Apply;
            if (apply != null)
            {
                apply();
            }
        }
    }
}
