using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGameIdeaCommandValidation : BaseCommandValidation<SaveGameIdeaCommand>
    {
        public SaveGameIdeaCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.GameIdea)
                .NotNull()
                .WithMessage("No Game Idea to save.");
        }
    }
}