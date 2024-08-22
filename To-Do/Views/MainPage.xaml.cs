using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Linq;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Navigation;
using To_Do.ViewModels;
using System.Collections.ObjectModel;
using To_Do.Models;
using System.Diagnostics;

namespace To_Do.Views
{
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static MainPage ins;
        public CustomNavigationViewItemViewModel viewModel;
        CustomNavigationViewItemModel selectedItem = null;
        bool canSearch = true;
        public NavigationTransitionInfo info;
        public int indexToParse;
        public NewNavigationViewItemDialog dialog;

        public MainPage()
        {

            this.InitializeComponent();
            ins = this;
            viewModel = (CustomNavigationViewItemViewModel)this.DataContext;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            UpdateTransitionCheckbox();
        }

        void UpdateTransitionCheckbox()
        {
            if (localSettings.Values["navStyle"] != null)
            {
                indexToParse = (int)localSettings.Values["navStyle"];
            }
            else
            {
                localSettings.Values["navStyle"] = 0;
                indexToParse = 0;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingUI.Visibility = Visibility.Visible;
            Ring.IsActive = true;
            await LoadCustomNavigationViewItemsFromFile();
            await Task.Delay(20);
            CustomNavigationViewItemModel MyItem = ((ObservableCollection<CustomNavigationViewItemModel>)nview.MenuItemsSource).ElementAt(0);
            nview.SelectedItem = MyItem;
            selectedItem = MyItem;
            base.OnNavigatedTo(e);
            LoadingUI.Visibility = Visibility.Collapsed;
            Ring.IsActive = false;

            // If loaded for the first time, show changelog
            if (localSettings.Values["firstLaunch"] == null)
            {
                // First launch
                ChangelogDialog dialog = new ChangelogDialog();
                _ = await dialog.ShowAsync();
            }

            //string argument = e.Parameter.ToString();

            //switch (argument)
            //{
            //    case "GoToPending":
            //        ContentFrame.Navigate(typeof(TaskPage), null, info);
            //        //await Task.Delay(10);
            //        //ContentFrame.Navigate(typeof(TaskPage));
            //        //nview.SelectedItem = Categories[0];
            //        parallax.Source = TaskPage.instance.listOfTasks;
            //        break;
            //    case "GoToSettings":
            //        ContentFrame.Navigate(typeof(TaskPage), null, info);
            //        await Task.Delay(10);
            //        ContentFrame.Navigate(typeof(Settings), null, info);
            //        nview.SelectedItem = nview.SettingsItem;
            //        parallax.Source = Settings.ins.scroller;
            //        break;
            //    default:
            //        break;
            //}
        }

        public async Task LoadCustomNavigationViewItemsFromFile()
        {
            var loadedList = await UtilityFunctions.LoadCustomNavigationViewItemsFromStorage("NavigationViewItems");
            if (loadedList != null)
            {
                viewModel.NavViewItemsList = new ObservableCollection<CustomNavigationViewItemModel>(loadedList);
            }
            else
            {
                viewModel.NavViewItemsList = new ObservableCollection<CustomNavigationViewItemModel>();
                viewModel.AddNavViewItem(await GenerateNewNavViewItem("Pending Tasks", "\uE823"));
            }
        }

        public async Task<CustomNavigationViewItemModel> GenerateNewNavViewItem(string listName, string glyph)
        {
            CustomNavigationViewItemModel item = new CustomNavigationViewItemModel()
            {
                Name = listName,
                Glyph = glyph,
                IdTag = UtilityFunctions.GetTimeStamp()
            };
            await Task.Delay(1);
            return item;
        }

        public void ImageInitialize()
        {
            if (localSettings.Values["useimg"] != null)
            {
                switch ((int)localSettings.Values["useimg"])
                {
                    case 0:
                        bgIMG.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        LoadIMG();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                bgIMG.Visibility = Visibility.Collapsed;
            }
        }

        public void RoundCornerInitialize()
        {
            if (localSettings.Values["useround"] != null)
            {
                switch ((int)localSettings.Values["useround"])
                {
                    case 0:
                        Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(0);
                        Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(1);
                        Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0);
                        break;
                    case 1:
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
                localSettings.Values["useround"] = 1;
                Application.Current.Resources["ControlCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["OverlayCornerRadius"] = new CornerRadius(8);
                Application.Current.Resources["ListViewItemCornerRadius"] = new CornerRadius(4);
                Application.Current.Resources["NavViewSplitViewCorners"] = new CornerRadius(0, 8, 8, 0);
            }
        }

        public void LoadTheme()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color fallBackPurple = new Color() { A = 255, R = 103, G = 123, B = 202 };

            if (localSettings.Values["ACCENT1_R"] != null && localSettings.Values["ACCENT2_R"] != null && localSettings.Values["BG_R"] != null)
            {
                byte a1R = (byte)localSettings.Values["ACCENT1_R"];
                byte a1G = (byte)localSettings.Values["ACCENT1_G"];
                byte a1B = (byte)localSettings.Values["ACCENT1_B"];

                byte a2R = (byte)localSettings.Values["ACCENT2_R"];
                byte a2G = (byte)localSettings.Values["ACCENT2_G"];
                byte a2B = (byte)localSettings.Values["ACCENT2_B"];

                byte bgR = (byte)localSettings.Values["BG_R"];
                byte bgG = (byte)localSettings.Values["BG_G"];
                byte bgB = (byte)localSettings.Values["BG_B"];
                byte bgA = (byte)localSettings.Values["BG_A"];
                int canUseMonet = (int)localSettings.Values["usemonet"];

                Color bgColorThatIsOpaque = new Color() { A = 255, R = bgR, G = bgG, B = bgB };
                Application.Current.Resources["SystemAccentColorDark1"] = new Color() { A = 255, R = a1R, G = a1G, B = a1B };
                Application.Current.Resources["SystemAccentColorDark2"] = new Color() { A = 255, R = a2R, G = a2G, B = a2B };
                Application.Current.Resources["SystemAccentColorLight2"] = (Color)Application.Current.Resources["SystemAccentColorDark2"] == Colors.White ? Application.Current.Resources["SystemAccentColorDark1"] : Application.Current.Resources["SystemAccentColorDark2"];
                Application.Current.Resources["SystemAccentColor"] = ThemeHelper.IsDarkTheme() ? (Color)Application.Current.Resources["SystemAccentColorLight2"] : (Color)Application.Current.Resources["SystemAccentColorDark1"];

                if (ThemeHelper.IsDarkTheme())
                {
                    SolidColorBrush darkbrush = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColorLight2"]);
                    Application.Current.Resources["NavigationViewItemForegroundSelected"] = darkbrush;
                    Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = darkbrush;
                    Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = darkbrush;
                }

                switch (canUseMonet)
                {
                    case 0:
                        if (ThemeHelper.IsDarkTheme())
                        {
                            Application.Current.Resources["SideBarColor"] = new SolidColorBrush(new Color() { A = 100, R = 33, G = 33, B = 33 });
                            Application.Current.Resources["ExpanderHeaderBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                            {
                                BlurAmount = 8,
                                TintOpacity = 0.95,
                                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                                TintColor = Color.FromArgb(255, 32, 32, 32)
                            };

                            Application.Current.Resources["ExpanderContentBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                            {
                                BlurAmount = 8,
                                TintOpacity = 0.95,
                                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                                TintColor = Color.FromArgb(255, 33, 33, 33)
                            };
                        }
                        else
                        {
                            Application.Current.Resources["SideBarColor"] = new SolidColorBrush(new Color() { A = 100, R = 245, G = 245, B = 245 });
                            Application.Current.Resources["ExpanderHeaderBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                            {
                                BlurAmount = 8,
                                TintOpacity = 0.95,
                                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                                TintColor = Colors.White
                            };

                            Application.Current.Resources["ExpanderContentBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                            {
                                BlurAmount = 8,
                                TintOpacity = 0.9,
                                BackgroundSource = AcrylicBackgroundSource.Backdrop,
                                TintColor = Color.FromArgb(255, 245, 245, 245)
                            };
                        }
                        break;
                    case 1:
                        Application.Current.Resources["SideBarColor"] = new SolidColorBrush(new Color() { A = 100, R = bgR, G = bgG, B = bgB });
                        Application.Current.Resources["ExpanderHeaderBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                        {
                            BlurAmount = 8,
                            TintOpacity = 0.95,
                            BackgroundSource = AcrylicBackgroundSource.Backdrop,
                            TintColor = UtilityFunctions.ChangeColorBrightness(bgColorThatIsOpaque, false),
                        };

                        Application.Current.Resources["ExpanderContentBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                        {
                            BlurAmount = 8,
                            TintOpacity = 0.9,
                            BackgroundSource = AcrylicBackgroundSource.Backdrop,
                            TintColor = UtilityFunctions.ChangeColorBrightness(bgColorThatIsOpaque, true),
                        };
                        break;
                    default:
                        break;
                }
                Application.Current.Resources["TextControlBackground"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];
                Application.Current.Resources["TextControlBackgroundPointerOver"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderHeaderBackground"];
                Application.Current.Resources["TextControlBackgroundFocused"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];

                //a is 150
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(new Color() { A = bgA, R = bgR, G = bgG, B = bgB });

                titleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemAccentColorDark2"];
                titleBar.ForegroundColor = titleBar.ButtonHoverBackgroundColor;

                titleBar.ButtonHoverForegroundColor = Colors.White;

                titleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"]).Color;
            }
            else
            {
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
                Application.Current.Resources["SystemAccentColorDark1"] = fallBackPurple;
                Application.Current.Resources["SystemAccentColorDark2"] = Colors.White;
                Application.Current.Resources["SystemAccentColorLight2"] = Application.Current.Resources["SystemAccentColorDark1"];
                Application.Current.Resources["SystemAccentColor"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];

                Application.Current.Resources["NavigationViewItemForegroundSelected"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = Application.Current.Resources["SystemAccentColor"];

                Color bgColor = fallBackPurple;
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(fallBackPurple);
                Application.Current.Resources["SideBarColor"] = new SolidColorBrush(new Color() { A = 100, R = fallBackPurple.R, G = fallBackPurple.G, B = fallBackPurple.B });
                titleBar.ForegroundColor = bgColor;
                titleBar.ButtonHoverBackgroundColor = bgColor;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = fallBackPurple;

                Application.Current.Resources["ExpanderHeaderBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                {
                    BlurAmount = 8,
                    TintOpacity = 0.95,
                    BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    TintColor = UtilityFunctions.ChangeColorBrightness(bgColor, false),
                };

                Application.Current.Resources["ExpanderContentBackground"] = new Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush()
                {
                    BlurAmount = 8,
                    TintOpacity = 0.9,
                    BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    TintColor = UtilityFunctions.ChangeColorBrightness(bgColor, true),
                };

                localSettings.Values["usemonet"] = 1;

                Application.Current.Resources["TextControlBackground"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];
                Application.Current.Resources["TextControlBackgroundPointerOver"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderHeaderBackground"];
                Application.Current.Resources["TextControlBackgroundFocused"] = (Microsoft.Toolkit.Uwp.UI.Media.AcrylicBrush)Application.Current.Resources["ExpanderContentBackground"];
            }
        }

        async void LoadIMG()
        {
            string token = (string)localSettings.Values["token"];
            if (token != null)
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                    if (file != null)
                    {
                        bgIMG.Visibility = Visibility.Visible;
                        using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            BitmapImage bitmapImage = new BitmapImage();

                            await bitmapImage.SetSourceAsync(fileStream);
                            bgIMG.Source = bitmapImage;
                        }
                    }
                    else
                    {
                        bgIMG.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                bgIMG.Visibility = Visibility.Collapsed;
            }
        }

        private void CoreTitlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        //public void CreateThreeTileNotifications()
        //{
        //    var TaskItems = TaskPage.instance.viewModel.TasksList;
        //    if (TaskItems.Count > 0)
        //    {
        //        List<string> tasks = new List<string>();
        //        string title = "Tasks";
        //        for (int i = 0; i < TaskItems.Count; i++)
        //        {
        //            tasks.Add(TaskItems[i].Description);
        //        }
        //        int amountOfTasks = TaskItems.Count;

        //        TileContent content = new TileContent()
        //        {
        //            Visual = new TileVisual()
        //            {
        //                Branding = TileBranding.None,
        //                TileSmall = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        TextStacking = TileTextStacking.Center,
        //                        Children =
        //                            {
        //                                new AdaptiveText()
        //                                {
        //                                    Text = TaskItems.Count.ToString(),
        //                                    HintAlign = AdaptiveTextAlign.Center,
        //                                    HintWrap = true,
        //                                    HintStyle = AdaptiveTextStyle.Caption
        //                                },
        //                                new AdaptiveText()
        //                                {
        //                                    Text = "Tasks",
        //                                    HintAlign = AdaptiveTextAlign.Center,
        //                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                                }
        //                            }
        //                    }
        //                },
        //                TileMedium = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        Children = {
        //                            new AdaptiveText()
        //                            {
        //                                Text = title,
        //                                HintStyle = AdaptiveTextStyle.Base,
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = "1) " + tasks[0],
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {

        //                                Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {

        //                                Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                        }
        //                    }
        //                },

        //                TileWide = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        Children = {
        //                            new AdaptiveText()
        //                            {
        //                                Text = title,
        //                                HintStyle = AdaptiveTextStyle.Base
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = "1) " + tasks[0],
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            }
        //                        }
        //                    }
        //                },
        //                TileLarge = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        Children = {
        //                            new AdaptiveText()
        //                            {
        //                                Text = title,
        //                                HintStyle = AdaptiveTextStyle.Base
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = "1) " + tasks[0],
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 3 ? "4) " + tasks[3] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 4 ? "5) " + tasks[4] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 5 ? "6) " + tasks[5] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 6 ? "7) " + tasks[6] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 7 ? "8) " + tasks[7] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 8 ? "9) " + tasks[8] : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },

        //                            new AdaptiveText()
        //                            {
        //                                Text = tasks.Count > 9 ? $"+{amountOfTasks - 9} task(s)" : "",
        //                                HintStyle = AdaptiveTextStyle.CaptionSubtle
        //                            },
        //                        }
        //                    }
        //                }
        //            }
        //        };
        //        var notif = new TileNotification(content.GetXml());
        //        TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
        //    }
        //    else
        //    {
        //        //all tasks complete
        //        TileContent content = new TileContent()
        //        {
        //            Visual = new TileVisual()
        //            {
        //                Branding = TileBranding.None,
        //                TileSmall = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        TextStacking = TileTextStacking.Center,
        //                        Children =
        //                            {
        //                                new AdaptiveGroup()
        //                                {
        //                                    Children =
        //                                    {
        //                                        new AdaptiveSubgroup()
        //                                        {
        //                                            Children =
        //                                            {
        //                                                new AdaptiveImage()
        //                                                {
        //                                                    Source = "Images/tileIcon.png",
        //                                                    HintAlign = AdaptiveImageAlign.Center
        //                                                },
        //                                            }
        //                                        },
        //                                    }
        //                                },
        //                                new AdaptiveGroup()
        //                                {
        //                                    Children =
        //                                    {
        //                                        new AdaptiveSubgroup()
        //                                        {
        //                                            Children =
        //                                            {
        //                                                new AdaptiveText()
        //                                                {
        //                                                    Text = "Done!",
        //                                                    HintAlign = AdaptiveTextAlign.Center,
        //                                                    HintWrap = true,
        //                                                    HintMaxLines = 2,
        //                                                    HintStyle = AdaptiveTextStyle.Base
        //                                                }
        //                                            }
        //                                        },
        //                                    }
        //                                },
        //                            }
        //                    }
        //                },
        //                TileMedium = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        TextStacking = TileTextStacking.Center,
        //                        Children = {
        //                                new AdaptiveImage()
        //                                {
        //                                    Source = "Images/tileIcon.png",
        //                                    HintAlign = AdaptiveImageAlign.Center
        //                                },

        //                                new AdaptiveText()
        //                                {
        //                                    Text = "All Done!",
        //                                    HintAlign = AdaptiveTextAlign.Center,
        //                                    HintWrap = true,
        //                                    HintMaxLines = 2,
        //                                    HintStyle = AdaptiveTextStyle.Base
        //                                }
        //                            }
        //                    }
        //                },

        //                TileWide = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        TextStacking = TileTextStacking.Center,
        //                        Children = {

        //                                new AdaptiveImage()
        //                                {
        //                                    Source = "Images/tileIcon.png",
        //                                    HintAlign = AdaptiveImageAlign.Center
        //                                },
        //                                new AdaptiveText()
        //                                {
        //                                    Text = "All Tasks Completed!",
        //                                    HintAlign = AdaptiveTextAlign.Center,
        //                                    HintWrap = true,
        //                                    HintStyle = AdaptiveTextStyle.Base
        //                                }
        //                            }
        //                    }
        //                },
        //                TileLarge = new TileBinding()
        //                {
        //                    Content = new TileBindingContentAdaptive()
        //                    {
        //                        TextStacking = TileTextStacking.Center,
        //                        Children = {
        //                                new AdaptiveImage()
        //                                {
        //                                    Source = "Images/tileIcon.png",
        //                                    HintAlign = AdaptiveImageAlign.Center
        //                                },
        //                                new AdaptiveText()
        //                                {
        //                                    Text = "All Tasks Completed!",
        //                                    HintAlign = AdaptiveTextAlign.Center,
        //                                    HintWrap = true,
        //                                    HintStyle = AdaptiveTextStyle.Base
        //                                }
        //                            }
        //                    }
        //                }
        //            }
        //        };
        //        var notif = new TileNotification(content.GetXml())
        //        {
        //            ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(1)
        //        };
        //        TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
        //    }
        //}

        public async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var def = e.GetDeferral();
            LoadingUI.Visibility = Visibility.Visible;
            //if (Environment.OSVersion.Version.Build < 22000)
            //{
            //    CreateThreeTileNotifications();
            //}
            //CreateThreeTileNotifications();

            if (ContentFrame.Content.GetType() != typeof(Settings))
            {
                // Close the split view of current page if opened
                // This ensures all changes are actually saved before saving current list tasks
                if (((TaskPage)ContentFrame.Content).moreOptionsSplitView.IsPaneOpen)
                {
                    ((TaskPage)ContentFrame.Content).TryEditTask();
                    ((TaskPage)ContentFrame.Content).moreOptionsSplitView.IsPaneOpen = false;
                }
                // Save currently open list
                await ((TaskPage)ContentFrame.Content).ManualSave();
            }

            await UtilityFunctions.SaveCustomNavigationViewItemsToStorage("NavigationViewItems", viewModel.NavViewItemsList);
            def.Complete();
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Height = coreTitleBar.Height;

            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            Ring.IsActive = true;
            LoadingUI.Visibility = Visibility.Visible;

            int styleIndex = (int)localSettings.Values["navStyle"];
            switch (styleIndex)
            {
                case 0:
                    info = new EntranceNavigationTransitionInfo();
                    break;
                case 1:
                    info = new DrillInNavigationTransitionInfo();
                    break;
                case 2:
                    info = new SuppressNavigationTransitionInfo();
                    break;
                case 3:
                    info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
                    break;
                case 4:
                    info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
                    break;
                default:
                    info = new EntranceNavigationTransitionInfo();
                    break;
            }

            if (args.IsSettingsSelected)
            {
                nview.AutoSuggestBox.IsEnabled = false;
                ContentFrame.Navigate(typeof(Settings), null, info);
            }
            else
            {
                nview.AutoSuggestBox.IsEnabled = true;
                selectedItem = (CustomNavigationViewItemModel)args.SelectedItem;
                if (selectedItem != null)
                {
                    ContentFrame.Navigate(typeof(TaskPage), selectedItem, info);
                }
            }
            LoadingUI.Visibility = Visibility.Collapsed;
            Ring.IsActive = false;
        }

        private void nview_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 48;

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        private List<QueryFormat> SearchControls(string query)
        {
            var suggestions = new List<QueryFormat>();
            var querySplit = query.Split(" ");
            string currentPageName = TaskPage.instance.nameOfThisPage;

            var matchingItems = TaskPage.instance.viewModel.TasksList.ToList().Where(
                item =>
                {
                    bool flag = true;
                    foreach (string queryToken in querySplit)
                    {
                        // Check if token is not in string 
                        if (item.Description.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                        {
                            // Token is not in string, so we ignore this item. 
                            flag = false;
                        }
                    }
                    return flag;
                });
            foreach (var item in matchingItems)
            {
                string glyph = item.IsCompleted ? "\uE73E" : "\uEA3A";
                suggestions.Add(new QueryFormat(item.Description, currentPageName, glyph));
            }

            return suggestions.OrderByDescending(i => i.Title.StartsWith(query, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title).ToList();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (canSearch)
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    var suggestions = SearchControls(sender.Text);

                    if (suggestions.Count > 0)
                    {
                        sender.ItemsSource = suggestions;
                    }
                    else
                    {
                        sender.ItemsSource = new List<QueryFormat> { new QueryFormat("No results found", "Everywhere", "\uE711") };
                    }
                }
            }
        }

        private async void SelectControl(QueryFormat desc)
        {
            if (desc != null && desc.location != "Everywhere")
            {
                if (ContentFrame.CurrentSourcePageType == typeof(TaskPage))
                {
                    var list = TaskPage.instance.viewModel.TasksList;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (desc.Title.Equals(list[j].Description))
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => calc(j, TaskPage.instance.listOfTasks));
                            break;
                        }
                    }
                }
            }
            LoseFocus(searchbox);
        }

        public async void calc(int index, ListView choice)
        {
            await ScrollToIndex(choice, index);
            searchbox.Text = string.Empty;
            LoseFocus(searchbox);
        }

        private void LoseFocus(object sender)
        {
            var control = sender as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }

        public ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer viewer)
            {
                return viewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }

        public async Task ScrollToIndex(ListView listViewBase, int index)
        {
            bool isVirtualizing = default;
            double previousHorizontalOffset = default, previousVerticalOffset = default;

            // get the ScrollViewer withtin the ListView/GridView
            var scrollViewer = GetScrollViewer(listViewBase);
            if (scrollViewer != null)
            {
                // get the SelectorItem to scroll to
                var selectorItem = listViewBase.ContainerFromIndex(index) as ListViewItem;

                // when it's null, means virtualization is on and the item hasn't been realized yet
                if (selectorItem == null)
                {
                    isVirtualizing = true;

                    previousHorizontalOffset = scrollViewer.HorizontalOffset;
                    previousVerticalOffset = scrollViewer.VerticalOffset;

                    // call task-based ScrollIntoViewAsync to realize the item
                    await ScrollIntoViewAsync(listViewBase, listViewBase.Items[index]);

                    // this time the item shouldn't be null again
                    selectorItem = (ListViewItem)listViewBase.ContainerFromIndex(index);
                }

                // calculate the position object in order to know how much to scroll to
                var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
                var position = transform.TransformPoint(new Point(0, 0));

                // when virtualized, scroll back to previous position without animation
                if (isVirtualizing)
                {
                    await ChangeViewAsync(scrollViewer, previousHorizontalOffset, previousVerticalOffset, true);
                }

                // scroll to desired position with animation!
                scrollViewer.ChangeView(position.X, position.Y, null);
            }

        }

        public async Task ScrollIntoViewAsync(ListView listViewBase, object item)
        {
            var tcs = new TaskCompletionSource<object>();
            var scrollViewer = GetScrollViewer(listViewBase);

            EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
            try
            {
                scrollViewer.ViewChanged += viewChanged;
                listViewBase.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                await tcs.Task;
            }
            finally
            {
                scrollViewer.ViewChanged -= viewChanged;
            }
        }

        public async Task ChangeViewAsync(ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, bool disableAnimation)
        {
            var tcs = new TaskCompletionSource<object>();

            EventHandler<ScrollViewerViewChangedEventArgs> viewChanged = (s, e) => tcs.TrySetResult(null);
            try
            {
                scrollViewer.ViewChanged += viewChanged;
                scrollViewer.ChangeView(horizontalOffset, verticalOffset, null, disableAnimation);
                await tcs.Task;
            }
            finally
            {
                scrollViewer.ViewChanged -= viewChanged;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is QueryFormat)
            {
                //User selected an item, take an action
                SelectControl(args.ChosenSuggestion as QueryFormat);
            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                //Do a fuzzy search based on the text
                var suggestions = SearchControls(sender.Text);
                if (suggestions.Count > 0)
                {
                    SelectControl(suggestions.FirstOrDefault());
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is QueryFormat control)
            {
                if (control.Title != "To_Do.QueryFormat")
                {
                    sender.Text = control.Title;

                }
            }
        }

        private async void AddNewListToNavView(object sender, RoutedEventArgs e)
        {
            dialog = new NewNavigationViewItemDialog();
            Grid.SetRowSpan(dialog, 2);
            dialog.CancelButton.Style = (Style)Application.Current.Resources["ButtonStyle1"];

            Grid grid = (Grid)dialog.Content;
            TextBox EditTextBox = (TextBox)VisualTreeHelper.GetChild(grid, 0);
            dialog.OKButton.IsEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
            TextChanged(EditTextBox);
            await dialog.ShowAsync();
            if (dialog._CustomResult == CustomResult.OK)
            {
                //do create new list
                string name = (string)localSettings.Values["NEWlistName"];
                string glyph = (string)localSettings.Values["NEWlistIcon"];
                var item = await GenerateNewNavViewItem(name, glyph);
                viewModel.AddNavViewItem(item);
            }
        }

        public void TextChanged(TextBox b)
        {
            if (dialog != null)
                dialog.OKButton.IsEnabled = !(string.IsNullOrEmpty(b.Text) || !string.IsNullOrWhiteSpace(b.Text));
        }

        private void searchbox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (canSearch)
            {
                var suggestions = SearchControls(((AutoSuggestBox)sender).Text);
                if (suggestions.Count > 0)
                {
                    ((AutoSuggestBox)sender).ItemsSource = suggestions;
                }
                else
                {
                    ((AutoSuggestBox)sender).ItemsSource = new List<QueryFormat> { new QueryFormat("No results found", "Everywhere", "\uE711") };
                }
            }
        }

        private void NavigationViewItem_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var parentControl = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender;
            Button btn = UtilityFunctions.FindControl<Button>(parentControl, typeof(Button), "deleteListButton");
            btn.Visibility = Visibility.Collapsed;
        }

        private void NavigationViewItem_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var item = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender;
            var model = (CustomNavigationViewItemModel)item.DataContext;
            Button btn = UtilityFunctions.FindControl<Button>(item, typeof(Button), "deleteListButton");

            if (model.IdTag == viewModel.NavViewItemsList[0].IdTag)
            {
                // this is first so hide button
                btn.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn.Visibility = Visibility.Visible;
            }

        }

        private void NavigationViewItem_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var parentControl = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender;
            Button btn = UtilityFunctions.FindControl<Button>(parentControl, typeof(Button), "deleteListButton");
            btn.Visibility = Visibility.Collapsed;
        }

        private async void DeleteList(object sender, RoutedEventArgs e)
        {
            Ring.IsActive = true;
            LoadingUI.Visibility = Visibility.Visible;
            
            var itemModel = (CustomNavigationViewItemModel)((sender as Button).DataContext);

            nview.AutoSuggestBox.IsEnabled = true;
            await Task.Delay(20);
            CustomNavigationViewItemModel MyItem = ((ObservableCollection<CustomNavigationViewItemModel>)nview.MenuItemsSource).ElementAt(0);
            nview.SelectedItem = MyItem;
            selectedItem = MyItem;

            //delete the task we just had right now
            viewModel.DeleteNavViewItem(itemModel.IdTag);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = (StorageFile)await folder.TryGetItemAsync($"{itemModel.IdTag}.json");
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

            LoadingUI.Visibility = Visibility.Collapsed;
            Ring.IsActive = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTheme();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);
            Window.Current.SetTitleBar(AppTitleBar);
            coreTitleBar.LayoutMetricsChanged += (s, a) => UpdateTitleBarLayout(s);
            coreTitleBar.IsVisibleChanged += CoreTitlebar_IsVisibleChanged;
            ImageInitialize();
            RoundCornerInitialize();
            ContentFrame.Navigate(typeof(Settings), null, info);
            //parallax.Source = TaskPage.instance.listOfTasks;
            LoseFocus(searchbox);
        }
    }

    public class QueryFormat
    {
        public string Title = "";
        public string location = "";
        public string glyph = "";

        public QueryFormat(string _title, string _location, string _glyph)
        {
            Title = _title;
            location = _location;
            glyph = _glyph;
        }
    }
}
