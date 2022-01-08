using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.Security.Authorization.AppCapabilityAccess;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace To_Do
{
    public sealed partial class Settings : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ObservableCollection<GridThemeItem> gridItems = new ObservableCollection<GridThemeItem>();

        public static Settings ins;
        AppCapabilityAccessStatus status;
        public ContentDialog dialog;

        public Settings()
        {
            this.InitializeComponent();
            Loaded += Settings_Loaded;
            ins = this;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            navStyleCombo.SelectedIndex = MainPage.ins.indexToParse;
            AppCapability cap = AppCapability.Create("broadFileSystemAccess");
            status = cap.CheckAccess();
            LoadGridItems();
        }

        public void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            if (lightradio.Tag.ToString() == currentTheme)
            {
                lightradio.IsChecked = true;
            }
            else
            {
                darkradio.IsChecked = true;
            }
            if (localSettings.Values["useimg"] != null)
            {
                switch ((int)localSettings.Values["useimg"])
                {
                    case 0:
                        btntoggle.IsOn = false;
                        bgimgbutton.Visibility = Visibility.Collapsed;
                        MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                        MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                        MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        btntoggle.IsOn = true;
                        if (status == AppCapabilityAccessStatus.DeniedByUser)
                        {
                            Fallbackpanel.Visibility = Visibility.Visible;
                            bgimgbutton.Visibility = Visibility.Collapsed;
                            MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                            MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                            MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            Fallbackpanel.Visibility = Visibility.Collapsed;
                            bgimgbutton.Visibility = Visibility.Visible;

                            if (localSettings.Values["imgPath"] != null)
                            {
                                MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                                MainPage.ins.acrylic.Visibility = Visibility.Visible;
                                MainPage.ins.acrylictint.Visibility = Visibility.Visible;

                            }
                            else
                            {
                                MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                                MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                                MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Fallbackpanel.Visibility = Visibility.Collapsed;
                btntoggle.IsOn = false;
                bgimgbutton.Visibility = Visibility.Collapsed;
                localSettings.Values["useimg"] = 0;
            }

            if (localSettings.Values["useround"] != null)
            {
                switch ((int)localSettings.Values["useround"])
                {
                    case 0:
                        RoundCornerToggle.IsOn = false;
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(1);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0);
                        Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(1, 0, 0, 0);
                        break;
                    case 1:
                        RoundCornerToggle.IsOn = true;
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
                        Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(8, 0, 0, 0);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                RoundCornerToggle.IsOn = true;
                localSettings.Values["useround"] = 1;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
                Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(8, 0, 0, 0);
            }

            if (ThemeHelper.IsDarkTheme())
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");

            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
            }
        }

        private GridThemeItem CreateNewTheme(byte borderR, byte borderG, byte borderB, byte bgR, byte bgG, byte bgB)
        {
            return new GridThemeItem(new SolidColorBrush(Color.FromArgb(255, borderR, borderG, borderB)), new SolidColorBrush(Color.FromArgb(255, bgR, bgG, bgB)));
        }

        void LoadGridItems()
        {
            var lavender = CreateNewTheme(255, 255, 255, 103, 123, 202);
            lavender.darkThemeVariant = CreateNewTheme(150, 173, 255, 44, 57, 107);
            lavender.tooltip = "Lush Lavender";

            var pink = CreateNewTheme(255, 255, 255, 171, 107, 173);
            pink.darkThemeVariant = CreateNewTheme(227, 100, 232, 57, 13, 59);
            pink.tooltip = "Picturesque Pink";

            var red = CreateNewTheme(255, 255, 255, 211, 92, 124);
            red.darkThemeVariant = CreateNewTheme(222, 71, 71, 46, 0, 0);
            red.tooltip = "Smooth Cherry";

            var orange = CreateNewTheme(255, 255, 255, 218, 98, 94);
            orange.darkThemeVariant = CreateNewTheme(227, 115, 59, 66, 22, 0);
            orange.tooltip = "Definitely Orange";

            var green = CreateNewTheme(255, 255, 255, 62, 149, 110);
            green.darkThemeVariant = CreateNewTheme(50, 227, 103, 0, 54, 30);
            green.tooltip = "Natural Green";

            var aqua = CreateNewTheme(255, 255, 255, 56, 145, 140);
            aqua.darkThemeVariant = CreateNewTheme(49, 214, 207, 0, 48, 46);
            aqua.tooltip = "Cool Aqua";

            var grey = CreateNewTheme(255, 255, 255, 123, 137, 148);
            grey.darkThemeVariant = CreateNewTheme(137, 157, 173, 29, 34, 38);
            grey.tooltip = "Dull Grey";

            var blueTwoTone = CreateNewTheme(60, 108, 176, 223, 237, 249);
            blueTwoTone.darkThemeVariant = CreateNewTheme(51, 163, 255, 10, 10, 10);
            blueTwoTone.tooltip = "Blue, But Cooler";

            var purpleTwoTone = CreateNewTheme(132, 92, 154, 242, 231, 249);
            purpleTwoTone.darkThemeVariant = CreateNewTheme(191, 104, 237, 10, 10, 10);
            purpleTwoTone.tooltip = "Royal Purple";

            var redTwoTone = CreateNewTheme(190, 94, 122, 255, 228, 233);
            redTwoTone.darkThemeVariant = CreateNewTheme(250, 57, 57, 10, 10, 10);
            redTwoTone.tooltip = "Can't Get Enough Red";

            var orangeTwoTone = CreateNewTheme(178, 86, 62, 249, 232, 222);
            orangeTwoTone.darkThemeVariant = CreateNewTheme(237, 113, 59, 10, 10, 10);
            orangeTwoTone.tooltip = "Citrus-y";

            var coffee = CreateNewTheme(107, 54, 0, 255, 250, 245);
            coffee.darkThemeVariant = CreateNewTheme(209, 178, 144, 46, 23, 0);
            coffee.tooltip = "Strong Coffee";

            var discord = CreateNewTheme(71, 82, 196, 242, 243, 245);
            discord.darkThemeVariant = CreateNewTheme(104, 116, 242, 35, 37, 43);
            discord.tooltip = "Discord";

            var saphire = CreateNewTheme(40, 62, 97, 119, 161, 197);
            saphire.darkThemeVariant = CreateNewTheme(133, 165, 194, 42, 54, 76);
            saphire.tooltip = "Sapphire";

            var kimbie = CreateNewTheme(162, 125, 109, 255, 236, 211);
            kimbie.darkThemeVariant = CreateNewTheme(179, 87, 5, 31, 19, 5);
            kimbie.tooltip = "Kimbie";

            var memory = CreateNewTheme(1, 1, 255, 255, 255, 255);
            memory.darkThemeVariant = CreateNewTheme(180, 180, 180, 1, 1, 255);
            memory.tooltip = "Nostalgia";

            var kde = CreateNewTheme(255, 255, 255, 61, 174, 233);
            kde.darkThemeVariant = CreateNewTheme(61, 174, 233, 28, 38, 43);
            kde.tooltip = "KDE";

            var orangeBlueTone = CreateNewTheme(0, 116, 163, 245, 231, 220);
            orangeBlueTone.darkThemeVariant = CreateNewTheme(230, 122, 0, 10, 50, 92);
            orangeBlueTone.tooltip = "Beach and Inverse";

            var greenTwoTone = CreateNewTheme(51, 128, 96, 213, 241, 229);
            greenTwoTone.darkThemeVariant = CreateNewTheme(50, 194, 101, 10, 10, 10);
            greenTwoTone.tooltip = "Yay Green";

            var aquaTwoTone = CreateNewTheme(40, 128, 133, 212, 255, 254);
            aquaTwoTone.darkThemeVariant = CreateNewTheme(45, 207, 198, 10, 10, 10);
            aquaTwoTone.tooltip = "Another Aqua";

            var greyTwoTone = CreateNewTheme(98, 110, 121, 231, 236, 240);
            greyTwoTone.darkThemeVariant = CreateNewTheme(168, 180, 191, 10, 10, 10);
            greyTwoTone.tooltip = "More Dull Grey";


            gridItems.Add(lavender);
            gridItems.Add(pink);
            gridItems.Add(red);
            gridItems.Add(orange);
            gridItems.Add(green);
            gridItems.Add(aqua);
            gridItems.Add(grey);

            gridItems.Add(blueTwoTone);
            gridItems.Add(purpleTwoTone);
            gridItems.Add(redTwoTone);
            gridItems.Add(orangeTwoTone);
            gridItems.Add(coffee);
            gridItems.Add(greenTwoTone);
            gridItems.Add(aquaTwoTone);
            gridItems.Add(greyTwoTone);
            gridItems.Add(discord);
            gridItems.Add(saphire);
            gridItems.Add(kimbie);
            gridItems.Add(memory);
            gridItems.Add(kde);
            gridItems.Add(orangeBlueTone);
        }

        private void navStyleCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            localSettings.Values["navStyle"] = navStyleCombo.SelectedIndex;
        }

        void changecol(GridThemeItem item, bool chosenFromGrid, bool custom)
        {
            GridThemeItem n;
            if (chosenFromGrid)
            {
                n = item;
            }
            else
            {
                n = ThemeHelper.IsDarkTheme() ? item.darkThemeVariant : item;
            }

            if (custom)
            {
                localSettings.Values["colorindex"] = 666;
            }
            else
            {
                for (int i = 0; i < gridItems.Count; i++)
                {
                    if (gridItems[i] == item)
                    {
                        localSettings.Values["colorindex"] = i;
                    }
                }
            }
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            Color b = (n.backgroundBrush as SolidColorBrush).Color;
            Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(new Color() { A = 150, R = b.R, G = b.G, B = b.B });
            titleBar.ButtonHoverForegroundColor = Colors.White;
            Color bgcolor = ((SolidColorBrush)n.backgroundBrush).Color;
            localSettings.Values["BG_R"] = bgcolor.R;
            localSettings.Values["BG_G"] = bgcolor.G;
            localSettings.Values["BG_B"] = bgcolor.B;

            Application.Current.Resources["SystemAccentColorDark1"] = ((SolidColorBrush)n.borderBrush).Color == Colors.White ? ((SolidColorBrush)n.backgroundBrush).Color : ((SolidColorBrush)n.borderBrush).Color;
            Application.Current.Resources["SystemAccentColorDark2"] = ((SolidColorBrush)n.borderBrush).Color;
            titleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)n.borderBrush).Color == Colors.White ? (Color)Application.Current.Resources["SystemAccentColorDark1"] : (Color)Application.Current.Resources["SystemAccentColorDark2"];//ThemeHelper.IsDarkTheme() ? ((SolidColorBrush)n.borderBrush).Color : ((SolidColorBrush)n.backgroundBrush).Color;
            titleBar.ForegroundColor = titleBar.ButtonHoverBackgroundColor;
            Application.Current.Resources["SystemAccentColorLight2"] = ((SolidColorBrush)n.borderBrush).Color;
            Application.Current.Resources["SystemAccentColor"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];


            Application.Current.Resources["NavigationViewItemForegroundSelected"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];
            Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];
            Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];

            titleBar.InactiveForegroundColor = titleBar.ForegroundColor;
            titleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)n.borderBrush).Color == Colors.White ? (Color)Application.Current.Resources["SystemAccentColorDark1"] : (Color)Application.Current.Resources["SystemAccentColorDark2"];
            if (ThemeHelper.IsDarkTheme())
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");

            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
            }

            // time to save
            Color ac1;
            Color ac2;
            if (ThemeHelper.IsDarkTheme())
            {
                ac1 = (Color)Application.Current.Resources["SystemAccentColorLight1"];
                ac2 = (Color)Application.Current.Resources["SystemAccentColorLight2"];
            }
            else
            {
                ac1 = (Color)Application.Current.Resources["SystemAccentColorDark1"];
                ac2 = (Color)Application.Current.Resources["SystemAccentColorDark2"];
            }
            localSettings.Values["ACCENT1_R"] = ac1.R;
            localSettings.Values["ACCENT1_G"] = ac1.G;
            localSettings.Values["ACCENT1_B"] = ac1.B;
            localSettings.Values["ACCENT2_R"] = ac2.R;
            localSettings.Values["ACCENT2_G"] = ac2.G;
            localSettings.Values["ACCENT2_B"] = ac2.B;
        }

        private void themeGrid_ItemClick(object sender, ItemClickEventArgs e)
        {

            GridThemeItem clickedItem = ThemeHelper.IsDarkTheme() ? (e.ClickedItem as GridThemeItem).darkThemeVariant : e.ClickedItem as GridThemeItem;
            for (int i = 0; i < gridItems.Count; i++)
            {
                if (gridItems[i] == e.ClickedItem)
                {
                    localSettings.Values["colorindex"] = i;
                    break;
                }
                else
                {
                    localSettings.Values["colorindex"] = 666;
                }
            }
            changecol(clickedItem, true, false);

        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();

            if (selectedTheme != null)
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
                if (localSettings.Values["colorindex"] != null)
                {
                    if ((int)localSettings.Values["colorindex"] == 666)
                    {
                        Color lightthemebgcolor = Color.FromArgb(255, (byte)localSettings.Values["tempLightBG_R"], (byte)localSettings.Values["tempLightBG_G"], (byte)localSettings.Values["tempLightBG_B"]);
                        Color lightthemeaccentcolor = Color.FromArgb(255, (byte)localSettings.Values["tempLightACCENT_R"], (byte)localSettings.Values["tempLightACCENT_G"], (byte)localSettings.Values["tempLightACCENT_B"]);

                        Color darkthemebgcolor = Color.FromArgb(255, (byte)localSettings.Values["tempDarkBG_R"], (byte)localSettings.Values["tempDarkBG_G"], (byte)localSettings.Values["tempDarkBG_B"]);
                        Color darkthemeaccentcolor = Color.FromArgb(255, (byte)localSettings.Values["tempDarkACCENT_R"], (byte)localSettings.Values["tempDarkACCENT_G"], (byte)localSettings.Values["tempDarkACCENT_B"]);

                        var custom = CreateNewTheme(lightthemeaccentcolor.R, lightthemeaccentcolor.G, lightthemeaccentcolor.B, lightthemebgcolor.R, lightthemebgcolor.G, lightthemebgcolor.B);
                        custom.darkThemeVariant = CreateNewTheme(darkthemeaccentcolor.R, darkthemeaccentcolor.G, darkthemeaccentcolor.B, darkthemebgcolor.R, darkthemebgcolor.G, darkthemebgcolor.B);
                        custom.tooltip = "Custom";
                        changecol(custom, false, true);

                    }
                    else
                    {
                        changecol(gridItems[(int)localSettings.Values["colorindex"]], false, false);

                    }
                }
                else
                {
                    changecol(gridItems[0], false, false);
                }
            }

            if (ThemeHelper.IsDarkTheme())
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");
                lightthemeborder.Visibility = Visibility.Collapsed;
                darkthemeborder.Visibility = Visibility.Visible;

            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                lightthemeborder.Visibility = Visibility.Visible;
                darkthemeborder.Visibility = Visibility.Collapsed;
            }
        }

        private async void OnBGIMGToggled(object sender, RoutedEventArgs e)
        {
            if (btntoggle.IsOn)
            {
                localSettings.Values["useimg"] = 1;
                if (status == AppCapabilityAccessStatus.DeniedByUser)
                {
                    Fallbackpanel.Visibility = Visibility.Visible;
                    bgimgbutton.Visibility = Visibility.Collapsed;
                    MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                    MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                    MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Fallbackpanel.Visibility = Visibility.Collapsed;
                    bgimgbutton.Visibility = Visibility.Visible;
                    MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                    MainPage.ins.acrylic.Visibility = Visibility.Visible;
                    MainPage.ins.acrylictint.Visibility = Visibility.Visible;
                    if (localSettings.Values["imgPath"] != null)
                    {
                        string path = (string)localSettings.Values["imgPath"];
                        StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                        if (file != null)
                        {
                            MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                            MainPage.ins.acrylic.Visibility = Visibility.Visible;
                            MainPage.ins.acrylictint.Visibility = Visibility.Visible;
                            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                            {
                                BitmapImage bitmapImage = new BitmapImage();

                                await bitmapImage.SetSourceAsync(fileStream);
                                MainPage.ins.bgIMG.Source = bitmapImage;
                            }
                        }
                        else
                        {
                            MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                            MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                            MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
                        }
                    }
                }

            }
            else
            {

                Fallbackpanel.Visibility = Visibility.Collapsed;
                bgimgbutton.Visibility = Visibility.Collapsed;
                localSettings.Values["useimg"] = 0;
                MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                MainPage.ins.acrylic.Visibility = Visibility.Collapsed;
                MainPage.ins.acrylictint.Visibility = Visibility.Collapsed;
            }
        }

        private async void Chooseimg(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                localSettings.Values["imgPath"] = file.Path;
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(fileStream);
                    MainPage.ins.bgIMG.Source = bitmapImage;
                    MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                    MainPage.ins.acrylic.Visibility = Visibility.Visible;
                    MainPage.ins.acrylictint.Visibility = Visibility.Visible;
                }
            }
        }

        private async void LaunchSettings(object sender, RoutedEventArgs e)
        {
            MainPage.ins.OnCloseRequest(null, null);
            var pageType = Type.GetType("To_Do.Settings");
            MainPage.ins.ContentFrame.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
            _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
        }

        private void OnRoundedCornerToggled(object sender, RoutedEventArgs e)
        {
            if (RoundCornerToggle.IsOn)
            {
                localSettings.Values["useround"] = 1;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
                Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(8, 0, 0, 0);
            }
            else
            {
                localSettings.Values["useround"] = 0;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(0);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(0);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(1);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0);
                Application.Current.Resources["TopLeftNavViewContentCorner"] = new CornerRadius(1, 0, 0, 0);
            }

            if (ThemeHelper.IsDarkTheme())
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");

            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Dark");
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
            }
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static T FindControl<T>(UIElement parent, Type targetType, string ControlName) where T : FrameworkElement
        {

            if (parent == null) return null;

            if (parent.GetType() == targetType && ((T)parent).Name == ControlName)
            {
                return (T)parent;
            }
            T result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);

                if (FindControl<T>(child, targetType, ControlName) != null)
                {
                    result = FindControl<T>(child, targetType, ControlName);
                    break;
                }
            }
            return result;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var txtb = sender as TextBlock;
            txtb.Text = $"To-Do {GetAppVersion()}";
        }

        private void LightModeChosen(object sender, RoutedEventArgs e)
        {
            lightradio.IsChecked = true;
        }

        private void DarkModeChosen(object sender, RoutedEventArgs e)
        {
            darkradio.IsChecked = true;
        }

        private async void OpenCustomColorDialog(object sender, RoutedEventArgs e)
        {
            dialog = new CustomColorThemeContentDialog();
            Grid.SetRowSpan(dialog, 2);
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["ButtonStyle1"];
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Color lightthemebgcolor = Color.FromArgb(255, (byte)localSettings.Values["tempLightBG_R"], (byte)localSettings.Values["tempLightBG_G"], (byte)localSettings.Values["tempLightBG_B"]);
                Color lightthemeaccentcolor = Color.FromArgb(255, (byte)localSettings.Values["tempLightACCENT_R"], (byte)localSettings.Values["tempLightACCENT_G"], (byte)localSettings.Values["tempLightACCENT_B"]);

                Color darkthemebgcolor = Color.FromArgb(255, (byte)localSettings.Values["tempDarkBG_R"], (byte)localSettings.Values["tempDarkBG_G"], (byte)localSettings.Values["tempDarkBG_B"]);
                Color darkthemeaccentcolor = Color.FromArgb(255, (byte)localSettings.Values["tempDarkACCENT_R"], (byte)localSettings.Values["tempDarkACCENT_G"], (byte)localSettings.Values["tempDarkACCENT_B"]);

                var custom = CreateNewTheme(lightthemeaccentcolor.R, lightthemeaccentcolor.G, lightthemeaccentcolor.B, lightthemebgcolor.R, lightthemebgcolor.G, lightthemebgcolor.B);
                custom.darkThemeVariant = CreateNewTheme(darkthemeaccentcolor.R, darkthemeaccentcolor.G, darkthemeaccentcolor.B, darkthemebgcolor.R, darkthemebgcolor.G, darkthemebgcolor.B);
                custom.tooltip = "Custom";

                changecol(custom, false, true);
            }
        }

        private async void OpenBackupDialog(object sender, RoutedEventArgs e)
        {
            dialog = new BackupContentDialog();
            Grid.SetRowSpan(dialog, 2);
            dialog.PrimaryButtonStyle = (Style)Application.Current.Resources["ButtonStyle1"];
            _ = await dialog.ShowAsync();
        }
    }

    public class GridThemeItem
    {
        public Brush borderBrush { get; set; }
        public Brush backgroundBrush { get; set; }
        public GridThemeItem darkThemeVariant { get; set; }
        public string tooltip { get; set; }

        public GridThemeItem(Brush _border, Brush _bg)
        {
            borderBrush = _border;
            backgroundBrush = _bg;
        }
    }
}
