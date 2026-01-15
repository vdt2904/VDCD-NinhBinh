using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class Project
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public string? Json { get; set; }
        public int? CategoryId { get; set; }
        public string? Investor { get; set;}
        public string ? Location { get; set; }
        public string? Time { get; set; }
        public string? Image {  get; set; }
        public bool? IsActive { get; set; }
        public string? Plug {  get; set; }
    }
}
