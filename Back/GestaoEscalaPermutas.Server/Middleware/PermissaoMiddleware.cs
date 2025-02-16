

public class PermissaoMiddleware
{
    private readonly RequestDelegate _next;

    public PermissaoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var atributoPermissao = endpoint.Metadata.GetMetadata<PermissaoAttribute>();
            if (atributoPermissao != null)
            {
                var userClaims = context.User.Claims;
                var permissoesUsuario = userClaims.Where(c => c.Type == "Permissao").Select(c => c.Value).ToList();

                if (!permissoesUsuario.Contains(atributoPermissao.Nome))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Acesso negado: Você não tem permissão.");
                    return;
                }
            }
        }

        await _next(context);
    }
}