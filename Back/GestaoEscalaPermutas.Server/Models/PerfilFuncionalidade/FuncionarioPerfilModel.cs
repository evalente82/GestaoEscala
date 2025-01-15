using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class FuncionarioPerfilModel
    {
        public Guid IdFuncionario { get; set; }
        public Guid IdPerfil { get; set; }

        // Relacionamentos
        public Funcionario Funcionario { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
