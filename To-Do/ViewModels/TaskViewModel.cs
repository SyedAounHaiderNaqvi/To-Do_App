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
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        TaskService ObjTaskService;

        //private RelayCommand addCommand;
        //public RelayCommand AddCommand
        //{
        //    get { return addCommand; }
        //}

        //private RelayCommand randomCommand;
        //public RelayCommand RandomCommand
        //{
        //    get { return randomCommand; }
        //}

        public TaskViewModel()
        {
            ObjTaskService = new TaskService();
            LoadData();
            //addCommand = new RelayCommand(Add);
            //randomCommand = new RelayCommand(Randomize);
            Debug.WriteLine("Successfully constrcuted viewmodel and loadeddata");
        }

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

        public void LoadData()
        {
            TasksList = new ObservableCollection<TaskModel>(ObjTaskService.GetAll());
        }

        public void Add(TaskModel task)
        {
            Debug.WriteLine("Clicked");
            try
            {
                ObjTaskService.AddTask(task);
                LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Randomize()
        {
            Debug.WriteLine("Randomizing");
            Random random = new Random();
            var index = random.Next(0, TasksList.Count);
            TasksList[index].Description = random.Next().ToString();
            TasksList[index].Date = random.Next().ToString();
            TasksList[index].SubTasks.Add(new TaskModel() { Description = random.Next().ToString() });
            LoadData();
        }
    }
}