using System.ComponentModel;
using System.Reflection;

namespace VDCD.Helper
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            var field = value
                .GetType()
                .GetField(value.ToString());

            var attr =
                field?.GetCustomAttribute<DescriptionAttribute>();

            return attr?.Description ?? value.ToString();
        }

        // Search enum with Description
        public static TEnum? FromDescription<TEnum>(string text)
            where TEnum : struct, Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attr =
                    field.GetCustomAttribute<DescriptionAttribute>();

                if (attr != null &&
                    attr.Description == text)
                    return (TEnum)field.GetValue(null);

                if (field.Name == text)
                    return (TEnum)field.GetValue(null);
            }

            return null;
        }
    }
}