using Dev.Business.Interfaces;
using Dev.Business.Models;
using Dev.Business.Notificacoes;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Text;

namespace Dev.Business.Services
{
    public abstract class BaseService
    {
        private readonly INotificador _notificador;

        public BaseService(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string messagem)
        {
            // Propagar esse erro até a camada de apresentação
            _notificador.Handle(new Notificacao(messagem));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {

            var validator = validacao.Validate(entidade);

            if(validator.IsValid) return true; 

            Notificar(validator);

            return false;
        }
    }
}
