using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Dev.Business.Models;
using System.Linq;

namespace Dev.Data.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options): base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            

            //Inserção dos Mappings do banco
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);

            //Desativação da operação Cascade do banco
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e=>e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
