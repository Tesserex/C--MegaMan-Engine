using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Controls.ViewModels;

namespace MegaMan.Editor.Controls
{
    /// <summary>
    /// Interaction logic for EditableLabel.xaml
    /// </summary>
    public partial class EditableLabel : UserControl, INotifyPropertyChanged
    {
        private bool _editing;

        public static readonly DependencyProperty TextProp = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new PropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (EditableLabel)d;
            c.SetValue(e.Property, e.NewValue);
        }

        public string Text
        {
            get { return (string)GetValue(TextProp); }
            set { SetValue(TextProp, value); }
        }

        public string EditText { get; private set; }

        public ICommand SwapCommand { get; private set; }

        public Visibility LabelVisibility { get { return _editing ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility TextBoxVisibility { get { return _editing ? Visibility.Visible : Visibility.Collapsed; } }

        public Uri ButtonIcon
        {
            get
            {
                if (_editing)
                    return new Uri("pack://application:,,,/Resources/check.png");
                return new Uri("pack://application:,,,/Resources/pencil.png");
            }
        }

        public EditableLabel()
        {
            InitializeComponent();
            (Content as FrameworkElement).DataContext = this;
            SwapCommand = new RelayCommand(Swap);
        }

        private void Swap(object obj)
        {
            if (_editing)
                Text = EditText;
            else
                EditText = Text;

            _editing = !_editing;

            OnPropertyChanged("Text");
            OnPropertyChanged("EditText");
            OnPropertyChanged("LabelVisibility");
            OnPropertyChanged("TextBoxVisibility");
            OnPropertyChanged("ButtonIcon");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
