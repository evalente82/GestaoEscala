using AutoMapper;
using GestaoEscalaPermutas.Dominio.DTO;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Repository.Interfaces;
using DepInfra = GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;

namespace GestaoEscalaPermutas.Dominio.Services.EscalaPronta
{
    public class EscalaProntaService : IEscalaProntaService
    {
        private readonly IEscalaProntaRepository _escalaProntaRepository;
        private readonly IMapper _mapper;

        public EscalaProntaService(IEscalaProntaRepository escalaProntaRepository, IMapper mapper)
        {
            _escalaProntaRepository = escalaProntaRepository;
            _mapper = mapper;
        }

        public async Task<EscalaProntaDTO> Incluir(EscalaProntaDTO escalaProntaDTO)
        {
            if (escalaProntaDTO is null)
                return new EscalaProntaDTO { valido = false, mensagem = "Objeto não preenchido." };

            var escalaPronta = _mapper.Map<DepInfra.EscalaPronta>(escalaProntaDTO);
            var novaEscalaPronta = await _escalaProntaRepository.AdicionarAsync(escalaPronta);
            return _mapper.Map<EscalaProntaDTO>(novaEscalaPronta);
        }

        public async Task<EscalaProntaDTO> Alterar(Guid id, EscalaProntaDTO escalaProntaDTO)
        {
            if (id == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };

            var escalaProntaExistente = await _escalaProntaRepository.ObterPorIdAsync(id);
            if (escalaProntaExistente == null)
                return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };

            _mapper.Map(escalaProntaDTO, escalaProntaExistente);
            await _escalaProntaRepository.AtualizarAsync(escalaProntaExistente);
            return _mapper.Map<EscalaProntaDTO>(escalaProntaExistente);
        }

        public async Task<List<EscalaProntaDTO>> BuscarTodos()
        {
            var escalasProntas = await _escalaProntaRepository.ObterTodosAsync();
            return _mapper.Map<List<EscalaProntaDTO>>(escalasProntas);
        }

        public async Task<List<EscalaProntaDTO>> BuscarPorId(Guid idEscalaPronta)
        {
            if (idEscalaPronta == Guid.Empty)
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." } };

            var escalasProntas = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscalaPronta);
            if (!escalasProntas.Any())
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Escala Não encontrada." } };

