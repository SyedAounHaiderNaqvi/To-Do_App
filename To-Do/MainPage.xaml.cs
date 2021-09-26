using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using To_Do.NavigationPages;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Diagnostics;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.Authorization.AppCapabilityAccess;

namespace To_Do
{
    public sealed partial class MainPage : Page
    {
        public List<string> savingDescriptions = new List<string>();
        public List<string> savingDates = new List<string>();
        public List<string> savingCompletedDates = new List<string>();
        public List<string> completedSaving = new List<string>();
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static MainPage ins;
        public int indexToParse;
        public List<List<string>> tasksToParse = new List<List<string>>();
        AppCapabilityAccessStatus status;
        public int PendingTasksCount = 0;
        public double BlurAmount = 32.00;

        public MainPage()
        {
            this.InitializeComponent();
            ins = this;
            var currentTheme = ThemeHelper.RootTheme.ToString();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);
            Window.Current.SetTitleBar(AppTitleBar);
            coreTitleBar.LayoutMetricsChanged += (s, a) => UpdateTitleBarLayout(s);
            coreTitleBar.IsVisibleChanged += CoreTitlebar_IsVisibleChanged;
            AppCapability cap = AppCapability.Create("broadFileSystemAccess");
            status = cap.CheckAccess();
            ImageInitialize();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            LoadTheme();
            if (localSettings.Values["navStyle"] != null)
            {
                indexToParse = (int)localSettings.Values["navStyle"];
            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                localSettings.Values["navStyle"] = 0;
                indexToParse = 0;
            }
            currentTheme = ThemeHelper.RootTheme.ToString();
        }

