using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using To_Do.NavigationPages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace To_Do
{
    public sealed partial class BackupContentDialog : ContentDialog
    {
        public ElementTheme THEME;
        public List<string> savingDescriptions = new List<string>();
        public List<string> savingDates = new List<string>();
        public List<bool> savingImps = new List<bool>();
        public List<string> savingCompletedDates = new List<string>();
        public List<string> completedSaving = new List<string>();
        public List<List<string>> savingSteps = new List<List<string>>();

        public List<string> myDayTasksToSave = new List<string>();
        public List<string> myDayDatesToSave = new List<string>();
        public List<bool> myDayImpsToSave = new List<bool>();
        public List<List<string>> myDayTaskStepsToSave = new List<List<string>>();
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public BackupContentDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
        }

        private async void CreateBackup(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                //disable buttons and controls
                IsPrimaryButtonEnabled = false;
                normalui.Visibility = Visibility.Collapsed;
                backupui.Visibility = Visibility.Visible;
                (sender as Button).IsEnabled = false;


                int total = PendingTasks.instance.TaskItems.Count + MyDay.instance.TaskItems.Count + CompletedTasks.instance.CompleteTasks.Count;
                float step = 100 / total;

                //get list of data to store
                PendingTasks ins = PendingTasks.instance;
                foreach (TODOTask tODO in ins.TaskItems)
                {
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
                    backupprogress.Value += step;
                    await Task.Delay(100);
                }
                MyDay mDayins = MyDay.instance;

                foreach (TODOTask task in mDayins.TaskItems)
                {
                    string temp = task.Description;
                    string date = task.Date;
                    bool importance = task.IsStarred;
                    myDayTasksToSave.Add(temp);
                    myDayDatesToSave.Add(date);
                    myDayImpsToSave.Add(importance);

                    List<TODOTask> steps = task.SubTasks;
                    List<string> tempList = new List<string>();
                    for (int i = 0; i < steps.Count; i++)
                    {
                        tempList.Add(steps[i].Description);
                    }
                    if (steps != null)
                    {
                        myDayTaskStepsToSave.Add(tempList);
                    }
                    backupprogress.Value += step;
                    await Task.Delay(100);
                }
                foreach (TODOTask task in CompletedTasks.instance.CompleteTasks)
                {
                    string temp = task.Description;
                    string date = task.Date;
                    completedSaving.Add(temp);
                    savingCompletedDates.Add(date);
                    backupprogress.Value += step;
                    await Task.Delay(100);
                }
                string jsonFile = JsonConvert.SerializeObject(savingDescriptions);
                string compJsonFile = JsonConvert.SerializeObject(completedSaving);
                string dateJsonFile = JsonConvert.SerializeObject(savingDates);
                string compdateJsonFile = JsonConvert.SerializeObject(savingCompletedDates);
                string importanceJsonFile = JsonConvert.SerializeObject(savingImps);
                string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);

                string myDayJsonFile = JsonConvert.SerializeObject(myDayTasksToSave);
                string myDaydateJsonFile = JsonConvert.SerializeObject(myDayDatesToSave);
                string myDayImpJsonFile = JsonConvert.SerializeObject(myDayImpsToSave);
                string myDayStepsJsonFile = JsonConvert.SerializeObject(myDayTaskStepsToSave);

                StorageFile pendingdescjson = await folder.CreateFileAsync("pending_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingdescjson, jsonFile);

                StorageFile completeddescjson = await folder.CreateFileAsync("comp_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(completeddescjson, compJsonFile);

                StorageFile pendingdatesjson = await folder.CreateFileAsync("pending_dates.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingdatesjson, dateJsonFile);

                StorageFile compdatesjson = await folder.CreateFileAsync("comp_dates.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(compdatesjson, compdateJsonFile);

                StorageFile impdescjson = await folder.CreateFileAsync("imp_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(impdescjson, importanceJsonFile);

                StorageFile pendingstepsjson = await folder.CreateFileAsync("pending_steps.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(pendingstepsjson, stepsJsonFile);

                StorageFile mydaydescjson = await folder.CreateFileAsync("md_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mydaydescjson, myDayJsonFile);

                StorageFile mydaydatejson = await folder.CreateFileAsync("md_date.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mydaydatejson, myDaydateJsonFile);

                StorageFile mydayimpjson = await folder.CreateFileAsync("md_imp_desc.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mydayimpjson, myDayImpJsonFile);

                StorageFile mydaystepsjson = await folder.CreateFileAsync("md_steps.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(mydaystepsjson, myDayStepsJsonFile);

                //enable buttons and controls
                IsPrimaryButtonEnabled = true;
                normalui.Visibility = Visibility.Visible;
                backupui.Visibility = Visibility.Collapsed;
                backupprogress.Value = 0;
                (sender as Button).IsEnabled = true;
            }
            else
            {

            }
        }
    }
}
