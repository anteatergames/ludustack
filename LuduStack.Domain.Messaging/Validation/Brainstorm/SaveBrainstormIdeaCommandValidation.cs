using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveBrainstormIdeaCommandValidation : BaseUserCommandValidation<SaveBrainstormIdeaCommand>
    {
        public SaveBrainstormIdeaCommandValidation()
        {
            ValidateUserId();
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.BrainstormIdea)
                .NotNull()
                .WithMessage("No Idea to save.");
        }
    }
}