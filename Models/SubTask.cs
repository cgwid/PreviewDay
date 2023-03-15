using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PreviewDay.Models
{
    public class SubTask
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public bool IsChecked { get; set; }
    }
}