using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace To_Do
{
    public sealed partial class NewNavigationViewItemDialog : ContentDialog
    {
        public CustomResult _CustomResult { get; set; }
        public ElementTheme THEME;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string defaultIconGlyph = "\uEA37";

        public NewNavigationViewItemDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
            _CustomResult = CustomResult.Nothing;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = !string.IsNullOrEmpty(ListNameTextBox.Text) && !string.IsNullOrWhiteSpace(ListNameTextBox.Text);
        }

        private void Chooseicon(object sender, RoutedEventArgs e)
        {

        }

        private void NewListTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    if (this.OKButton.IsEnabled)
                    {
                        //store values
                        localSettings.Values["NEWlistName"] = ListNameTextBox.Text;
                        localSettings.Values["NEWlistIcon"] = defaultIconGlyph;

                        ListNameTextBox.Text = string.Empty;
                        _CustomResult = CustomResult.OK;
                        this.Hide();
                    }
                    break;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["NEWlistName"] = ListNameTextBox.Text;
            localSettings.Values["NEWlistIcon"] = defaultIconGlyph;

            ListNameTextBox.Text = string.Empty;
            _CustomResult = CustomResult.OK;
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _CustomResult = CustomResult.Cancel;
            this.Hide();
        }
    }
}
