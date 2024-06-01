using Dexma_cpt_ServerSide.Services.Auth;

namespace Dexma_cpt_ServerSide.Services
{
    public class UserManager
    {
        public int GetUserId(string token)
        {
            JwtManager jwtManager = new();

            return jwtManager.GetUserIdFromToken(token);
        }

        public bool ValidateToken(string token)
        {
            JwtManager jwtManager = new();

            return jwtManager.ValidateJwtToken(token);
        }
    }
}
