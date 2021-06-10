namespace LuduStack.Domain.Messaging
{
    public class DeleteForumCategoryCommandValidation : BaseCommandValidation<DeleteForumCategoryCommand>
    {
        public DeleteForumCategoryCommandValidation()
        {
            ValidateId();
        }
    }
}