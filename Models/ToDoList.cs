using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace PreviewDay.Models
{
    public class ToDoList
    {
        public string? ListName { get; set; }

        public List<ToDoTask> Tasks { get; set; } = new List<ToDoTask>();
    }
}