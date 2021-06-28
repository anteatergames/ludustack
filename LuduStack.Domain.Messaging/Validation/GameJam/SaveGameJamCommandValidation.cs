using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGameJamCommandValidation : BaseCommandValidation<SaveGameJamCommand>
    {
        public SaveGameJamCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.GameJam)
                .NotNull()
                .WithMessage("No Game Jam to save.");
        }
    }
}