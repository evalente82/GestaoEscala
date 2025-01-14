using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Dominio.DTO.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Infra.Data.Context;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using Microsoft.EntityFrameworkCore;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.Funcionario
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly DefesaCivilMaricaContext _context;
        private readonly IMapper _mapper;
        public FuncionarioService(DefesaCivilMaricaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<FuncionarioDTO> Incluir(FuncionarioDTO funcionarioDTO)
        {
            try
            {
                if (funcionarioDTO is null)
                {
                    return new FuncionarioDTO { valido = false, mensagem = "Objeto não preenchido." };
                }
                else
                {
                    var funcionario = _mapper.Map<DepInfra.Funcionario>(funcionarioDTO);

                    _context.Funcionarios.Add(funcionario);
                    await _context.SaveChangesAsync();
                    return _mapper.Map<FuncionarioDTO>(funcionario);
                }
            }
            catch (Exception e)
            {
                return new FuncionarioDTO { valido = false, mensagem = $"Erro ao receber o Objeto: {e.Message}" };
            }
        }
        public async Task<FuncionarioDTO> Alterar(int id, FuncionarioDTO funcionarioDTO)
        {
            try
            {
                if (id <= 0)
                {
                    return new FuncionarioDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var funcionarioExistente = await _context.Funcionarios.FindAsync(id);
                    if (funcionarioExistente == null)
                    {
                        return new FuncionarioDTO { valido = false, mensagem = "Funcionario não encontrado." };
                    }

                    // Mapeia os dados do DTO para o modelo existente (apenas as propriedades que você deseja atualizar)
                    _mapper.Map(funcionarioDTO, funcionarioExistente);

                    // O EF Core rastreará que o objeto foi modificado
                    _context.Funcionarios.Update(funcionarioExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    // Retorna o DTO atualizado (opcionalmente, você pode mapear de volta se quiser devolver os dados atualizados)
                    return _mapper.Map<FuncionarioDTO>(funcionarioExistente);
                }
            }
            catch (Exception e)
            {
                // Considerar usar um logger para registrar a exceção
                throw new Exception($"Erro ao alterar o objeto: {e.Message}");
            }
        }
        public async Task<List<FuncionarioDTO>> BuscarTodos()
        {
            try
            {
                var funcionarios = await _context.Funcionarios.ToListAsync();
                var funcionarioDTO = _mapper.Map<List<FuncionarioDTO>>(funcionarios);
                return funcionarioDTO;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<FuncionarioDTO> Deletar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new FuncionarioDTO { valido = false, mensagem = "Id fora do Range." };
                }
                else
                {
                    var funcionarioExistente = await _context.Funcionarios.FindAsync(id);
                    if (funcionarioExistente == null)
                    {
                        return new FuncionarioDTO { valido = false, mensagem = "Funacionário não encontrado." };
                    }


                    // O EF Core rastreará que o objeto foi modificado
                    _context.Funcionarios.Remove(funcionarioExistente);

                    // Salva as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    //retornar avido de deletado
                    return new FuncionarioDTO { valido = true, mensagem = "Funcionário deletado com sucesso." };
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
        public async Task<FuncionarioDTO[]> IncluirLista(FuncionarioDTO[] funcionarioDTOs)
        {
            try
            {
                if (funcionarioDTOs is null)
                {
                    return new FuncionarioDTO[] { 
                        new FuncionarioDTO { 
                            valido = false, mensagem = "Lista de funcionários vazia."
                        }
                    };
                }
                else
                {
                    var funcionarios = _mapper.Map<DepInfra.Funcionario[]>(funcionarioDTOs);

                    _context.Funcionarios.AddRange(funcionarios);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<FuncionarioDTO[]>(funcionarios);
                }
            }
            catch (Exception e)
            {
                return new FuncionarioDTO[] { new FuncionarioDTO { valido = false, mensagem = $"Erro ao incluir a lista de funcionários: {e.Message}" } };
            }
        }
        public async Task<List<FuncionarioDTO>> BuscarTodosAtivos()
        {
            try
            {
                var funcionarios = await _context.Funcionarios.Where(p => p.IsAtivo).ToListAsync();
                var funcionariosAtivos = _mapper.Map<List<FuncionarioDTO>>(funcionarios);
                return funcionariosAtivos;
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao receber o Objeto: {e.Message}");
            }
        }
    }
}
