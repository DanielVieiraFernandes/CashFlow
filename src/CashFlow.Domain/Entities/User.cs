using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Destinado ao JWT
    /// </summary>
    public Guid UserIdentifier { get; set; }

    /// <summary>
    /// Para fazer o controle por RBAC (Role-Based Access Control)
    /// </summary>
    public Roles Role { get; set; } = Roles.TEAM_MEMBER;

}
