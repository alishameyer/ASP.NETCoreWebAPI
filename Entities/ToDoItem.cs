using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebAPI_Sample.Entitys
{
    public class ToDoItem
    {
        public int Id { get; set; }

        public string Task { get; set; }

        public bool IsCompleted { get; set; }
    }
}
