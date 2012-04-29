using System.Windows.Forms;

namespace MegaMan.LevelEditor
{
    // all nodes in the project tree should have one of these as its tag
    public abstract class ProjectTreeHandler
    {
        protected TreeNode parentNode;
        public ProjectEditor Project { get; private set; }

        protected ProjectTreeHandler(ProjectEditor project, TreeNode node)
        {
            Project = project;
            parentNode = node;
        }

        public abstract void DoubleClick();
        public abstract void Properties();
    }

    public class ProjectNodeHandler : ProjectTreeHandler
    {
        public ProjectNodeHandler(ProjectEditor project, TreeNode node) : base(project, node) { }

        public override void Properties()
        {
            new ProjectProperties(Project).Show();
        }

        public override void DoubleClick() { }
    }

    public class StageNodeHandler : ProjectTreeHandler
    {
        public StageDocument Stage { get; private set; }
        private readonly string stageName;

        public StageNodeHandler(ProjectEditor project, TreeNode node, string stageName) : base(project, node)
        {
            Stage = null;
            this.stageName = stageName;
        }

        public StageNodeHandler(ProjectEditor project, TreeNode node, StageDocument stage) : base(project, node)
        {
            Stage = stage;
            stageName = stage.Name;
        }

        public override void DoubleClick()
        {
            if (Stage == null)
            {
                Stage = Project.StageByName(stageName);
                if (Stage == null) return;

                parentNode.Nodes.Clear();
                foreach (var screen in Stage.Screens)
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(Project, node, screen);
                    parentNode.Nodes.Add(node);
                }

                Stage.ScreenAdded += screen =>
                {
                    var node = new TreeNode(screen.Name);
                    node.ImageIndex = node.SelectedImageIndex = 3;
                    node.Tag = new ScreenNodeHandler(Project, node, screen);
                    parentNode.Nodes.Add(node);
                };
            }

            Stage.ReFocus();
        }

        public override void Properties()
        {
            if (Stage != null) StageProp.EditStage(Stage);
        }
    }

    public class ScreenNodeHandler : ProjectTreeHandler
    {
        private readonly ScreenDocument screen;

        public ScreenNodeHandler(ProjectEditor project, TreeNode node, ScreenDocument screen) : base(project, node)
        {
            this.screen = screen;
        }

        public override void DoubleClick() { }

        public override void Properties()
        {
            ScreenProp.EditScreen(screen);
        }
    }
}
