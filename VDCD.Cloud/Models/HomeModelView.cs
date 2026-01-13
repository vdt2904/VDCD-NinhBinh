using VDCD.Entities.Custom;

namespace VDCD.Models
{
    public class HomeModelView
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public List<Center> Centers { get; set; }
    }
}
