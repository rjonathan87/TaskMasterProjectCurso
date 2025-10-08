using System.Security.Claims;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, IList<string> roles);
    }
}