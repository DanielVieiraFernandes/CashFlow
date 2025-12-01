using CashFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Services.LoggedUser;

public interface ILoggedUser
{
    Task<User> Get();
}
