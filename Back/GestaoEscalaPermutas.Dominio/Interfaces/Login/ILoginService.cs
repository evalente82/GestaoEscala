using GestaoEscalaPermutas.Dominio.DTO.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Interfaces.Login
{
    public interface ILoginService
    {
        Task<LoginDTO> Incluir(LoginDTO loginDTO);
        Task<LoginResponseDTO> Autenticar(LoginDTO loginRequest);
    }
}
