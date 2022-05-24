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
using System.Threading;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Controls.Primitives;
using System.Diagnostics;
using Windows.UI.Core;

namespace To_Do.NavigationPages
{
    public sealed partial class pendingtasks : Page
    {
        public ObservableCollection<TODOTask> TaskItems;
        public List<string> _savingDescriptions = new List<string>();
        public List<string> _savingDates = new List<string>();
        public List<bool> _savingImps = new List<bool>();
        public List<List<string>> savingSteps = new List<List<string>>();

        public List<string> savingDescriptions;

        public static pendingtasks instance;
        public MainPage singletonReference = MainPage.ins;

        public string redate;
        //public int delay = 3000;
        public ContentDialog dialog;


        //for debug for now
        public string _name;
        public string _tag;
        public string lastDataParseTag = "pendingtasks";

        //bool isnavigated = false;

        public pendingtasks()
        {
            this.InitializeComponent();
            instance = this;
            InitializeData();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            listOfTasks.ItemsSource = TaskItems;
            listOfTasks.UpdateLayout();
            UpdateBadge();
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // What we have to do is save all data here to local files
            //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //        async () =>
            //        {
            //            //await Task.Factory.StartNew(SaveDataToFile);
            //            //List<Task> t = new List<Task>();
            //            //t.Add(Task.Run(SaveDataToFile));
            //            //await Task.WhenAll(t.ToArray());
            //            //Task.Run(SaveDataToFile);
            //            await SaveDataToFile();
            //        }
            //        );
            //await Task.Run(SaveDataToFile);
            //var task = SaveDataToFile();
            //task.Wait();
            //Task.Run(async () => { await SaveDataToFile(); }).Wait();

            //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
            //        async () =>
            //        {
                        //await Task.Run(async () => { await SaveDataToFile(); });
                        await SaveDataToFile();
                    //}
                    //);


            TaskItems.Clear();
            //isnavigated = false;
            //await HideInfoBar();
            base.OnNavigatingFrom(e);
        }

        public void AddATask(TODOTask newTask)
        {
            TaskItems.Add(newTask);
            listOfTasks.ItemsSource = TaskItems;

            listOfTasks.UpdateLayout();
            listOfTasks.ScrollIntoView(newTask);
            UpdateBadge();
        }

        public async Task SaveDataToFile()
        {
            var t = _tag;
            if (TaskItems.Count > 0)
            {
                foreach (TODOTask tODO in TaskItems)
                {
                    Debug.WriteLine("while fetching todo for saves tag: " + t);
                    string temp = tODO.Description;
                    string date = tODO.Date;
                    bool importance = tODO.IsStarred;
                    _savingDescriptions.Add(temp);
                    _savingDates.Add(date);
                    _savingImps.Add(importance);

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
                string jsonFile = JsonConvert.SerializeObject(_savingDescriptions);
                string dateJsonFile = JsonConvert.SerializeObject(_savingDates);
                string importanceJsonFile = JsonConvert.SerializeObject(_savingImps);
                string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);

                Debug.WriteLine("just before writing tag: " + t);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFolder rootFolder = await folder.CreateFolderAsync($"{t}", CreationCollisionOption.ReplaceExisting);
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
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{t}");
                if (rootFolder != null)
                {
                    await rootFolder.DeleteAsync();
                }
            }

            _savingDescriptions.Clear();
            _savingDates.Clear();
            _savingImps.Clear();
            savingSteps.Clear();
        }

        private void InitializeData()
        {
            if (TaskItems == null)
            {
                TaskItems = new ObservableCollection<TODOTask>();
                listOfTasks.ItemsSource = TaskItems;
            }
            MainPage.ins.initialLoadingUI.Visibility = Visibility.Collapsed;
        }

