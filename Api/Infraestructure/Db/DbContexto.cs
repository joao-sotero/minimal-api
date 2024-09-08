using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;

namespace minimal_api.Infraestructure.Db
{
    public class DbContexto : DbContext
    {
        //private readonly IConfiguration _configuracaoAppSettings;

        public DbContexto(DbContextOptions<DbContexto> options/*, IConfiguration configuracaoAppSettings*/)
           : base(options)
        {
            //_configuracaoAppSettings = configuracaoAppSettings;
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>()
                .Property(a => a.Perfil)
                .HasConversion(
                    v => v.ToString(),
                    v => (EPerfil)Enum.Parse(typeof(EPerfil), v)
                )
        .HasColumnType("varchar(20)");

            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Email = "Administrador@email.com",
                    Senha = "123456",
                    Perfil = EPerfil.adm
                });

            modelBuilder.Entity<Veiculo>().HasData(
               new Veiculo
               {
                   Id = 1,
                   Nome = "Yaris",
                   Marca = "toyota",
                   Ano = 2024
               });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);

            //if (!optionsBuilder.IsConfigured)
            //{
            //    var stringConexao = _configuracaoAppSettings.GetConnectionString("DefaultConnection")?.ToString();
            //    if (!string.IsNullOrEmpty(stringConexao))
            //    {
            //        optionsBuilder.UseMySql(
            //            stringConexao,
            //            ServerVersion.AutoDetect(stringConexao)
            //        );
            //    }
            //}
        }
    }
}