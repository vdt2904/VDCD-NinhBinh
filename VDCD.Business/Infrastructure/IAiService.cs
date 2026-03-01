using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Business.Infrastructure
{
    public interface IAiService
    {
        Task<string> GeneratePost(string topic);
    }
}
