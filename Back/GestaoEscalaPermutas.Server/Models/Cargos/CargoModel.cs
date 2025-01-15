namespace GestaoEscalaPermutas.Server.Models.Cargos
{
    public class CargoModel : RetornoModel
    {
        public Guid IdCargos { get; set; }

        public string NmNome { get; set; } = null!;

        public string? NmDescricao { get; set; }

        public bool IsAtivo { get; set; }

        public DateTime DtCriacao { get; set; }
    }
}
