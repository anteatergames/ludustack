using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveTeamCommandValidation : BaseCommandValidation<SaveTeamCommand>
    {
        public SaveTeamCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Team)
                .NotNull()
                .WithMessage("No Team to save.");
        }
    }
}