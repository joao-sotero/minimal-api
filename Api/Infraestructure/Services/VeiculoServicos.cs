using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infraestructure.Db;

namespace minimal_api.Infraestructure.Services
{
    public class VeiculoServicos : IVeiculoServicos
    {
        private readonly DbContexto _dbContext;

        public VeiculoServicos(DbContexto dbContext)
        {
            _dbContext = dbContext;
        }

        public void Apagar(int id)
        {
            var veiculo = BuscaPorId(id);
            _dbContext.Veiculos.Remove(veiculo);
            _dbContext.SaveChanges();
        }

        public Veiculo Atualiza(Veiculo veiculo)
        {
            var veiculoBanco = BuscaPorId(veiculo.Id);
            veiculoBanco.Nome = veiculo.Nome;
            veiculoBanco.Marca = veiculo.Marca;
            veiculoBanco.Ano = veiculo.Ano;

            _dbContext.Veiculos.Update(veiculoBanco);
            _dbContext.SaveChanges();

            return veiculoBanco;
        }

        public Veiculo BuscaPorId(int id) 
            => _dbContext.Veiculos.AsNoTracking().SingleOrDefault(x => x.Id == id)
                ?? throw new Exception("Não existe Veiculo para esse id");

        public Veiculo Incluir(Veiculo veiculo)
        {
             var entity = _dbContext.Veiculos.Add(veiculo).Entity;
            _dbContext.SaveChanges();
            return entity;
        }

        public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _dbContext.Veiculos.AsNoTracking().ToList();

            if (!string.IsNullOrWhiteSpace(nome))
            {
               query = query.Where(v =>  EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%")).ToList();
            }
            if (!string.IsNullOrWhiteSpace(marca))
            {
                query = query.Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{marca.ToLower()}%")).ToList();
            }

            query = query.Skip((pagina-1) * 10).Take(10).ToList();

            return query;
        }
    }
}
