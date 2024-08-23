using System;
using System.Linq;
using To_Do.Views;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace To_Do
{
    public sealed partial class BackupContentDialog : ContentDialog
    {
        public ElementTheme THEME;
        StorageFolder appFolder = ApplicationData.Current.LocalFolder;
        Windows.Storage.Pickers.FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        };
        

        public BackupContentDialog()
        {
            this.InitializeComponent();
            folderPicker.FileTypeFilter.Add("*");
            THEME = ThemeHelper.ActualTheme;
        }

        private async void CreateBackup(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageFolder rootFolder = await folder.CreateFolderAsync("To-Do Backup", CreationCollisionOption.ReplaceExisting);
                
                //disable buttons and controls
                IsPrimaryButtonEnabled = false;
                normalui.Visibility = Visibility.Collapsed;
                backupui.Visibility = Visibility.Visible;
                (sender as Button).IsEnabled = false;
                restorebtn.IsEnabled = false;
                
                // Then copy all the files and folders from there to the specified rootFolder
                var itemList = await appFolder.GetItemsAsync();
                var localContent = itemList.ToList();
                int total = localContent.Count;
                float step = 100 / total;
                foreach (var item in localContent)
                {
                    backupprogress.Value += step;
                    if (item.IsOfType(StorageItemTypes.File))
                    {
                        await ((StorageFile)item).CopyAsync(rootFolder, ((StorageFile)item).Name, NameCollisionOption.ReplaceExisting);
                    }
                    else if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        StorageFile file = await ((StorageFolder)item).GetFileAsync("NavigationViewItems.json");
                        StorageFolder listFolder = await rootFolder.CreateFolderAsync("NavigationViewItems", CreationCollisionOption.ReplaceExisting);
                        await file.CopyAsync(listFolder, file.Name, NameCollisionOption.ReplaceExisting);
                    }
                    
                }

                // Enable the buttons and controls
                IsPrimaryButtonEnabled = true;
                normalui.Visibility = Visibility.Visible;
                backupui.Visibility = Visibility.Collapsed;
                backupprogress.Value = 0;
                (sender as Button).IsEnabled = true;
                restorebtn.IsEnabled = true;
            }
        }

        private async void RestoreData(object sender, RoutedEventArgs e)
        {
            bool canProceed = false;
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                var itemList = await folder.GetItemsAsync();
                var localContent = itemList.ToList();

                // Checks to ensure local data are as original as possible
                // Need more checks as needed
                foreach (var item in localContent)
                {
                    if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        StorageFolder storageFolder = (StorageFolder)item;
                        //Check if name is NavigationViewItems and it has the file inside
                        if (storageFolder.Name.Equals("NavigationViewItems") && await storageFolder.TryGetItemAsync("NavigationViewItems.json") != null)
                        {
                            canProceed = true;
                            break;
                        }
                    }
                }

                if (canProceed)
                {
                    RestoreErrorText.Visibility = Visibility.Collapsed;
                    IsPrimaryButtonEnabled = false;
                    normalrestoreui.Visibility = Visibility.Collapsed;
                    restoringui.Visibility = Visibility.Visible;
                    (sender as Button).IsEnabled = false;
                    backupbtn.IsEnabled = false;

                    int total = localContent.Count;
                    float step = 100 / total;

                    foreach (var item in localContent)
                    {
                        if (item.IsOfType(StorageItemTypes.File))
                        {
                            await ((StorageFile)item).CopyAsync(appFolder, ((StorageFile)item).Name, NameCollisionOption.ReplaceExisting);
                        }
                        else if (item.IsOfType(StorageItemTypes.Folder))
                        {
                            StorageFile file = await ((StorageFolder)item).GetFileAsync("NavigationViewItems.json");
                            StorageFolder listFolder = await appFolder.CreateFolderAsync("NavigationViewItems", CreationCollisionOption.ReplaceExisting);
                            await file.CopyAsync(listFolder, file.Name, NameCollisionOption.ReplaceExisting);
                        }
                        restoreprogressbar.Value += step;
                    }

                    IsPrimaryButtonEnabled = true;
                    normalrestoreui.Visibility = Visibility.Visible;
                    restoringui.Visibility = Visibility.Collapsed;
                    (sender as Button).IsEnabled = true;
                    backupbtn.IsEnabled = true;
                    restoreprogressbar.Value = 0;

                    // Manually load the navigation view lists
                    await MainPage.ins.LoadCustomNavigationViewItemsFromFile();
                }
                else
                {
                    RestoreErrorText.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
