using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do.Models;

namespace To_Do.ViewModels
{
    public class CustomNavigationViewItemViewModel : INotifyPropertyChanged
    {
        #region OnPropertyChangedHandler
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private ObservableCollection<CustomNavigationViewItemModel> navViewItemsList;
        public ObservableCollection<CustomNavigationViewItemModel> NavViewItemsList
        {
            get { return navViewItemsList; }
            set
            {
                navViewItemsList = value;
                OnPropertyChanged("NavViewItemsList");
            }
        }

        public CustomNavigationViewItemViewModel()
        {
            NavViewItemsList = new ObservableCollection<CustomNavigationViewItemModel>();
        }

        public void AddNavViewItem(CustomNavigationViewItemModel item)
        {
            NavViewItemsList.Add(item);
        }

        public bool DeleteTask(string idTag)
        {
            bool isDeleted = false;
            for (int i = 0; i < NavViewItemsList.Count; i++)
            {
                if (NavViewItemsList[i].IdTag == idTag)
                {
                    NavViewItemsList.RemoveAt(i);
                    isDeleted = true;
                    break;
                }
            }
            return isDeleted;
        }
    }
}
