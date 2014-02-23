using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kanji.Interface.Controls
{
    public class SrsEntryFilterControlBase : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty IsInlineProperty =
            DependencyProperty.Register(
            "IsInline",
            typeof(bool),
            typeof(SrsEntryFilterControlBase),
            new PropertyMetadata(true));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value indicating if the control is to be
        /// rendered inline or if it may use a more vertical layout.
        /// </summary>
        public bool IsInline
        {
            get { return (bool)GetValue(IsInlineProperty); }
            set { SetValue(IsInlineProperty, value); }
        }

        #endregion

        #region Constructors

        public SrsEntryFilterControlBase()
        {
            
        }

        #endregion
    }
}
