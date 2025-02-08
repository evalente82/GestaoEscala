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
        Task<LoginResponseDTO> Autenticar(LoginRequestDTO loginRequest);
        Task<LoginResponseDTO> Incluir(LoginDTO loginDTO);
        Task<LoginResponseDTO> GerarTokenRedefinicaoSenha(string email);
        Task<LoginResponseDTO> RedefinirSenha(RedefinirSenhaRequestDTO request);
    }
}
