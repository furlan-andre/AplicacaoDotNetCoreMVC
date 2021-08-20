using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jureg.App.Dto;
using Jureg.Business.Interfaces;
using AutoMapper;
using Jureg.Business.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authorization;
using Jureg.App.Extensions;

namespace Jureg.App.Controllers
{
    [Authorize]
    [Route("fornecedores")]    
    public class FornecedoresController : BaseController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IFornecedorService fornecedorService,
                                      IMapper mapper,
                                      INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        
        [Route("lista")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<FornecedorDto>>(await _fornecedorRepository.ObterTodos()));
        }

        
        [Route("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var fornecedorDto = await ObterFornecedorEndereco(id);

            if (fornecedorDto == null)
            {
                return NotFound();
            }
            
            return View(fornecedorDto);
        }

        [ClaimsAuthorize("Fornecedor","Adicionar")]
        [Route("novo")]
        public IActionResult Create()
        {
            return View();
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [Route("novo")]
        [HttpPost]
        public async Task<IActionResult> Create(FornecedorDto fornecedorDto)
        {
            if (!ModelState.IsValid) return View(fornecedorDto);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDto);
            await _fornecedorService.Adicionar(fornecedor);
            
            if (!OperacaoValida()) return View(fornecedor);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var fornecedorDto = await ObterFornecedorProdutosEndereco(id);
            
            if (fornecedorDto == null)
            {
                return NotFound();
            }

            return View(fornecedorDto);
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, FornecedorDto fornecedorDto)
        {
            if (id != fornecedorDto.Id) return NotFound();
            

            if (!ModelState.IsValid) return View(fornecedorDto);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDto);
            await _fornecedorService.Atualizar(fornecedor);

            if (!OperacaoValida()) return View(fornecedor);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir/{id:guid}")]                
        public async Task<IActionResult> Delete(Guid id)
        {
            var fornecedorDto = await ObterFornecedorEndereco(id);

            if(fornecedorDto == null)
            {
                return NotFound();
            }
            
            return View(fornecedorDto);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedorDto = await ObterFornecedorEndereco(id);

            if (fornecedorDto == null) return NotFound();

            await _fornecedorService.Remover(id);

            if (!OperacaoValida()) return View(fornecedorDto);

            TempData["Suceso"] = "Fornecedor excluído com sucesso!";

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [Route("obterEndereco/{id:guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedorDto = await ObterFornecedorEndereco(id);

            if (fornecedorDto == null) return NotFound();

            return PartialView("_DetalhesEndereco", fornecedorDto);
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("atualizarEndereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedorDto = await ObterFornecedorEndereco(id);

            if (fornecedorDto == null) return NotFound();

            return PartialView("_AtualizarEndereco", new FornecedorDto { Endereco = fornecedorDto.Endereco });
        }

        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("atualizarEndereco/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> AtualizarEndereco(FornecedorDto fornecedorDto)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");

            if (!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorDto);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(fornecedorDto.Endereco));


            if (!OperacaoValida()) return PartialView("_AtualizarEndereco", fornecedorDto);

            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorDto.Endereco.FornecedorId });

            return Json(new { success = true, url });
        }

        private async Task<FornecedorDto> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorDto>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        private async Task<FornecedorDto> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorDto>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}
