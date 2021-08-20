using Jureg.Business.Models;
using System;
using System.Threading.Tasks;

namespace Jureg.Business.Interfaces
{
    public interface IProdutoService : IDisposable
    {
        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);        
    }
}
