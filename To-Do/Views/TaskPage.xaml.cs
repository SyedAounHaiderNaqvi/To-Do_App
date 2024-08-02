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
using To_Do.Models;
using System.Windows.Input;
using To_Do.ViewModels;
using System.Diagnostics;
using Windows.UI.Xaml.Data;
using Windows.UI.Composition;
using Windows.UI;

namespace To_Do.Views
{
    public sealed partial class TaskPage : Page
    {
        public List<string> _savingDescriptions = new List<string>();
        public List<string> _savingDates = new List<string>();
        public List<bool> _savingImps = new List<bool>();
        public List<bool> _savingCompletedState = new List<bool>();
        public List<List<string>> savingSteps = new List<List<string>>();

        public List<string> savingDescriptions;

        public static TaskPage instance;

        public TaskViewModel viewModel;
        TaskModel selectedTask = null;

        //public string redate;
        public EditDialogContent dialog;

        //for debug for now
        public string _name = "Pending Tasks";
        public string _tag = "TaskPage";
        public string lastDataParseTag = "TaskPage";
        //bool loadedForFirstTime = true;
        public bool finallyLoaded = false;

        public TaskPage()
        {
            this.InitializeComponent();
            instance = this;
            viewModel = (TaskViewModel)this.DataContext;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            //UpdateBadge();
        }

        //public async Task SaveDataToFile()
        //{
        //    var t = _tag;
        //    if (_tasks.Count > 0)
        //    {
        //        foreach (TaskModel tODO in _tasks)
        //        {
        //            string temp = tODO.Description;
        //            string date = tODO.Date;
        //            bool importance = tODO.IsStarred;
        //            _savingDescriptions.AddTask(temp);
        //            _savingDates.AddTask(date);
        //            _savingImps.AddTask(importance);

        //            List<TaskModel> steps = tODO.SubTasks;
        //            List<string> tempList = new List<string>();
        //            for (int i = 0; i < steps.Count; i++)
        //            {
        //                tempList.AddTask(steps[i].Description);
        //            }
        //            if (steps != null)
        //            {
        //                savingSteps.AddTask(tempList);
        //            }
        //        }
        //        string jsonFile = JsonConvert.SerializeObject(_savingDescriptions);
        //        string dateJsonFile = JsonConvert.SerializeObject(_savingDates);
        //        string importanceJsonFile = JsonConvert.SerializeObject(_savingImps);
        //        string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);

        //        StorageFolder folder = ApplicationData.Current.LocalFolder;
        //        StorageFolder rootFolder = await folder.CreateFolderAsync($"{t}", CreationCollisionOption.ReplaceExisting);
        //        StorageFile pendingdescjson = await rootFolder.CreateFileAsync($"{t}_desc.json", CreationCollisionOption.ReplaceExisting);
        //        await FileIO.WriteTextAsync(pendingdescjson, jsonFile);
        //        StorageFile pendingdatesjson = await rootFolder.CreateFileAsync($"{t}_dates.json", CreationCollisionOption.ReplaceExisting);
        //        await FileIO.WriteTextAsync(pendingdatesjson, dateJsonFile);
        //        StorageFile impdescjson = await rootFolder.CreateFileAsync($"{t}_imp_desc.json", CreationCollisionOption.ReplaceExisting);
        //        await FileIO.WriteTextAsync(impdescjson, importanceJsonFile);
        //        StorageFile pendingstepsjson = await rootFolder.CreateFileAsync($"{t}_steps.json", CreationCollisionOption.ReplaceExisting);
        //        await FileIO.WriteTextAsync(pendingstepsjson, stepsJsonFile);
        //    }
        //    else
        //    {
        //        StorageFolder folder = ApplicationData.Current.LocalFolder;
        //        StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{t}");
        //        if (rootFolder != null)
        //        {
        //            await rootFolder.DeleteAsync();
        //        }
        //    }

        //    _savingDescriptions.Clear();
        //    _savingDates.Clear();
        //    _savingImps.Clear();
        //    savingSteps.Clear();
        //}

