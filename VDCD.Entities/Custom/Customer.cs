using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class Customer
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? Image { get; set;}
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public string? Address { get; set; }
        public bool IsShow { get; set; } = false;
    }
}
