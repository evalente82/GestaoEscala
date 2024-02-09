using AutoMapper;
using GestaoEscalaPermuta.Infra.Data.Context;
using DepInfra = GestaoEscalaPermuta.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GestaoEscalaPermutas.Dominio.Services.Departamento
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        //private readonly IDepartamentoService _DepartamentoService;
        public DepartamentoService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DepartamentoDTO> Incluir(DepartamentoDTO departamentoDTO)
        {
            try
            {
                if (departamentoDTO is null)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {                   
                    var departamento = _mapper.Map<DepInfra.Departamento>(departamentoDTO);

                    _context.Departamentos.Add(departamento);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<DepartamentoDTO>(departamento);
                }
            }
            catch (Exception e)
            {
                return new DepartamentoDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
    }
}
