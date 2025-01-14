namespace GestaoEscalaPermutas.Server.Models.Departamento
{
    public class DepartamentoModel : RetornoModel
    {    
        public int IdDepartamento { get; set; }
        
        public string NmNome { get; set; } = null!;
        
        public string? NmDescricao { get; set; }
        
        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }
    }
}
