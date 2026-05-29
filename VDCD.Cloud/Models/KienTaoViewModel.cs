using VDCD.Entities.Custom;

namespace VDCD.Models
{
    public class KienTaoViewModel
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public List<Center> Centers { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int? CurrentCategoryId { get; set; }
    }
}
