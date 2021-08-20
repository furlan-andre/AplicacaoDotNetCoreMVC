using AutoMapper;
using Jureg.App.Dto;
using Jureg.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jureg.App.AutoMapper
{
    public class ModelMappingProfile : Profile
    {
        public ModelMappingProfile()
        {
            CreateMap<Fornecedor, FornecedorDto>().ReverseMap();
            CreateMap<Produto, ProdutoDto>().ReverseMap();
            CreateMap<Endereco, EnderecoDto>().ReverseMap();

        }
    }
}
