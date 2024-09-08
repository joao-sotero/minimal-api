using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Domain;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infraestructure.Db;

namespace minimal_api.Infraestructure.Services
{
    public class AdministradorServico : IAdministradorServicos
    {
        private readonly DbContexto _dbContext;

        public AdministradorServico(DbContexto dbContext)
        {
            _dbContext = dbContext;
        }

        public Administrador Login(LoginDTO login)
        {
            return _dbContext.Administradores.AsNoTracking().SingleOrDefault(a => a.Email == login.Email && a.Senha == login.Senha);
        }

        public Administrador Incluir(Administrador administrador)
        {
            var entity = _dbContext.Administradores.Add(administrador).Entity;
            _dbContext.SaveChanges();
            return entity;
        }

        public Administrador BuscaPorId(int id)
           => _dbContext.Administradores.AsNoTracking().SingleOrDefault(x => x.Id == id)
               ?? throw new Exception("Não existe ADM para esse id");


        public List<Administrador> Todos(int pagina = 1, string? email = null, string? perfil = null)
        {
            var query = _dbContext.Administradores.AsNoTracking().ToList();

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(v => EF.Functions.Like(v.Email.ToLower(), $"%{email.ToLower()}%")).ToList();
            }
            if (!string.IsNullOrWhiteSpace(perfil))
            {
                query = query.Where(v => EF.Functions.Like(v.Perfil.ToString().ToLower(), $"%{perfil.ToLower()}%")).ToList();
            }

            query = query.Skip((pagina - 1) * 10).Take(10).ToList();

            return query;
        }
    }
}
