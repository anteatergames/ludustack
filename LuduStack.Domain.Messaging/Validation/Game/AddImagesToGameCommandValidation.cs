using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class AddImagesToGameCommandValidation : BaseCommandValidation<AddImagesToGameCommand>
    {
        public AddImagesToGameCommandValidation()
        {
            ValidateId();
            ValidateMedia();
        }

        protected void ValidateMedia()
        {
            RuleFor(c => c.Media)
                .NotNull()
                .NotEmpty()
                .WithMessage("No Media to save.");
        }
    }
}