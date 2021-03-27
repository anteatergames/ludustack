﻿using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public abstract class UserContentValidation<T> : AbstractValidator<T> where T : BaseCommand
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty);
        }
    }
}