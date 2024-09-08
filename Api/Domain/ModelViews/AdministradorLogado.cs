using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.ModelViews
{
    public record AdministradorLogado
    {
        public string Email { get; set; } = default!;

        [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
        public EPerfil Perfil { get; set; } = default!;

        public string Token { get; set; } = default!;
    }
}
