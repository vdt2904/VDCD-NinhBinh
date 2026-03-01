using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class AiPostService : IAiPostService
    {
        private readonly IAiService _ai;
        private readonly IRepository<FbPost> _repo;
        private readonly AppDbContext _db;

        public AiPostService(IAiService ai, IRepository<FbPost> repo, AppDbContext db)
        {
            _ai = ai;
            _repo = repo;
            _db = db;
        }

        public async Task<FbPost> GenerateAndSave(string topic)
        {
            var content = await _ai.GeneratePost(topic);

            var post = new FbPost
            {
                Topic = topic,
                Content = content,
                Status = "Draft"
            };

            _repo.Create(post);
            await _db.SaveChangesAsync();

            return post;
        }
    }
}
