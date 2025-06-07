using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.EscalaExtra;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaExtra;
using GestaoEscalaPermutas.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaExtra
{
    public class EscalaExtraService : IEscalaExtraService
    {
        private readonly IEscalaExtraRepository _EscalaExtraRepository;
        private readonly IMapper _mapper;
        public Task<List<EscalaExtraDTO>> BuscarPorId(Guid idEscalaExtra)
        {
            throw new NotImplementedException();
        }
        
        public Task<List<EscalaExtraDTO>> BuscarTodos()
        {
            var escalaExtra = await _EscalaExtraRepository.ObterTodosAsync();
            return _mapper.Map<List<EscalaExtraDTO>>(escalaExtra);
        }

        public Task<EscalaExtraDTO[]> IncluirLista(EscalaExtraDTO[] escalaExtraDTOs)
        {
            throw new NotImplementedException();
        }
    }
}
