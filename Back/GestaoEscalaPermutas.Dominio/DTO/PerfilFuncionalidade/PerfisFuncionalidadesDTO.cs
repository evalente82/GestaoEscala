using System.Text.Json.Serialization;

namespace GestaoEscalaPermutas.Dominio.DTO.PerfilFuncionalidade
{
    public class PerfisFuncionalidadesDTO
    {
        public Guid IdPerfil { get; set; }
        
        public string NomePerfil { get; set; } = null!;
        public Guid IdFuncionalidade { get; set; }
        
        public string NomeFuncionalidade { get; set; } = null!;
    }
}
