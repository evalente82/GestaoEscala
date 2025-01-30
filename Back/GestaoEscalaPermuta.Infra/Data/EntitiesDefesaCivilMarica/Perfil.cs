using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    public class Perfil
    {
        [Key]
        public Guid IdPerfil { get; set; }

        [StringLength(100)]
        public string Nome { get; set; } = null!;

        public string? Descricao { get; set; } // Descrição opcional do perfil

        // Relacionamentos com Perfis
        public ICollection<PerfisFuncionalidades>? PerfisFuncionalidades { get; set; }

        // Relacionamento com Cargos
        public ICollection<CargoPerfis> CargoPerfis { get; set; } = new List<CargoPerfis>();
    }
}
