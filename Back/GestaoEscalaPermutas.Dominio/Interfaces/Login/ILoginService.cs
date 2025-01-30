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
        /// <summary>
        /// Autentica um usuário e retorna um token JWT contendo suas permissões.
        /// </summary>
        Task<LoginResponseDTO> Autenticar(LoginRequestDTO loginRequest);

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        Task<LoginResponseDTO> Incluir(LoginDTO loginDTO);
    }
}