        private void listOfTasks_LayoutUpdated(object sender, object e)
        {
            AllDone.Visibility = viewModel.TasksList.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        //public async Task LoadDataFromFile()
        //{
        //    var t = _tag;
        //    StorageFolder folder = ApplicationData.Current.LocalFolder;
        //    StorageFolder rootFolder = (StorageFolder)await folder.TryGetItemAsync($"{t}");

        //    if (rootFolder != null && _tasks.Count <= 0)
        //    {
        //        StorageFile descriptionFile = await rootFolder.GetFileAsync($"{t}_desc.json");
        //        StorageFile datesFile = await rootFolder.GetFileAsync($"{t}_dates.json");
        //        StorageFile importanceFile = await rootFolder.GetFileAsync($"{t}_imp_desc.json");
        //        StorageFile stepsFile = await rootFolder.GetFileAsync($"{t}_steps.json");

        //        string jsonLoaded = await FileIO.ReadTextAsync(descriptionFile);
        //        string jsonOfDatesLoaded = await FileIO.ReadTextAsync(datesFile);
        //        string jsonOfImpLoaded = await FileIO.ReadTextAsync(importanceFile);
        //        string jsonOfStepsLoaded = await FileIO.ReadTextAsync(stepsFile);

        //        List<string> loadedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonLoaded);
        //        List<string> loadedDates = JsonConvert.DeserializeObject<List<string>>(jsonOfDatesLoaded);
        //        List<bool> loadedImportance = JsonConvert.DeserializeObject<List<bool>>(jsonOfImpLoaded);
        //        List<List<string>> loadedSteps = JsonConvert.DeserializeObject<List<List<string>>>(jsonOfStepsLoaded);
        //        if (loadedDescriptions != null)
        //        {
        //            _tasks.Clear();
        //            for (int i = 0; i < loadedDescriptions.Count; i++)
        //            {
        //                TaskModel newTask = new TaskModel() { Description = loadedDescriptions[i], Date = loadedDates[i], IsStarred = loadedImportance[i] };
        //                newTask.SubTasks = new List<TaskModel>();
        //                for (int x = 0; x < loadedSteps[i].Count; x++)
        //                {
        //                    string descOfStep = loadedSteps[i][x];
        //                    newTask.SubTasks.AddTask(new TaskModel() { Description = descOfStep });
        //                }
        //                AddATask(newTask);
        //            }
        //        }
        //        List<TaskModel> newList = new List<TaskModel>(_tasks);
        //        newList.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
        //        _tasks = new ObservableCollection<TaskModel>(newList);
        //        listOfTasks.ItemsSource = _tasks;
        //    }
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            finallyLoaded = false;
            //if (e != null)
            //{
            //    List<string> parsed = e.Parameter as List<string>;

            //    if (parsed != null && parsed.Count > 0)
            //    {

            //        if (!loadedForFirstTime)
            //        {
            //            await SaveDataToFile();
            //            _tasks.Clear();
            //        }
            //        if (loadedForFirstTime)
            //        {
            //            _tag = parsed[1];
            //            await LoadDataFromFile();
            //        }
            //        loadedForFirstTime = false;

            //        _tag = parsed[1];
            //        if (lastDataParseTag != _tag)
            //        {
            //            await LoadDataFromFile();
            //            lastDataParseTag = _tag;
            //        }
            //        _name = parsed[0];
            //        this.Name = _name;
            //        this.Tag = _tag;
            //        pageTitle.Text = _name;
            //    }
            //}
            //MainPage.ins.parallax.Source = listOfTasks;
            //SortingDropDown.Content = "Date Created";
            //opt1.IsChecked = true;
            //Sort("Date Created");

            //base.OnNavigatedTo(e);
            finallyLoaded = true;
            //DataContext = e.Parameter as TaskViewModel;
        }

