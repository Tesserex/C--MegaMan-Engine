using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public partial class GraphicalOverlay : Component
    {
        public event EventHandler<PaintEventArgs> Paint;
        private Form form;
        private List<Control> controlList;

        private bool visible;
        public bool Visible
        {
            private get { return visible; }
            set
            {
                visible = value;
                Invalidate();
            }
        }

        protected GraphicalOverlay()
        {
            InitializeComponent();
        }

        private IEnumerable<Control> ControlList
        {
            get
            {
                if (controlList == null)
                {
                    controlList = new List<Control>();

                    Control control = form.GetNextControl(form, true);

                    while (control != null)
                    {
                        controlList.Add(control);
                        control = form.GetNextControl(control, true);
                    }

                    controlList.Add(form);
                }

                return controlList;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Form Owner
        {
            protected get { return form; }
            set 
            {
                if (form != null)
                    form.Resize -= Form_Resize;

                form = value;
                form.Resize += Form_Resize;

                form.ControlAdded += form_ControlAdded;

                foreach (Control control in ControlList)
                    control.Paint += Control_Paint;
            }
        }

        void form_ControlAdded(object sender, ControlEventArgs e)
        {
            Add(e.Control);
        }

        public void Invalidate()
        {
            foreach (Control control in ControlList)
                control.Invalidate();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void Add(Control control)
        {
            controlList.Add(control);
            control.Paint += Control_Paint;
        }

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            if (Visible) OnPaint(sender, e);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (Paint != null)
                Paint(sender, e);
        }
    }
}