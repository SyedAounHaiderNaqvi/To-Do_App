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
            "Designed this very changelog UI to ensure all users of To-Do remain up-to-date with the newest features, as well as any known issues.",
            "Improved the layout of the Settings cards to better match the Windows 11 Settings UX.",
            "Added flyouts to tasks, from which currently users can edit or delete the task. More functionality will be added soon!",
            "Replaced images used in the dark and light theme toggle buttons.",
            "Added two new themes: GitHub and Fizzle! Give them a go!",
            "Removed the Nostalgia theme due to accessibility issues.",
            "The 'Choose Image' button is now hidden when the appropriate toggle is off, for visual clarity.",
            "The ability to choose custom navigation transitions is back! Choose from Entrance, Drill, Left, Right, or Suppress animations!",
        };

        public List<string> Functionality = new List<string>()
        {
            "Improved accessibility with keyboard for some dialogs."
        };

        public List<string> BugFixes = new List<string>();

        public List<string> KnownIssues = new List<string>();

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
