using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace SistemaTallerAutomorizWPF.CustomControls
{
    internal class PlaceHolderforTextBox : TextBox
    {
        public static readonly DependencyProperty PlaceholderProperty =
    DependencyProperty.Register(nameof(PlaceHolder), typeof(string), typeof(PlaceHolderforTextBox), new PropertyMetadata(string.Empty));

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        private bool _isPlaceHolderVisible = true;

        //evento del PlaceHolder para que se muestre cuando el TextBox este vacio
        public PlaceHolderforTextBox()
        {
            Loaded += PlaceHolderforTextBox_Loaded;
            GotFocus += RemovePlaceholder;
            LostFocus += ShowPlaceholder;
            TextChanged += CheckIfTextIsCleared;
        }

        //mètodo para verificar si el TextBox esta vacio y mostrar el PlaceHolder
        private void CheckIfTextIsCleared(object sender, TextChangedEventArgs e)
        {
            if (!_isPlaceHolderVisible && string.IsNullOrWhiteSpace(Text) && !IsFocused)
            {
                ShowPlaceholder(null, null);
            }
        }

        private void PlaceHolderforTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPlaceholder(null, null);
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (_isPlaceHolderVisible)
            {
                Text = string.Empty;
                Foreground = Brushes.Black;
                _isPlaceHolderVisible = false;
            }
        }

        private void ShowPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = PlaceHolder;
                Foreground = Brushes.Gray;
                _isPlaceHolderVisible = true;
            }
        }

        public bool IsPlaceHolderVisible => _isPlaceHolderVisible;
    }
}
