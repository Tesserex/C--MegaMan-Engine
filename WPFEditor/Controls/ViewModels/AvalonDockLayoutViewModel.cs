using System.IO;
using System.Windows.Input;
using MegaMan.Editor.AppData;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace MegaMan.Editor.Controls.ViewModels
{
    /// <summary>
    /// Class implements a viewmodel to support the
    /// <seealso cref="AvalonDockLayoutSerializer"/>
    /// attached behavior which is used to implement
    /// load/save of layout information on application
    /// start and shut-down.
    /// </summary>
    public class AvalonDockLayoutViewModel
    {
        #region fields
        private RelayCommand mLoadLayoutCommand;
        private RelayCommand mSaveLayoutCommand;
        #endregion fields

        #region command properties
        /// <summary>
        /// Implement a command to load the layout of an AvalonDock-DockingManager instance.
        /// This layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="DockingManager"/> instance to
        /// work correctly. Not supplying that reference results in not loading a layout (silent return).
        /// </summary>
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (mLoadLayoutCommand == null)
                {
                    mLoadLayoutCommand = new RelayCommand(p => {
                        DockingManager docManager = p as DockingManager;

                        if (docManager == null)
                            return;

                        LoadDockingManagerLayout(docManager);
                    });
                }

                return mLoadLayoutCommand;
            }
        }

        /// <summary>
        /// Implements a command to save the layout of an AvalonDock-DockingManager instance.
        /// This layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="string"/> instance to
        /// work correctly. The string is supposed to contain the XML layout persisted
        /// from the DockingManager instance. Not supplying that reference to the string
        /// results in not saving a layout (silent return).
        /// </summary>
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (mSaveLayoutCommand == null)
                {
                    mSaveLayoutCommand = new RelayCommand(p => {
                        string xmlLayout = p as string;

                        if (xmlLayout == null)
                            return;

                        SaveDockingManagerLayout(xmlLayout);
                    });
                }

                return mSaveLayoutCommand;
            }
        }
        #endregion command properties

        #region methods
        #region LoadLayout
        /// <summary>
        /// Loads the layout of a particular docking manager instance from persistence
        /// and checks whether a file should really be reloaded (some files may no longer
        /// be available).
        /// </summary>
        /// <param name="docManager"></param>
        private void LoadDockingManagerLayout(DockingManager docManager)
        {
            string layoutFileName = Path.Combine(StoredAppData.GetDirectory(), "layout.xml");

            if (File.Exists(layoutFileName) == false)
                return;

            var layoutSerializer = new XmlLayoutSerializer(docManager);

            layoutSerializer.LayoutSerializationCallback += (s, args) => {
                // This can happen if the previous session was loading a file
                // but was unable to initialize the view ...
                if (args.Model.ContentId == null)
                {
                    args.Cancel = true;
                }
            };

            //layoutSerializer.Deserialize(layoutFileName);
        }

        #endregion LoadLayout

        #region SaveLayout
        private void SaveDockingManagerLayout(string xmlLayout)
        {
            // Create XML Layout file on close application (for re-load on application re-start)
            if (xmlLayout == null)
                return;

            string fileName = Path.Combine(StoredAppData.GetDirectory(), "layout.xml");

            File.WriteAllText(fileName, xmlLayout);
        }
        #endregion SaveLayout
        #endregion methods
    }
}
