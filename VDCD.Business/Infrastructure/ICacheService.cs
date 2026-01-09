using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Business.Infrastructure
{
    public interface ICacheService
    {
        bool TryGet<T>(string key, out T value);
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
        void Remove(string key);
        void ClearAll();
    }

}
