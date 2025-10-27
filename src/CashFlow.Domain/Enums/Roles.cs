using System.ComponentModel;

namespace CashFlow.Domain.Enums;
public enum Roles
{
    [Description("administrator")]
    ADMIN,
    [Description("teamMember")]
    TEAM_MEMBER,
}
