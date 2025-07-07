using GestaoEscalaPermutas.Dominio.Interfaces.Feriados;
using GestaoEscalaPermutas.Infra.Data.EntitiesDefesaCivilMarica;
using GestaoEscalaPermutas.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEscalaPermutas.Dominio.Services.Feriados
{
    public class FeriadoService : IFeriadoService
    {
        private readonly IFeriadoRepository _feriadoRepository;

        public FeriadoService(IFeriadoRepository feriadoRepository)
        {
            _feriadoRepository = feriadoRepository;
        }

        public async Task<HashSet<DateTime>> ObterDatasFeriadosAsync(int ano)
        {
            var feriadosDoBanco = await _feriadoRepository.ObterPorAnoAsync(ano);
            var feriadosNacionais = GerarFeriadosNacionais(ano);

            var todasAsDatas = feriadosDoBanco.Select(f => f.Data.Date)
                .Union(feriadosNacionais.Select(f => f.Data.Date));

            return new HashSet<DateTime>(todasAsDatas);
        }

        private IEnumerable<Feriado> GerarFeriadosNacionais(int ano)
        {
            var feriados = new List<Feriado>();
            var pascoa = CalcularPascoa(ano);

            // Feriados Fixos Nacionais
            feriados.Add(new Feriado { Data = new DateTime(ano, 1, 1), Descricao = "Confraternização Universal", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 4, 21), Descricao = "Tiradentes", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 5, 1), Descricao = "Dia do Trabalho", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 9, 7), Descricao = "Independência do Brasil", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 10, 12), Descricao = "Nossa Senhora Aparecida", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 11, 2), Descricao = "Finados", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 11, 15), Descricao = "Proclamação da República", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = new DateTime(ano, 12, 25), Descricao = "Natal", Tipo = TipoFeriado.Nacional });

            // Feriados Móveis Nacionais
            feriados.Add(new Feriado { Data = pascoa.AddDays(-47), Descricao = "Carnaval", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = pascoa.AddDays(-2), Descricao = "Sexta-feira Santa", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = pascoa, Descricao = "Páscoa", Tipo = TipoFeriado.Nacional });
            feriados.Add(new Feriado { Data = pascoa.AddDays(60), Descricao = "Corpus Christi", Tipo = TipoFeriado.Nacional });

            return feriados;
        }

        private DateTime CalcularPascoa(int ano)
        {
            // Algoritmo de Gauss para cálculo da Páscoa
            int a = ano % 19;
            int b = ano / 100;
            int c = ano % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int mes = (h + l - 7 * m + 114) / 31;
            int dia = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(ano, mes, dia);
        }
    }
}