using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using To_Do.Models;
using To_Do.Commands;
using System;
using System.Diagnostics;
using System.Collections.Generic;

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

        //public bool UpdateTask(TaskModel taskToUpdate)
        //{
        //    bool isUpdated = false;

        //    foreach (TaskModel task in TasksList)
        //    {
        //        if (task.Id == taskToUpdate.Id)
        //        {
        //            task.Description = taskToUpdate.Description;
        //            task.Date = taskToUpdate.Date;
        //            task.IsCompleted = taskToUpdate.IsCompleted;
        //            task.IsStarred = taskToUpdate.IsStarred;
        //            task.SubTasks = taskToUpdate.SubTasks;
        //            isUpdated = true;
        //            break;
        //        }
        //    }

        //    return isUpdated;
        //}

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