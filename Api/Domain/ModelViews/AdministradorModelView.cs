using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.ModelViews
{
    public class AdministradorModelView
    {
        public AdministradorModelView(int id, string email, EPerfil perfil)
        {
            Id = id;
            Email = email;
            Perfil = perfil;
        }

        public int Id { get; }

        public string Email { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public EPerfil Perfil { get; set; }
    }
}