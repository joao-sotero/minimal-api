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
    public class VeiculoServicosTest
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

            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            return context;
        }

        private static Veiculo Veiculo()
           => new Veiculo
           {
               Id = 1,
               Marca = "toyota",
               Ano = 2024,
               Nome = "etios"
           };

        private void ArrangeTest(out Veiculo veiculo, out VeiculoServicos veiculoServicos)
        {
            var context = CriarContextoDeTeste();
            veiculo = Veiculo();
            veiculoServicos = new VeiculoServicos(context);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange
            ArrangeTest(out Veiculo veiculo, out VeiculoServicos veiculoServicos);

            // Act
            veiculoServicos.Incluir(veiculo);

            // Assert
            Assert.AreEqual(1, veiculoServicos.Todos(1).Count());
        }

        [TestMethod]
        public void TestandoBuscaPorId()
        {
            // Arrange
            ArrangeTest(out Veiculo veiculo, out VeiculoServicos veiculoServicos);

            // Act
            veiculoServicos.Incluir(veiculo);
            var admDoBanco = veiculoServicos.BuscaPorId(veiculo.Id);

            // Assert
            Assert.AreEqual(1, admDoBanco?.Id);
        }
    }
}