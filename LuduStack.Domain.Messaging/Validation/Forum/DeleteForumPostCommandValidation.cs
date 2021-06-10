namespace LuduStack.Domain.Messaging
{
    public class DeleteForumPostCommandValidation : BaseCommandValidation<DeleteForumPostCommand, Models.ForumPost>
    {
        public DeleteForumPostCommandValidation()
        {
            ValidateId();
        }
    }
}