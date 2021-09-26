using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;

namespace To_Do.NavigationPages
{
    public sealed partial class PendingTasks : Page
    {
        public ObservableCollection<TODOTask> TaskItems;

        public List<string> savingDescriptions;

        public static PendingTasks instance;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public MainPage singletonReference = MainPage.ins;
        public int undoIndex;
        public string undoText;
        public string undoDate;
        public int delay = 3000;
        CancellationTokenSource token;
        public ContentDialog dialog;

        public PendingTasks()
        {
            this.InitializeComponent();
            instance = this;
            InitializeData();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            listOfTasks.ItemsSource = TaskItems;
            UpdateBadge();
        }

        public void AddATask(string taskDescription, string date)
        {
            TaskItems.Add(new TODOTask() { Description = taskDescription, Date = date });
            scroll.UpdateLayout();
            scroll.ChangeView(0, scroll.ScrollableHeight, 1);

            listOfTasks.ItemsSource = TaskItems;
            UpdateBadge();
        }

        private void InitializeData()
        {
            if (TaskItems == null)
            {
                TaskItems = new ObservableCollection<TODOTask>();
                listOfTasks.ItemsSource = TaskItems;
            }
            if (localSettings.Values["Tasks"] != null)
            {
                string jsonLoaded = localSettings.Values["Tasks"] as string;
                string jsonOfDatesLoaded = localSettings.Values["DateOfTasks"] as string;
                List<string> loadedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonLoaded);
                List<string> loadedDates = JsonConvert.DeserializeObject<List<string>>(jsonOfDatesLoaded);
                if (loadedDescriptions != null)
                {
                    for (int i = 0; i < loadedDescriptions.Count; i++)
                    {
                        AddATask(loadedDescriptions[i], loadedDates[i]);
                    }
                }
            }
        }

