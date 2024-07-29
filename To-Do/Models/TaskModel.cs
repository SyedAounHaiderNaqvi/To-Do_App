using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace To_Do.Models
{
    public class TaskModel
    {
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        public string Date { get; set; }
        public bool IsCompleted = false;
        public bool IsStarred = false;
        public List<TaskModel> SubTasks;


        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
