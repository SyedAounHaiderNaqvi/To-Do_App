using System.Collections.ObjectModel;
using System.ComponentModel;
using To_Do.Models;

namespace To_Do.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        #region OnPropertyChangedHandler
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private ObservableCollection<TaskModel> tasksList;
        public ObservableCollection<TaskModel> TasksList
        {
            get { return tasksList; }
            set
            {
                tasksList = value;
                OnPropertyChanged("TasksList");
            }
        }

        public TaskViewModel()
        {
            TasksList = new ObservableCollection<TaskModel>();
        }

        public void AddTask(TaskModel task)
        {
            TasksList.Add(task);
        }

        public bool DeleteTask(string id)
        {
            bool isDeleted = false;
            for (int i = 0; i < TasksList.Count; i++)
            {
                if (TasksList[i].Id == id)
                {
                    TasksList.RemoveAt(i);
                    isDeleted = true;
                    break;
                }
            }
            return isDeleted;
        }
    }
}