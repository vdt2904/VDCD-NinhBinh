using VDCD.Entities.Custom;
using VDCD.Entities.DTO;

namespace VDCD.Models
{
    public class HomeModelView
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public List<Center> Centers { get; set; }
        public List<Project> Projects { get; set; }
        public List<Posts> Blogs { get; set; }
        public List<Customer> Customers { get; set; }
    }
}
