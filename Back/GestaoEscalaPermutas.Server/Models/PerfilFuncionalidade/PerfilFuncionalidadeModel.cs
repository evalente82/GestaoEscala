
namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class PerfilFuncionalidadesModel : RetornoModel
    {
        public Guid IdPerfil { get; set; }
        public Guid IdFuncionalidade { get; set; }

        public string NomePerfil { get; set; } = string.Empty; // Nome do perfil associado
        public string NomeFuncionalidade { get; set; } = string.Empty; // Nome da funcionalidade associada
    }
}
