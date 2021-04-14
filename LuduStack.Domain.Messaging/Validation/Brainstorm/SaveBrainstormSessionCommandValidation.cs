using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveBrainstormSessionCommandValidation : BaseUserCommandValidation<SaveBrainstormSessionCommand>
    {
        public SaveBrainstormSessionCommandValidation()
        {
            ValidateUserId();
            ValidateEntity();
            ValidateType();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.BrainstormSession)
                .NotNull()
                .WithMessage("No Brainstorm Session to save.");
        }

        protected void ValidateType()
        {
            RuleFor(c => c.BrainstormSession.Type)
                .NotNull()
                .WithMessage("The Brainstorm Session must have a Type.");
        }
    }
}