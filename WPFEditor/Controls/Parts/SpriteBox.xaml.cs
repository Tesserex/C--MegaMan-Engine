using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Common;

namespace MegaMan.Editor.Controls.Parts
{
    /// <summary>
    /// Interaction logic for SpriteBox.xaml
    /// </summary>
    public partial class SpriteBox : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SpriteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(SpriteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty SpriteProperty = DependencyProperty.Register("Sprite", typeof(Sprite), typeof(SpriteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SpriteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ParamProperty  = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SpriteBox), new PropertyMetadata(null));

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

        public Sprite Sprite
        {
            get { return (Sprite)GetValue(SpriteProperty); }
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

        public SpriteBox()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
