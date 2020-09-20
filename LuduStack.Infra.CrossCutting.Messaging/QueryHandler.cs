using FluentValidation.Results;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public abstract class QueryHandler
    {
        protected ValidationResult ValidationResult;

        protected QueryHandler()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AddError(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }
    }
}