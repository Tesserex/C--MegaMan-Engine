using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine.Forms
{
    public partial class DeleteConfigs : Form
    {
        private UserSettings userSettingObject = null;
        private string settingsPath, fileName;

        public DeleteConfigs()
        {
            InitializeComponent();
        }

        public void PrepareFormAndShowIfNeeded(UserSettings currentUserSettingObject, string settingsPath, string fileName)
        {
            List<string> configNames = null;

            this.settingsPath = settingsPath;
            this.fileName = fileName;

            cbxConfigToDelete.Enabled = true;
            btnDelete.Enabled = true;
            btnDeleteAll.Enabled = true;

            userSettingObject = currentUserSettingObject;

            configNames = userSettingObject.GetAllConfigsGameNameFromCurrentUserSettings();

            if (configNames.Count == 0)
            {
                MessageBox.Show("No configs!", "C# MegaMan Engine", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Buil list
                // In the list, no game string value is nothing. Replace it with No Game so user understands it
                cbxConfigToDelete.Items.Clear();

                for (int x = 0; x < configNames.Count; x++)
                {
                    if (configNames[x] == "") configNames[x] = Constants.noGameConfigNameToDisplayToUser;
                    cbxConfigToDelete.Items.Add(configNames[x]);
                }

                cbxConfigToDelete.SelectedIndex = 0;

                this.ShowDialog();
            }
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

        /// <summary>
        /// When there are no more elements in combo box, lock form.
        /// </summary>
        private void lockForm()
        {
            cbxConfigToDelete.Enabled = false;
            btnDelete.Enabled = false;
            btnDeleteAll.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int indexToSelectAfterDeletion = cbxConfigToDelete.SelectedIndex;
            string configSelected = cbxConfigToDelete.Items[indexToSelectAfterDeletion].ToString();

            if ((MessageBox.Show("Are you sure you want to delete configuration for " + configSelected + "?", "C# MegaMan Engine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) == DialogResult.No) return;

            // Delete configuration
            cbxConfigToDelete.Items.Remove(configSelected);
            userSettingObject.deleteSetting(indexToSelectAfterDeletion);
            XML.SaveToConfigXML(userSettingObject, settingsPath, fileName);

            if (cbxConfigToDelete.Items.Count == 0)
            {
                lockForm();
                return;
            }

            // Still at least one item it list, select subsequent, or last one if no subsequent
            if (indexToSelectAfterDeletion >= cbxConfigToDelete.Items.Count)
                indexToSelectAfterDeletion = cbxConfigToDelete.Items.Count - 1;

            cbxConfigToDelete.SelectedIndex = indexToSelectAfterDeletion;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Are you sure you want to delete ALL configurations ?", "C# MegaMan Engine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) == DialogResult.No) return;

            // Delete configurations
            cbxConfigToDelete.Items.Clear();
            userSettingObject.deleteAllSetting();
            XML.SaveToConfigXML(userSettingObject, settingsPath, fileName);

            lockForm();
        }
    }
}
