using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using System.ComponentModel.DataAnnotations;

namespace GestaoEscalaPermutas.Server.Models.PerfilFuncionalidade
{
    public class PerfilFuncionalidadeModel: RetornoModel
    {       
        public int IdPerfil { get; set; }
        
        public int IdFuncionalidade { get; set; }

        // Relacionamentos
        public Perfil Perfil { get; set; } = null!;
        public Funcionalidade Funcionalidade { get; set; } = null!;
    }
}
