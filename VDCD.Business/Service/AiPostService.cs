using System;
using System.Collections.Generic;
using System.Text.Json;
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

        // Accept attachments and persist them as serialized JSON in FbPost.Files
        public async Task<FbPost> GenerateAndSave(string topic, List<string>? fbAttachmentsList = null)
        {
            var content = await _ai.GeneratePost(topic, fbAttachmentsList);

            var post = new FbPost
            {
                Topic = topic,
                Content = content,
                Status = "Draft",
                Files = fbAttachmentsList is null ? null : JsonSerializer.Serialize(fbAttachmentsList)
            };

            _repo.Create(post);
            await _db.SaveChangesAsync();

            return post;
        }
    }
}
