using System.Security.Claims;

namespace GestaoEscalaPermutas.Server.Helper
{
    public class AutenticacaoHelper
    {
        public static int getIdByToken(ClaimsPrincipal user)
        {
            var id = 0;
            int.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty, out id);
            return id;
        }
    }
}
