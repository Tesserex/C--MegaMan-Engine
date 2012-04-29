using System;
using System.Drawing;
using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    public partial class ProjectForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ProjectForm()
        {
            InitializeComponent();
            projectView.NodeMouseDoubleClick += projectView_NodeMouseDoubleClick;
            var imagelist = new ImageList {ColorDepth = ColorDepth.Depth32Bit};
            imagelist.Images.Add(Properties.Resources.Folder_16x16);
            imagelist.Images.Add(Properties.Resources.FolderOpen_16x16_72);
            imagelist.Images.Add(Properties.Resources.stage);
            imagelist.Images.Add(Properties.Resources.screen);
            projectView.ImageList = imagelist;
            projectView.BeforeCollapse += projectView_BeforeCollapse;
            projectView.BeforeExpand += projectView_BeforeExpand;
        }

        void projectView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 0) e.Node.ImageIndex = 1;
        }

        void projectView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1) e.Node.ImageIndex = 0;
        }

        void projectView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = (e.Node.Tag as ProjectTreeHandler);
            if (tag != null) tag.DoubleClick();
        }

        public void AddProject(ProjectEditor project)
        {
            var projectNode = projectView.Nodes.Add(project.Name);
            projectNode.NodeFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
            projectNode.Tag = new ProjectNodeHandler(project, projectNode);

            var stagesNode = projectNode.Nodes.Add("Stages");
            stagesNode.ImageIndex = 0;
            foreach (var stage in project.StageNames)
            {
                var stagenode = stagesNode.Nodes.Add(stage);
                stagenode.ImageIndex = stagenode.SelectedImageIndex = 2;
                stagenode.Tag = new StageNodeHandler(project, stagenode, stage);
            }

            project.StageAdded += stage =>
            {
                var stagenode = stagesNode.Nodes.Add(stage.Name);
                stagenode.ImageIndex = stagenode.SelectedImageIndex = 2;
                stagenode.Tag = new StageNodeHandler(project, stagenode, stage);
            };
        }

        public void CloseProject()
        {
            projectView.Nodes.Clear();
        }

        private void buttonNewStage_Click(object sender, EventArgs e)
        {
            if (projectView.SelectedNode != null)
            {
                var node = projectView.SelectedNode;
                var tag = node.Tag as ProjectTreeHandler;
                while (tag == null && node.Parent != null)
                {
                    node = node.Parent;
                    tag = node.Tag as ProjectTreeHandler;
                }

                if (tag != null)
                {
                    StageProp.CreateStage(tag.Project);
                }
            }
        }

        private void buttonNewScreen_Click(object sender, EventArgs e)
        {
            if (projectView.SelectedNode != null)
            {
                var node = projectView.SelectedNode;
                var tag = node.Tag as StageNodeHandler;
                while (tag == null && node.Parent != null)
                {
                    node = node.Parent;
                    tag = node.Tag as StageNodeHandler;
                }

                if (tag != null && tag.Stage != null)
                {
                    ScreenProp.CreateScreen(tag.Stage);
                }
            }
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            if (projectView.SelectedNode != null)
            {
                var tag = projectView.SelectedNode.Tag as ProjectTreeHandler;
                if (tag != null) tag.Properties();
            }
        }
    }
}
