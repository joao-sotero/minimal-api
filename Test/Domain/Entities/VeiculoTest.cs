using minimal_api.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var veiculo = new Veiculo();

            // Act
            veiculo.Id = 1;
            veiculo.Marca = "toyota";
            veiculo.Ano = 2024;
            veiculo.Nome = "etios";

            // Assert
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("toyota", veiculo.Marca);
            Assert.AreEqual(2024, veiculo.Ano);
            Assert.AreEqual("etios", veiculo.Nome);
        }
    }
}
