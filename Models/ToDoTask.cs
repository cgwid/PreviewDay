using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace PreviewDay.Models
{
    public class ToDoTask
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public Importance? Importance { get; set; }
        public List<SubTask> SubTasks { get; set; } = new List<SubTask>();
    }
}