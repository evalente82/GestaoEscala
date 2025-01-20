using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.Cargo
{
    public class CargoDTO : RetornoDTO
    {
        public CargoDTO() 
        {
            IdCargo = Guid.NewGuid();
            DtCriacao = DateTime.UtcNow;
        } 

        public Guid IdCargo { get; set; }
        public string NmNome { get; set; } = null!;
        public string NmDescricao { get; set; } = null!;
        public bool IsAtivo { get; set; }
        [JsonIgnore]
        public DateTime DtCriacao { get; set; }

    }
}
