using minimal_api.Domain.Domain;
using minimal_api.Domain.ModelViews;
using System.Net;
using System.Text.Json;
using System.Text;
using Test.Helpers;
using minimal_api.Domain.DTO;
using System.Net.Http.Headers;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;

namespace Test.Domain.Requests
{
    [TestClass]
    public class VeiculoRequestTest
    {
        [ClassInitialize]
        public static async Task ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
            await Common.GetJwtAsync();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestarCriarVeiculo()
        {
            // Arrange
            var veiculoDTO = new VeiculoDTO
            { 
                Nome = "Teste",
                Marca = "toyota",
                Ano = 2024
            };

            var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "Application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/veiculo")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            //// Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual(veiculoDTO?.Marca, veiculo.Marca);
            Assert.AreEqual(veiculoDTO?.Nome, veiculo.Nome);
            Assert.AreEqual(veiculoDTO?.Ano, veiculo.Ano);
        }

        [TestMethod]
        public async Task TestaPegaUm()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/veiculo/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual("toyota", veiculo.Marca);
            Assert.AreEqual("sw4", veiculo.Nome);
            Assert.AreEqual(2024, veiculo.Ano);
        }

        [TestMethod]
        public async Task TestaTodos()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/veiculo?pagina=1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculo = JsonSerializer.Deserialize<List<Veiculo>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual(2, veiculo.Count);
            Assert.IsNotNull(veiculo[0].Id);
            Assert.IsNotNull(veiculo[0].Marca);
            Assert.IsNotNull(veiculo[0].Ano);
            Assert.IsNotNull(veiculo[0].Nome);
            Assert.IsNotNull(veiculo[1].Id);
            Assert.IsNotNull(veiculo[1].Marca);
            Assert.IsNotNull(veiculo[1].Ano);
            Assert.IsNotNull(veiculo[1].Nome);
        }

        [TestMethod]
        public async Task TestarAtualizar()
        {
            // Arrange
            var veiculoDTO = new VeiculoDTO
            {
                Nome = "Teste",
                Marca = "toyota",
                Ano = 2024
            };

            var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "Application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "/veiculo/1")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            //// Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual(veiculoDTO?.Marca, veiculo.Marca);
            Assert.AreEqual(veiculoDTO?.Nome, veiculo.Nome);
            Assert.AreEqual(veiculoDTO?.Ano, veiculo.Ano);
        }

        [TestMethod]
        public async Task TestaApagar()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, "/veiculo/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
