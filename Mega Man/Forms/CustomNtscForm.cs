using System;
using System.ComponentModel;
using System.Windows.Forms;
using MegaMan.Engine.Forms.Settings;

namespace MegaMan.Engine
{
    public partial class CustomNtscForm : Form
    {
        public event Action Apply;

        public double Hue
        {
            get { return hueTrack.Value / 180.0; }
            set
            {
                hueTrack.Value = (int)(value * 180.0);
                hueValue.Text = hueTrack.Value.ToString();
            }
        }

        public double Saturation
        {
            get { return satTrack.Value / 100.0; }
            set
            {
                satTrack.Value = (int)(value * 100.0);
                satValue.Text = satTrack.Value.ToString();
            }
        }

        public double Brightness
        {
            get { return brightTrack.Value / 100.0; }
            set
            {
                brightTrack.Value = (int)(value * 100.0);
                brightValue.Text = brightTrack.Value.ToString();
            }
        }

        public double Contrast
        {
            get { return contTrack.Value / 100.0; }
            set
            {
                contTrack.Value = (int)(value * 100.0);
                contValue.Text = contTrack.Value.ToString();
            }
        }

        public double Sharpness
        {
            get { return sharpTrack.Value / 100.0; }
            set
            {
                sharpTrack.Value = (int)(value * 100.0);
                sharpValue.Text = sharpTrack.Value.ToString();
            }
        }

        public double Resolution
        {
            get { return resTrack.Value / 100.0; }
            set
            {
                resTrack.Value = (int)(value * 100.0);
                resValue.Text = resTrack.Value.ToString();
            }
        }

        public double Artifacts
        {
            get { return artTrack.Value / 100.0 - 1; }
            set
            {
                artTrack.Value = (int)((1 + value) * 100.0);
                artValue.Text = artTrack.Value.ToString();
            }
        }

        public double Gamma
        {
            get { return gammaTrack.Value / 100.0; }
            set
            {
                gammaTrack.Value = (int)(value * 100.0);
                gammaValue.Text = gammaTrack.Value.ToString();
            }
        }

        public double Fringing
        {
            get { return fringeTrack.Value / 100.0 - 1; }
            set
            {
                fringeTrack.Value = (int)((1 + value) * 100.0);
                fringeValue.Text = fringeTrack.Value.ToString();
            }
        }

        public double Bleed
        {
            get { return bleedTrack.Value / 100.0 - 1; }
            set
            {
                bleedTrack.Value = (int)((1 + value) * 100.0);
                bleedValue.Text = bleedTrack.Value.ToString();
            }
        }

        public CustomNtscForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form isn't closed, we just hide it and show it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;    // If closing it, there will be a failure on call of show method.
            base.OnClosing(e);
            Hide();
        }

        private void gammaTrack_Scroll(object sender, EventArgs e)
        {
            gammaValue.Text = (gammaTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void hueTrack_Scroll(object sender, EventArgs e)
        {
            hueValue.Text = hueTrack.Value.ToString();
            RaiseApply();
        }

        private void satTrack_Scroll(object sender, EventArgs e)
        {
            satValue.Text = (satTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void brightTrack_Scroll(object sender, EventArgs e)
        {
            brightValue.Text = (brightTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void contTrack_Scroll(object sender, EventArgs e)
        {
            contValue.Text = (contTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void sharpTrack_Scroll(object sender, EventArgs e)
        {
            sharpValue.Text = (sharpTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void resTrack_Scroll(object sender, EventArgs e)
        {
            resValue.Text = (resTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void artTrack_Scroll(object sender, EventArgs e)
        {
            artValue.Text = (artTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void fringeTrack_Scroll(object sender, EventArgs e)
        {
            fringeValue.Text = (fringeTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void bleedTrack_Scroll(object sender, EventArgs e)
        {
            bleedValue.Text = (bleedTrack.Value / 100.0).ToString();
            RaiseApply();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            hueTrack.Value = 0; hueValue.Text = "0";
            satTrack.Value = 0; satValue.Text = "0";
            brightTrack.Value = 0; brightValue.Text = "0";
            contTrack.Value = 0; contValue.Text = "0";
            sharpTrack.Value = 0; sharpValue.Text = "0";
            gammaTrack.Value = 0; gammaValue.Text = "0";
            resTrack.Value = 0; resValue.Text = "0";
            artTrack.Value = 0; artValue.Text = "0";
            fringeTrack.Value = 0; fringeValue.Text = "0";
            bleedTrack.Value = 0; bleedValue.Text = "0";
            RaiseApply();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            RaiseApply();
            Close();
        }

        private void RaiseApply()
        {
            var apply = Apply;
            if (apply != null)
            {
                apply();
            }
        }

        public NTSC_CustomOptions GetOptions()
        {
            return new NTSC_CustomOptions {
                Hue = Hue,
                Saturation = Saturation,
                Brightness = Brightness,
                Contrast = Contrast,
                Sharpness = Sharpness,
                Gamma = Gamma,
                Resolution = Resolution,
                Artifacts = Artifacts,
                Fringing = Fringing,
                Bleed = Bleed,
                Merge_Fields = true
            };
        }

        public void SetOptions(NTSC_CustomOptions options)
        {
            Hue = options.Hue;
            Saturation = options.Saturation;
            Brightness = options.Brightness;
            Contrast = options.Contrast;
            Sharpness = options.Sharpness;
            Gamma = options.Gamma;
            Resolution = options.Resolution;
            Artifacts = options.Artifacts;
            Fringing = options.Fringing;
            Bleed = options.Bleed;
        }
    }
}
