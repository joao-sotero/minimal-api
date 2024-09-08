using minimal_api.Domain.DTO;
using minimal_api.Domain.Enuns;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Domain.Entities
{
    public class Administrador
    {
        public Administrador()
        {
        }

        public Administrador(AdministradorDTO administradorDTO)
        {
            Email = administradorDTO.Email;
            Senha = administradorDTO.Senha;
            Perfil = administradorDTO.Perfil;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Senha { get; set; } 

        [Required]
        [StringLength(10)]
        public EPerfil Perfil { get; set; } 
    }
}