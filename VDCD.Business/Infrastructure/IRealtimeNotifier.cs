using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Business.Infrastructure
{
    // Business project
    public interface IRealtimeNotifier
    {
        Task Notify(string eventName, object data);
    }

}
