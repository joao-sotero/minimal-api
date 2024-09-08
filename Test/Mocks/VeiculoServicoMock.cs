using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServicos
{
    private static List<Veiculo> Veiculos = new List<Veiculo>(){
        new Veiculo
            {
                Id = 1,
                Nome = "sw4",
                Marca = "toyota",
                Ano = 2024
            },
       new Veiculo
            {
                Id = 1,
                Nome = "Yaris",
                Marca = "toyota",
                Ano = 2024
            }
    };

    public void Apagar(int id)
    {
        var veiculosBanco = BuscaPorId(id);
        Veiculos.Remove(veiculosBanco);
    }

    public Veiculo Atualiza(Veiculo veiculo)
    {
        var veiculosBanco = BuscaPorId(veiculo.Id);
        veiculosBanco.Nome = veiculo.Nome;
        veiculosBanco.Marca = veiculo.Marca;
        veiculosBanco.Ano = veiculo.Ano;

        return veiculosBanco;
    }

    public Veiculo Incluir(Veiculo veiculo)
    {
        veiculo.Id = Veiculos.Count() + 1;
        Veiculos.Add(veiculo);

        return veiculo;
    }

    public Veiculo BuscaPorId(int id)
    {
        return Veiculos.FirstOrDefault(a => a.Id == id)
            ?? throw new Exception("Não existe Veiculo para esse id");
    }

    public List<Veiculo> Todos(int pagina, string? nome, string? marca)
    {
        return Veiculos;
    }
}