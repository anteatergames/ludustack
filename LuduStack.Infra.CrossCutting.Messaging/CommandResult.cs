using FluentValidation.Results;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public class CommandResult
    {
        public bool Success { get; set; }

        public int PointsEarned { get; set; }

        public ValidationResult Validation { get; set; }

        public CommandResult() : this(true)
        {
        }

        public CommandResult(bool success)
        {
            Success = success;
            Validation = new ValidationResult();
        }

        public CommandResult(ValidationResult validationResult)
        {
            Success = validationResult.IsValid;
            Validation = validationResult;
        }
    }

    public class CommandResult<T> : CommandResult
    {
        public T Result { get; set; }

        public CommandResult()
        {
        }

        public CommandResult(T result)
        {
            Result = result;
        }

        public CommandResult(T result, ValidationResult validationResult)
        {
            Result = result;
            Success = validationResult.IsValid;
            Validation = validationResult;
        }
    }
}