// Thank you, Adi Lester (from http://stackoverflow.com/questions/12941707/keybinding-in-usercontrol-doesnt-work-when-textbox-has-the-focus)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kanji.Interface.Behaviors
{
    public class InputBindingsBehavior
    {
        public static readonly DependencyProperty TakesInputBindingPrecedenceProperty =
            DependencyProperty.RegisterAttached("TakesInputBindingPrecedence", typeof(bool), typeof(InputBindingsBehavior), new UIPropertyMetadata(false, OnTakesInputBindingPrecedenceChanged));

        public static bool GetTakesInputBindingPrecedence(UIElement obj)
        {
            return (bool)obj.GetValue(TakesInputBindingPrecedenceProperty);
        }

        public static void SetTakesInputBindingPrecedence(UIElement obj, bool value)
        {
            obj.SetValue(TakesInputBindingPrecedenceProperty, value);
        }

        private static void OnTakesInputBindingPrecedenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).PreviewKeyDown += new KeyEventHandler(InputBindingsBehavior_PreviewKeyDown);
        }

        private static void InputBindingsBehavior_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uielement = (UIElement)sender;

            var foundBinding = uielement.InputBindings
                .OfType<KeyBinding>()
                .FirstOrDefault(kb => kb.Key == e.Key && kb.Modifiers == e.KeyboardDevice.Modifiers);

            if (foundBinding != null)
            {
                e.Handled = false;
                if (foundBinding.Command.CanExecute(foundBinding.CommandParameter))
                {
                    foundBinding.Command.Execute(foundBinding.CommandParameter);
                }
            }
        }
    }
}
