using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Cache
{
    public static class CacheParam
    {
        /* ================== SETTING ================== */

        // Cache 1 setting theo key
        public const string SettingByKey = "$domain.setting.{0}";
        public const int SettingByKeyTimeout = 60;

        // Cache toàn bộ setting
        public const string SettingAllKey = "$domain.setting.all";
        public const int SettingAllTimeout = 60;

        // Cache toàn bộ file
        public const string FileAllKey = "$domain.file.all";
        public const int FileAllTimeout = 60;
        /* ================== DATA TABLE ================== */

        // Cache dữ liệu của 1 bảng bất kỳ
        public const string DataOfTableKey = "$domain.table.{0}";
        public const int DataOfTableTimeout = 30;


        /* ================== USER ================== */

        public const string UserById = "$domain.user.all";
        public const int UserByIdTimeout = 30;
        public const string CategoryAll = "$domain.category.all";
        public const int CategoryAllTimeout = 30;
        public const string ProjectAll = "$domain.center.all";
        public const int ProjectAllTimeout = 30;
        public const string CenterAll = "$domain.center.all";
        public const int CenterAllTimeout = 30;
        /* ================== COMMON ================== */

        public const int OneMinute = 1;
        public const int FiveMinutes = 5;
        public const int OneHour = 60;
        public const int OneDay = 1440;
    }
}