        private void NewTaskBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string d = NewTaskBox.Text;
                if (!string.IsNullOrEmpty(d) && !string.IsNullOrWhiteSpace(d))
                {
                    string id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    TaskModel newTask = new TaskModel() { Id = id, Description = d, Date = DateTime.Now.ToString("dd-MMMM-yyyy hh:mm:ss tt"), IsStarred = false, SubTasks = new List<TaskModel>() };
                    var vm = (TaskViewModel)this.DataContext;
                    vm.AddTask(newTask);
                    NewTaskBox.Text = string.Empty;
                    e.Handled = true;
                    Sort((string)SortingDropDown.Content);
                    listOfTasks.ScrollIntoView(newTask);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Sort((string)SortingDropDown.Content);
        }

        private async void StepCheckToggled(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            Grid g = checkbox.Parent as Grid;
            TextBlock block = VisualTreeHelper.GetChild(g, 3) as TextBlock;
            block.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
            block.Opacity = 0.6f;
            await Task.Delay(100);
            checkbox.IsChecked = false;
            block.TextDecorations = Windows.UI.Text.TextDecorations.None;
            block.Opacity = 1;
            UserControl top = checkbox.DataContext as UserControl;
            TaskModel step = top.DataContext as TaskModel;

            UserControl root = UtilityFunctions.FindParent<UserControl>(top, "UserControl");
            TaskModel context = root.DataContext as TaskModel;

            int index = 0;
            for (int i = 0; i < viewModel.TasksList.Count; i++)
            {
                if (viewModel.TasksList[i].Equals(context))
                {
                    //store index
                    index = i;
                }
            }

            var list = new List<TaskModel>(viewModel.TasksList[index].SubTasks);
            list.Remove(step);
            viewModel.TasksList[index].SubTasks = new List<TaskModel>(list);
            Sort((string)SortingDropDown.Content);
        }

        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            moreOptionsSplitView.IsPaneOpen = false;
            if (selectedTask != null)
            {
                viewModel.DeleteTask(selectedTask.Id);
            }
            selectedTask = null;
        }

        private void DeleteSubTask(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            UserControl top = item.DataContext as UserControl;
            TaskModel step = top.DataContext as TaskModel;

            UserControl root = UtilityFunctions.FindParent<UserControl>(top, "UserControl");
            TaskModel context = root.DataContext as TaskModel;
            int index = 0;
            for (int i = 0; i < viewModel.TasksList.Count; i++)
            {
                if (viewModel.TasksList[i].Equals(context))
                {
                    //store index
                    index = i;
                }
            }

            var list = new List<TaskModel>(viewModel.TasksList[index].SubTasks);
            list.Remove(step);
            viewModel.TasksList[index].SubTasks = new List<TaskModel>(list);
            Sort((string)SortingDropDown.Content);
        }

        private async void AddStep(object sender, RoutedEventArgs e)
        {
            EditDialogContent dialog = new EditDialogContent();
            Grid.SetRowSpan(dialog, 2);
            dialog.CancelButton.Style = (Style)Application.Current.Resources["ButtonStyle1"];
            dialog.Title = "Add Step";
            int index = 0;
            Button btn = sender as Button;
            TaskModel task = btn.DataContext as TaskModel;

            for (int i = 0; i < viewModel.TasksList.Count; i++)
            {
                if (viewModel.TasksList[i].Equals(task))
                {
                    //store index
                    index = i;
                }
            }

            Grid grid = (Grid)dialog.Content;
            TextBox EditTextBox = (TextBox)VisualTreeHelper.GetChild(grid, 0);
            dialog.OKButton.IsEnabled = !string.IsNullOrEmpty(EditTextBox.Text) && !string.IsNullOrWhiteSpace(EditTextBox.Text);
            TextChanged(EditTextBox);
            await dialog.ShowAsync();
            if (dialog._CustomResult == CustomResult.OK)
            {
                //do create new task
                TaskModel newStep = new TaskModel() { Description = EditTextBox.Text };
                var list = new List<TaskModel>(viewModel.TasksList[index].SubTasks)
                {
                    newStep
                };
                viewModel.TasksList[index].SubTasks = new List<TaskModel>(list);
                EditTextBox.Text = string.Empty;
                Sort((string)SortingDropDown.Content);
            }
        }

