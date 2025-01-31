using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.Setor
{
    public class SetorModel : RetornoModel
    {
        public Guid IdSetor { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmDescricao { get; set; }

        public bool IsAtivo { get; set; }

        //public ICollection<PostoTrabalho> PostosTrabalho { get; set; } = new List<PostoTrabalho>();
    }
}
