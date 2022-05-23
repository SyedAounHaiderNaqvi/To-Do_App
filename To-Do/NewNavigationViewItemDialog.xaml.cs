using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace To_Do
{
    public sealed partial class NewNavigationViewItemDialog : ContentDialog
    {
        public ElementTheme THEME;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string defaultIconGlyph = "\uEA37";

        public NewNavigationViewItemDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
            this.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(ListNameTextBox.Text) && !string.IsNullOrWhiteSpace(ListNameTextBox.Text);
        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //parse values back to MainPage
            string extractTag = RemoveWhitespace(ListNameTextBox.Text);
            error.Visibility = Visibility.Collapsed;
            localSettings.Values["NEWlistName"] = ListNameTextBox.Text;
            localSettings.Values["NEWlistTag"] = extractTag;
            localSettings.Values["NEWlistIcon"] = defaultIconGlyph;

            ListNameTextBox.Text = string.Empty;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isUniqueTag = true;
            if (!string.IsNullOrEmpty(ListNameTextBox.Text) && !string.IsNullOrWhiteSpace(ListNameTextBox.Text))
            {
                string extractTag = RemoveWhitespace(ListNameTextBox.Text).ToLower();
                foreach (DefaultCategory item in MainPage.ins.Categories)
                {
                    if (extractTag.Equals(item.Tag))
                    {
                        isUniqueTag = false;
                    }
                }
                if (isUniqueTag)
                {
                    //proceed
                    error.Visibility = Visibility.Collapsed;
                    this.IsPrimaryButtonEnabled = true;
                }
                else
                {
                    // error
                    error.Visibility = Visibility.Visible;
                    this.IsPrimaryButtonEnabled = false;
                }
            }
            else
            {
                this.IsPrimaryButtonEnabled = false;
            }
        }

        private void Chooseicon(object sender, RoutedEventArgs e)
        {

        }
    }
}