        private void listOfTasks_LayoutUpdated(object sender, object e)
        {
            AllDone.Visibility = TaskItems.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task LoadDataFromFile()
        {
            var t = _tag;
            Debug.WriteLine("before fetching folders tag: " + t);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{t}");

            if (rootFolder != null && TaskItems.Count <= 0)
            {
                Debug.WriteLine("just before reading tag: " + t);
                StorageFile descriptionFile = await rootFolder.GetFileAsync($"{t}_desc.json");
                StorageFile datesFile = await rootFolder.GetFileAsync($"{t}_dates.json");
                StorageFile importanceFile = await rootFolder.GetFileAsync($"{t}_imp_desc.json");
                StorageFile stepsFile = await rootFolder.GetFileAsync($"{t}_steps.json");

                string jsonLoaded = await FileIO.ReadTextAsync(descriptionFile);
                string jsonOfDatesLoaded = await FileIO.ReadTextAsync(datesFile);
                string jsonOfImpLoaded = await FileIO.ReadTextAsync(importanceFile);
                string jsonOfStepsLoaded = await FileIO.ReadTextAsync(stepsFile);

                List<string> loadedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonLoaded);
                List<string> loadedDates = JsonConvert.DeserializeObject<List<string>>(jsonOfDatesLoaded);
                List<bool> loadedImportance = JsonConvert.DeserializeObject<List<bool>>(jsonOfImpLoaded);
                List<List<string>> loadedSteps = JsonConvert.DeserializeObject<List<List<string>>>(jsonOfStepsLoaded);
                Debug.WriteLine("just after reading tag: " + t);
                if (loadedDescriptions != null)
                {
                    TaskItems.Clear();
                    for (int i = 0; i < loadedDescriptions.Count; i++)
                    {
                        TODOTask newTask = new TODOTask() { Description = loadedDescriptions[i], Date = loadedDates[i], IsStarred = loadedImportance[i] };
                        newTask.SubTasks = new List<TODOTask>();
                        for (int x = 0; x < loadedSteps[i].Count; x++)
                        {
                            string descOfStep = loadedSteps[i][x];
                            newTask.SubTasks.Add(new TODOTask() { Description = descOfStep });
                        }
                        AddATask(newTask);
                    }
                }
                List<TODOTask> newList = new List<TODOTask>(TaskItems);
                newList.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
                TaskItems = new ObservableCollection<TODOTask>(newList);
                listOfTasks.ItemsSource = TaskItems;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                List<string> parsed = e.Parameter as List<string>;

                if (parsed != null && parsed.Count > 0)
                {
                    _name = parsed[0];
                    _tag = parsed[1];
                    this.Name = _name;
                    this.Tag = _tag;
                    pageTitle.Text = _name;
                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    //async () =>
                    //{
                    Debug.WriteLine("ON NAVIGATING TO");
                    Debug.WriteLine("last tag: " + lastDataParseTag);
                    Debug.WriteLine("current tag: " + _tag);
                    if (lastDataParseTag != _tag)
                    {

                        await LoadDataFromFile();
                        lastDataParseTag = _tag;
                    }
                        //    await Task.Factory.StartNew(LoadDataFromFile);
                        //    //List<Task> t = new List<Task>();
                        //    //t.Add(Task.Run(LoadDataFromFile));
                        //    //await Task.WhenAll(t.ToArray());
                    //}
                    //);
                //await Task.Run(LoadDataFromFile);
                //Task.Run(async () => { await LoadDataFromFile(); }).Wait();
                }
            }
            MainPage.ins.parallax.Source = listOfTasks;
            base.OnNavigatedTo(e);
        }



        private void NewTaskBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string d = NewTaskBox.Text;
                if (!string.IsNullOrEmpty(d) && !string.IsNullOrWhiteSpace(d))
                {
                    TODOTask newTask = new TODOTask() { Description = d, Date = DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt"), IsStarred = false };
                    newTask.SubTasks = new List<TODOTask>();
                    AddATask(newTask);
                    NewTaskBox.Text = string.Empty;
                    e.Handled = true;
                    Sort((string)SortingDropDown.Content);
                }
            }
        }

