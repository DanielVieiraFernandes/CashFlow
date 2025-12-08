using CashFlow.Domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace CashFlow.Domain;
public static class EnumExtensions
{
    public static string GetEnumDescription<T>(this T enumValue) where T : Enum
    {
        var enumFieldName = enumValue.ToString();
        var field = typeof(T).GetField(enumFieldName);
        if (field is null)
        {
            return enumFieldName;
        }

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute is null ? enumFieldName : attribute.Description;
    }
}
