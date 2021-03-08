using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class UnlikeUnlikeUserContentCommandValidation : UserContentValidation<UnlikeUserContentCommand>
    {
        public UnlikeUnlikeUserContentCommandValidation()
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