using VDCD.Entities.Custom;

namespace VDCD.Models
{
    public class ProjectsViewModel
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public List<Project>? Projects { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
