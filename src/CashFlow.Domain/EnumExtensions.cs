using CashFlow.Domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace CashFlow.Domain;
public static class EnumExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => "Dinheiro",
            PaymentType.CreditCard => "Cartão de Crédito",
            PaymentType.DebitCard => "Cartão de Débito",
            PaymentType.EletronicTransfer => "Transferência Bancária",
            _ => throw new NotImplementedException()
        };
    }

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
