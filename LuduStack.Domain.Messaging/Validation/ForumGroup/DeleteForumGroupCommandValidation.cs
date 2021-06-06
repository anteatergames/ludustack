namespace LuduStack.Domain.Messaging
{
    public class DeleteForumGroupCommandValidation : BaseCommandValidation<DeleteForumGroupCommand>
    {
        public DeleteForumGroupCommandValidation()
        {
            ValidateId();
        }
    }
}