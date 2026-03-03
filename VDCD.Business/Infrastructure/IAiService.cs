using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Business.Infrastructure
{
    public interface IAiService
    {
        // Accept optional list of attachment URLs/paths for reference
        Task<string> GeneratePost(string topic, List<string>? fbAttachmentsList = null);
    }
}
