using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace To_Do
{
    public sealed partial class EditDialogContent : ContentDialog
    {
        public ElementTheme THEME;
        public EditDialogContent()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
        }

        private void EditBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
        }
    }
}
