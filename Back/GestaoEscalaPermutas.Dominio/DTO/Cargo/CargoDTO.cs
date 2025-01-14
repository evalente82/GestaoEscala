using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.Cargo
{
    public class CargoDTO : RetornoDTO
    {
        public CargoDTO() => DtCriacao = DateTime.Now;

        public int IdCargos { get; set; }
        public string NmNome { get; set; } = null!;
        public string NmDescricao { get; set; } = null!;
        public bool IsAtivo { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; }

    }
}
