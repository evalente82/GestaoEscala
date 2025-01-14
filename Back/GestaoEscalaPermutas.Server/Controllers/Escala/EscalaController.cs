using AutoMapper;
using GestaoEscalaPermutas.Dominio.Interfaces.Escala;
using GestaoEscalaPermutas.Server.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoEscalaPermutas.Dominio.DTO.Escala;
using GestaoEscalaPermutas.Server.Models.Escala;
using GestaoEscalaPermutas.Dominio.Interfaces.PostoTrabalho;
using GestaoEscalaPermutas.Dominio.Interfaces.Funcionarios;
using GestaoEscalaPermutas.Dominio.Interfaces.TipoEscala;
using GestaoEscalaPermutas.Dominio.DTO.EscalaPronta;
using GestaoEscalaPermutas.Dominio.Interfaces.EscalaPronta;
using GestaoEscalaPermutas.Server.Models.Funcionarios;
using GestaoEscalaPermutas.Dominio.DTO.Funcionario;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Server.Models.EscalaPronta;

namespace GestaoEscalaPermutas.Server.Controllers.Escala
{
    [ApiController]
    [Route("escala")]
    public class EscalaController:ControllerBase
    {
        private readonly IEscalaService _escalaService;
        private readonly IEscalaProntaService _escalaProntaService;
        private readonly IPostoTrabalhoService _postoTrabalhoService;
        private readonly IFuncionarioService _funcionarioService;
        private readonly ITipoEscalaService _tipoEscalaService;
        private readonly IMapper _mapper;

        public EscalaController(IEscalaService escalaService, IEscalaProntaService escalaProntaService, IMapper mapper, IPostoTrabalhoService postoTrabalhoService, IFuncionarioService funcionarioService, ITipoEscalaService tipoEscalaService) 
        {
            _escalaService = escalaService;
            _escalaProntaService = escalaProntaService;
            _mapper = mapper;
            _postoTrabalhoService = postoTrabalhoService;
            _funcionarioService = funcionarioService;
            _tipoEscalaService = tipoEscalaService;
        }
        [HttpPost]
        [Route("Incluir/")]
        public async Task<ActionResult> IncluirEscala([FromBody] EscalaDTO escala)
        {
            var EscalaDTO = await _escalaService.Incluir(_mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);

            return (escalaModel.Valido) ? Ok(escalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaModel.Mensagem });
        }

