using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;
using minimal_api.Infraestructure.Db;
using minimal_api.Infraestructure.Services;
using System.Reflection;

namespace Test.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicosTest
    {
        private DbContexto CriarContextoDeTeste()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            var connection = configuration.GetConnectionString("DefaultConnection")?.ToString();

            var options = new DbContextOptionsBuilder<DbContexto>()
              .UseMySql(connection, ServerVersion.AutoDetect(connection))
              .Options;

            var context = new DbContexto(options);

            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            return context;
        }

        private static Administrador Administrador()
           => new Administrador
           {
               Email = "teste@teste.com",
               Senha = "teste",
               Perfil = EPerfil.adm,
           };

        private void ArrangeTest(out Administrador adm, out AdministradorServico administradorServico)
        {
            var context = CriarContextoDeTeste();
            adm = Administrador();
            administradorServico = new AdministradorServico(context);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange
            ArrangeTest(out Administrador adm, out AdministradorServico administradorServico);

            // Act
            administradorServico.Incluir(adm);

            // Assert
            Assert.AreEqual(1, administradorServico.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscaPorId()
        {
            // Arrange
            ArrangeTest(out Administrador adm, out AdministradorServico administradorServico);

            // Act
            administradorServico.Incluir(adm);
            var admDoBanco = administradorServico.BuscaPorId(adm.Id);

            // Assert
            Assert.AreEqual(1, admDoBanco?.Id);
        }       
    }
}