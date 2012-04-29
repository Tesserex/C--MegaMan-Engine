using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public partial class JoinForm : Form
    {
        private readonly Join join;
        private JoinType type;

        private JoinType Type
        {
            get { return type; }
            set
            {
                type = value;
                AdjustLabels();
                joinType.SelectedIndex = (type == JoinType.Vertical) ? 0 : 1;
            }
        }

        private int JoinWidth
        {
            get { return (int)width.Value; }
            set { width.Value = value; }
        }

        private string ScreenOne
        {
            get { return screenOne.SelectedItem.ToString(); }
            set { screenOne.SelectedItem = value; }
        }

        private string ScreenTwo
        {
            get { return screenTwo.SelectedItem.ToString(); }
            set { screenTwo.SelectedItem = value; }
        }

        private int OffsetOne
        {
            get { return (int)offsetOne.Value; }
            set { offsetOne.Value = value; }
        }

        private int OffsetTwo
        {
            get { return (int)offsetTwo.Value; }
            set { offsetTwo.Value = value; }
        }

        private JoinDirection Direction
        {
            get
            {
                if (forward.Checked) return JoinDirection.ForwardOnly;
                if (backward.Checked) return JoinDirection.BackwardOnly;
                return JoinDirection.Both;
            }
            set
            {
                if (value == JoinDirection.ForwardOnly) forward.Checked = true;
                else if (value == JoinDirection.BackwardOnly) backward.Checked = true;
                else bidirectional.Checked = true;
            }
        }

        public event Action OK;

        public JoinForm(Join join, IEnumerable<ScreenDocument> screens)
        {
            InitializeComponent();
            this.join = join;
            joinType.SelectedIndex = 0;

            foreach (ScreenDocument s in screens)
            {
                screenOne.Items.Add(s.Name);
                screenTwo.Items.Add(s.Name);
            }

            Direction = join.direction;
            JoinWidth = join.Size;
            ScreenOne = join.screenOne;
            ScreenTwo = join.screenTwo;
            OffsetOne = join.offsetOne;
            OffsetTwo = join.offsetTwo;
            Type = join.type;
        }

        private void AdjustLabels()
        {
            if (type == JoinType.Horizontal)
            {
                s1label.Text = "Upper Screen";
                s2label.Text = "Lower Screen";
                forward.Text = "Downward Only";
                backward.Text = "Upward Only";
            }
            else
            {
                s1label.Text = "Left Screen";
                s2label.Text = "Right Screen";
                forward.Text = "Rightward Only";
                backward.Text = "Leftward Only";
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            join.type = Type;
            join.direction = Direction;
            join.offsetOne = OffsetOne;
            join.offsetTwo = OffsetTwo;
            join.screenOne = ScreenOne;
            join.screenTwo = ScreenTwo;
            join.Size = JoinWidth;
            if (OK != null) OK();
            Close();
        }

        private void joinType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type = (joinType.SelectedIndex == 0) ? JoinType.Vertical : JoinType.Horizontal;
        }
    }
}