        [HttpPatch]
        [Route("Atualizar/{id:int}")]
        public async Task<ActionResult> AtualizarTipoEscala(int id, [FromBody] EscalaDTO escala)
        {
            escala.IdEscala = id;
            var EscalaDTO = await _escalaService.Alterar(id, _mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);
            return (escalaModel.Valido) ? Ok(escalaModel) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalaModel.Mensagem });
        }

        [HttpGet]
        [Route("buscarTodos")]
        public async Task<ActionResult> BuscarEscalas()
        {
            var escalas = await _escalaService.BuscarTodos();

            foreach (var escala in escalas)
            {
                if (!escala.valido)
                {
                    return BadRequest(new RetornoModel { Valido = false, Mensagem = escala.mensagem });
                }
            }
            return Ok(escalas);
        }

        [HttpDelete]
        [Route("Deletar/{id:int}")]
        public async Task<ActionResult> DeletarEscala(int id)
        {
            var escalasDTO = await _escalaService.Deletar(id);
            var escalasModel = _mapper.Map<EscalaModel>(escalasDTO);
            return (escalasModel.Valido) ? Ok(escalasModel.Mensagem) : BadRequest(new RetornoModel { Valido = false, Mensagem = escalasModel.Mensagem });
        }

        [HttpPost]
        [Route("montarEscala/")]
        public async Task<ActionResult> MontarEscala([FromBody] int idEscala)
        {
            //busca a escala selecionada
            var escala = await _escalaService.BuscarPorId(idEscala);

            if (escala.IsGerada)
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = $" A Escala {escala.NmNomeEscala} já foi gerada !" });
            }

            //buscar lista de funcionarios ativos
            var listFuncionarios = await _funcionarioService.BuscarTodosAtivos();

            //buscar lista de postos
            var listPostos = await _postoTrabalhoService.BuscarTodosAtivos();

            //buscar o tipo de escala
            var tipoEscala = await _tipoEscalaService.BuscarPorId(escala.IdTipoEscala);

            //fazer as validações para montar a escala

            //variaveis
            var anoAtual = DateTime.Now.Year;
            var mesAtual = DateTime.Now.Month;
            var horasDoDia = 24;
            var ht = tipoEscala.NrHorasTrabalhada;  //horas trabalhada(HT) //HT = 12h
            var hf = tipoEscala.NrHorasFolga; //horas folgadas(HF) //HF = 60h             
            var mesReferencia = escala.NrMesReferencia; //mesReferencia = 5
            var pessoaPorPosto = escala.NrPessoaPorPosto; //pessoa por posto(ppp) //PPP = 2
            var qtdFuncionario = listFuncionarios.Count(); //qtdFuncionarios = 20
            var qtdPostos = listPostos.Count(); //qtdPostos = 12
            var pessoasNecessarias = qtdPostos * pessoaPorPosto; //pessoasNecessarias = qtdPostos * ppp //pessoasNecessarias = 24
            int qtdDias = DateTime.DaysInMonth(anoAtual, mesReferencia); //maio mes 5 qtdDias = 31
            var alasPorPosto = (ht + hf) / horasDoDia; //HT + HF = 72 dividido por horasDoDIa = 24
            int ppp_X_TipoEscala = alasPorPosto * pessoaPorPosto; // TipoEscala é qto de alas para cobrir o posto
            int escalaPosto = ppp_X_TipoEscala * listPostos.Count;
            int resultado = escalaPosto - listFuncionarios.Count;


            Console.WriteLine("-----------XXX------------");
            

            Console.WriteLine("-----------XXX------------");

            Console.WriteLine($"Qtd de postos: {listPostos.Count}");
            Console.WriteLine($"Qtd de Funcionários: {listFuncionarios.Count}");
            Console.WriteLine($"Tipo da escala: {tipoEscala.NmNome}");

            Console.WriteLine($"Qtd de Pessoas por dia em cada posto:  {pessoaPorPosto}"); //2, 3

            Console.WriteLine("Qtd de pessoas para cobrir um posto: " + ppp_X_TipoEscala);//4, 6

            Console.WriteLine("Qtd de pessoas necessárias: " + escalaPosto);

            Console.WriteLine(resultado <= 0 ? $"Está SOBRANDO: {resultado} na escala!" : $"Está FALTANDO: {resultado} para completar a escala!");

            Console.WriteLine("-----------XXX------------");
            Console.WriteLine("-----------XXX------------");

            int count = listFuncionarios.Count;

            List<string> funcList = new List<string>();
            List<string> removeFuncList = new List<string>();
            foreach (var item in listFuncionarios)
            {
                funcList.Add(item.NmNome);
                removeFuncList.Add(item.NmNome);
            }

            List<EscalaProntaDTO> listEscalaPronta = new List<EscalaProntaDTO>();


            int countfunc = funcList.Count;

            foreach (var item in listPostos)//percorre a lista de postos
            {
                Console.WriteLine($"Posto:{item.NmNome}");


                int countTpEscala = 0; // contador de qtas pessoas cobrem um posto
                // Loop para percorrer cada dia do mês
                for (int dia = 1; dia <= qtdDias; dia++)
                {
                    // Loop para percorrer a qtd de pessoa por posto
                    for (int i = 0; i < pessoaPorPosto; i++)
                    {
                        EscalaProntaDTO escalaPronta = new EscalaProntaDTO(); // obj que vai receber a escala pronta

                        string nomeFunc = "";
                        int ano = DateTime.Now.Year;
                        string dataStr = $"{dia}-{mesReferencia}-{ano}";

                        if (countTpEscala == ppp_X_TipoEscala) // verifico se chegou a qtd de pessoas
                                                               // para cobrir o posto e zero
                        {
                            countTpEscala = 0;
                        }

                        if (i == 0) // verifico se é a primeira vez que passa no dia
                        {
                            Console.WriteLine($"DATA: {dataStr}");
                        }

                        if (funcList.Count != 0) //verifico se a lista de funcionaros esta vazia
                        {
                            if (i + countTpEscala >= 0 && i + countTpEscala < funcList.Count && funcList[countTpEscala] != null)
                            {
                                Console.WriteLine($"Funcionario: {funcList[countTpEscala]}");
                                escalaPronta.IdFuncionario = listFuncionarios.FirstOrDefault(x => x.NmNome == funcList[countTpEscala]).IdFuncionario;
                                nomeFunc = listFuncionarios.FirstOrDefault(x => x.NmNome == funcList[i]).NmNome;
                                countTpEscala++;
                            }
                            else
                            {
                                Console.WriteLine($"Sem Funcionario");
                                countTpEscala++;
                            }
                        }
                        else
                        {
                            if (i == 1)
                            {
                                if (funcList.Count != 0)
                                {
                                    Console.WriteLine($"Funcionario: {funcList[countTpEscala]}");
                                    nomeFunc = listFuncionarios.FirstOrDefault(x => x.NmNome == funcList[countTpEscala]).NmNome;
                                    escalaPronta.IdFuncionario = listFuncionarios.FirstOrDefault(x => x.NmNome == funcList[countTpEscala]).IdFuncionario;
                                    countTpEscala++;
                                }
                                else
                                {
                                    Console.WriteLine($"Sem Funcionario");
                                    escalaPronta.IdFuncionario = 78;
                                    countTpEscala++;
                                }

                            }
                            else
                            {
                                Console.WriteLine($"Sem Funcionario");
                                escalaPronta.IdFuncionario = 78;
                                countTpEscala++;
                            }

                        }
                        escalaPronta.IdEscala = escala.IdEscala;
                        escalaPronta.DtDataServico = Convert.ToDateTime(dataStr).Date;
                        escalaPronta.IdPostoTrabalho = item.IdPostoTrabalho;

                        //add a lista 
                        if (escalaPronta.IdFuncionario == 0)
                        {
                            Console.WriteLine($"Funcionario: {escalaPronta.IdFuncionario}");
                            escalaPronta.IdFuncionario = 78;
                            Console.WriteLine($"Funcionario: {escalaPronta.IdFuncionario}");
                        }
                        listEscalaPronta.Add(escalaPronta);
                        
                    }
                }

                if (countfunc - ppp_X_TipoEscala <= 0) // verifico se a subtracao é menor que zer e zero a lista
                {
                    funcList.Clear();
                    countfunc = 0;
                }
                else
                {
                    funcList.RemoveRange(0, ppp_X_TipoEscala); // removo a qtd de nomes de acordo com a qtd de pessoas
                                                               // que já estão alocadas nos postos
                    countfunc -= ppp_X_TipoEscala;
                }
            }
            Console.WriteLine("-----------XXX------------");


            Console.WriteLine("-----------XXX------------");
            Console.WriteLine($"IMPRESSÃO DA ESCALA COMO VAI PARA O BANCO");
            Console.WriteLine("-----------XXX------------");


            Console.WriteLine("-----------XXX------------");
            foreach (var item5 in listEscalaPronta)
            {
                Console.WriteLine($"id da escala: {item5.IdEscala}");
                Console.WriteLine($"data dos erviço: {item5.DtDataServico.Date}");
                Console.WriteLine($"Id do funcionario: {item5.IdFuncionario}");
                Console.WriteLine($"Id do posto: {item5.IdPostoTrabalho}");
            }

            //salvar a lista no banco de dados            

            var ListEscalaProntaDTOs = await _escalaProntaService.IncluirLista(_mapper.Map<EscalaProntaDTO[]>(listEscalaPronta));

            var escalaProntaModels = _mapper.Map<List<EscalaProntaModel>>(ListEscalaProntaDTOs);
            var escalaProntaInvalidos = escalaProntaModels.Where(fm => !fm.Valido).ToList();

            //atualizar a escala para gerada true
            escala.IsGerada = true;
            var EscalaDTO = await _escalaService.Alterar(escala.IdEscala, _mapper.Map<EscalaDTO>(escala));
            var escalaModel = _mapper.Map<EscalaModel>(EscalaDTO);

            if (escalaProntaInvalidos.Any())
            {
                return BadRequest(new RetornoModel { Valido = false, Mensagem = string.Join(", ", escalaProntaInvalidos.Select(fm => fm.Mensagem)) });
            }

            return Ok(escalaProntaModels);
        }

    }
}
