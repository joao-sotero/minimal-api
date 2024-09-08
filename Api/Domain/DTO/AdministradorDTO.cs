using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.DTO
{
    public class AdministradorDTO
    {
        public string Senha { get; set; }

        public string Email { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public EPerfil Perfil { get; set; }
    }
}