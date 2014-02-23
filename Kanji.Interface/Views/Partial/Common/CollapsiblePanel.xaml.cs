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

namespace Kanji.Interface.Controls
{
    public class CollapsiblePanelBase : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CollapsiblePanelBase));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CollapsiblePanelBase));

        public static readonly DependencyProperty IsContentShownProperty =
            DependencyProperty.Register("IsContentShown", typeof(bool), typeof(CollapsiblePanelBase));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CollapsiblePanelBase));

        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(object), typeof(CollapsiblePanelBase));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the command triggered when the panel is activated.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter of the command triggered when the panel is activated.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current visibility of the panel content.
        /// </summary>
        public bool IsContentShown
        {
            get { return (bool)GetValue(IsContentShownProperty); }
            set { SetValue(IsContentShownProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text written in the header (always visible) section.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the header.
        /// </summary>
        public object HeaderContent
        {
            get { return GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        #endregion

        #region Methods



        #endregion
    }

    public partial class CollapsiblePanel : CollapsiblePanelBase
    {
        public CollapsiblePanel()
        {
            InitializeComponent();
        }

        private void OnCommandButtonClick(object sender, RoutedEventArgs e)
        {
            if (Command == null)
            {
                IsContentShown = !IsContentShown;
            }
        }
    }
}
