using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.Departamento
{
    public class DepartamentoDTO : RetornoDTO
    {
        public DepartamentoDTO()
        {
            IdDepartamento = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        }
        
        public Guid IdDepartamento { get; set; }
        public string NmNome { get; set; } = null!;
        public string NmDescricao { get; set; } = null!;
        public bool IsAtivo { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; }

    }
}
