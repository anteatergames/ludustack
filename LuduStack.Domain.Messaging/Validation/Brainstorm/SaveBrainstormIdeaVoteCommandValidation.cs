using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveBrainstormIdeaVoteCommandValidation : BaseUserCommandValidation<SaveBrainstormIdeaVoteCommand>
    {
        public SaveBrainstormIdeaVoteCommandValidation()
        {
            ValidateId();
            ValidateUserId();
            ValidateVote();
        }

        protected void ValidateVote()
        {
            RuleFor(c => c.Vote)
                .NotEmpty()
                .WithMessage("No vote informed.");
        }
    }
}