using minimal_api.Domain.Domain;
using minimal_api.Domain.ModelViews;
using System.Net;
using System.Text.Json;
using System.Text;
using Test.Helpers;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.DTO;
using System.Net.Http.Headers;

namespace Test.Domain.Requests
{
    [TestClass]
    public class AdministradorRequestTest
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
        public async Task<string> TestarLogin()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Senha = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrador/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogado?.Email);
            Assert.AreEqual(EPerfil.adm, admLogado?.Perfil);
            Assert.IsNotNull(admLogado?.Token);

            return admLogado?.Token ?? string.Empty;
        }

        [TestMethod]
        public async Task TestarCriar()
        {
            // Arrange
            var admDTO = new AdministradorDTO
            {
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = EPerfil.adm,
            };

            var content = new StringContent(JsonSerializer.Serialize(admDTO), Encoding.UTF8, "Application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/administrador")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var adm = JsonSerializer.Deserialize<AdministradorModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual(admDTO.Email, adm?.Email);
            Assert.AreEqual(EPerfil.adm, adm?.Perfil);
        }

        [TestMethod]
        public async Task TestaPegaUm()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/administrador/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var adm = JsonSerializer.Deserialize<AdministradorModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(adm?.Id);
            Assert.IsNotNull( adm?.Email);
            Assert.AreEqual(EPerfil.adm, adm?.Perfil);
        }

        [TestMethod]
        public async Task TestaTodos()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/administrador?pagina=1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Common.jwt);

            // Act
            var response = await Setup.client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var adm = JsonSerializer.Deserialize<List<AdministradorModelView>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.AreEqual(2, adm.Count);
            Assert.IsNotNull(adm[0].Id);
            Assert.IsNotNull(adm[0].Email);
            Assert.AreEqual(EPerfil.adm, adm[0].Perfil);
            Assert.IsNotNull(adm[1].Id);
            Assert.IsNotNull(adm[1].Email);
            Assert.AreEqual(EPerfil.editor, adm[1].Perfil);
        }
    }
}
