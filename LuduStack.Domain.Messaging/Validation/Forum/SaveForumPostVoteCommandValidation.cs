using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumPostVoteCommandValidation : BaseUserCommandValidation<SaveForumPostVoteCommand, int>
    {
        public SaveForumPostVoteCommandValidation()
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