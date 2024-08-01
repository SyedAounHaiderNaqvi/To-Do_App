using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace To_Do.Models
{
    public class TaskService
    {
        public static ObservableCollection<TaskModel> ObjTasksList;

        public TaskService()
        {
            ObjTasksList = new ObservableCollection<TaskModel>()
            {
                new TaskModel()
                {
                    Id = 0,
                    Description = "Test",
                    IsCompleted = false,
                    IsStarred = false,
                    SubTasks = new List<TaskModel>()
                    {
                        new TaskModel()
                        {
                            Id=10,
                            Description="Subbtsdasdasd"
                        }
                    },
                    Date = System.DateTime.Now.ToString()
                }
            };
            Debug.WriteLine("Constructed Task Service with one task in the model");
        }

        public ObservableCollection<TaskModel> GetAll()
        {
            return ObjTasksList;
        }

        public bool AddTask(TaskModel task)
        {
            ObjTasksList.Add(task);
            return true;
        }

        public bool UpdateTask(TaskModel taskToUpdate)
        {
            bool isUpdated = false;

            foreach (TaskModel task in ObjTasksList)
            {
                if (task.Id == taskToUpdate.Id)
                {
                    task.Description = taskToUpdate.Description;
                    task.Date = taskToUpdate.Date;
                    task.IsCompleted = taskToUpdate.IsCompleted;
                    task.IsStarred = taskToUpdate.IsStarred;
                    task.SubTasks = taskToUpdate.SubTasks;
                    isUpdated = true;
                    break;
                }
            }

            return isUpdated;
        }

        public bool DeleteTask(int id)
        {
            bool isDeleted = false;
            for (int i = 0; i < ObjTasksList.Count; i++)
            {
                if (ObjTasksList[i].Id == id)
                {
                    ObjTasksList.RemoveAt(i);
                    isDeleted = true;
                    break;
                }
            }
            return isDeleted;
        }
    }
}
