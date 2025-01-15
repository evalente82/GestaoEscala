using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class PerfilModel
    {
        public Guid IdPerfil { get; set; }

        [StringLength(100)]
        public string NmNome { get; set; } = null!;

        // Relacionamentos
        public ICollection<FuncionarioPerfil>? FuncionariosPerfis { get; set; }
        public ICollection<PerfilFuncionalidadeModel>? PerfisFuncionalidades { get; set; }
    }
}
