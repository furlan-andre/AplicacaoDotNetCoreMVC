using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jureg.App.Dto
{
    public class ErrorDto
    {
        public int ErroCode { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
    }
}
