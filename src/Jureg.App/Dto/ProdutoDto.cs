using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Jureg.App.Dto
{
    public class ProdutoDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Fornecedor")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage ="O campo {0} precisa ter entre {1} e {2} caracteres.", MinimumLength = 2)]
        public string Nome { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, ErrorMessage = "O campo {0} precisa ter entre {1} e {2} caracteres.", MinimumLength = 2)]
        public string Descricao { get; set; }

        [DisplayName("Imagem do Produto")]
        [NotMapped]
        [ScaffoldColumn(false)]
        public IFormFile ImagemFile { get; set; }
        public string Imagem { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório")]        
        public decimal Valor { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
        [DisplayName("Ativo?")]
        public bool Ativo { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public FornecedorDto Fornecedor { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public IEnumerable<FornecedorDto> Fornecedores { get; set; }
                
    }
}
