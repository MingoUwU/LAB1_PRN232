using System.Threading.Tasks;
using Shared.Models;

namespace IdentityService.Services
{
    public interface IAuthService
    {
        Task<TokenResponse?> LoginAsync(LoginRequest request);
        Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> RevokeTokenAsync(string token);
    }
}
