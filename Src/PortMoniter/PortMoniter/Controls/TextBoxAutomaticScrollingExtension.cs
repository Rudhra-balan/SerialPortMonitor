using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PortMoniter.Controls
{
    public class TextBoxAutomaticScrollingExtension
    {
        private static readonly Dictionary<TextBox, TextBoxScrollingTrigger> _textBoxesDictionary =
            new Dictionary<TextBox, TextBoxScrollingTrigger>();

        public static readonly DependencyProperty ScrollOnTextChangedProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnTextChanged",
                typeof(bool),
                typeof(TextBoxAutomaticScrollingExtension),
                new UIPropertyMetadata(false, OnScrollOnTextChanged));

        private static void OnScrollOnTextChanged(DependencyObject dependencyObject,
                                                  DependencyPropertyChangedEventArgs e)
        {
            var textBox = dependencyObject as TextBox;
            if (textBox == null)
            {
                return;
            }

            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            if (newValue == oldValue)
            {
                return;
            }

            if (newValue)
            {
                _textBoxesDictionary[textBox] = new TextBoxScrollingTrigger(textBox);
            }
            else
            {
                _textBoxesDictionary[textBox].Dispose();
                _textBoxesDictionary.Remove(textBox);
               
            }
        }

        private static void OnTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.Loaded -= OnTextBoxLoaded;
            _textBoxesDictionary[textBox] = new TextBoxScrollingTrigger(textBox);
        }

        private static void OnTextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.Unloaded -= OnTextBoxUnloaded;
            _textBoxesDictionary[textBox].Dispose();
        }

        public static bool GetScrollOnTextChanged(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ScrollOnTextChangedProperty);
        }

        public static void SetScrollOnTextChanged(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ScrollOnTextChangedProperty, value);
        }

        private class TextBoxScrollingTrigger : IDisposable
        {
            private TextBox TextBox { get; set; }

            public TextBoxScrollingTrigger(TextBox textBox)
            {
                TextBox = textBox;
                TextBox.TextChanged += OnTextBoxTextChanged;
            }

            private void OnTextBoxTextChanged(object sender, TextChangedEventArgs args)
            {
                TextBox.ScrollToEnd();
            }

            public void Dispose()
            {
                TextBox.TextChanged -= OnTextBoxTextChanged;
            }
        }

    }
}
