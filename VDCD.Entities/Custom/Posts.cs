using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class Posts
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public string? Thumbnail {  get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public int? CategoryId { get; set; }
    }
}
