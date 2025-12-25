using System.ComponentModel;

namespace CashFlow.Domain.Enums;

public enum Tag
{
    [Description("Saúde")]
    Health = 0,

    [Description("Essencial")]
    Essential = 1,

    [Description("Variável")]
    Variable = 2,

    [Description("Fixo")]
    Fixed = 3,

    [Description("Pessoal")]
    Personal = 4,

    [Description("Emergência")]
    Emergency = 5,

    [Description("Investimento")]
    Investment = 6,

    [Description("Lazer")]
    Leisure = 7,

    [Description("Educação")]
    Education = 8,

    [Description("Transporte")]
    Transportation = 9,
}
