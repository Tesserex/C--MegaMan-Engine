using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Tools;
using MegaMan.Editor.Mediator;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class LayoutEditingViewModel : IToolProvider, INotifyPropertyChanged
    {
        private IToolCursor _toolCursor;

        private IToolBehavior _toolBehavior;

        private StageDocument _currentStage;

        public event EventHandler<ToolChangedEventArgs> ToolChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutEditingViewModel()
        {
            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);

            AddScreenCommand = new RelayCommand(p => AddScreen(), p => HasStage());

            ChangeToolCommand = new RelayCommand(ChangeTool, p => HasStage());
        }

        public ICommand AddScreenCommand { get; private set; }

        public ICommand ChangeToolCommand { get; private set; }

        private bool HasStage()
        {
            return (_currentStage != null);
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
            var stage = _currentStage;

            if (stage != null)
            {
                if (!stage.Tileset.Any())
                {
                    CustomMessageBox.ShowError("You need to create some tiles before you can start creating your stage.", "Tiles Needed");
                    return;
                }

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
                    _toolBehavior = new LayoutToolBehavior();
                    break;

                case "VSplit":
                    ToolCursor = new StandardToolCursor("vsplit.cur");
                    _toolBehavior = new CleaveScreenVerticalToolBehavior();
                    break;
            }

            if (ToolChanged != null)
            {
                ToolChanged(this, new ToolChangedEventArgs(_toolBehavior));
            }
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            _currentStage = e.Stage;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("HasStage"));
            }
        }
    }
}
