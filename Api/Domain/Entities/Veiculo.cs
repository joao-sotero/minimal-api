using minimal_api.Domain.DTO;
using System.ComponentModel.DataAnnotations;

namespace minimal_api.Domain.Entities
{
    public class Veiculo
    {
        public Veiculo()
        {
        }

        public Veiculo(VeiculoDTO veiculoDTO)
        {
            Nome = veiculoDTO.Nome;
            Marca = veiculoDTO.Marca;
            Ano = veiculoDTO.Ano;
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        public int Ano { get; set; }
    }
}
