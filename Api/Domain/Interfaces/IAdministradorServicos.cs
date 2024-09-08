using minimal_api.Domain.Domain;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdministradorServicos
    {
        Administrador Login(LoginDTO login);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int pagina = 1, string? email = null, string? perfil = null);
        Administrador BuscaPorId(int id);
    }
}
