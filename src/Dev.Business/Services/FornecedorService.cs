﻿using Dev.Business.Interfaces;
using Dev.Business.Models;
using Dev.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.Business.Services
{
    public class FornecedorService : BaseService, IFornecedorService
    {
        protected readonly IFornecedorRepository _fornecedorRepository;
        protected readonly IEnderecoRepository _enderecoRepository;
        protected readonly INotificador _notificador;

        public FornecedorService(IFornecedorRepository fornecedorRepository,
                                 IEnderecoRepository enderecoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _notificador = notificador;
        }

        public async Task Adicionar(Fornecedor fornecedor)
        {
            // Validar o estado da entidade!
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor) 
                || !ExecutarValidacao(new EnderecoValidation(), fornecedor.Endereco)) return;

            //Validar se o documento já existe
              if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento).Result.Any()) return;

            //Insere no banco
            await _fornecedorRepository.Adicionar(fornecedor);
        }

        public async Task Atualizar(Fornecedor fornecedor)
        {
            // Valida o estado da entidade
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor)) return;

            // Valida se o Documento já existe e se o Id está correto
            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento && f.Id != fornecedor.Id).Result.Any())
            {
                Notificar("Já existe um fornecedor com este documento");
                return;
            }
        }

        public async Task AtualizarEndereco(Endereco endereco)
        {
            if (!ExecutarValidacao(new EnderecoValidation(), endereco)) return;

            await _enderecoRepository.Atualizar(endereco);
        }        

        public async Task Remover(Guid id)
        {
            if (_fornecedorRepository.ObterFornecedorProdutosEndereco(id).Result.Produtos.Any())
            {
                Notificar("O fornecedor possui produtos cadastrados");
                return;
            }


            await _enderecoRepository.Remover(_enderecoRepository.ObterEnderecoPorFornecedor(id).Result.Id);
            await _fornecedorRepository.Remover(id);
        }

        public void Dispose()
        {        
            _fornecedorRepository?.Dispose();
            _enderecoRepository?.Dispose();
        }
    }
}
