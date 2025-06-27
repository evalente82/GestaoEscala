using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.ENUM
{
    public enum StatusInscricaoEnum
    {
        Confirmado = 1, // Conseguiu cadstrar a vaga do serviço extra
        FilaDeEspera = 2, // Conseguiu cadastrar para a fila de espera
        Cancelado = 3, // Usuario Cancelou a inscrição da vaga do extra
        Efetivado = 4, // Usuario trabalhou no dia que agendou
        Ausente = 5, // Usuario faltou ao serviço extra que foi agendado
        Atestado = 6 // Usuario apresentou atestado medico no dia do extra
    }
}
