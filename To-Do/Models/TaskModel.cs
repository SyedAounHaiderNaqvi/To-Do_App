using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace To_Do.Models
{
    public class TaskModel
    {
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        private bool isCompleted = false;
        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }

        private bool isStarred = false;
        public bool IsStarred
        {
            get { return isStarred; }
            set
            {
                isStarred = value;
                OnPropertyChanged("IsStarred");
            }
        }

        private List<TaskModel> subTasks;
        public List<TaskModel> SubTasks
        {
            get { return subTasks; }
            set
            {
                subTasks = value;
                OnPropertyChanged("SubTasks");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
