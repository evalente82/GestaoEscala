using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class FuncionalidadeModel: RetornoModel
    {
        
        public int IdFuncionalidade { get; set; }

        public string NmNome { get; set; } = null!;

        public string NmDescricao { get; set; } = null!;

        // Relacionamentos
        public ICollection<PerfilFuncionalidadeModel>? PerfisFuncionalidades { get; set; }
    }
}
