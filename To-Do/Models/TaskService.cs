using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace To_Do.Models
{
    public class ////TaskService
    {
        public static ObservableCollection<TaskModel> ObjTasksList;

        public //TaskService()
        {
            ObjTasksList = new ObservableCollection<TaskModel>();
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

        
    }
}
