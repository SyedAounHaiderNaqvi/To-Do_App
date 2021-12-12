using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace To_Do.NavigationPages
{
    public sealed partial class CompletedTasks : Page
    {
        public ObservableCollection<TODOTask> CompleteTasks;
        public static CompletedTasks instance;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public MainPage singletonReference = MainPage.ins;

        public CompletedTasks()
        {
            this.InitializeComponent();
            instance = this;
            InitializeData();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            listOfTasks.UpdateLayout();
        }

        private void InitializeData()
        {
            if (CompleteTasks == null)
            {
                CompleteTasks = new ObservableCollection<TODOTask>();
            }
            if (localSettings.Values["TasksDone"] != null)
            {
                string jsonLoaded = localSettings.Values["TasksDone"] as string;
                string jsonOfDateLoaded = localSettings.Values["DateOfTasksDone"] as string;
                List<string> loadedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonLoaded);
                List<string> loadedDates = JsonConvert.DeserializeObject<List<string>>(jsonOfDateLoaded);
                if (loadedDescriptions != null)
                {
                    for (int i = 0; i < loadedDescriptions.Count; i++)
                    {
                        AddATask(loadedDescriptions[i], loadedDates[i]);
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PendingTasks.instance.HideInfoBar();

            var parsedDescriptions = (List<List<string>>)e.Parameter;
            if (parsedDescriptions != null)
            {
                foreach (List<string> item in parsedDescriptions)
                {
                    AddATask(item[0], item[1]);
                }
            }

            ClearListBtn.Visibility = CompleteTasks.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void AddATask(string taskDescription, string date)
        {
            TODOTask newTask = new TODOTask() { Description = taskDescription, Date = date };
            CompleteTasks.Add(newTask);

            listOfTasks.UpdateLayout();
            listOfTasks.ScrollIntoView(newTask);
        }

        private void listOfTasks_LayoutUpdated(object sender, object e)
        {
            AllDone.Visibility = CompleteTasks.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearCompletedTaskList(object sender, RoutedEventArgs e)
        {
            CompleteTasks.Clear();
            ClearListBtn.Visibility = Visibility.Collapsed;
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                var c = sender as Control;
                var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
                var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
                panel.Translation = System.Numerics.Vector3.Zero;
                panel.Opacity = 1;
                block.Translation = System.Numerics.Vector3.Zero;
            }
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
            var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
            panel.Translation = new System.Numerics.Vector3(0, 60, 0);
            panel.Opacity = 0;
            block.Translation = new System.Numerics.Vector3(0, 12, 0);
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
    }
}
