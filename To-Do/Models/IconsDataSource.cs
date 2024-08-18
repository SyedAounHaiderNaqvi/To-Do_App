using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Microsoft.Toolkit.Collections;
using System.Threading;
using System.Linq;

namespace To_Do.Models
{
    public class IconData
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string[] Tags { get; set; } = new string[] { };

        public string Character => char.ConvertFromUtf32(Convert.ToInt32(Code, 16));
    }

    public class IconsDataSource : IIncrementalSource<IconData>
    {
        private readonly List<IconData> icons;

        public IconsDataSource()
        {
            icons = new List<IconData>();
            LoadIconsFromFile();
        }

        internal async void LoadIconsFromFile()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Models/IconsData.json"));
            string loadedJson = await FileIO.ReadTextAsync(file);
            var loadedIcons = JsonConvert.DeserializeObject<List<IconData>>(loadedJson);
            foreach (var icon in loadedIcons)
            {
                icons.Add(icon);
            }
        }

        public async Task<IEnumerable<IconData>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in icons
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            // Simulates a longer request...
            await Task.Delay(1000);
            return result;
        }
    }
}
