
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PermissaoAttribute : Attribute, IAuthorizationFilter
{
    public string Nome { get; } // Propriedade pública que pode ser acessada pelo middleware

    public PermissaoAttribute(string permissao)
    {
        Nome = permissao; // Atribui o valor à propriedade pública
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userClaims = context.HttpContext.User.Claims;
        var permissoesUsuario = userClaims.Where(c => c.Type == "Permissao").Select(c => c.Value).ToList();

        if (!permissoesUsuario.Contains(Nome))
        {
            context.Result = new ForbidResult();
        }
    }
}
