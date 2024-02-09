using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.DTO
{
    public class RetornoDTO
    {
        public RetornoDTO()
        {
            this.valido = false;
            this.mensagem = "";
        }
        public RetornoDTO(bool valido, string mensagem)
        {
            this.valido = valido;
            this.mensagem = mensagem;
        }
        [JsonIgnore]
        public bool valido { get; set; }
        [JsonIgnore]
        public string mensagem { get; set; }
    }
}
