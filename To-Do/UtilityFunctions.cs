using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json;
using Windows.Storage;
using To_Do.Models;
using Windows.ApplicationModel;

namespace To_Do
{
    /// <summary>
    /// Provides useful static functions to be used in any other class
    /// </summary>
    public static class UtilityFunctions
    {
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

        public static T FindParent<T>(DependencyObject child, string parentName)
            where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                if (frameworkElement.Name == parentName && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }

        public static Color ChangeColorBrightness(Color c, bool isContent)
        {
            float r, g, b;
            if (isContent)
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = Lerp(c.R, 125f, 0.2f);
                    g = Lerp(c.G, 125f, 0.2f);
                    b = Lerp(c.B, 125f, 0.2f);

                }
                else
                {
                    r = Lerp(c.R, 255f, 0.7f);
                    g = Lerp(c.G, 255f, 0.7f);
                    b = Lerp(c.B, 255f, 0.7f);
                }
            }
            else
            {
                if (ThemeHelper.IsDarkTheme())
                {

                    r = Lerp(c.R, 255f, 0.1f);
                    g = Lerp(c.G, 255f, 0.1f);
                    b = Lerp(c.B, 255f, 0.1f);

                }
                else
                {
                    r = Lerp(c.R, 255f, 0.8f);
                    g = Lerp(c.G, 255f, 0.8f);
                    b = Lerp(c.B, 255f, 0.8f);
                }
            }


            return Color.FromArgb(c.A, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }

        public static float Lerp(float a, float b, float f)
        {
            return (float)((a * (1.0 - f)) + (b * f));
        }

        public static async Task<bool> SaveListDataToStorage(string fileTag, object saveData)
        {
            string output = JsonConvert.SerializeObject(saveData);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync($"{fileTag}.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, output);
            return true;
        }

        public static async Task<List<TaskModel>> LoadListDataFromStorage(string fileTag)
        {
            // Access the appfolder
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile tasksFile = (StorageFile)await folder.TryGetItemAsync($"{fileTag}.json");
            if (tasksFile == null)
            {
                return null;
            }
            else
            {
                string loadedJson = await FileIO.ReadTextAsync(tasksFile);
                List<TaskModel> loadedList = JsonConvert.DeserializeObject<List<TaskModel>>(loadedJson);
                if (loadedList != null)
                {
                    return loadedList;
                }
                else
                {
                    return null;
                }
            }
        }

        public static string GetTimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }

        public static async Task<List<CustomNavigationViewItemModel>> LoadCustomNavigationViewItemsFromStorage(string fileTag)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder specialFolder = (StorageFolder)await folder.TryGetItemAsync($"{fileTag}");
            if (specialFolder == null)
            {
                return null;
            }
            else
            {
                StorageFile itemsFile = (StorageFile)await specialFolder.TryGetItemAsync($"{fileTag}.json");
                if (itemsFile == null)
                {
                    return null;
                }
                else
                {
                    string loadedJson = await FileIO.ReadTextAsync(itemsFile);
                    List<CustomNavigationViewItemModel> loadedList = JsonConvert.DeserializeObject<List<CustomNavigationViewItemModel>>(loadedJson);
                    if (loadedList != null)
                    {
                        return loadedList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
        }

        public static async Task SaveCustomNavigationViewItemsToStorage(string fileTag, object saveData)
        {
            string output = JsonConvert.SerializeObject(saveData);
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder specialFolder = await folder.CreateFolderAsync($"{fileTag}", CreationCollisionOption.OpenIfExists);
            StorageFile file = await specialFolder.CreateFileAsync($"{fileTag}.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, output);
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
