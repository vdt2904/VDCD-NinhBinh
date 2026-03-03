using System.Collections.Generic;
using System.Threading.Tasks;
using VDCD.Entities.Custom;

namespace VDCD.Business.Infrastructure
{
    public interface IAiPostService
    {
        // Updated to accept attachments so caller can persist them together with generated content
        Task<FbPost> GenerateAndSave(string topic, List<string>? fbAttachmentsList = null);
    }
}
