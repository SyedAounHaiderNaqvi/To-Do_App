using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace To_Do
{
    public enum CustomResult
    {
        OK,
        Cancel,
        Nothing
    }

    public sealed partial class EditDialogContent : ContentDialog
    {
        public CustomResult _CustomResult { get; set; }
        public ElementTheme THEME;
        public EditDialogContent()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
            _CustomResult = CustomResult.Nothing;
        }

        private void EditBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
        }

        private void EditTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    if (this.OKButton.IsEnabled)
                    {
                        _CustomResult = CustomResult.OK;
                        this.Hide();
                    }
                    break;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
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
