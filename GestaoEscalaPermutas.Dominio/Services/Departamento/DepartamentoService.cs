using AutoMapper;
using GestaoEscalaPermutas.Infra.Data.Context;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using GestaoEscalaPermutas.Dominio.Interfaces.Departamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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
       
        public async Task<DepartamentoDTO> Alterar(int id, DepartamentoDTO departamentoModel)
        {
            try
            {
                if (id <= 0)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var departamentoExistente = await _context.Departamentos.FindAsync(id);
                    if (departamentoExistente == null)
                    {
                        return new DepartamentoDTO { valido = false, mensagem = "Departamento não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(departamentoModel, departamentoExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Departamentos.Update(departamentoExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<DepartamentoDTO>(departamentoExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }


        public async Task<List<DepartamentoDTO>> BuscarTodos()
        {
            try
            {
                var departamentos = await _context.Departamentos.ToListAsync();
                var departamentosDTO = _mapper.Map<List<DepartamentoDTO>>(departamentos);
                return departamentosDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }

        public async Task<DepartamentoDTO> Deletar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new DepartamentoDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var departamentoExistente = await _context.Departamentos.FindAsync(id);
                    if (departamentoExistente == null)
                    {
                        return new DepartamentoDTO { valido = false, mensagem = "Departamento não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.Departamentos.Remove(departamentoExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new DepartamentoDTO { valido = true, mensagem = "Departamento deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
    }
}
