using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica
{
    [Table("Setor")]
    public class Setor
    {
        [Key]
        public Guid IdSetor { get; set; }

        [StringLength(200)]
        [Unicode(false)]
        public string NmNome { get; set; } = null!;

        [StringLength(200)]
        [Unicode(false)]
        public string? NmDescricao { get; set; }

        public bool IsAtivo { get; set; }

        //public ICollection<PostoTrabalho> PostosTrabalho { get; set; } = new List<PostoTrabalho>();
    }
}
