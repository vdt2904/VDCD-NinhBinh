using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Entities.Custom;

namespace VDCD.Business.Infrastructure
{
    public interface IAiPostService
    {
        Task<FbPost> GenerateAndSave(string topic);
    }
}
