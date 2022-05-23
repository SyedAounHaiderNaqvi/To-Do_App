using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using To_Do.NavigationPages;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                StorageFolder rootFolder = await folder.CreateFolderAsync("To-Do Backup", CreationCollisionOption.ReplaceExisting);
                if (rootFolder != null)
                {
                    //disable buttons and controls
                    IsPrimaryButtonEnabled = false;
                    normalui.Visibility = Visibility.Collapsed;
                    backupui.Visibility = Visibility.Visible;
                    (sender as Button).IsEnabled = false;
                    restorebtn.IsEnabled = false;

                    int total = pendingtasks.instance.TaskItems.Count + completedtasks.instance.CompleteTasks.Count;
                    float step = 100 / total;

                    //get list of data to store
                    pendingtasks ins = pendingtasks.instance;
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
                    }
                    
                    foreach (TODOTask task in completedtasks.instance.CompleteTasks)
                    {
                        string temp = task.Description;
                        string date = task.Date;
                        completedSaving.Add(temp);
                        savingCompletedDates.Add(date);
                        backupprogress.Value += step;
                    }
                    string jsonFile = JsonConvert.SerializeObject(savingDescriptions);
                    string compJsonFile = JsonConvert.SerializeObject(completedSaving);
                    string dateJsonFile = JsonConvert.SerializeObject(savingDates);
                    string compdateJsonFile = JsonConvert.SerializeObject(savingCompletedDates);
                    string importanceJsonFile = JsonConvert.SerializeObject(savingImps);
                    string stepsJsonFile = JsonConvert.SerializeObject(savingSteps);


                    StorageFile pendingdescjson = await rootFolder.CreateFileAsync("pending_desc.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(pendingdescjson, jsonFile);

                    StorageFile completeddescjson = await rootFolder.CreateFileAsync("comp_desc.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(completeddescjson, compJsonFile);

                    StorageFile pendingdatesjson = await rootFolder.CreateFileAsync("pending_dates.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(pendingdatesjson, dateJsonFile);

                    StorageFile compdatesjson = await rootFolder.CreateFileAsync("comp_dates.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(compdatesjson, compdateJsonFile);

                    StorageFile impdescjson = await rootFolder.CreateFileAsync("imp_desc.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(impdescjson, importanceJsonFile);

                    StorageFile pendingstepsjson = await rootFolder.CreateFileAsync("pending_steps.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(pendingstepsjson, stepsJsonFile);


                    //enable buttons and controls
                    IsPrimaryButtonEnabled = true;
                    normalui.Visibility = Visibility.Visible;
                    backupui.Visibility = Visibility.Collapsed;
                    backupprogress.Value = 0;
                    (sender as Button).IsEnabled = true;
                    restorebtn.IsEnabled = true;
                }
            }
        }

        private async void RestoreData(object sender, RoutedEventArgs e)
        {
            string[] namesofFiles = new string[] { "pending_desc.json", "comp_desc.json", "pending_dates.json", "comp_dates.json", "imp_desc.json", "pending_steps.json" };
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                var files = await folder.GetFilesAsync();
                int canProceedCount = 0;
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        for (int i = 0; i < namesofFiles.Length; i++)
                        {
                            if (file.Name == namesofFiles[i])
                            {
                                canProceedCount++;
                            }
                        }
                    }
                }
                if (canProceedCount.Equals(9))
                {
                    RestoreErrorText.Visibility = Visibility.Collapsed;
                    IsPrimaryButtonEnabled = false;
                    normalrestoreui.Visibility = Visibility.Collapsed;
                    restoringui.Visibility = Visibility.Visible;
                    (sender as Button).IsEnabled = false;
                    backupbtn.IsEnabled = false;

                    //load data
                    List<string> loadedDescriptions = new List<string>();
                    List<string> loadedDates = new List<string>();
                    List<bool> loadedImportance = new List<bool>();
                    List<List<string>> loadedSteps = new List<List<string>>();

                    //completed tasks
                    List<string> loadedCompletedDescriptions = new List<string>();
                    List<string> loadedCompletedDates = new List<string>();

                    foreach (var file in files)
                    {
                        var jsonText = await FileIO.ReadTextAsync(file);
                        switch (file.Name)
                        {
                            case "pending_desc.json":
                                loadedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonText);
                                break;
                            case "pending_dates.json":
                                loadedDates = JsonConvert.DeserializeObject<List<string>>(jsonText);
                                break;
                            case "imp_desc.json":
                                loadedImportance = JsonConvert.DeserializeObject<List<bool>>(jsonText);
                                break;
                            case "pending_steps.json":
                                loadedSteps = JsonConvert.DeserializeObject<List<List<string>>>(jsonText);
                                break;

                            case "comp_desc.json":
                                loadedCompletedDescriptions = JsonConvert.DeserializeObject<List<string>>(jsonText);
                                break;
                            case "comp_dates.json":
                                loadedCompletedDates = JsonConvert.DeserializeObject<List<string>>(jsonText);
                                break;

                            default:
                                break;
                        }
                    }
                    int total = loadedCompletedDescriptions.Count + loadedDescriptions.Count;
                    float step = 100 / total;
                    if (loadedDescriptions != null)
                    {
                        for (int i = 0; i < loadedDescriptions.Count; i++)
                        {
                            TODOTask newTask = new TODOTask() { Description = loadedDescriptions[i], Date = loadedDates[i], IsStarred = loadedImportance[i] };
                            newTask.SubTasks = new List<TODOTask>();
                            for (int x = 0; x < loadedSteps[i].Count; x++)
                            {
                                string descOfStep = loadedSteps[i][x];
                                newTask.SubTasks.Add(new TODOTask() { Description = descOfStep });
                            }
                            pendingtasks.instance.AddATask(newTask);
                            restoreprogressbar.Value += step;
                        }
                    }

                    if (loadedCompletedDescriptions != null)
                    {
                        for (int i = 0; i < loadedCompletedDescriptions.Count; i++)
                        {
                            completedtasks.instance.AddATask(loadedCompletedDescriptions[i], loadedCompletedDates[i]);
                            restoreprogressbar.Value += step;
                        }
                    }

                    IsPrimaryButtonEnabled = true;
                    normalrestoreui.Visibility = Visibility.Visible;
                    restoringui.Visibility = Visibility.Collapsed;
                    (sender as Button).IsEnabled = true;
                    backupbtn.IsEnabled = true;
                    restoreprogressbar.Value = 0;
                }
                else
                {
                    RestoreErrorText.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
