namespace GestaoEscalaPermutas.Dominio.DTO.Setor
{
    public class SetorDTO : RetornoDTO
    {
        public SetorDTO()
        {
            IdSetor = Guid.NewGuid();
        }

        public Guid IdSetor { get; set; }
        public string NmNome { get; set; } = null!;
        public string? NmDescricao { get; set; }
        public bool IsAtivo { get; set; }
    }
}
