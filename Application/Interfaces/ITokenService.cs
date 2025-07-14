using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string email, string[] roles);
    
    }
}
