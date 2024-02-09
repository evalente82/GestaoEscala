using System.ComponentModel;

namespace GestaoEscalaPermutas.Server.Models
{
    public class RetornoModel
    {
        public bool Valido { get; set; }

        [DefaultValue("Mensagem de retorno")]
        public string Mensagem { get; set; }
    }
}
