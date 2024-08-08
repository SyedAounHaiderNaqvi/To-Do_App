using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace To_Do
{
    public sealed partial class ChangelogDialog : ContentDialog
    {
        public ElementTheme THEME;
        public ChangelogDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var txtb = sender as TextBlock;
            txtb.Text = $"Version {UtilityFunctions.GetAppVersion()}";
        }

        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