        public void TextChanged(TextBox b)
        {
            if (dialog != null)
                dialog.OKButton.IsEnabled = !(string.IsNullOrEmpty(b.Text) || !string.IsNullOrWhiteSpace(b.Text));
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
                var panel = UtilityFunctions.FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
                var block = UtilityFunctions.FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
                panel.Translation = System.Numerics.Vector3.Zero;
                panel.Opacity = 1;
                block.Translation = System.Numerics.Vector3.Zero;
            }
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var panel = UtilityFunctions.FindControl<StackPanel>(c, typeof(StackPanel), "timeStampPanel");
            var block = UtilityFunctions.FindControl<TextBlock>(c, typeof(TextBlock), "TaskDesc");
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
            var list = new List<TaskModel>(viewModel.TasksList);
            switch (typeOfSort)
            {
                case "Date Created":
                    list.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
                    break;
                case "Text":
                    list.Sort((x, y) => string.Compare(x.Description, y.Description));
                    break;
                case "Steps":
                    list = list.OrderBy(x => x.SubTasks.Count).Reverse().ToList();
                    break;
                case "Importance":
                    var query = from task in list
                                orderby !task.IsStarred
                                select task;
                    list = query.ToList();
                    break;
                case "Completed":
                    var q = from task in list
                            orderby !task.IsCompleted
                            select task;
                    list = q.ToList();
                    break;
                default:
                    break;
            }
            viewModel.TasksList = new ObservableCollection<TaskModel>(list);
        }

        private void SubTaskPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var c = cb.DataContext as Control;
            var strip = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "rect");
            var delbtn = UtilityFunctions.FindControl<Button>(c, typeof(Button), "delsubtask");
            delbtn.Translation = new System.Numerics.Vector3(50, 0, 0);
            delbtn.Opacity = 0;
            strip.Opacity = 0;
            var back = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        private void SubTaskPointerExited(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var strip = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "rect");
            strip.Translation = new System.Numerics.Vector3(0, 10, 0);
            var delbtn = UtilityFunctions.FindControl<Button>(c, typeof(Button), "delsubtask");
            delbtn.Translation = new System.Numerics.Vector3(50, 0, 0);
            delbtn.Opacity = 0;
            var back = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        private void SubTaskPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var c = sender as Control;
            var strip = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "rect");
            var delbtn = UtilityFunctions.FindControl<Button>(c, typeof(Button), "delsubtask");
            delbtn.Translation = System.Numerics.Vector3.Zero;
            delbtn.Opacity = 1;
            strip.Translation = new System.Numerics.Vector3(0, 2, 0);
            var back = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0.1f;
        }

        private void BTN_SubTaskPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Button delbtn = sender as Button;
            var c = delbtn.DataContext as Control;
            var strip = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "rect");
            strip.Translation = new System.Numerics.Vector3(0, 10, 0);
            delbtn.Translation = new System.Numerics.Vector3(50, 0, 0);
            delbtn.Opacity = 0;
            var back = UtilityFunctions.FindControl<Grid>(c, typeof(Grid), "backplate");
            back.Opacity = 0;
        }

        private void OpenSplitView(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            selectedTask = btn.DataContext as TaskModel;
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
            for (int i = 0; i < viewModel.TasksList.Count; i++)
            {
                if (viewModel.TasksList[i].Equals(selectedTask))
                {
                    //store index
                    index = i;
                }
            }
            viewModel.TasksList[index].Description = edittasktextbox.Text;
            moreOptionsSplitView.IsPaneOpen = false;
            edittasktextbox.Text = string.Empty;
            Sort((string)SortingDropDown.Content);
        }

        private void TaskDesc_Loaded(object sender, RoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            TaskModel tm = (TaskModel)textBlock.DataContext;
            if (tm!=null)
            {
                if (tm.IsCompleted)
                {
                    textBlock.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
                    textBlock.Opacity = 0.6f;
                }
                else
                {
                    textBlock.TextDecorations = Windows.UI.Text.TextDecorations.None;
                    textBlock.Opacity = 1;
                }
            }
            
        }
    }
}