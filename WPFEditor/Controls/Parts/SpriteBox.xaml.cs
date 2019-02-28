using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls.Parts
{
    /// <summary>
    /// Interaction logic for SpriteBox.xaml
    /// </summary>
    public partial class SpriteBox : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SpriteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(SpriteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty SpriteProperty = DependencyProperty.Register("Sprite", typeof(SpriteModel), typeof(SpriteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SpriteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ParamProperty  = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SpriteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SpriteBox), new PropertyMetadata(false, SelectedChanged));

        private bool isHovered;

        private void SpriteBox_MouseLeave(object sender, MouseEventArgs e)
        {
            isHovered = false;
            OnPropertyChanged("BackgroundBrush");
            OnPropertyChanged("TitleBrush");
        }

        private void SpriteBox_MouseEnter(object sender, MouseEventArgs e)
        {
            isHovered = true;
            OnPropertyChanged("BackgroundBrush");
            OnPropertyChanged("TitleBrush");
        }

        private static void SelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (SpriteBox)d;
            box.OnPropertyChanged("BackgroundBrush");
            box.OnPropertyChanged("TitleBrush");
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public SpriteModel Sprite
        {
            get { return (SpriteModel)GetValue(SpriteProperty); }
            set { SetValue(SpriteProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(ParamProperty); }
            set { SetValue(ParamProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public object BackgroundBrush
        {
            get
            {
                if (IsSelected) return FindResource("NesLightBlueShadeBrush");

                return FindResource("NesDarkGrayBrush");
            }
        }

        public object TitleBrush
        {
            get
            {
                if (IsSelected) return FindResource("ActiveShadowBrush");

                return (isHovered && CommandParameter == null) ? FindResource("ActiveShadowBrush") : FindResource("DarkShadowBrush");
            }
        }

        public SpriteBox()
        {
            InitializeComponent();
            (Content as FrameworkElement).DataContext = this;
            MouseEnter += SpriteBox_MouseEnter;
            MouseLeave += SpriteBox_MouseLeave;
        }

        public Visibility ButtonVisible
        {
            get
            {
                return (CommandParameter != null) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
