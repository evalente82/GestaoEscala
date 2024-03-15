using GestaoEscalaPermutas.Dominio.DTO.Departamento;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TesteUnitario
{
    public class DepartamentoTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DepartamentoTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestarIncluirDepartamento()
        {
            // Criar um cliente HTTP
            using (var client = _factory.CreateClient())
            {
                // Criar um novo departamento
                var departamento = new DepartamentoDTO
                {
                    NmNome = "Departamento Teste",
                    NmDescricao = "Descrição do Departamento Teste"
                };

                // Serializar o departamento para JSON
                var content = new StringContent(JsonConvert.SerializeObject(departamento), Encoding.UTF8, "application/json");

                // Enviar a requisição POST para a API
                var response = await client.PostAsync("/departamentos/incluir", content);

                // Validar o código de status da resposta
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                // Analisar o conteúdo da resposta
                var jsonResult = JsonConvert.DeserializeObject<DepartamentoDTO>(await response.Content.ReadAsStringAsync());

                // Validar os dados do departamento
                Assert.Equal(departamento.NmNome, jsonResult.NmNome);
                Assert.Equal(departamento.NmDescricao, jsonResult.NmDescricao);
            }
        }
    }
}
