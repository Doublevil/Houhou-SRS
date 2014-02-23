using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kanji.Common.Helpers;
using Kanji.Interface.Helpers;

namespace Kanji.Interface.Controls
{
    class CommandTextBox : TextBox
    {
        #region Constants

        /// <summary>
        /// Defines the keys that cause validation when pressed
        /// and control has focus.
        /// </summary>
        private static readonly Key[] ValidationKeys = new Key[]
        {
            Key.Enter
        };

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsKanaInputProperty =
            DependencyProperty.Register("IsKanaInput",
            typeof(bool), typeof(CommandTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty ValidationCommandProperty =
            DependencyProperty.Register("ValidationCommand",
            typeof(ICommand), typeof(CommandTextBox), null);

        public static readonly DependencyProperty ValidationCommandParameterProperty =
            DependencyProperty.Register("ValidationCommandParameter",
            typeof(object), typeof(CommandTextBox), null);

        #endregion

        #region Fields

        /// <summary>
        /// Stores the last value validated.
        /// Used to avoid double validation of the same value.
        /// </summary>
        private string _lastValidatedValue;

        private object _textLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean value defining if the input should be
        /// converted to kana.
        /// </summary>
        public bool IsKanaInput
        {
            get { return (bool)GetValue(IsKanaInputProperty); }
            set { SetValue(IsKanaInputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command fired when the input is to be validated.
        /// </summary>
        public ICommand ValidationCommand
        {
            get { return (ICommand)GetValue(ValidationCommandProperty); }
            set { SetValue(ValidationCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter of the validation command.
        /// </summary>
        public object ValidationCommandParameter
        {
            get { return GetValue(ValidationCommandParameterProperty); }
            set { SetValue(ValidationCommandParameterProperty, value); }
        }

        #endregion

        #region Constructors

        public CommandTextBox()
        {
            _lastValidatedValue = string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates the input.
        /// </summary>
        private void ValidateInput()
        {
            if (IsKanaInput)
            {
                Text = KanaHelper.RomajiToKana(Text);
            }

            if (Text != _lastValidatedValue)
            {
                _lastValidatedValue = Text;
                if (ValidationCommand != null
                    && ValidationCommand.CanExecute(ValidationCommandParameter))
                {
                    ValidationCommand.Execute(ValidationCommandParameter);
                }
            }
        }

        #region Event handlers

        /// <summary>
        /// Overrides the TextInput event handler to alter the text being typed
        /// if needed.
        /// </summary>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            lock (_textLock)
            {
                base.OnTextInput(e);

                if (IsKanaInput)
                {
                    int caretIndexBefore = this.CaretIndex;
                    int lengthBefore = Text.Length;
                    Text = KanaHelper.RomajiToKana(Text, true);
                    this.CaretIndex = caretIndexBefore + (Text.Length - lengthBefore);
                }
            }
        }

        /// <summary>
        /// Overrides the LostFocus event handler to attempt to validate the input
        /// when the control loses focus.
        /// </summary>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            ValidateInput();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Overrides the KeyUp event handler to attempt to validate the input
        /// when a validation key is pressed.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (ValidationKeys.Contains(e.Key))
            {
                ValidateInput();
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Overrides the MouseUp event handler to erase the content when the
        /// middle click is pressed on the textbox.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.Text = string.Empty;
                ValidateInput();
            }

            base.OnMouseUp(e);
        }

        #endregion

        #endregion
    }
}
