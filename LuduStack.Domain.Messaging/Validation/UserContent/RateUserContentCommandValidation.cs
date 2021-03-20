using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class RateUserContentCommandValidation : UserContentValidation<RateUserContentCommand>
    {
        public RateUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }

        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty);
        }
    }
}