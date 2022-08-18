using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using To_Do.NavigationPages;

namespace To_Do
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        private async Task ConfigureJumpList()
        {
            JumpList jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();

            JumpListItem jl1 = JumpListItem.CreateWithArguments("GoToPending", "Pending Tasks");
            jl1.Logo = new Uri("ms-appx:///Images/clock.png");
            jl1.GroupName = "Quick Actions";
            jumpList.Items.Add(jl1);

            JumpListItem jl3 = JumpListItem.CreateWithArguments("GoToSettings", "Settings");
            jl3.Logo = new Uri("ms-appx:///Images/settingsIcon.png");
            jl3.GroupName = "Quick Actions";
            jumpList.Items.Add(jl3);

            await jumpList.SaveAsync();
        }

        async void JumplistTransferCode(LaunchActivatedEventArgs e)
        {
            MainPage.ins.ContentFrame.Navigate(typeof(pendingtasks));
            MainPage.ins.ContentFrame.Navigate(typeof(Settings));

            switch (e.Arguments)
            {
                case "GoToPending":
                    MainPage.ins.ContentFrame.Navigate(typeof(pendingtasks));
                    await Task.Delay(10);
                    MainPage.ins.nview.SelectedItem = MainPage.ins.Categories[0];
                    break;
                case "GoToSettings":
                    while (MainPage.ins.ContentFrame.CurrentSourcePageType != typeof(Settings))
                    {
                        MainPage.ins.ContentFrame.Navigate(typeof(Settings));
                    }
                    await Task.Delay(10);
                    MainPage.ins.nview.SelectedItem = MainPage.ins.nview.SettingsItem;
                    break;
                default:
                    break;
            }
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await ConfigureJumpList();
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    JumplistTransferCode(e);
                }
                else
                {
                    JumplistTransferCode(e);
                }
                // Ensure the current window is active
                ThemeHelper.Initialize();
                Window.Current.Activate();
            }
            ThemeHelper.Initialize();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
