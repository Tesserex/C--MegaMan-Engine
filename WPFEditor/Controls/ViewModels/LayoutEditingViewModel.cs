using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Tools;
using System.ComponentModel;
using MegaMan.Editor.Bll;
using System.Windows.Input;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class LayoutEditingViewModel : IToolProvider, INotifyPropertyChanged
    {
        private IStageProvider _stageProvider;

        private IToolCursor _toolCursor;

        private IToolBehavior _toolBehavior;

        public event EventHandler<ToolChangedEventArgs> ToolChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutEditingViewModel(IStageProvider stageProvider)
        {
            _stageProvider = stageProvider;

            _stageProvider.StageChanged += StageChanged;

            AddScreenCommand = new RelayCommand(p => AddScreen(), p => HasStage);

            ChangeToolCommand = new RelayCommand(ChangeTool, p => HasStage);
        }

        public ICommand AddScreenCommand { get; private set; }

        public ICommand ChangeToolCommand { get; private set; }

        public bool HasStage
        {
            get
            {
                return (_stageProvider.CurrentStage != null);
            }
        }

        public IToolBehavior Tool
        {
            get { return _toolBehavior; }
        }

        public IToolCursor ToolCursor
        {
            get
            {
                return _toolCursor;
            }
            private set
            {
                if (_toolCursor != null)
                {
                    _toolCursor.Dispose();
                }

                _toolCursor = value;
            }
        }

        private void AddScreen()
        {
            var stage = _stageProvider.CurrentStage;

            if (stage != null)
            {
                int nextScreenId = stage.FindNextScreenId();

                stage.AddScreen(nextScreenId.ToString(), 16, 14);
            }
        }

        private void DeleteScreen(ScreenDocument screen)
        {
            
        }

        private void ChangeTool(object toolParam)
        {
            switch (toolParam.ToString())
            {
                case "Hand":
                    ToolCursor = new StandardToolCursor("hand.cur");
                    break;

                case "VSplit":
                    ToolCursor = new StandardToolCursor("vsplit.cur");
                    break;
            }

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_toolBehavior));
            }
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("HasStage"));
            }
        }
    }
}