        private void listOfTasks_LayoutUpdated(object sender, object e)
        {
            AllDone.Visibility = TaskItems.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NewTaskBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string d = NewTaskBox.Text;
                if (!string.IsNullOrEmpty(d) && !string.IsNullOrWhiteSpace(d))
                {
                    AddATask(d, DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt"));
                    NewTaskBox.Text = string.Empty;
                    e.Handled = true;
                }
            }
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            token = new CancellationTokenSource();
            token.Token.ThrowIfCancellationRequested();
            if (confirmDoneNotif.IsOpen)
            {
                notif.Translation = new System.Numerics.Vector3(0, 170, 0);
                notif.Opacity = 0;
                confirmDoneNotif.IsOpen = false;
            }
            // get checkbox that sent this function
            CheckBox cb = sender as CheckBox;
            Grid cbparent = VisualTreeHelper.GetParent(cb) as Grid;
            StackPanel panel = VisualTreeHelper.GetChild(cbparent, 1) as StackPanel;
            TextBlock block = VisualTreeHelper.GetChild(panel, 0) as TextBlock;
            undoText = block.Text;
            singletonReference.tasksToParse.Add(new List<string>() { block.Text, DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt") });
            block.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
            await Task.Delay(100);
            cb.IsChecked = false;
            UserControl top = cb.DataContext as UserControl;
            TODOTask context = top.DataContext as TODOTask;
            undoDate = context.Date;
            block.TextDecorations = Windows.UI.Text.TextDecorations.None;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if (TaskItems[i] == context)
                {
                    undoIndex = i;
                }
            }
            TaskItems.Remove(context);
            UpdateBadge();
            confirmDoneNotif.IsOpen = true;
            notif.Translation = System.Numerics.Vector3.Zero;
            notif.Opacity = 1;
            try
            {
                await Task.Delay(delay, token.Token);
            }
            catch
            {
                Debug.WriteLine("Cancelledddd");
            }
            if (confirmDoneNotif.IsOpen)
            {
                notif.Translation = new System.Numerics.Vector3(0, 170, 0);
                notif.Opacity = 0;
                await Task.Delay(200);
                confirmDoneNotif.IsOpen = false;
                token.Cancel();
            }
        }

        private async void UndoDelete(object sender, RoutedEventArgs e)
        {
            singletonReference.tasksToParse.RemoveAt(singletonReference.tasksToParse.Count - 1);
            TaskItems.Insert(undoIndex, new TODOTask() { Description = undoText, Date = undoDate });
            scroll.UpdateLayout();
            scroll.ChangeView(0, scroll.ScrollableHeight, 1);
            UpdateBadge();
            listOfTasks.ItemsSource = TaskItems;
            token.Cancel();
            notif.Translation = new System.Numerics.Vector3(0, 170, 0);
            notif.Opacity = 0;
            await Task.Delay(200);
            confirmDoneNotif.IsOpen = false;
        }

        public async void HideInfoBar()
        {
            notif.Translation = new System.Numerics.Vector3(0, 170, 0);
            notif.Opacity = 0;
            await Task.Delay(200);
            confirmDoneNotif.IsOpen = false;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Delete a task
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            UserControl top = item.DataContext as UserControl;
            TODOTask context = top.DataContext as TODOTask;
            TaskItems.Remove(context);
            UpdateBadge();
        }

        private async void LaunchEditBox(object sender, RoutedEventArgs e)
        {
            dialog = new EditDialogContent();
            Grid.SetRowSpan(dialog, 2);
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["ButtonStyle1"];
            int index = 0;
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            UserControl top = item.DataContext as UserControl;
            TODOTask context = top.DataContext as TODOTask;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if (TaskItems[i].Equals(context))
                {
                    //store index
                    index = i;
                }
            }
            //launch contentdialog
            Grid grid = (Grid)dialog.Content;
            TextBox EditTextBox = (TextBox)VisualTreeHelper.GetChild(grid, 0);
            //EditTextBox.TextChanged += EditBoxTextChanged;
            EditTextBox.Text = TaskItems[index].Description;
            EditTextBox.SelectionStart = EditTextBox.Text.Length;
            dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
            EditBoxTextChanged(EditTextBox);
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //do change text
                TaskItems[index].Description = EditTextBox.Text;
                EditTextBox.Text = string.Empty;
            }
        }

        private void EditBoxTextChanged(TextBox EditTextBox)
        {
            dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
        }

        public void UpdateBadge()
        {
            singletonReference.inf.Value = TaskItems.Count;
            if (TaskItems.Count > 0)
            {
                singletonReference.inf.Visibility = Visibility.Visible;
            }
            else
            {
                singletonReference.inf.Visibility = Visibility.Collapsed;
            }
        }

        private async void NewTaskBox_GotFocus(object sender, RoutedEventArgs e)
        {
            boxIcon.Opacity = 0;
            await Task.Delay(150);
            boxIcon.Glyph = "\uEA3A";
            boxIcon.Opacity = 1;
        }

        private async void NewTaskBox_LostFocus(object sender, RoutedEventArgs e)
        {
            boxIcon.Opacity = 0;
            await Task.Delay(150);
            boxIcon.Glyph = "\uE710";
            boxIcon.Opacity = 1;
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                //VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
                var c = sender as Control;
                //StackPanel panel = VisualTreeHelper.GetChild(c, 0) as StackPanel;
                var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
                var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
                var btn = FindControl<Button>(c, typeof(Button), "DeleteTaskButton");
                panel.Translation = System.Numerics.Vector3.Zero;
                panel.Opacity = 1;
                btn.Opacity = 1;
                btn.Translation = System.Numerics.Vector3.Zero;
                block.Translation = System.Numerics.Vector3.Zero;
            }
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
            var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
            var btn = FindControl<Button>(c, typeof(Button), "DeleteTaskButton");
            panel.Translation = new System.Numerics.Vector3(0, 60, 0);
            panel.Opacity = 0;
            btn.Opacity = 0;
            btn.Translation = new System.Numerics.Vector3(60, 0, 0);
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

        private void CheckBox_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var c = cb.DataContext as Control;
            var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
            var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
            var btn = FindControl<Button>(c, typeof(Button), "DeleteTaskButton");
            panel.Translation = new System.Numerics.Vector3(0, 60, 0);
            panel.Opacity = 0;
            btn.Opacity = 0;
            btn.Translation = new System.Numerics.Vector3(60, 0, 0);
            block.Translation = new System.Numerics.Vector3(0, 12, 0);
        }
    }

    public class TODOTask : INotifyPropertyChanged
    {
        private string description { get; set; }
        private string date { get; set; }

        public string Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
