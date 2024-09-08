using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IVeiculoServicos
    {
        List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo BuscaPorId(int id);
        Veiculo Incluir(Veiculo veiculo);
        Veiculo Atualiza(Veiculo veiculo);
        void Apagar(int id);
    }
}