        public void ImageInitialize()
        {
            if (localSettings.Values["useimg"] != null)
            {
                switch ((int)localSettings.Values["useimg"])
                {
                    case 0:
                        bgIMG.Visibility = Visibility.Collapsed;
                        acrylic.Visibility = Visibility.Collapsed;
                        acrylictint.Visibility = Visibility.Collapsed;
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
                acrylic.Visibility = Visibility.Collapsed;
                acrylictint.Visibility = Visibility.Collapsed;
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

                Color bgColor = new Color() { A = 255, R = bgR, G = bgG, B = bgB };
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(bgColor);
                titleBar.ForegroundColor = bgColor;
                titleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SystemAccentColor"];//ThemeHelper.IsDarkTheme() ? new Color() { A = 255, R = a2R, G = a2G, B = a2B } : bgColor;

                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"]).Color;//new Color() { A = 255, R = a2R, G = a2G, B = a2B };
            }
            else
            {
                Application.Current.Resources["SystemAccentColorDark1"] = fallBackPurple;
                Application.Current.Resources["SystemAccentColorDark2"] = Colors.White;
                Application.Current.Resources["SystemAccentColorLight2"] = (Color)Application.Current.Resources["SystemAccentColorDark2"] == Colors.White ? Application.Current.Resources["SystemAccentColorDark1"] : Application.Current.Resources["SystemAccentColorDark2"];
                Application.Current.Resources["SystemAccentColor"] = ThemeHelper.IsDarkTheme() ? (Color)Application.Current.Resources["SystemAccentColorLight2"] : (Color)Application.Current.Resources["SystemAccentColorDark1"];

                Application.Current.Resources["NavigationViewItemForegroundSelected"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = Application.Current.Resources["SystemAccentColor"];

                Color bgColor = fallBackPurple;
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(bgColor);
                titleBar.ForegroundColor = bgColor;
                titleBar.ButtonHoverBackgroundColor = bgColor;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = fallBackPurple;
            }
        }

        async void LoadIMG()
        {
            if (localSettings.Values["imgPath"] != null)
            {
                if (status == AppCapabilityAccessStatus.DeniedByUser)
                {
                    bgIMG.Visibility = Visibility.Collapsed;
                    acrylic.Visibility = Visibility.Collapsed;
                    acrylictint.Visibility = Visibility.Collapsed;
                    Debug.WriteLine("DENIED");
                }
                else
                {
                    string path = (string)localSettings.Values["imgPath"];
                    StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                    if (file != null)
                    {
                        bgIMG.Visibility = Visibility.Visible;
                        acrylic.Visibility = Visibility.Visible;
                        acrylictint.Visibility = Visibility.Visible;
                        Debug.WriteLine("ok ok");
                        using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            // Set the image source to the selected bitmap
                            BitmapImage bitmapImage = new BitmapImage();

                            await bitmapImage.SetSourceAsync(fileStream);
                            bgIMG.Source = bitmapImage;
                        }
                    }
                    else
                    {
                        bgIMG.Visibility = Visibility.Collapsed;
                        acrylic.Visibility = Visibility.Collapsed;
                        acrylictint.Visibility = Visibility.Collapsed;
                        Debug.WriteLine("DENIED");
                    }
                }
            }
            else
            {
                bgIMG.Visibility = Visibility.Collapsed;
                acrylic.Visibility = Visibility.Collapsed;
                acrylictint.Visibility = Visibility.Collapsed;
            }
        }

        private void CoreTitlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void CreateThreeTileNotifications()
        {
            var TaskItems = PendingTasks.instance.TaskItems;
            if (TaskItems.Count > 0)
            {
                List<string> tasks = new List<string>();
                string title = "Tasks to do";
                for (int i = 0; i < TaskItems.Count; i++)
                {
                    tasks.Add(TaskItems[i].Description);
                }
                int amountOfTasks = TaskItems.Count;

                TileContent content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.None,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = TaskItems.Count.ToString(),
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Caption
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "Tasks",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                }
                            }
                        },
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base,
                                },

                                new AdaptiveText()
                                {
                                    Text = "O " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 1 ? "O " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 2 ? "O " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base
                                },

                                new AdaptiveText()
                                {
                                    Text = "O " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "O " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "O " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? $"+{amountOfTasks - 3} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                            }
                        },
                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children = {
                                new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Base
                                },

                                new AdaptiveText()
                                {
                                    Text = "O " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "O " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "O " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? "O " + tasks[3] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 4 ? "O " + tasks[4] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 5 ? "O " + tasks[5] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 6 ? "O " + tasks[6] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 7 ? "O " + tasks[7] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 8 ? "O " + tasks[8] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 9 ? $"+{amountOfTasks - 9} task(s)" : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                            }
                        }
                    }
                };
                var notif = new TileNotification(content.GetXml());
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
            }
            else
            {
                //all tasks complete
                TileContent content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.None,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                                {
                                    new AdaptiveGroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveSubgroup()
                                            {
                                                Children =
                                                {
                                                    new AdaptiveImage()
                                                    {
                                                        Source = "Images/tileIcon.png",
                                                        HintAlign = AdaptiveImageAlign.Center
                                                    },
                                                }
                                            },
                                        }
                                    },
                                    new AdaptiveGroup()
                                    {
                                        Children =
                                        {
                                            new AdaptiveSubgroup()
                                            {
                                                Children =
                                                {
                                                    new AdaptiveText()
                                                    {
                                                        Text = "Done!",
                                                        HintAlign = AdaptiveTextAlign.Center,
                                                        HintWrap = true,
                                                        HintMaxLines = 2,
                                                        HintStyle = AdaptiveTextStyle.Base
                                                    }
                                                }
                                            },
                                        }
                                    },
                                }
                            }
                        },
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {
                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },

                                    new AdaptiveText()
                                    {
                                        Text = "All Done!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintMaxLines = 2,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {

                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "All Tasks Completed!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        },
                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children = {
                                    new AdaptiveImage()
                                    {
                                        Source = "Images/tileIcon.png",
                                        HintAlign = AdaptiveImageAlign.Center
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "All Tasks Completed!",
                                        HintAlign = AdaptiveTextAlign.Center,
                                        HintWrap = true,
                                        HintStyle = AdaptiveTextStyle.Base
                                    }
                                }
                            }
                        }
                    }
                };
                var notif = new TileNotification(content.GetXml())
                {
                    ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(1)
                };
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notif);
            }
        }

        public void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            PendingTasks ins = PendingTasks.instance;
            //save all descriptions
            foreach (TODOTask tODO in ins.TaskItems)
            {
                string temp = tODO.Description;
                string date = tODO.Date;
                savingDescriptions.Add(temp);
                savingDates.Add(date);
            }
            Type pageType = Type.GetType("To_Do.NavigationPages.CompletedTasks");
            ContentFrame.Navigate(pageType, tasksToParse, new SuppressNavigationTransitionInfo());
            tasksToParse.Clear();
            foreach (TODOTask task in CompletedTasks.instance.CompleteTasks)
            {
                string temp = task.Description;
                string date = task.Date;
                completedSaving.Add(temp);
                savingCompletedDates.Add(date);
            }
            string jsonFile = JsonConvert.SerializeObject(savingDescriptions);
            string compJsonFile = JsonConvert.SerializeObject(completedSaving);
            string dateJsonFile = JsonConvert.SerializeObject(savingDates);
            string compdateJsonFile = JsonConvert.SerializeObject(savingCompletedDates);
            localSettings.Values["Tasks"] = jsonFile;
            localSettings.Values["TasksDone"] = compJsonFile;
            localSettings.Values["DateOfTasks"] = dateJsonFile;
            localSettings.Values["DateOfTasksDone"] = compdateJsonFile;
            CreateThreeTileNotifications();
        }


        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Height = coreTitleBar.Height;

            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            LoadingUI.Visibility = Visibility.Visible;
            int styleIndex = (int)localSettings.Values["navStyle"];
            NavigationTransitionInfo info;
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
            Type pageType;
            //Debug.WriteLine(args.SelectedItem.GetType().FullName);
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            if (args.IsSettingsSelected)
            {
                pageType = Type.GetType("To_Do.Settings");
                //nview.Header = "Settings";
                ContentFrame.Navigate(pageType, null, info);
            }
            else
            {
                string selectedItemTag = selectedItem.Tag.ToString();
                string pageName = "To_Do.NavigationPages." + selectedItemTag;
                pageType = Type.GetType(pageName);
                switch (selectedItemTag)
                {
                    case "CompletedTasks":
                        //nview.Header = "Completed Tasks";
                        ContentFrame.Navigate(pageType, tasksToParse, info);
                        tasksToParse.Clear();
                        break;
                    case "PendingTasks":
                        //nview.Header = "Pending Tasks";
                        ContentFrame.Navigate(pageType, null, info);
                        break;
                    default:
                        break;
                }
            }
            LoadingUI.Visibility = Visibility.Collapsed;
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
                bgIMG.Margin = new Thickness(0, -60, 0, 0);
                acrylic.Margin = new Thickness(-4, -61, -4, -4);
                acrylictint.Margin = new Thickness(-4, -61, -4, -4);
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                bgIMG.Margin = new Thickness(0, -82, 0, 0);
                acrylic.Margin = new Thickness(-4, -84, -4, -4);
                acrylictint.Margin = new Thickness(-4, -84, -4, -4);
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }
    }
}
