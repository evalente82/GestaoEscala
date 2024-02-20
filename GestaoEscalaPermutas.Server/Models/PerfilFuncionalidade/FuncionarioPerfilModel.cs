using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class FuncionarioPerfilModel
    {
        public int IdFuncionario { get; set; }
        public int IdPerfil { get; set; }

        // Relacionamentos
        public Funcionario Funcionario { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
