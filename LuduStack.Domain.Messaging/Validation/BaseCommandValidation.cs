using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public abstract class BaseCommandValidation<T> : AbstractValidator<T> where T : BaseCommand
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("The Id must be provided.");
        }
    }

    public abstract class BaseCommandValidation<T, TResult> : AbstractValidator<T> where T : BaseCommand<TResult>
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("The Id must be provided.");
        }
    }

    public abstract class BaseUserCommandValidation<T> : BaseCommandValidation<T> where T : BaseUserCommand
    {
        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage("The UserId must be provided.");
        }
    }

    public abstract class BaseUserCommandValidation<T, TResult> : BaseCommandValidation<T, TResult> where T : BaseUserCommand<TResult>
    {
        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage("The UserId must be provided.");
        }
    }
}