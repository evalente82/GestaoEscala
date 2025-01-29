using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class FuncionalidadeModel: RetornoModel
    {
        public Guid IdFuncionalidade { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; }
    }
}