        private void StarChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            UserControl ct = cb.DataContext as UserControl;
            TODOTask task = ct.DataContext as TODOTask;
            var truth = (bool)cb.IsChecked;
            cb.ClearValue(CheckBox.IsCheckedProperty);
            task.IsStarred = truth;
            var list = TaskItems.ToList();
            if ((string)SortingDropDown.Content != "Custom")
            {
                switch ((string)SortingDropDown.Content)
                {
                    case "Date Created":
                        list.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
                        break;
                    case "Text":
                        list.Sort((x, y) => string.Compare(x.Description, y.Description));
                        break;
                    case "Importance":
                        var query = from x in list
                                    orderby !x.IsStarred
                                    select x;
                        list = query.ToList();
                        break;
                    default:
                        break;
                }

                TaskItems = new ObservableCollection<TODOTask>(list);
                listOfTasks.ItemsSource = TaskItems;
            }
            return;
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //token = new CancellationTokenSource();
            //token.Token.ThrowIfCancellationRequested();
            //if (confirmDoneNotif.IsOpen)
            //{
            //    notif.Translation = new System.Numerics.Vector3(0, 170, 0);
            //    notif.Opacity = 0;
            //    confirmDoneNotif.IsOpen = false;
            //}
            // get checkbox that sent this function
            CheckBox cb = sender as CheckBox;
            if (VisualTreeHelper.GetParent(cb) is Grid cbparent)
            {
                StackPanel panel = VisualTreeHelper.GetChild(cbparent, 1) as StackPanel;
                TextBlock block = VisualTreeHelper.GetChild(panel, 0) as TextBlock;
                block.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                await Task.Delay(100);
                cb.IsChecked = false;
                UserControl top = cb.DataContext as UserControl;
                TODOTask context = top.DataContext as TODOTask;
                block.TextDecorations = Windows.UI.Text.TextDecorations.None;
                redate = DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt");
                completedtasks.instance.AddATask(block.Text, redate);
                TaskItems.Remove(context);
                UpdateBadge();
            }
            //confirmDoneNotif.IsOpen = true;
            //notif.Translation = System.Numerics.Vector3.Zero;
            //notif.Opacity = 1;
            //try
            //{
            //    await Task.Delay(delay, token.Token);
            //}
            //catch
            //{

            //}
            //if (confirmDoneNotif.IsOpen)
            //{
            //    notif.Translation = new System.Numerics.Vector3(0, 170, 0);
            //    notif.Opacity = 0;
            //    await Task.Delay(200);
            //    confirmDoneNotif.IsOpen = false;
            //    token.Cancel();
            //}

