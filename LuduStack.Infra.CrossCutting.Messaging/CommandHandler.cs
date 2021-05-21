using FluentValidation.Results;
using LuduStack.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public abstract class CommandHandler
    {
        protected ValidationResult ValidationResult;

        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AddError(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        protected async Task<ValidationResult> Commit(IUnitOfWork uow, string message)
        {
            try
            {
                await uow.Commit();
            }
            catch (Exception ex)
            {
                AddError(message);
                AddError(ex.Message);
            }

            return ValidationResult;
        }

        protected async Task<ValidationResult> Commit(IUnitOfWork uow)
        {
            return await Commit(uow, string.Empty).ConfigureAwait(false);
        }
    }
}