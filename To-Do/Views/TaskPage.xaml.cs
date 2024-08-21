using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Linq;
using To_Do.Models;
using To_Do.ViewModels;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls.Primitives;
using System.Diagnostics;

namespace To_Do.Views
{
    public sealed partial class TaskPage : Page
    {
        public static TaskPage instance;
        public TaskViewModel viewModel;
        TaskModel selectedTask = null;
        public EditDialogContent dialog;

        public string nameOfThisPage;
        public string idTagOfThisPage;

        string nameOfSelectedTask = string.Empty;

        public TaskPage()
        {
            this.InitializeComponent();
            instance = this;
            viewModel = (TaskViewModel)this.DataContext;
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await UtilityFunctions.SaveListDataToStorage(idTagOfThisPage, viewModel.TasksList);

            base.OnNavigatingFrom(e);
        }

        public async Task ManualSave()
        {
            await UtilityFunctions.SaveListDataToStorage(idTagOfThisPage, viewModel.TasksList);
        }

        private void listOfTasks_LayoutUpdated(object sender, object e)
        {
            AllDone.Visibility = viewModel.TasksList.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task LoadDataFromFile(string fileTag)
        {
            var newList = await UtilityFunctions.LoadListDataFromStorage(fileTag);
            if (newList != null)
            {
                newList.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.Date), Convert.ToDateTime(y.Date)));
                viewModel.TasksList = new ObservableCollection<TaskModel>(newList);
            }
            else
            {
                viewModel.TasksList = new ObservableCollection<TaskModel>();
            }
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                CustomNavigationViewItemModel parsed = e.Parameter as CustomNavigationViewItemModel;
                if (parsed != null)
                {
                    nameOfThisPage = parsed.Name;

                    idTagOfThisPage = parsed.IdTag;

                    pageTitle.Text = nameOfThisPage;

                    await LoadDataFromFile(parsed.IdTag);
                    MainPage.ins.parallax.Source = listOfTasks;
                    SortingDropDown.Content = "Date Created";
                    opt1.IsChecked = true;
                }
            }
            
            Sort("Date Created");

            // Randomly choose a placeholder for task box
            NewTaskBox.PlaceholderText = UtilityFunctions.GetRandomPlaceholder();

            base.OnNavigatedTo(e);
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
                    NewTaskBox.PlaceholderText = UtilityFunctions.GetRandomPlaceholder();
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
            nameOfSelectedTask = string.Empty;
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
            nameOfSelectedTask = selectedTask.Description;
            edittasktextbox.SelectionStart = edittasktextbox.Text.Length;
            DateCreatedTextBlock.Text = selectedTask.Date;
            moreOptionsSplitView.IsPaneOpen = true;
        }

        private void CloseSplitView(object sender, RoutedEventArgs e)
        {
            moreOptionsSplitView.IsPaneOpen = false;
            nameOfSelectedTask = string.Empty;
            DateCreatedTextBlock.Text = string.Empty;
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

        private void UserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var tappedItem = (UIElement)e.OriginalSource;
            var attachedFlyout = (MenuFlyout)FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);

            attachedFlyout.ShowAt(tappedItem, e.GetPosition(tappedItem));
        }

        private void EditFlyoutItem(object sender, RoutedEventArgs e)
        {
            var flyoutItem = sender as MenuFlyoutItem;
            selectedTask = flyoutItem.DataContext as TaskModel;
            edittasktextbox.Text = selectedTask.Description;
            nameOfSelectedTask = selectedTask.Description;
            edittasktextbox.SelectionStart = edittasktextbox.Text.Length;
            DateCreatedTextBlock.Text = selectedTask.Date;
            moreOptionsSplitView.IsPaneOpen = true;
        }

        private void DeleteTaskFlyoutItem(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            selectedTask = item.DataContext as TaskModel;
            moreOptionsSplitView.IsPaneOpen = false;
            viewModel.DeleteTask(selectedTask.Id);
            selectedTask = null;
        }

        private void OnTaskSplitViewClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            Debug.WriteLine("Closing Split View");
            TryEditTask();
        }

        public void TryEditTask()
        {
            if (selectedTask != null)
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
                if (!string.IsNullOrEmpty(edittasktextbox.Text) && !string.IsNullOrWhiteSpace(edittasktextbox.Text))
                {
                    Debug.WriteLine("Edited successfully");
                    viewModel.TasksList[index].Description = edittasktextbox.Text;
                }
                else
                {
                    // In this case, textbox was sus so revert to original name
                    Debug.WriteLine("Reverted");
                    viewModel.TasksList[index].Description = nameOfSelectedTask;
                }

                edittasktextbox.Text = string.Empty;
                nameOfSelectedTask = string.Empty;
                DateCreatedTextBlock.Text = string.Empty;
                Sort((string)SortingDropDown.Content);
            }
        }
    }
}