            return _mapper.Map<List<EscalaProntaDTO>>(escalasProntas);
        }

        public async Task<EscalaProntaDTO> Deletar(Guid id)
        {
            if (id == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id fora do Range." };

            await _escalaProntaRepository.RemoverAsync(id);
            return new EscalaProntaDTO { valido = true, mensagem = "Escala deletada com sucesso." };
        }

        public async Task<EscalaProntaDTO[]> IncluirLista(EscalaProntaDTO[] escalaProntaDTOs)
        {
            if (escalaProntaDTOs is null || escalaProntaDTOs.Length == 0)
                return new EscalaProntaDTO[] { new() { valido = false, mensagem = "Lista de escala vazia." } };

            var escalasProntas = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);
            var novasEscalas = await _escalaProntaRepository.AdicionarListaAsync(escalasProntas);
            return _mapper.Map<EscalaProntaDTO[]>(novasEscalas);
        }

        public async Task<EscalaProntaDTO[]> AlterarEscalaPronta(Guid idEscala, EscalaProntaDTO[] escalaProntaDTOs)
        {
            if (escalaProntaDTOs == null || escalaProntaDTOs.Length == 0)
                return new EscalaProntaDTO[] { new() { valido = false, mensagem = "Lista de Escala vazia." } };

            await _escalaProntaRepository.RemoverListaPorEscalaAsync(idEscala);
            var novasEscalas = _mapper.Map<DepInfra.EscalaPronta[]>(escalaProntaDTOs);
            await _escalaProntaRepository.AdicionarListaAsync(novasEscalas);
            return _mapper.Map<EscalaProntaDTO[]>(novasEscalas);
        }

        public async Task<RetornoDTO> RecriarEscalaProximoMes(Guid idEscala)
        {
            if (idEscala == Guid.Empty)
                return new RetornoDTO { valido = false, mensagem = "Id fora do Range." };

            var escalasProntas = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscala);
            if (!escalasProntas.Any())
                return new RetornoDTO { valido = false, mensagem = "Nenhuma escala encontrada para recriação." };

            var novasEscalas = escalasProntas
                .Select(e => new DepInfra.EscalaPronta
                {
                    IdEscalaPronta = Guid.NewGuid(),
                    IdEscala = idEscala,
                    IdPostoTrabalho = e.IdPostoTrabalho,
                    IdFuncionario = e.IdFuncionario,
                    DtDataServico = e.DtDataServico.AddMonths(1),
                    DtCriacao = DateTime.UtcNow
                }).ToList();

            await _escalaProntaRepository.AdicionarListaAsync(novasEscalas.ToArray());
            return new RetornoDTO { valido = true, mensagem = "Escala recriada com sucesso para o próximo mês." };
        }

        public async Task<EscalaProntaDTO> DeletarOcorrenciaFuncionario(Guid idFuncionario, Guid idEscala)
        {
            if (idFuncionario == Guid.Empty || idEscala == Guid.Empty)
                return new EscalaProntaDTO { valido = false, mensagem = "Id inválido!" };

            var ocorrencias = await _escalaProntaRepository.ObterPorEscalaIdAsync(idEscala);
            ocorrencias = ocorrencias.Where(ep => ep.IdFuncionario == idFuncionario).ToList();

            if (!ocorrencias.Any())
                return new EscalaProntaDTO { valido = false, mensagem = "Nenhuma ocorrência encontrada para o funcionário." };

            foreach (var ocorrencia in ocorrencias)
            {
                ocorrencia.IdFuncionario = Guid.Empty;
                await _escalaProntaRepository.AtualizarAsync(ocorrencia);
            }

            return new EscalaProntaDTO { valido = true, mensagem = "Ocorrências do funcionário atualizadas com sucesso." };
        }

        public async Task<EscalaProntaDTO> IncluirFuncionarioEcala(EscalaProntaDTO incluiFuncEscalaProntaDTO)
        {
            if (incluiFuncEscalaProntaDTO is null)
                return new EscalaProntaDTO { valido = false, mensagem = "Dados não preenchidos." };

            var escalas = await _escalaProntaRepository.ObterPorEscalaIdAsync(incluiFuncEscalaProntaDTO.IdEscala);
            if (escalas == null || !escalas.Any())
                return new EscalaProntaDTO { valido = false, mensagem = "Escala não encontrada." };

            var funcionarioJaExiste = escalas.Any(x => x.IdFuncionario == incluiFuncEscalaProntaDTO.IdFuncionario);
            if (funcionarioJaExiste)
                return new EscalaProntaDTO { valido = false, mensagem = "Funcionário já existe na Escala." };

            var funcionarioSemVaga = escalas
                .Where(e => e.IdPostoTrabalho == incluiFuncEscalaProntaDTO.IdPostoTrabalho)
                .FirstOrDefault(e => e.IdFuncionario == Guid.Empty);

            if (funcionarioSemVaga != null)
            {
                funcionarioSemVaga.IdFuncionario = incluiFuncEscalaProntaDTO.IdFuncionario;
                await _escalaProntaRepository.AtualizarAsync(funcionarioSemVaga);
                return new EscalaProntaDTO { valido = true, mensagem = "Funcionário incluído na escala com sucesso!" };
            }

            return new EscalaProntaDTO { valido = false, mensagem = "Nenhuma vaga disponível na escala." };
        }

        public async Task<List<EscalaProntaDTO>> BuscarPorIdFuncionario(Guid idFuncionario)
        {
            if (idFuncionario == Guid.Empty)
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "IdFuncionario inválido." } };

            var escalasProntas = await _escalaProntaRepository.BuscarPorIdFuncionario(idFuncionario);

            if (!escalasProntas.Any())
                return new List<EscalaProntaDTO> { new EscalaProntaDTO { valido = false, mensagem = "Nenhum dado encontrado para o funcionário." } };

            return escalasProntas.Select(ep => new EscalaProntaDTO
            {
                DtDataServico = ep.DtDataServico,
                IdEscala = ep.IdEscala,
                NmNomeEscala = ep.Escala?.NmNomeEscala ?? "Sem Nome" // Evita erro caso Escala seja null
            }).ToList();
        }

        public Task<EscalaProntaDTO> IncluirFuncionarioEscala(EscalaProntaDTO escalaProntaDTO)
        {
            throw new NotImplementedException();
        }
    }
}