using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveShortUrlCommandValidation : BaseCommandValidation<SaveShortUrlCommand>
    {
        public SaveShortUrlCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.ShortUrl)
                .NotNull()
                .WithMessage("No Short Url to save.");
        }
    }
}