using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Jureg.App.Dto;

namespace Jureg.App.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }        
        public DbSet<Jureg.App.Dto.ProdutoDto> ProdutoDto { get; set; }
        public DbSet<Jureg.App.Dto.EnderecoDto> EnderecoDto { get; set; }
    }
}
