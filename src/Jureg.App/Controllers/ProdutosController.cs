using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jureg.App.Dto;
using Jureg.Business.Interfaces;
using AutoMapper;
using Jureg.Business.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Jureg.App.Extensions;

namespace Jureg.App.Controllers
{
    [Authorize]
    [Route("produtos")]
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        
        public ProdutosController(IProdutoRepository produtoRepository,
                                  IFornecedorRepository fornecedorRepository,
                                  IProdutoService produtoService,
                                  IMapper mapper,
                                  INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [Route("lista")]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoDto>>(await _produtoRepository.ObterProdutosFornecedores()));
        }
        
        [AllowAnonymous]
        [Route("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var produtoDto = await ObterProduto(id);

            if (produtoDto == null)
            {
                return NotFound();
            }

            return View(produtoDto);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [Route("novo")]
        public async Task<IActionResult> Create()
        {
            var produtoDto = await PopularFornecedor(new ProdutoDto());

            return View(produtoDto);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [Route("novo")]    
        [HttpPost]
        public async Task<IActionResult> Create(ProdutoDto produtoDto)
        {
            produtoDto = await PopularFornecedor(produtoDto);
            if (!ModelState.IsValid) return View(produtoDto);

            var imgPrefixo = Guid.NewGuid() + "_";

            if(! await UploadArquivo(produtoDto.ImagemFile, imgPrefixo))
            {
                return View(produtoDto);
            }

            produtoDto.Imagem = imgPrefixo + produtoDto.ImagemFile.FileName;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoDto));

            if (!OperacaoValida()) return View(produtoDto);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Produto", "Editar")]
        [Route("editar/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var produtoDto = await ObterProduto(id);

            if (produtoDto == null)
            {
                return NotFound();
            }

            return View(produtoDto);
        }

        [ClaimsAuthorize("Produto", "Editar")]
        [Route("editar/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ProdutoDto produtoDto)
        {
            if (id != produtoDto.Id) return NotFound();
            
            var produtoAtualizacao = await ObterProduto(id);
            produtoDto.Fornecedor = produtoAtualizacao.Fornecedor;
            produtoDto.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return View(produtoDto);

            if(produtoDto.ImagemFile != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";

                if (!await UploadArquivo(produtoDto.ImagemFile, imgPrefixo))
                {
                    return View(produtoDto);
                }

                produtoAtualizacao.Imagem = imgPrefixo + produtoDto.ImagemFile.FileName;                
            }

            produtoAtualizacao.Nome = produtoDto.Nome;
            produtoAtualizacao.Descricao = produtoDto.Descricao;
            produtoAtualizacao.Valor = produtoDto.Valor;
            produtoAtualizacao.Ativo = produtoDto.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            if (!OperacaoValida()) return View(produtoDto);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("excluir/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var produto = await ObterProduto(id);

            if (produto == null) return NotFound();

            return View(produto); 
        }

        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("excluir/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produto = await ObterProduto(id);

            if (produto == null) return NotFound();

            await _produtoService.Remover(id);

            if (!OperacaoValida()) return View(produto);

            TempData["Suceso"] = "Produto excluído com sucesso!";

            return RedirectToAction("Index");
        }

        
        private async Task<ProdutoDto> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoDto>(await _produtoRepository.ObterProdutoForncedor(id));
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorDto>>(await _fornecedorRepository.ObterTodos());

            return produto;
        }

        private async Task<ProdutoDto> PopularFornecedor(ProdutoDto produtoDto)
        {
            produtoDto.Fornecedores = _mapper.Map<IEnumerable<FornecedorDto>>(await _fornecedorRepository.ObterTodos());

            return produtoDto;
        }

        private async Task<bool> UploadArquivo(IFormFile imagemFile, string imgPrefixo)
        {
            if (imagemFile.Length <= 0) return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + imagemFile.FileName);

            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imagemFile.CopyToAsync(stream);
            }

            return true;
        }
    }
}
