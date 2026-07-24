using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class GeneratePostRequest
    {
        public string Topic { get; set; }

        // New: list of attachment URLs / identifiers coming from the client
        public List<string>? FbAttachmentsList { get; set; }
    }
}
