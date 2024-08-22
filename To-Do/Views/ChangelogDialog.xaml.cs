using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace To_Do
{
    public sealed partial class ChangelogDialog : ContentDialog
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ElementTheme THEME;
        public List<string> Visuals = new List<string>()
        {
            "Added random fun placeholders to the task creation textbox!",
            "Added icons to promote visual clarity in settings cards.",
            "Populated the Splitview with more information about each task."
        };

        public List<string> Functionality = new List<string>()
        {
            "You can now toggle completion and importance, as well as rename each task from its Splitview (More Options).",
            "Completion of subtasks is also possible from the Splitview! More functionality will be added soon!",
        };

        public List<string> BugFixes = new List<string>()
        {
            "Fixed a runtime error when users deleted subtasks too quickly."
        };

        public List<string> KnownIssues = new List<string>()
        {
            "There is a very small chance that data from the default Pending Tasks list are lost when user rapidly deletes multiple lists from the sidebar. This bug was not reproduced on further trials.",
            "The icons grid does not respond to the corner radius setting.",
        };

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

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            localSettings.Values["firstLaunch"] = 1;
        }
    }
}
