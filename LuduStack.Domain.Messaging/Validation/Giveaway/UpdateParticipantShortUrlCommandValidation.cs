using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class UpdateParticipantShortUrlCommandValidation : BaseCommandValidation<UpdateParticipantShortUrlCommand>
    {
        public UpdateParticipantShortUrlCommandValidation()
        {
            ValidateId();
            ValidateEmail();
            ValidateShorUrl();
        }

        protected void ValidateEmail()
        {
            RuleFor(c => c.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("No Email provided.");
        }

        protected void ValidateShorUrl()
        {
            RuleFor(c => c.ShortUrl)
                .NotNull()
                .NotEmpty()
                .WithMessage("No ShortUrl provided.");
        }
    }
}