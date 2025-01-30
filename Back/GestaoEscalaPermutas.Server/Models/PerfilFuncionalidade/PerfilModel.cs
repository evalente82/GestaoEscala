using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class PerfilModel : RetornoModel
    {
        public Guid IdPerfil { get; set; }

        [StringLength(100)]
        public string Nome { get; set; } = null!;

        public string? Descricao { get; set; }

        // Relacionamentos
        public ICollection<PerfilFuncionalidadesModel>? PerfisFuncionalidades { get; set; }
    }
}
