using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Tools;
using System.ComponentModel;
using MegaMan.Editor.Bll;
using System.Windows.Input;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class LayoutEditingViewModel : IToolProvider, INotifyPropertyChanged
    {
        private IStageProvider _stageProvider;

        public event EventHandler<ToolChangedEventArgs> ToolChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutEditingViewModel(IStageProvider stageProvider)
        {
            _stageProvider = stageProvider;

            _stageProvider.StageChanged += StageChanged;

            AddScreenCommand = new RelayCommand(p => AddScreen(), p => HasStage);
        }

        public ICommand AddScreenCommand { get; private set; }

        public bool HasStage
        {
            get
            {
                return (_stageProvider.CurrentStage != null);
            }
        }

        public Bll.Tools.IToolBehavior Tool
        {
            get { throw new NotImplementedException(); }
        }

        public IToolCursor ToolCursor
        {
            get { throw new NotImplementedException(); }
        }

        public void AddScreen()
        {
            var stage = _stageProvider.CurrentStage;

            if (stage != null)
            {
                int nextScreenId = FindNextScreenId(stage);

                stage.AddScreen(nextScreenId.ToString(), 16, 14);
            }
        }

        public void DeleteScreen(ScreenDocument screen)
        {
            
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("HasStage"));
            }
        }

        private static int FindNextScreenId(StageDocument stage)
        {
            int stageCount = stage.Screens.Count();
            int nextScreenId = stageCount + 1;
            while (stage.Screens.Any(s => s.Name == nextScreenId.ToString()))
            {
                nextScreenId++;
            }
            return nextScreenId;
        }
    }
}
