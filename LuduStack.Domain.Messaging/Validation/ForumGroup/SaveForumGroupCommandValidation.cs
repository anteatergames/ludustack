using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumGroupCommandValidation : BaseCommandValidation<SaveForumGroupCommand>
    {
        public SaveForumGroupCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.ForumGroup)
                .NotNull()
                .WithMessage("No Forum Group to save.");
        }
    }
}