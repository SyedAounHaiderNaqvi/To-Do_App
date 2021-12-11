using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace To_Do
{
    public sealed partial class CustomColorThemeContentDialog : ContentDialog
    {
        public ElementTheme THEME;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public CustomColorThemeContentDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
        }

        private void CloseLightThemeBGFlyout(object sender, RoutedEventArgs e)
        {
            lightthemebgflyout.Hide();
        }

        private void CloseLightThemeAccentFlyout(object sender, RoutedEventArgs e)
        {
            lightthemeaccentflyout.Hide();
        }

        private void CloseDarkThemeBGFlyout(object sender, RoutedEventArgs e)
        {
            darkthemebgflyout.Hide();
        }

        private void CloseDarkThemeAccentFlyout(object sender, RoutedEventArgs e)
        {
            darkthemeaccentflyout.Hide();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            localSettings.Values["tempLightBG_R"] = lightBGColorPicker.Color.R;
            localSettings.Values["tempLightBG_G"] = lightBGColorPicker.Color.G;
            localSettings.Values["tempLightBG_B"] = lightBGColorPicker.Color.B;

            localSettings.Values["tempLightACCENT_R"] = lightaccentColorPicker.Color.R;
            localSettings.Values["tempLightACCENT_G"] = lightaccentColorPicker.Color.G;
            localSettings.Values["tempLightACCENT_B"] = lightaccentColorPicker.Color.B;

            localSettings.Values["tempDarkBG_R"] = darkBGColorPicker.Color.R;
            localSettings.Values["tempDarkBG_G"] = darkBGColorPicker.Color.G;
            localSettings.Values["tempDarkBG_B"] = darkBGColorPicker.Color.B;

            localSettings.Values["tempDarkACCENT_R"] = darkaccentColorPicker.Color.R;
            localSettings.Values["tempDarkACCENT_G"] = darkaccentColorPicker.Color.G;
            localSettings.Values["tempDarkACCENT_B"] = darkaccentColorPicker.Color.B;
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values["tempLightBG_R"] != null)
            {
                lightBGColorPicker.Color = Color.FromArgb(255, (byte)localSettings.Values["tempLightBG_R"], (byte)localSettings.Values["tempLightBG_G"], (byte)localSettings.Values["tempLightBG_B"]);
                lightaccentColorPicker.Color = Color.FromArgb(255, (byte)localSettings.Values["tempLightACCENT_R"], (byte)localSettings.Values["tempLightACCENT_G"], (byte)localSettings.Values["tempLightACCENT_B"]);
                darkBGColorPicker.Color = Color.FromArgb(255, (byte)localSettings.Values["tempDarkBG_R"], (byte)localSettings.Values["tempDarkBG_G"], (byte)localSettings.Values["tempDarkBG_B"]);
                darkaccentColorPicker.Color = Color.FromArgb(255, (byte)localSettings.Values["tempDarkACCENT_R"], (byte)localSettings.Values["tempDarkACCENT_G"], (byte)localSettings.Values["tempDarkACCENT_B"]);
            } else
            {
                lightBGColorPicker.Color = Color.FromArgb(255, 103, 123, 202);
                lightaccentColorPicker.Color = Colors.White;
                darkBGColorPicker.Color = Color.FromArgb(255, 44, 57, 107);
                darkaccentColorPicker.Color = Color.FromArgb(255, 150, 173, 255);
            }
        }
    }
}
