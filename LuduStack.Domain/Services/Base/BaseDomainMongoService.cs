namespace LuduStack.Domain.Services
{
    public abstract class BaseDomainMongoService<T, TRepository>
    {
        protected readonly TRepository repository;

        protected BaseDomainMongoService(TRepository repository)
        {
            this.repository = repository;
        }
    }
}