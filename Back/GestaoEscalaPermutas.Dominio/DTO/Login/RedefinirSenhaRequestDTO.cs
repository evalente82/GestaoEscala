namespace GestaoEscalaPermutas.Dominio.DTO.Login
{
    public class RedefinirSenhaRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}
