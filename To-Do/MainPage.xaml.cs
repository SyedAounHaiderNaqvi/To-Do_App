using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.System.Profile;
using System.Linq;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.Storage.AccessCache;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace To_Do
{
    public sealed partial class MainPage : Page
    {
        public List<string> savingDescriptions = new List<string>();
        public List<string> savingDates = new List<string>();
        public List<bool> savingImps = new List<bool>();
        public List<List<string>> savingSteps = new List<List<string>>();

        public List<string> navListsNames = new List<string>();
        public List<string> navListsTags = new List<string>();
        public List<string> navListsGlyphs = new List<string>();

        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static MainPage ins;
        public int indexToParse;
        public List<List<string>> tasksToParse = new List<List<string>>();
        public int PendingTasksCount = 0;
        bool canSearch = true;
        public NavigationTransitionInfo info;

        public ObservableCollection<CustomNavViewItem> Categories { get; }
        public ContentDialog dialog;
        StorageFolder folder;

        public MainPage()
        {
            this.InitializeComponent();
            ins = this;
            folder = ApplicationData.Current.LocalFolder;
            Categories = new ObservableCollection<CustomNavViewItem>();
            Task.Run(() => LoadNavViewLists()).Wait();
            //Task.Run(() => DeletionOfUnnecessaryLists()).Wait();
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
            ImageInitialize();
            RoundCornerInitialize();
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

            Waiter();
            parallax.Source = pendingtasks.instance.listOfTasks;
            LoseFocus(searchbox);
        }

        public async void Waiter()
        {
            List<string> dataToParse = new List<string>
            {
                "Pending Tasks",
                "pendingtasks"
            };
            ContentFrame.Navigate(typeof(pendingtasks), dataToParse, new SuppressNavigationTransitionInfo());
            await Task.Delay(10);
            ContentFrame.Navigate(typeof(Settings), null, new SuppressNavigationTransitionInfo());
            await Task.Delay(10);
            ContentFrame.Navigate(typeof(pendingtasks), dataToParse, new SuppressNavigationTransitionInfo());
            LoadingUI.Visibility = Visibility.Collapsed;
            nview.SelectedItem = Categories[0];
        }

        async Task LoadNavViewLists()
        {
            StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"navlists");

            if (rootFolder != null)
            {
                StorageFile nameFile = await rootFolder.GetFileAsync($"list_names.json");
                StorageFile glyphFile = await rootFolder.GetFileAsync($"list_glyphs.json");
                StorageFile tagFile = await rootFolder.GetFileAsync($"list_tags.json");

                string nameLoaded = await FileIO.ReadTextAsync(nameFile);
                string glyphLoaded = await FileIO.ReadTextAsync(glyphFile);
                string tagLoaded = await FileIO.ReadTextAsync(tagFile);

                List<string> loadedNames = JsonConvert.DeserializeObject<List<string>>(nameLoaded);
                List<string> loadedGlyphs = JsonConvert.DeserializeObject<List<string>>(glyphLoaded);
                List<string> loadedTags = JsonConvert.DeserializeObject<List<string>>(tagLoaded);
                if (loadedNames != null)
                {
                    Categories.Clear();
                    for (int i = 0; i < loadedNames.Count; i++)
                    {
                        CustomNavViewItem cat = new CustomNavViewItem { Name = loadedNames[i], Glyph = loadedGlyphs[i], Tag = loadedTags[i] };
                        Categories.Add(cat);
                    }
                }
            }
            else
            {
                Categories.Add(new CustomNavViewItem { Name = "Pending Tasks", Glyph = "\uE823", Tag = "pendingtasks" });
            }
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

                titleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"]).Color;//new Color() { A = 255, R = a2R, G = a2G, B = a2B };
            }
            else
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>("Light");
                Application.Current.Resources["SystemAccentColorDark1"] = fallBackPurple;
                Application.Current.Resources["SystemAccentColorDark2"] = Colors.White;
                Application.Current.Resources["SystemAccentColorLight2"] = Application.Current.Resources["SystemAccentColorDark1"];
                Application.Current.Resources["SystemAccentColor"] = (Color)Application.Current.Resources["SystemAccentColorDark1"];

                Application.Current.Resources["NavigationViewItemForegroundSelected"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPointerOver"] = Application.Current.Resources["SystemAccentColor"];
                Application.Current.Resources["NavigationViewItemForegroundSelectedPressed"] = Application.Current.Resources["SystemAccentColor"];

                Color bgColor = fallBackPurple;
                Application.Current.Resources["NavigationViewContentBackground"] = new SolidColorBrush(fallBackPurple);
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string argument = e.Parameter.ToString();

            switch (argument)
            {
                case "GoToPending":
                    ContentFrame.Navigate(typeof(pendingtasks));
                    nview.SelectedItem = Categories[0];
                    parallax.Source = pendingtasks.instance.listOfTasks;
                    break;
                case "GoToSettings":
                    ContentFrame.Navigate(typeof(pendingtasks));
                    await Task.Delay(10);
                    ContentFrame.Navigate(typeof(Settings));
                    nview.SelectedItem = nview.SettingsItem;
                    parallax.Source = Settings.ins.scroller;
                    break;
                default:
                    break;
            }
        }

        async Task SaveCurrentPageData()
        {
            savingDescriptions.Clear();
            savingDates.Clear();
            savingImps.Clear();
            savingSteps.Clear();

            StorageFolder rootFolder;
            pendingtasks ins = pendingtasks.instance;
            string t = ins._tag;
            if (ins.TaskItems.Count > 0)
            {
                foreach (TODOTask tODO in ins.TaskItems)
                {
                    Debug.WriteLine("while fetching todo for saves tag: " + t);
                    string temp = tODO.Description;
                    string date = tODO.Date;
                    bool importance = tODO.IsStarred;
                    savingDescriptions.Add(temp);
                    savingDates.Add(date);
                    savingImps.Add(importance);

                    List<TODOTask> steps = tODO.SubTasks;
                    List<string> tempList = new List<string>();
                    for (int i = 0; i < steps.Count; i++)
                    {
                        tempList.Add(steps[i].Description);
                    }
                    if (steps != null)
                    {
                        savingSteps.Add(tempList);
                    }
                }
                string jsonFile = JsonConvert.SerializeObject(savingDescriptions);
                string dateJsonFile = JsonConvert.SerializeObject(savingDates);
                string importanceJsonFile = JsonConvert.SerializeObject(savingImps);
                string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);


                rootFolder = await folder.CreateFolderAsync($"{t}", CreationCollisionOption.ReplaceExisting);
                StorageFile pendingdescjson = await rootFolder.CreateFileAsync($"{t}_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingdescjson, jsonFile);
                StorageFile pendingdatesjson = await rootFolder.CreateFileAsync($"{t}_dates.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingdatesjson, dateJsonFile);
                StorageFile impdescjson = await rootFolder.CreateFileAsync($"{t}_imp_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(impdescjson, importanceJsonFile);
                StorageFile pendingstepsjson = await rootFolder.CreateFileAsync($"{t}_steps.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingstepsjson, stepsJsonFile);
                Debug.WriteLine("after writing tag: " + t);
            }
            else
            {
                rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{t}");
                if (rootFolder != null)
                {
                    await rootFolder.DeleteAsync();
                }
            }
        }

        async Task SaveNavigationPageItems()
        {
            navListsGlyphs.Clear();
            navListsNames.Clear();
            navListsTags.Clear();
            StorageFolder rootFolder = await folder.CreateFolderAsync($"navlists", CreationCollisionOption.ReplaceExisting);

            for (int i = 0; i < Categories.Count; i++)
            {
                navListsTags.Add(Categories[i].Tag);
                navListsNames.Add(Categories[i].Name);
                navListsGlyphs.Add(Categories[i].Glyph);
            }
            string tag_File = JsonConvert.SerializeObject(navListsTags);
            string name_File = JsonConvert.SerializeObject(navListsNames);
            string glyph_File = JsonConvert.SerializeObject(navListsGlyphs);

            StorageFile name_json = await rootFolder.CreateFileAsync($"list_names.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(name_json, name_File);
            StorageFile tag_json = await rootFolder.CreateFileAsync($"list_tags.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(tag_json, tag_File);
            StorageFile glyph_json = await rootFolder.CreateFileAsync($"list_glyphs.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(glyph_json, glyph_File);

        }

        public void CreateThreeTileNotifications()
        {
            var TaskItems = pendingtasks.instance.TaskItems;
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
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {

                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
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
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
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
                                    Text = "1) " + tasks[0],
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 1 ? "2) " + tasks[1] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 2 ? "3) " + tasks[2] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 3 ? "4) " + tasks[3] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 4 ? "5) " + tasks[4] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 5 ? "6) " + tasks[5] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 6 ? "7) " + tasks[6] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 7 ? "8) " + tasks[7] : "",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText()
                                {
                                    Text = tasks.Count > 8 ? "9) " + tasks[8] : "",
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

        public async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            var def = e.GetDeferral();
            LoadingUI.Visibility = Visibility.Visible;
            await SaveCurrentPageData();
            await SaveNavigationPageItems();
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong build = (version & 0x00000000FFFF0000L) >> 16;

            if (Convert.ToInt16(build) < 22000)
            {
                CreateThreeTileNotifications();
            }

            def.Complete();
        }

        async Task DeletionOfUnnecessaryLists()
        {
            var allFolders = await folder.GetFoldersAsync();

            for (int i = 0; i < allFolders.Count; i++)
            {
                if (allFolders[i].Name != "navlists")
                {
                    //check if each subholder has a match in Categories, if not delete that folder.
                    bool matchFound = false;
                    for (int j = 0; j < Categories.Count; j++)
                    {
                        if (allFolders[i].Name.Equals(Categories[j].Tag))
                        {
                            matchFound = true;
                            Debug.WriteLine("match found for " + Categories[j].Tag);
                        }
                    }
                    if (!matchFound)
                    {
                        if (allFolders[i] != null)
                        {
                            Debug.WriteLine(allFolders[i].Name);
                            await allFolders[i].DeleteAsync();
                        }
                    }
                }

            }

        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Height = coreTitleBar.Height;

            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private async void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            LoadingUI.Visibility = Visibility.Visible;
            //int styleIndex = (int)localSettings.Values["navStyle"];
            //switch (styleIndex)
            //{
            //    case 0:
            //        info = new EntranceNavigationTransitionInfo();
            //        break;
            //    case 1:
            //        info = new DrillInNavigationTransitionInfo();
            //        break;
            //    case 2:
            //        info = new SuppressNavigationTransitionInfo();
            //        break;
            //    case 3:
            //        info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
            //        break;
            //    case 4:
            //        info = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            //        break;
            //    default:
            //        info = new EntranceNavigationTransitionInfo();
            //        break;
            //}
            info = new SuppressNavigationTransitionInfo();
            Type pageType;

            if (args.IsSettingsSelected)
            {
                await SaveCurrentPageData();
                pageType = Type.GetType("To_Do.Settings");
                ContentFrame.Navigate(pageType, null, info);
                searchBoxGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                searchBoxGrid.Visibility = Visibility.Visible;
                var selectedItem = (CustomNavViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    pageType = Type.GetType("To_Do.pendingtasks");
                    List<string> dataToParse = new List<string>
                    {
                        selectedItem.Name,
                        selectedItem.Tag
                    };
                    ContentFrame.Navigate(pageType, dataToParse, info);
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
            string currentPageTag = pendingtasks.instance._tag;
            string currentPageName = pendingtasks.instance._name;

            var matchingItems = pendingtasks.instance.TaskItems.ToList().Where(
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
                string glyph = item.IsCompleted ? "\uE73E" : "\uF08F";
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
                if (ContentFrame.CurrentSourcePageType == typeof(pendingtasks))
                {
                    var list = pendingtasks.instance.TaskItems;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (desc.Title.Equals(list[j].Description))
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => calc(j, pendingtasks.instance.listOfTasks));
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            dialog = new NewNavigationViewItemDialog();
            Grid.SetRowSpan(dialog, 2);
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["ButtonStyle1"];
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // create list here
                string name = (string)localSettings.Values["NEWlistName"];
                string tag = ((string)localSettings.Values["NEWlistTag"]);
                string glyph = (string)localSettings.Values["NEWlistIcon"];
                Categories.Add(new CustomNavViewItem { Tag = tag, Name = name, Glyph = glyph });
            }
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
            var parentControl = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender;
            Button btn = UtilityFunctions.FindControl<Button>(parentControl, typeof(Button), "deleteListButton");
            if ((string)parentControl.Tag == "pendingtasks")
            {
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
            DEBUG_DeletePrompt.Visibility = Visibility.Visible;
            //hasNavigated = false;
            //nview.IsEnabled = false;
            //nview.IsEnabled = true;
            //get the current index
            //load the previous list in array Categories, then delete this item after deleting the folder.
            Microsoft.UI.Xaml.Controls.NavigationViewItem parent = (Microsoft.UI.Xaml.Controls.NavigationViewItem)(sender as Button).DataContext;
            for (int i = 0; i < Categories.Count; i++)
            {
                if ((string)parent.Tag == Categories[i].Tag)
                {
                    //found the match, load previous one
                    //hasNavigated = true;
                    var selectedItem = Categories[i - 1];
                    if (selectedItem != null)
                    {
                        string selectedItemTag = selectedItem.Tag;
                        string pageName;
                        if (selectedItemTag.Equals("completedtasks"))
                        {
                            pageName = "To_Do." + selectedItemTag;
                        }
                        else
                        {
                            pageName = "To_Do.pendingtasks";
                        }
                        Type pageType = Type.GetType(pageName);
                        List<string> dataToParse = new List<string>
                        {
                            selectedItem.Name,
                            selectedItem.Tag
                        };
                        switch (selectedItemTag)
                        {
                            case "completedtasks":
                                pendingtasks.instance.lastDataParseTag = "completedtasks";
                                ContentFrame.Navigate(pageType, tasksToParse, info);
                                tasksToParse.Clear();
                                break;
                            default:
                                ContentFrame.Navigate(pageType, dataToParse, info);
                                break;
                        }
                        await Task.Delay(100);
                        //now delete the previous one's files                        
                        //StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{Categories[i].Tag}");
                        //while (rootFolder != null)
                        //{
                        //    await rootFolder.DeleteAsync();
                        //    rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{Categories[i].Tag}");
                        //}
                        Categories.RemoveAt(i);
                        nview.SelectedItem = selectedItem;
                        break;
                    }
                }
            }
            DEBUG_DeletePrompt.Visibility = Visibility.Collapsed;
            //CustomNavViewItem rootCat = parent.DataContext as CustomNavViewItem;
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

    public class CustomNavViewItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Glyph { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
