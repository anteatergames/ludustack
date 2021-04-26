using FluentValidation.Results;
using MediatR;
using System;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public abstract class Query<TResult> : Message, IRequest<TResult>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        protected Query()
        {
            Timestamp = DateTime.Now;
            ValidationResult = new ValidationResult();
        }

        public virtual bool IsValid()
        {
            return ValidationResult.IsValid;
        }
    }
}