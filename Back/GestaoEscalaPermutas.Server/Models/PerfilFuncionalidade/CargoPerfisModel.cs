using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class CargoPerfisModel
    {
        public Guid IdFuncionario { get; set; }
        public Guid IdPerfil { get; set; }

        // Relacionamentos
        public Cargo Funcionario { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