            await SaveDataToFile();
        }

        private async void StepCheckToggled(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            //step complete
            Grid g = checkbox.Parent as Grid;
            TextBlock block = VisualTreeHelper.GetChild(g, 3) as TextBlock;
            block.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
            await Task.Delay(100);
            checkbox.IsChecked = false;
            block.TextDecorations = Windows.UI.Text.TextDecorations.None;
            UserControl top = checkbox.DataContext as UserControl;
            TODOTask step = top.DataContext as TODOTask;
            var lviewitempresenter = VisualTreeHelper.GetParent(top) as ListViewItemPresenter;
            ListViewItem lvi = VisualTreeHelper.GetParent(lviewitempresenter) as ListViewItem;
            ItemsStackPanel isp = VisualTreeHelper.GetParent(lvi) as ItemsStackPanel;
            ItemsPresenter ip = VisualTreeHelper.GetParent(isp) as ItemsPresenter;
            ScrollContentPresenter scp = VisualTreeHelper.GetParent(ip) as ScrollContentPresenter;
            Grid grid = VisualTreeHelper.GetParent(scp) as Grid;
            Border b = VisualTreeHelper.GetParent(grid) as Border;
            ScrollViewer s = VisualTreeHelper.GetParent(b) as ScrollViewer;
            Border b2 = VisualTreeHelper.GetParent(s) as Border;
            ListView l = VisualTreeHelper.GetParent(b2) as ListView;
            Grid grid2 = l.Parent as Grid;
            Expander expander = grid2.Parent as Expander;
            UserControl root = VisualTreeHelper.GetParent(expander) as UserControl;
            TODOTask context = root.DataContext as TODOTask;

            int index = 0;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if (TaskItems[i].Equals(context))
                {
                    //store index
                    index = i;
                }
            }

            var list = new List<TODOTask>(TaskItems[index].SubTasks);
            list.Remove(step);
            TaskItems[index].SubTasks = new List<TODOTask>(list);
            Grid grid3 = expander.Content as Grid;
            ListView rootList = VisualTreeHelper.GetChild(grid3, 0) as ListView;
            rootList.ItemsSource = TaskItems[index].SubTasks;
        }

        //private async void UndoDelete(object sender, RoutedEventArgs e)
        //{
        //    completedtasks.instance.DeleteTaskFromExternal(redate);
        //    TODOTask reMadeTask = new TODOTask() { Description = undoText, Date = undoDate, IsStarred = undoStar };
        //    reMadeTask.SubTasks = new List<TODOTask>(undoSteps);
        //    TaskItems.Insert(undoIndex, reMadeTask);
        //    UpdateBadge();
        //    listOfTasks.ItemsSource = TaskItems;
        //    listOfTasks.UpdateLayout();
        //    token.Cancel();
        //    notif.Translation = new System.Numerics.Vector3(0, 170, 0);
        //    notif.Opacity = 0;
        //    await Task.Delay(200);
        //    confirmDoneNotif.IsOpen = false;
        //}

        //public async Task HideInfoBar()
        //{
        //    notif.Translation = new System.Numerics.Vector3(0, 170, 0);
        //    notif.Opacity = 0;
        //    await Task.Delay(200);
        //    confirmDoneNotif.IsOpen = false;
        //}

        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            // Delete a task
            moreOptionsSplitView.IsPaneOpen = false;

            TaskItems.Remove(selectedTask);
            selectedTask = null;
            UpdateBadge();
        }

        private void DeleteSubTask(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            UserControl top = item.DataContext as UserControl;
            TODOTask step = top.DataContext as TODOTask;
            var lviewitempresenter = VisualTreeHelper.GetParent(top) as ListViewItemPresenter;
            ListViewItem lvi = VisualTreeHelper.GetParent(lviewitempresenter) as ListViewItem;
            ItemsStackPanel isp = VisualTreeHelper.GetParent(lvi) as ItemsStackPanel;
            ItemsPresenter ip = VisualTreeHelper.GetParent(isp) as ItemsPresenter;
            ScrollContentPresenter scp = VisualTreeHelper.GetParent(ip) as ScrollContentPresenter;
            Grid grid = VisualTreeHelper.GetParent(scp) as Grid;
            Border b = VisualTreeHelper.GetParent(grid) as Border;
            ScrollViewer s = VisualTreeHelper.GetParent(b) as ScrollViewer;
            Border b2 = VisualTreeHelper.GetParent(s) as Border;
            ListView l = VisualTreeHelper.GetParent(b2) as ListView;
            Grid grid2 = l.Parent as Grid;
            Expander expander = grid2.Parent as Expander;
            UserControl root = VisualTreeHelper.GetParent(expander) as UserControl;
            TODOTask context = root.DataContext as TODOTask;
            int index = 0;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if (TaskItems[i].Equals(context))
                {
                    //store index
                    index = i;
                }
            }

            var list = new List<TODOTask>(TaskItems[index].SubTasks);
            list.Remove(step);
            TaskItems[index].SubTasks = new List<TODOTask>(list);
            Grid g = expander.Content as Grid;
            ListView rootList = VisualTreeHelper.GetChild(g, 0) as ListView;
            rootList.ItemsSource = TaskItems[index].SubTasks;
        }

        private async void AddStep(object sender, RoutedEventArgs e)
        {
            dialog = new EditDialogContent();
            Grid.SetRowSpan(dialog, 2);
            dialog.CloseButtonStyle = (Style)Application.Current.Resources["ButtonStyle1"];
            dialog.Title = "Add Step";
            int index = 0;
            Button item = sender as Button;
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

            Grid grid = (Grid)dialog.Content;
            TextBox EditTextBox = (TextBox)VisualTreeHelper.GetChild(grid, 0);
            dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
            TextChanged(EditTextBox);
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //do create new task
                TODOTask newStep = new TODOTask() { Description = EditTextBox.Text };
                var list = new List<TODOTask>(TaskItems[index].SubTasks)
                {
                    newStep
                };
                TaskItems[index].SubTasks = new List<TODOTask>(list);
                Expander expander = VisualTreeHelper.GetChild(top, 0) as Expander;
                Grid g = expander.Content as Grid;
                ListView rootList = VisualTreeHelper.GetChild(g, 0) as ListView;
                rootList.ItemsSource = TaskItems[index].SubTasks;
                EditTextBox.Text = string.Empty;
            }
        }

        public void TextChanged(TextBox b)
        {
            dialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(b.Text) && !string.IsNullOrWhiteSpace(b.Text);
        }

        public void UpdateBadge()
        {
            for (int i = 0; i < MainPage.ins.Categories.Count; i++)
            {
                if (MainPage.ins.Categories[i].Tag == (string)this.Tag)
                {
                    MainPage.ins.Categories[i].badgeNum = TaskItems.Count;
                    if (TaskItems.Count > 0)
                    {
                        MainPage.ins.Categories[i].opacity = 1f;
                    }
                    else
                    {
                        MainPage.ins.Categories[i].opacity = 0f;
                    }
                }
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
            panel.Translation = new System.Numerics.Vector3(0, 20, 0);
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

        private void CheckBox_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var c = cb.DataContext as Control;
            var panel = FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
            var block = FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
            panel.Translation = new System.Numerics.Vector3(0, 20, 0);
            panel.Opacity = 0;
            block.Translation = new System.Numerics.Vector3(0, 12, 0);
        }

        private void SortingOptionClicked(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            SortingDropDown.Content = item.Text;
            Sort(item.Text);
        }

        void Sort(string typeOfSort)
        {
            if (typeOfSort != "Custom")
            {
                var list = new List<TODOTask>(TaskItems);
                switch (typeOfSort)
                {
                    case "Date Created":
                        list.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
                        break;
                    case "Text":
                        list.Sort((x, y) => string.Compare(x.Description, y.Description));
                        break;
                    case "Importance":
                        var query = from task in list
                                    orderby !task.IsStarred
                                    select task;
                        list = query.ToList();
                        break;
                    default:
                        break;
                }

                TaskItems = new ObservableCollection<TODOTask>(list);
                listOfTasks.ItemsSource = TaskItems;
            }
        }

        private void listOfTasks_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (args != null)
            {
                SortingDropDown.Content = "Custom";
                opt1.IsChecked = false;
                opt2.IsChecked = false;
                opt3.IsChecked = false;
            }
        }

        private void SubTaskPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var c = cb.DataContext as Control;
            var strip = FindControl<Grid>(c, typeof(Grid), "rect");
            var delbtn = FindControl<Button>(c, typeof(Button), "delsubtask");
            delbtn.Translation = new System.Numerics.Vector3(50, 0, 0);
            delbtn.Opacity = 0;
            strip.Opacity = 0;
            var back = FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        private void SubTaskPointerExited(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var strip = FindControl<Grid>(c, typeof(Grid), "rect");
            strip.Opacity = 0;
            var delbtn = FindControl<Button>(c, typeof(Button), "delsubtask");
            delbtn.Translation = new System.Numerics.Vector3(50, 0, 0);
            delbtn.Opacity = 0;

            var back = FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        private void SubTaskPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                var c = sender as Control;
                var strip = FindControl<Grid>(c, typeof(Grid), "rect");
                var delbtn = FindControl<Button>(c, typeof(Button), "delsubtask");
                delbtn.Translation = System.Numerics.Vector3.Zero;
                delbtn.Opacity = 1;
                strip.Opacity = 1;


                var back = FindControl<Grid>(c, typeof(Grid), "backplate");
                back.Opacity = 0.2f;
            }
        }

        private void BTN_SubTaskPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Button cb = sender as Button;
            var c = cb.DataContext as Control;
            var strip = FindControl<Grid>(c, typeof(Grid), "rect");
            cb.Translation = new System.Numerics.Vector3(50, 0, 0);
            cb.Opacity = 0;
            strip.Opacity = 0;
            var back = FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        TODOTask selectedTask = null;

        private void OpenSplitView(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            UserControl top = item.DataContext as UserControl;
            selectedTask = top.DataContext as TODOTask;
            edittasktextbox.Text = selectedTask.Description;
            edittasktextbox.SelectionStart = edittasktextbox.Text.Length;
            moreOptionsSplitView.IsPaneOpen = true;
        }

        private void CloseSplitView(object sender, RoutedEventArgs e)
        {
            moreOptionsSplitView.IsPaneOpen = false;
        }

        private void EditBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            confirmchangesBTN.IsEnabled = !string.IsNullOrEmpty(edittasktextbox.Text) && !string.IsNullOrWhiteSpace(edittasktextbox.Text);
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            int index = 0;
            for (int i = 0; i < TaskItems.Count; i++)
            {
                if (TaskItems[i].Equals(selectedTask))
                {
                    //store index
                    index = i;
                }
            }
            TaskItems[index].Description = edittasktextbox.Text;
            moreOptionsSplitView.IsPaneOpen = false;
            edittasktextbox.Text = string.Empty;
            Sort((string)SortingDropDown.Content);
        }

    }

    public class TODOTask : INotifyPropertyChanged
    {
        private string description { get; set; }
        private string date { get; set; }
        private bool isStarred = false;
        private List<TODOTask> subTasks;

        public string Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public List<TODOTask> SubTasks
        {
            get => subTasks;
            set
            {
                subTasks = value;
                OnPropertyChanged("SubTasks");
            }
        }

        public bool IsStarred
        {
            get => isStarred;
            set
            {
                isStarred = value;
                OnPropertyChanged("IsStarred");
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
