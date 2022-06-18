using System;
using System.Collections.ObjectModel;
using To_Do.NavigationPages;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Media;

namespace To_Do
{
    public sealed partial class Settings : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ObservableCollection<GridThemeItem> gridItems = new ObservableCollection<GridThemeItem>();

        public static Settings ins;
        public ContentDialog dialog;

        public Settings()
        {
            this.InitializeComponent();
            Loaded += Settings_Loaded;
            ins = this;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            navStyleCombo.SelectedIndex = MainPage.ins.indexToParse;
            LoadGridItems();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainPage.ins.parallax.Source = scroller;
            pendingtasks.instance.lastDataParseTag = "settings";

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
                        bgimgbutton.IsEnabled = false;
                        MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        btntoggle.IsOn = true;
                        bgimgbutton.IsEnabled = true;

                        if (localSettings.Values["token"] != null)
                        {
                            MainPage.ins.bgIMG.Visibility = Visibility.Visible;

                        }
                        else
                        {
                            MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                btntoggle.IsOn = false;
                bgimgbutton.IsEnabled = false;
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
                        break;
                    case 1:
                        RoundCornerToggle.IsOn = true;
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
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
            lavender.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Lavender.png"));

            var pink = CreateNewTheme(255, 255, 255, 171, 107, 173);
            pink.darkThemeVariant = CreateNewTheme(227, 100, 232, 57, 13, 59);
            pink.tooltip = "Pretty Pink";
            pink.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_PrettyPink.png"));

            var red = CreateNewTheme(255, 255, 255, 211, 92, 124);
            red.darkThemeVariant = CreateNewTheme(222, 71, 71, 46, 0, 0);
            red.tooltip = "Rich Red";
            red.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_SmoothCherry.png"));

            var orange = CreateNewTheme(255, 255, 255, 218, 98, 94);
            orange.darkThemeVariant = CreateNewTheme(227, 115, 59, 66, 22, 0);
            orange.tooltip = "Bright Orange";
            orange.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_DefinitelyOrange.png"));

            var green = CreateNewTheme(255, 255, 255, 62, 149, 110);
            green.darkThemeVariant = CreateNewTheme(50, 227, 103, 0, 54, 30);
            green.tooltip = "Natural Green";
            green.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_NaturalGreen.png"));

            var aqua = CreateNewTheme(255, 255, 255, 56, 145, 140);
            aqua.darkThemeVariant = CreateNewTheme(49, 214, 207, 0, 48, 46);
            aqua.tooltip = "Cool Aqua";
            aqua.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_CoolAqua.png"));

            var grey = CreateNewTheme(255, 255, 255, 123, 137, 148);
            grey.darkThemeVariant = CreateNewTheme(137, 157, 173, 29, 34, 38);
            grey.tooltip = "Dull Grey";
            grey.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_DullGrey.png"));

            var blueTwoTone = CreateNewTheme(60, 108, 176, 223, 237, 249);
            blueTwoTone.darkThemeVariant = CreateNewTheme(51, 163, 255, 0, 0, 0);
            blueTwoTone.tooltip = "Navy Blue";
            blueTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_BlueButCooler.png"));

            var purpleTwoTone = CreateNewTheme(132, 92, 154, 242, 231, 249);
            purpleTwoTone.darkThemeVariant = CreateNewTheme(191, 104, 237, 0, 0, 0);
            purpleTwoTone.tooltip = "Royal Purple";
            purpleTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_RoyalPurple.png"));

            var redTwoTone = CreateNewTheme(190, 94, 122, 255, 228, 233);
            redTwoTone.darkThemeVariant = CreateNewTheme(250, 57, 57, 0, 0, 0);
            redTwoTone.tooltip = "Cold Red";
            redTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_CantGetEnoughRed.png"));

            var orangeTwoTone = CreateNewTheme(178, 86, 62, 249, 232, 222);
            orangeTwoTone.darkThemeVariant = CreateNewTheme(237, 113, 59, 0, 0, 0);
            orangeTwoTone.tooltip = "Citrus-y";
            orangeTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Citrusy.png"));

            var discord = CreateNewTheme(71, 82, 196, 242, 243, 245);
            discord.darkThemeVariant = CreateNewTheme(104, 116, 242, 35, 37, 43);
            discord.tooltip = "Discord";
            discord.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Discord.png"));

            var saphire = CreateNewTheme(40, 62, 97, 119, 161, 197);
            saphire.darkThemeVariant = CreateNewTheme(133, 165, 194, 42, 54, 76);
            saphire.tooltip = "Sapphire";
            saphire.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Sapphire.png"));

            var kimbie = CreateNewTheme(162, 125, 109, 255, 236, 211);
            kimbie.darkThemeVariant = CreateNewTheme(179, 87, 5, 31, 19, 5);
            kimbie.tooltip = "Kimbie";
            kimbie.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Kimbie.png"));

            var memory = CreateNewTheme(1, 1, 255, 255, 255, 255);
            memory.darkThemeVariant = CreateNewTheme(180, 180, 180, 1, 1, 255);
            memory.tooltip = "Nostalgia";
            memory.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_Nostalgia.png"));

            var kde = CreateNewTheme(255, 255, 255, 61, 174, 233);
            kde.darkThemeVariant = CreateNewTheme(61, 174, 233, 28, 38, 43);
            kde.tooltip = "KDE";
            kde.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_KDE.png"));

            var orangeBlueTone = CreateNewTheme(0, 116, 163, 245, 231, 220);
            orangeBlueTone.darkThemeVariant = CreateNewTheme(230, 122, 0, 10, 50, 92);
            orangeBlueTone.tooltip = "Beach";
            orangeBlueTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_BeachAndInverse.png"));

            var greenTwoTone = CreateNewTheme(51, 128, 96, 213, 241, 229);
            greenTwoTone.darkThemeVariant = CreateNewTheme(50, 194, 101, 0, 0, 0);
            greenTwoTone.tooltip = "Fresher Green";
            greenTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_YayGreen.png"));

            var aquaTwoTone = CreateNewTheme(40, 128, 133, 212, 255, 254);
            aquaTwoTone.darkThemeVariant = CreateNewTheme(45, 207, 198, 0, 0, 0);
            aquaTwoTone.tooltip = "Another Aqua";
            aquaTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_AnotherAqua.png"));

            var greyTwoTone = CreateNewTheme(98, 110, 121, 231, 236, 240);
            greyTwoTone.darkThemeVariant = CreateNewTheme(168, 180, 191, 0, 0, 0);
            greyTwoTone.tooltip = "Smoky Day";
            greyTwoTone.imgSource = new BitmapImage(new Uri("ms-appx:///Images/ItemTemplates/ItemTemplate_MoreDullGrey.png"));


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
            //a is 150
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

            Application.Current.Resources["ExpanderHeaderBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
            {
                BlurAmount = 8,
                TintOpacity = 0.95,
                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                TintColor = ChangeColorBrightness(b, false),
            };

            Application.Current.Resources["ExpanderContentBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
            {
                BlurAmount = 8,
                TintOpacity = 0.9,
                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                TintColor = ChangeColorBrightness(b, true),
            };

            Application.Current.Resources["TextControlBackground"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];
            Application.Current.Resources["TextControlBackgroundPointerOver"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderHeaderBackground"];

            Application.Current.Resources["TextControlBackgroundFocused"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];


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

        public Color ChangeColorBrightness(Color c, bool isContent)
        {
            float r, g, b;
            if (isContent)
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = lerp(c.R, 125f, 0.2f);
                    g = lerp(c.G, 125f, 0.2f);
                    b = lerp(c.B, 125f, 0.2f);

                }
                else
                {
                    r = lerp(c.R, 255f, 0.7f);
                    g = lerp(c.G, 255f, 0.7f);
                    b = lerp(c.B, 255f, 0.7f);
                }
            }
            else
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = lerp(c.R, 255f, 0.1f);
                    g = lerp(c.G, 255f, 0.1f);
                    b = lerp(c.B, 255f, 0.1f);

                }
                else
                {
                    r = lerp(c.R, 255f, 0.8f);
                    g = lerp(c.G, 255f, 0.8f);
                    b = lerp(c.B, 255f, 0.8f);
                }
            }
            
            
            return Color.FromArgb(c.A, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        float lerp(float a, float b, float f)
        {
            return (float)((a * (1.0 - f)) + (b * f));
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
                bgimgbutton.IsEnabled = true;
                MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                string token = (string)localSettings.Values["token"];
                if (token != null)
                {
                    if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                    {
                        var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                        if (file != null)
                        {
                            MainPage.ins.bgIMG.Visibility = Visibility.Visible;
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
                        }
                    }
                }

            }
            else
            {
                bgimgbutton.IsEnabled = false;
                localSettings.Values["useimg"] = 0;
                MainPage.ins.bgIMG.Visibility = Visibility.Collapsed;
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
                localSettings.Values["token"] = StorageApplicationPermissions.FutureAccessList.Add(file);
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(fileStream);
                    MainPage.ins.bgIMG.Source = bitmapImage;
                    MainPage.ins.bgIMG.Visibility = Visibility.Visible;
                }
            }
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
            }
            else
            {
                localSettings.Values["useround"] = 0;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(0);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(0);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(1);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0);
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

        private void HideBlurredDetails(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Fetching appropriate elements
            Grid parent = sender as Grid;
            TextBlock block = VisualTreeHelper.GetChild(parent, 6) as TextBlock;
            Border color = VisualTreeHelper.GetChild(parent, 2) as Border;
            Grid Tri = VisualTreeHelper.GetChild(parent, 4) as Grid;
            Grid transparentTri = VisualTreeHelper.GetChild(parent, 3) as Grid;

            // Animating them
            color.Opacity = 0;
            block.Translation = new System.Numerics.Vector3(0, 20, 0);
            transparentTri.Translation = new System.Numerics.Vector3(-50, 0, 0);
            Tri.Translation = new System.Numerics.Vector3(-50, 0, 0);
        }

        private void ShowBlurredDetails(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Fetching appropriate elements
            Grid parent = sender as Grid;
            Grid transparentTriGrid = VisualTreeHelper.GetChild(parent, 3) as Grid;
            Grid triGrid = VisualTreeHelper.GetChild(parent, 4) as Grid;
            Grid transparentTri = VisualTreeHelper.GetChild(transparentTriGrid, 0) as Grid;
            Grid tri = VisualTreeHelper.GetChild(triGrid, 0) as Grid;
            TextBlock block = VisualTreeHelper.GetChild(parent, 6) as TextBlock;
            Border color = VisualTreeHelper.GetChild(parent, 2) as Border;

            // Fetching hovered item's backend class
            var gridThemeItemInstance = parent.DataContext as GridThemeItem;
            Brush brushToUse;
            if (ThemeHelper.IsDarkTheme())
            {
                brushToUse = gridThemeItemInstance.darkThemeVariant.borderBrush;
            }
            else
            {
                // Set color to light background one, if foreground is white
                if ((gridThemeItemInstance.borderBrush as SolidColorBrush).Color == Colors.White)
                {
                    brushToUse = gridThemeItemInstance.backgroundBrush;
                }
                else
                {
                    brushToUse = gridThemeItemInstance.borderBrush;
                }
            }
            // Here, we set dark or light colors of hover triangles depending on theme
            transparentTri.Background = brushToUse;
            tri.Background = brushToUse;

            // Animating implicitly appropriate elements
            block.Translation = System.Numerics.Vector3.Zero;
            color.Opacity = 0.7;
            transparentTriGrid.Translation = System.Numerics.Vector3.Zero;
            triGrid.Translation = System.Numerics.Vector3.Zero;
            
        }
    }

    public class GridThemeItem
    {
        public Brush borderBrush { get; set; }
        public Brush backgroundBrush { get; set; }
        public GridThemeItem darkThemeVariant { get; set; }
        public string tooltip { get; set; }
        public BitmapImage imgSource;

        public GridThemeItem(Brush _border, Brush _bg)
        {
            borderBrush = _border;
            backgroundBrush = _bg;
        }
    }
}
