using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class CargoPerfisModel: RetornoModel
    {
        public Guid IdCargo { get; set; }
        public Guid IdPerfil { get; set; }
        public Cargo Cargo { get; set; } = null!;
        public Perfil Perfil { get; set; } = null!;
    }
}
