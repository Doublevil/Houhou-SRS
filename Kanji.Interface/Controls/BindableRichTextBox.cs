// Credits to Leom Burke:
// http://www.codeproject.com/Articles/137209/Binding-and-styling-text-to-a-RichTextBox-in-WPF
// Thanks man.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Kanji.Interface.Controls
{
    class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(FlowDocument),
            typeof(BindableRichTextBox), new FrameworkPropertyMetadata
            (null, new PropertyChangedCallback(OnDocumentChanged)));

        public new FlowDocument Document
        {
            get
            {
                return (FlowDocument)this.GetValue(DocumentProperty);
            }

            set
            {
                this.SetValue(DocumentProperty, value);
            }
        }

        public static void OnDocumentChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            RichTextBox rtb = (RichTextBox)obj;
            rtb.Dispatcher.Invoke(() =>
            {
                rtb.Document = ((FlowDocument)args.NewValue);
            });
        }
    }
}
