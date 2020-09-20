using FluentValidation.Results;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public class CommandResult
    {
        public bool Success { get; set; }

        public ValidationResult Validation { get; set; }

        public CommandResult(bool success)
        {
            Success = success;
            Validation = new ValidationResult();
        }

        public CommandResult() : this(true)
        {
        }
    }
}